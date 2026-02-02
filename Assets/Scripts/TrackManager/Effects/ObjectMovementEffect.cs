//using System.Collections.Generic;
//using TrackManager.Animation;
//using UnityEditor.XR;
//using UnityEngine;

//namespace TrackManager.Animation
//{
//    public class ObjectMovementEffect : ControlledObjectEffect
//    {
//        public bool reactToGravity;
//        public bool reactToPhysics;
//        private ControlledObject[] targets;

//        private Dictionary<int, Vector3> positionDic;
//        private Dictionary<int, Quaternion> rotationDic;
//        private Dictionary<int, Vector3> scaleDic;
//        private Dictionary<int, bool> gravityDic;
//        private Dictionary<int, bool> physicsDic;

//        private List<int> sortedRecordedFrames;

//        protected void Awake()
//        {
//            sortedRecordedFrames = new List<int>();
//            positionDic = new Dictionary<int, Vector3>();
//            rotationDic = new Dictionary<int, Quaternion>();
//            scaleDic = new Dictionary<int, Vector3>();
//            gravityDic = new Dictionary<int, bool>();
//            physicsDic = new Dictionary<int, bool>();
//        }
//        protected new void Start()
//        {
//            base.Start();
//        }
//        public override bool SetTarget(ControlledObject target)
//        {
//            if (target == null) return false;
//            this.target = target;
//            reactToGravity = this.target.IsReactToGravity;
//            reactToPhysics = this.target.IsReactToPhysics;
//            StoreFrame(0);
//            return true;
//        }
//        public override void PlayRelativeFrame(int relativeFrame)
//        {
//            if (sortedRecordedFrames.Contains(relativeFrame))
//            {
//                PlayExactFrame(relativeFrame);
//            }
//            else
//            {
//                var (lower, upper) = Utils.FindClosestNumbers(sortedRecordedFrames, relativeFrame);
//                PlayInterpolatedFrame(lower, upper, relativeFrame);
//            }
//        }

//        protected void PlayInterpolatedFrame(int? lowerFrame, int? upperFrame, int frame)
//        {
//            if (!lowerFrame.HasValue)
//            {
//                if (upperFrame.HasValue) PlayRelativeFrame((int)upperFrame);
//            }
//            if (!upperFrame.HasValue)
//            {
//                if (lowerFrame.HasValue) PlayRelativeFrame((int)lowerFrame);
//            }
//            if (lowerFrame.HasValue && upperFrame.HasValue)
//            {
//                Vector3 output1, output2;
//                Quaternion rotOutput1, rotOutput2;
//                var lerpV = (float)(frame - lowerFrame.Value) / (upperFrame.Value - lowerFrame.Value);
//                positionDic.TryGetValue(lowerFrame.Value, out output1);
//                positionDic.TryGetValue(upperFrame.Value, out output2);
//                target.transform.localPosition = Vector3.Slerp(output1, output2, lerpV);

//                rotationDic.TryGetValue(lowerFrame.Value, out rotOutput1);
//                rotationDic.TryGetValue(upperFrame.Value, out rotOutput2);
//                target.transform.localRotation = Quaternion.Slerp(rotOutput1, rotOutput2, lerpV);

//                scaleDic.TryGetValue(lowerFrame.Value, out output1);
//                scaleDic.TryGetValue(upperFrame.Value, out output2);
//                target.transform.localScale = Vector3.Slerp(output1, output2, lerpV);

//                if (gravityDic.TryGetValue(lowerFrame.Value, out bool gravity1))
//                {
//                    ApplyReactToGravity(gravity1);
//                }
//                else if (gravityDic.TryGetValue(upperFrame.Value, out bool gravity2))
//                {
//                    ApplyReactToGravity(gravity2);
//                }


//                if (physicsDic.TryGetValue(lowerFrame.Value, out bool physics1))
//                {
//                    ApplyReactToPhysics(physics1);
//                }
//                else if (physicsDic.TryGetValue(upperFrame.Value, out bool physics2))
//                {
//                    ApplyReactToPhysics(physics2);
//                }
//            }
//            return;
//        }

//        protected void PlayExactFrame(int relativeFrame)
//        {
//            Vector3 output;
//            Quaternion rotOutput;

//            if (positionDic.TryGetValue(relativeFrame, out output))
//            {
//                target.transform.localPosition = output;
//            }
//            if (rotationDic.TryGetValue(relativeFrame, out rotOutput))
//            {
//                target.transform.localRotation = rotOutput;
//            }
//            if (scaleDic.TryGetValue(relativeFrame, out output))
//            {
//                target.transform.localScale = output;
//            }
//            if (gravityDic.TryGetValue(relativeFrame, out bool gravity))
//            {
//                ApplyReactToGravity(gravity);
//            }
//            if (physicsDic.TryGetValue(relativeFrame, out bool physics))
//            {
//                ApplyReactToPhysics(physics);
//            }
//        }

//        public void StoreFrame(int relativeFrame)
//        {
//            if (!sortedRecordedFrames.Contains(relativeFrame))
//            {
//                sortedRecordedFrames.Add(relativeFrame);
//                sortedRecordedFrames.Sort();
//            }
//            positionDic[relativeFrame] = target.transform.localPosition;
//            rotationDic[relativeFrame] = target.transform.localRotation;
//            scaleDic[relativeFrame] = target.transform.localScale;
//            gravityDic[relativeFrame] = reactToGravity;
//            physicsDic[relativeFrame] = reactToPhysics;
//        }

//        public override void SetTargets(ControlledObject[] targets)
//        {
//            this.targets = targets;
//        }

//        private void ApplyReactToGravity(bool react)
//        {
//            reactToGravity = react;
//            if (this.target != null)
//            {
//                this.target.SetReactToGravity(react);
//            }
//        }
//        private void ApplyReactToPhysics(bool react)
//        {
//            reactToPhysics = react;
//            if (this.target != null)
//            {
//                this.target.SetReactToPhysics(react);
//            }
//        }

//        public override void RecordKeyFrame(int relativeFrame)
//        {
//            StoreFrame(relativeFrame);
//        }

//        public override void SetReferenceSpace(Transform space)
//        {
//            throw new System.NotImplementedException();
//        }
//        public override bool SetFieldValue(string fieldName, object value)
//        {
//            switch (fieldName)
//            {
//                case "target":
//                    if (value is ControlledObject)
//                    {
//                        SetTarget((ControlledObject)value);
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                    break;
//                case "reactToGravity":
//                    if (value is bool)
//                    {
//                        ApplyReactToGravity((bool)value);
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                    break;
//                case "reactToPhysics":
//                    if (value is bool)
//                    {
//                        ApplyReactToPhysics((bool)value);
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                    break;
//            }
//            return true;
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