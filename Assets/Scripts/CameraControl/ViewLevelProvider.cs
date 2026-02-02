using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.Subsystems;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

public class ViewLevelProvider : LocomotionProvider
{
    /// <summary>
    /// check transition and view state, in case only making animation on ground
    /// </summary>
    public bool IsViewOnGroundAndStable => !isEagleView && !isTransitioning;
    [SerializeField] private float floorOffsetInRunTime = 0;
    [SerializeField] private ContinuousMoveProviderBase moveLocomotion = default;
    [SerializeField] private Transform onGroundForwardSource = default;
    [SerializeField] private Transform eagleViewForwardSource = default;
    [SerializeField] private GameObject landingPointObj = default;
    [SerializeField, Range(5, 50), Description("Eagle View Height (metre)")] private float height = 10.0f;
    [SerializeField, Range(0.1f, 5), Description("Switch transition duration")] private float transitionDuration = 2.0f;
    [SerializeField] private float panningFactor = 4f;
    [SerializeField] private float zoomFactor = 4f;
    [SerializeField] private bool speedAdaptiveToHeight = false;
    [SerializeField] private List<GameObject> enableCutoutsOnMapView = default;

    // View level
    public bool isEagleView { private set; get; } = false;
    private Coroutine transitionCoroutine;
    private bool isTransitioning = false;
    private Transform originTransform;
    private float onGroundRotY;

    // Panning and zooming
    private MRTKHandsAggregatorSubsystem aggregator;
    private bool isPanningZooming = false;
    private Vector3 initialAvgPos = Vector3.zero;
    private float initialDistance = 0f;
    private Vector3 initialOriginPos = Vector3.zero;
    private bool panZoomPressed;


    private void Start()
    {
        aggregator = (MRTKHandsAggregatorSubsystem)XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
        originTransform = system.xrOrigin.Origin.transform;
        landingPointObj?.SetActive(isEagleView);
        //set offset
        UpdateFloorOffset();
    }


    /// <summary>
    /// Public API for a button that trigger this action
    /// </summary>
    public void SwitchView()
    {
        if (isTransitioning) return;

        BeginLocomotion();

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        var startPos = originTransform.position;
        var startRot = originTransform.rotation;
        var startCameraOffsetRot = system.xrOrigin.transform.localRotation;
        var targetPos = system.xrOrigin.Camera.transform.position;

        //  if it is currently ground view but changing to eagle view
        targetPos.y = height;
        var targetRot = Quaternion.Euler(90, 0, 0);
        //var angles = system.xrOrigin.Camera.transform.localRotation.eulerAngles;
        //angles.z *= -1;
        //angles.y *= -1;
        var targetCameraOffsetRot = Quaternion.Euler(-system.xrOrigin.Camera.transform.localRotation.eulerAngles);
        // override if it is currently eagle view but changing to ground view 
        if (isEagleView)
        {
            targetPos.y = floorOffsetInRunTime;
            targetRot = Quaternion.Euler(0, onGroundRotY, 0);
            targetCameraOffsetRot = Quaternion.identity;
        }
        else
        {
            onGroundRotY = system.xrOrigin.transform.rotation.eulerAngles.y;
        }
        isEagleView = !isEagleView;
        if (landingPointObj != null)
        {
            landingPointObj.SetActive(isEagleView);
            var cameraRotY = Quaternion.LookRotation(system.xrOrigin.Camera.transform.forward, Vector3.up);
            cameraRotY.x = 0;
            cameraRotY.z = 0;
            landingPointObj.transform.rotation = cameraRotY;
        }
        SwitchForwardSource();

        transitionCoroutine = StartCoroutine(SmoothTransition(originTransform, startPos, startRot, targetPos, targetRot, startCameraOffsetRot, targetCameraOffsetRot, transitionDuration));
    }

    private void SwitchForwardSource()
    {
        if (moveLocomotion == null) return;
        if (isEagleView && eagleViewForwardSource != null)
        {
            moveLocomotion.forwardSource = eagleViewForwardSource;
            moveLocomotion.enableFly = true;
        }
        else if (onGroundForwardSource != null)
        {
            moveLocomotion.forwardSource = onGroundForwardSource;
            moveLocomotion.enableFly = false;
        }
    }

    private IEnumerator SmoothTransition(Transform target, Vector3 startPosition, Quaternion startRotation,
        Vector3 targetPosition, Quaternion targetRotation, Quaternion startCameraOffsetRot, Quaternion targetCameraOffsetRot, float duration = 1.0f)
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            target.position = Vector3.Lerp(startPosition, targetPosition, t);
            target.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            system.xrOrigin.transform.localRotation = Quaternion.Slerp(startCameraOffsetRot, targetCameraOffsetRot, 2 * t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.position = targetPosition;
        target.rotation = targetRotation;
        system.xrOrigin.transform.localRotation = targetCameraOffsetRot;
        EndLocomotion();
        isTransitioning = false;
    }


    /// <summary>
    /// Controls the panning and zooming interaction based on the positions of both hands or controllers.
    /// The movement direction and distance between the hands/controllers determine the panning and zooming effect.
    /// </summary>
    /// <param name="rightPos">The current position of the right hand or controller.</param>
    /// <param name="leftPos">The current position of the left hand or controller.</param>
    private void PanningAndZooming(Vector3 rightPos, Vector3 leftPos)
    {
        // Calculate movement direction in local space
        var rightLocalPos = this.transform.InverseTransformPoint(rightPos);
        var leftLocalPos = this.transform.InverseTransformPoint(leftPos);

        Vector3 avgPos = (rightLocalPos + leftLocalPos) / 2;
        float currentDistance = Vector3.Distance(leftLocalPos, rightLocalPos);
        if (isPanningZooming)
        {


            // Average movement for panning
            Vector3 averageMovement = avgPos - initialAvgPos;
            averageMovement = averageMovement * panningFactor;
            // no need for averageMovement.z in panning, used for zooming

            // Calculate distance for zooming
            float distanceChange = currentDistance - initialDistance;

            // Apply zooming
            averageMovement.z = distanceChange * zoomFactor;
            // Adaptive speed to height
            if (speedAdaptiveToHeight)
            {
                averageMovement *= Mathf.Max(originTransform.position.y, 1);
            }
            Vector3 worldSpaceMovement = new Vector3(
                averageMovement.x,     // World X (pan left/right)
                averageMovement.z,     // World Y (zoom height)
                averageMovement.y      // World Z (pan forward/backward)
            );

            originTransform.localPosition = initialOriginPos - worldSpaceMovement;
            //originTransform.localPosition = initialOriginPos - new Vector3(averageMovement.x, averageMovement.z, averageMovement.y);
        }
        else
        {
            initialAvgPos = avgPos;
            initialDistance = currentDistance;
            initialOriginPos = originTransform.localPosition;
        }
        isPanningZooming = true;
    }

    public void PanZoomStarted()
    {
        panZoomPressed = true;
    }
    public void PanZoomEnded()
    {
        panZoomPressed = false;
    }

    private void Update()
    {
        if (isEagleView && panZoomPressed)
        {
            var leftPos = CurrentXRNodePos(XRNode.LeftHand);
            var rightPos = CurrentXRNodePos(XRNode.RightHand);
            Debug.Log("Pan and zoom, left: " + leftPos + ", right: " + rightPos);
            if (leftPos != null && rightPos != null)
            {
                PanningAndZooming((Vector3)rightPos, (Vector3)leftPos);
            }
        }
        else
        {
            isPanningZooming = false;
        }
        if (landingPointObj != null && landingPointObj.activeSelf)
        {
            var originPos = system.xrOrigin.Camera.transform.position;
            originPos.y = floorOffsetInRunTime;
            landingPointObj.transform.position = originPos;

            var cameraRotY = system.xrOrigin.Camera.transform.localRotation;
            cameraRotY.x = 0;
            cameraRotY.z = 0;
            landingPointObj.transform.rotation = cameraRotY;
        }
        if (IsViewOnGroundAndStable)
        {
            foreach (var go in enableCutoutsOnMapView)
            {
                if (go.activeSelf) go.SetActive(false);
            }
            UpdateFloorOffset();
        }
        else if (isEagleView)
        {
            foreach (var go in enableCutoutsOnMapView)
            {
                if (!go.activeSelf) go.SetActive(true);
            }
        }

    }

    public Vector3? CurrentXRNodePos(XRNode node)
    {
        if (aggregator.TryGetJoint(TrackedHandJoint.IndexTip, node, out HandJointPose pose))
        {
            return pose.Position;
        }
        else
        {
            InputDevice controller = InputDevices.GetDeviceAtXRNode(node);
            if (controller.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
            {
                return position;
            }
        }
        return null;
    }

    private void UpdateFloorOffset()
    {
        if (originTransform != null)
        {
            var pos = originTransform.position;
            pos.y = floorOffsetInRunTime;
            originTransform.position = pos;
        }
    }
}
