using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachObjectToHand : MonoBehaviour
{
    [SerializeField] private Transform rightHandBone;
    [SerializeField] private Transform leftHandBone;

    private bool isFollowing = false;
    private Transform follower;
    private Transform targetHand;

    private Vector3 localPositionOffset;
    private Quaternion localRotationOffset;

    private void LateUpdate()
    {
        if (!isFollowing || targetHand == null || follower == null || !AnimationRecorder.instance.isRecording)
            return;

        follower.SetPositionAndRotation(targetHand.TransformPoint(localPositionOffset), targetHand.rotation * localRotationOffset);
    }

    public void GrabStarted(SelectEnterEventArgs args)
    {
        if (args.interactableObject == null)
            return;

        isFollowing = true;
        follower = args.interactableObject.transform;

        targetHand = GetClosestHand(follower.position);

        localPositionOffset = targetHand.InverseTransformPoint(follower.position);
        localRotationOffset = Quaternion.Inverse(targetHand.rotation) * follower.rotation;
    }

    public void GrabEnded(SelectExitEventArgs args)
    {
        isFollowing = false;
        follower = null;
        targetHand = null;
    }


    private Transform GetClosestHand(Vector3 position)
    {
        float distanceToRight = Vector3.SqrMagnitude(rightHandBone.position - position);
        float distanceToLeft = Vector3.SqrMagnitude(leftHandBone.position - position);

        return distanceToRight <= distanceToLeft ? rightHandBone : leftHandBone;
    }

    private void OnValidate()
    {
        if (rightHandBone == null)
            Debug.LogWarning("Right Hand Bone is not assigned.", this);

        if (leftHandBone == null)
            Debug.LogWarning("Left Hand Bone is not assigned.", this);
    }
}
