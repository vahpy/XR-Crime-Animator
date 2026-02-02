using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.Subsystems;
using System.Collections;
using System.Collections.Generic;
using TrackManager.Animation;
using TrackManager.BodyTracking;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class VRBodyTracker : MonoBehaviour
{
    //private InputDevice rightHandController = default;
    //private InputDevice leftHandController = default;
    //private ArticulatedHandController rightXRHandController = default;
    //private ArticulatedHandController leftXRHandController = default;
    //singelton pattern
    [SerializeField] private XROrigin xrOrigin = default;
    public static VRBodyTracker instance;
    public AvatarAnimationControl currentAvatar { private set; get; }

    private bool isTracking;
    private Camera vrCamera;


    private readonly Vector3 rightControllerHandRotOffset = new Vector3(0, 0, -90);
    private readonly Vector3 leftControllerHandRotOffset = new Vector3(0, 0, 90);

    private MRTKHandsAggregatorSubsystem handSubsystem;
    //private readonly Quaternion rotOffset = Quaternion.Euler(new Vector3(90, 0, 0));

    private HeadHandPose lastPose;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        isTracking = false;
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }

        //rightHandController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        //leftHandController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        handSubsystem = (MRTKHandsAggregatorSubsystem)XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
        StartCoroutine(EnableWhenSubsystemAvailable());
    }
    private void Update()
    {
        if (currentAvatar == null || !isTracking) return;

        bool rightHandTracked = false;
        bool leftHandTracked = false;

        // Head
        HeadHandPose newPose = new HeadHandPose(vrCamera.transform.position, vrCamera.transform.forward);


        // Hand 
        if (!handSubsystem.running)
        {
            Debug.LogError("Hand subsystem not working!");
        }
        if (handSubsystem.TryGetJoint(TrackedHandJoint.Wrist, XRNode.LeftHand, out HandJointPose pose))
        {
            var locPose = currentAvatar.transform.InverseTransformPose(pose);

            newPose.leftWristLocPos = locPose.position;
            newPose.leftWristLocRot = locPose.rotation;
            leftHandTracked = true;
        }

        if (handSubsystem.TryGetJoint(TrackedHandJoint.Wrist, XRNode.RightHand, out pose))
        {
            var locPose = currentAvatar.transform.InverseTransformPose(pose);
            newPose.rightWristLocPos = locPose.position;
            newPose.rightWristLocRot = locPose.rotation;
            rightHandTracked = true;
        }


        // Controller
        if (!rightHandTracked || !leftHandTracked)
        {
            List<XRNodeState> states = new List<XRNodeState>();
            InputTracking.GetNodeStates(states);
            
            foreach (XRNodeState state in states)
            {
                if (!leftHandTracked && state.nodeType == XRNode.LeftHand)
                {
                    if (state.TryGetPosition(out Vector3 pos) && state.TryGetRotation(out Quaternion rot))
                    {
                        var lPose = new Pose(pos, Quaternion.Euler(rot.eulerAngles + leftControllerHandRotOffset));
                        lPose = xrOrigin.transform.TransformPose(lPose);
                        var locPose = currentAvatar.transform.InverseTransformPose(lPose);
                        newPose.leftWristLocPos = locPose.position;
                        newPose.leftWristLocRot = locPose.rotation;
                        leftHandTracked = true;
                    }
                }
                else if (!rightHandTracked && state.nodeType == XRNode.RightHand)
                {
                    if (state.TryGetPosition(out Vector3 pos) && state.TryGetRotation(out Quaternion rot))
                    {
                        var rPose = new Pose(pos, Quaternion.Euler(rot.eulerAngles + rightControllerHandRotOffset));
                        rPose = xrOrigin.transform.TransformPose(rPose);
                        var locPose = currentAvatar.transform.InverseTransformPose(rPose);
                        newPose.rightWristLocPos = locPose.position;
                        newPose.rightWristLocRot = locPose.rotation;
                        rightHandTracked = true;
                    }
                }
            }
        }
        
        // Warning: Don't lastPose = newPose, because it will be updated in the update head hand pose (should be fixed later);
        //currentAvatar.UpdateFixHeadHandPose(newPose);
        // Rotation fixed in newPose, now it can be stored
        lastPose = newPose;
    }

    public void SetAvatar(AvatarAnimationControl currentAvatar)
    {
        this.currentAvatar = currentAvatar;
        //isTracking = false;
    }

    public void EnableTracking(bool enable)
    {
        isTracking = enable;
        //currentAvatar.SetAnimate(enable);
    }

    IEnumerator EnableWhenSubsystemAvailable()
    {
        yield return new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>() != null);
    }

    public HeadHandPose GetBodyPose()
    {
        return lastPose;
    }
}
