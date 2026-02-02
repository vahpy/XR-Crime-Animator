//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.XR;
//using Unity.VisualScripting;
//using LightBuzz.Kinect4Azure;
//using System;
//using MixedReality.Toolkit;
//using Microsoft.MixedReality.OpenXR;
//using MixedReality.Toolkit.Input;
//using UnityEngine.SubsystemsImplementation.Extensions;
//using MixedReality.Toolkit.Subsystems;
//using System.Collections;
//using System.Linq;
//using System.ComponentModel.Design.Serialization;
//using Microsoft.Azure.Kinect.BodyTracking;
//using Unity.XR.CoreUtils;
//using LightBuzz;

//namespace TrackManager.BodyTracking
//{
//    public class BodyTracker : MonoBehaviour
//    {
//        enum TrackingMode
//        {
//            Kinect,
//            VR,
//            Both
//        }
//        [SerializeField] TrackingMode trackingMode = default;
//        [SerializeField] private Configuration _configuration;
//        //[SerializeField] private UniformImage _image;
//        //[SerializeField] private StickmanManager _stickmanManager;
//        //[SerializeField] private Vector3 _headsetPositionOffset = Vector3.one;
//        [SerializeField] private Avatar[] _avatars;
//        [SerializeField] private GameObject avatarRootHead;

//        Dictionary<JointType, Joint> defaultJoints;
//        private KinectSensor sensor;
//        private Camera VRCamera;

//        private MRTKHandsAggregatorSubsystem handSubsystem;
//        [DoNotSerialize] public Dictionary<JointType, Joint> lastPose { get; private set; }
//        // counts the number of frames passed after updating the last pose variable
//        [DoNotSerialize] public int lastPoseAge { get; private set; }
//        private void Start()
//        {
//            if (XRSettings.enabled && XRSettings.isDeviceActive)
//            {
//                //inVR = true;
//            }
//            else
//            {
//                //inVR = false;

//                if (trackingMode == TrackingMode.VR || trackingMode == TrackingMode.Both)
//                {
//                    Debug.LogWarning("Tracking mode is changed to simulation mode not actual VR.");
//                    //trackingMode = TrackingMode.Kinect;
//                }
//            }

//            VRCamera = Camera.main;

//            if (trackingMode != TrackingMode.VR)
//            {
//                sensor = KinectSensor.Create(_configuration);

//                if (sensor == null)
//                {
//                    Debug.LogError("Sensor not connected!");
//                    return;
//                }

//                sensor.Open();
//            }
//            else
//            {
//                handSubsystem = (MRTKHandsAggregatorSubsystem)XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

//                //handSubsystem = new MRTKHandsAggregatorSubsystem();
//                StartCoroutine(EnableWhenSubsystemAvailable());
//            }

//            var av = _avatars.FirstOrDefault().AvatarRoot.transform;
//            defaultJoints = new()
//            {
//                { JointType.Head, new Joint(Utils.FindDeepChild(av, "Head").position) },
//                { JointType.Neck, new Joint(Utils.FindDeepChild(av, "Neck").position) },
//                { JointType.ShoulderLeft, new Joint(Utils.FindDeepChild(av, "ShoulderLeft").position) },
//                { JointType.ShoulderRight, new Joint(Utils.FindDeepChild(av, "ShoulderRight").position) },
//                { JointType.ElbowLeft, new Joint(Utils.FindDeepChild(av, "ElbowLeft").position) },
//                { JointType.ElbowRight, new Joint(Utils.FindDeepChild(av, "ElbowRight").position) },
//                { JointType.WristLeft, new Joint(Utils.FindDeepChild(av, "WristLeft").position) },
//                { JointType.WristRight, new Joint(Utils.FindDeepChild(av, "WristRight").position) },
//                { JointType.HandLeft, new Joint(Utils.FindDeepChild(av, "HandLeft").position) },
//                { JointType.HandRight, new Joint(Utils.FindDeepChild(av, "HandRight").position) },
//                { JointType.HipLeft, new Joint(Utils.FindDeepChild(av, "HipLeft").position) },
//                { JointType.HipRight, new Joint(Utils.FindDeepChild(av, "HipRight").position) },
//                { JointType.KneeLeft, new Joint(Utils.FindDeepChild(av, "KneeLeft").position) },
//                { JointType.KneeRight, new Joint(Utils.FindDeepChild(av, "KneeRight").position) },
//                { JointType.AnkleLeft, new Joint(Utils.FindDeepChild(av, "AnkleLeft").position) },
//                { JointType.AnkleRight, new Joint(Utils.FindDeepChild(av, "AnkleRight").position) },
//                { JointType.FootLeft, new Joint(Utils.FindDeepChild(av, "FootLeft").position) },
//                { JointType.FootRight, new Joint(Utils.FindDeepChild(av, "FootRight").position) },

//                { JointType.Nose, new Joint(Utils.FindDeepChild(av, "Head").position) },
//                { JointType.Pelvis, new Joint((Utils.FindDeepChild(av, "HipRight").position+Utils.FindDeepChild(av, "HipLeft").position)/2.0f) },
//                { JointType.ThumbLeft, new Joint(Utils.FindDeepChild(av, "HandLeft").position) },
//                { JointType.ThumbRight, new Joint(Utils.FindDeepChild(av, "HandRight").position) },
//                { JointType.SpineNaval, new Joint(Utils.FindDeepChild(av, "SpineBase").position) },
//                { JointType.SpineChest, new Joint(Utils.FindDeepChild(av, "SpineMid").position) }
//            };
//        }

//        IEnumerator EnableWhenSubsystemAvailable()
//        {
//            yield return new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>() != null);
//        }

//        private void Update()
//        {
//            if (trackingMode == TrackingMode.Both)
//            {
//                Debug.LogWarning("Not supported yet.");
//                return;
//            }
//            lastPoseAge++;
//            if (trackingMode == TrackingMode.Kinect || trackingMode == TrackingMode.Both)
//            {
//                UpdateKinect();
//            }
//            if (trackingMode == TrackingMode.VR || trackingMode == TrackingMode.Both)
//            {
//                UpdateVR();
//            }
//        }

//        void UpdateKinect()
//        {
//            if (sensor == null || !sensor.IsOpen) return;

//            LightBuzz.Kinect4Azure.Frame frame = sensor.Update();

//            if (frame != null)
//            {
//                if (frame.BodyFrameSource != null)
//                {
//                    UpdateAvatarsKinect(frame.BodyFrameSource.Bodies);
//                }
//            }
//        }

//        void UpdateVR()
//        {
//            if (!handSubsystem.running)
//            {
//                Debug.LogError("Hand subsystem not working!");
//            }
//            var ps = handSubsystem.GetProvider();
//            if (ps == null)
//            {
//                Debug.LogError("provider null");
//            }
//            //else
//            //{
//            //    Debug.LogError("provider "+ps.running);
//            //}

//            Body body = new Body();
            

//            Vector3D pos = VRCamera.transform.position;
            
//            var defaultHeadPos = defaultJoints[JointType.Head].Position;
//            foreach (var joint in defaultJoints)
//            {
//                var distanceVector = joint.Value.Position - defaultHeadPos;
//                body.Joints.Add(joint.Key, new Joint(joint.Key, pos + distanceVector));
//            }

//            // VR tracked joints
//            body.Joints[JointType.Head].Position = pos;
//            if (handSubsystem.TryGetJoint(TrackedHandJoint.Wrist, XRNode.LeftHand, out HandJointPose pose))
//            {
//                //body.Joints[JointType.WristLeft].Position = pose.Position;
//            }

//            if (handSubsystem.TryGetJoint(TrackedHandJoint.Wrist, XRNode.RightHand, out pose))
//            {
//                //body.Joints[JointType.WristRight].Position = pose.Position;
//            }

//            UpdateAvatarsVR(body);
//        }

//        private void OnDestroy()
//        {
//            handSubsystem?.Stop();
//            sensor?.Close();
//        }

//        //public void DoTPose()
//        //{
//        //    foreach (Avatar item in _avatars)
//        //    {
//        //        item.DoTPose();
//        //    }
//        //}

//        private void UpdateAvatarsVR(Body body)
//        {
//            if (body == null) return;
//            lastPose = body.Joints;
//            lastPoseAge = 0;

//            foreach (Avatar avatar in _avatars)
//            {
//                avatar.Update(body);
//            }
//        }

//        private void UpdateAvatarsKinect(IList<LightBuzz.Kinect4Azure.Body> bodies)
//        {
//            if (bodies == null || bodies.Count == 0) return;
//            if (_avatars == null || _avatars.Length == 0) return;

//            LightBuzz.Kinect4Azure.Body lzbody = bodies.Closest();
//            if (lzbody == null || lzbody.Joints == null) return;

//            // convert LightBuzz to My namespace (to be able to edit internal values)
//            Body body = new Body(lzbody);


//            // all joints are transfered into camera coordinate
//            TransformPoints(body.Joints);

//            // for recording purposes
//            lastPose = body.Joints;
//            lastPoseAge = 0;
//            //
//            foreach (Avatar avatar in _avatars)
//            {
//                avatar.Update(body);

//                avatar.PositionAt(avatar.AvatarRoot.transform.position - (avatarRootHead.transform.position - VRCamera.transform.position));

//            }
//        }

//        private void TransformPoints(Dictionary<JointType, Joint> joints)
//        {
//            var offset = joints[JointType.Head].Position;

//            foreach (var joint in joints)
//            {
//                joint.Value.Position -= offset;
//            }

//            var orientVector = joints[JointType.Head].Orientation * Vector3.forward;
//            Debug.Log(orientVector);
//            var rotMat = UnityEngine.Quaternion.FromToRotation(orientVector, VRCamera.transform.forward);
//            rotMat.Normalize();

//            foreach (var joint in joints)
//            {
//                joint.Value.Position = rotMat * joint.Value.Position;
//                joint.Value.Position += offset;
//            }
//        }
//    }
//}