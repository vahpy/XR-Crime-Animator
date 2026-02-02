//using MixedReality.Toolkit.SpatialManipulation;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using static UnityEngine.GraphicsBuffer;

//namespace TrackManager.Animation
//{
//    public class FallingObjectEffect : ControlledObjectEffect
//    {
//        // recorded data
//        public SortedList<int, bool> recordedGravityFrames;
//        //public SortedList<int, Vector3> recordedLocPos;

//        // Effect properties
//        public bool applyGravity = false;
//        public bool applyPhysics = false;
//        //public bool applyAnimation = false;

//        private readonly float defaultMass = 1.0f;

//        // helper fields
//        private Rigidbody body;
//        private ObjectManipulator objManipulator;
//        private Vector3 startLocalPos;

//        private void Start()
//        {

//            recordedGravityFrames = new SortedList<int, bool>
//            {
//                { 0, applyGravity }
//            };
//        }
//        public override void PlayRelativeFrame(int relativeFrame)
//        {
//            bool gravityOn;
//            if (recordedGravityFrames.TryGetValue(relativeFrame, out gravityOn))
//            {
//                SetApplyGravity(gravityOn);
//            }
//            if (relativeFrame == 0)
//            {
//                SetLocalPosition(startLocalPos);
//            }
//        }

//        public override bool SetTarget(ControlledObject target)
//        {
//            if (target == null) return false;
//            this.target = target;
//            body = this.target.GetComponent<Rigidbody>();
//            objManipulator = this.target?.GetComponent<ObjectManipulator>();
//            startLocalPos = target.transform.localPosition;
//            if (body == null)
//            {
//                body = this.target.AddComponent<Rigidbody>();
//                body.mass = defaultMass;
//                body.useGravity = applyGravity;
//                body.isKinematic = !applyPhysics;
//                body.interpolation = RigidbodyInterpolation.Interpolate;
//                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
//            }
//            return true;
//        }

//        public override void SetTargets(ControlledObject[] targets)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override void RecordKeyFrame(int absoluteFrameNum)
//        {
//            Debug.Log(absoluteFrameNum + "," + GetSlot().startFrame + ", r:" + (absoluteFrameNum - GetSlot().startFrame));
//            int relativeFrameNum = GetRelativeFrameNum(absoluteFrameNum);
//            if (relativeFrameNum < 0 || absoluteFrameNum > GetSlot().endFrame) return;
//            if (relativeFrameNum == 0)
//            {
//                startLocalPos = target.transform.localPosition;
//            }
//            recordedGravityFrames[relativeFrameNum] = applyGravity;
//        }

//        public override void SetReferenceSpace(Transform space)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override bool SetFieldValue(string fieldName, object value)
//        {
//            switch (fieldName)
//            {
//                case "applyGravity":
//                    SetApplyGravity((bool)value);
//                    break;
//                case "applyPhysics":
//                    SetApplyPhysics((bool)value);
//                    break;
//                //case "applyAnimation":
//                //    SetApplyAnimation((bool)value);
//                //    break;
//                case "target":
//                    if (value is ControlledObject)
//                    {
//                        return SetTarget((ControlledObject)value);
//                    }
//                    else
//                    {
//                        return false;
//                    }
//            }
//            return true;
//        }
//        public void SetLocalPosition(Vector3 locPos)
//        {
//            if (target == null) return;
//            target.transform.localPosition = locPos;
//        }
//        public void SetApplyGravity(bool applyGravity)
//        {
//            var body = this.target?.GetComponent<Rigidbody>();
//            if (body == null) return;
//            this.applyGravity = applyGravity;
//            body.useGravity = applyGravity;
//            body.velocity = Vector3.zero;
//        }
//        private void SetApplyPhysics(bool value)
//        {
//            applyPhysics = value;
//            this.target.GetComponent<Rigidbody>().isKinematic = !applyPhysics;
//        }

//        public override void RecordEachFrame(int absoluteFrameNum)
//        {

//        }

//        public override bool SaveAsFile(string filePath)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override ControlledObjectEffect LoadFromFile(string filePath)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}