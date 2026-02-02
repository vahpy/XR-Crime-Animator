using System;
using UnityEngine;

namespace TrackManager.BodyTracking
{
    [Serializable]
    public class HeadHandPose
    {
        [SerializeField] public Vector3 head;
        [SerializeField] public Vector3 headForward;

        [SerializeField] public Vector3 leftWristLocPos;
        [SerializeField] public Vector3 rightWristLocPos;

        [SerializeField] public Quaternion leftWristLocRot;
        [SerializeField] public Quaternion rightWristLocRot;

        public HeadHandPose(Vector3 head, Vector3 headForward)
        {
            this.head = head;
            this.headForward = headForward;
            this.leftWristLocPos = Vector3.zero;
            this.rightWristLocPos = Vector3.zero;
        }
    }
}