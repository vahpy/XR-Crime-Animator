using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrackManager.BodyTracking
{
    [Serializable]
    public class FullBodyPose
    {
        [SerializeField] public Transform locCenterTransform;

        // pose in center transform coordination
        [SerializeField] public Vector3 head;
        [SerializeField] public Quaternion headDir;

        // Spine
        [SerializeField] public Vector3 spineBase;
        [SerializeField] public Vector3 spineMid;
        [SerializeField] public Vector3 spineShoulder;
        [SerializeField] public Quaternion spineDir;

        // right arm
        [SerializeField] public Vector3 rightShoulder;
        [SerializeField] public Vector3 rightElbow;
        [SerializeField] public Vector3 rightWrist;
        [SerializeField] public Vector3 rightHand;
        [SerializeField] public Quaternion rightHandDir;

        // left arm
        [SerializeField] public Vector3 leftShoulder;
        [SerializeField] public Vector3 leftElbow;
        [SerializeField] public Vector3 leftWrist;
        [SerializeField] public Vector3 leftHand;
        [SerializeField] public Quaternion leftHandDir;

        // Right leg
        [SerializeField] public Vector3 rightHip;
        [SerializeField] public Vector3 rightKnee;
        [SerializeField] public Vector3 rightAnkle;
        [SerializeField] public Vector3 rightFoot;

        // Left leg
        [SerializeField] public Vector3 leftHip;
        [SerializeField] public Vector3 leftKnee;
        [SerializeField] public Vector3 leftAnkle;
        [SerializeField] public Vector3 leftFoot;
    }
}