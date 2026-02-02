//using Palmmedia.ReportGenerator.Core.Reporting.Builders;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Security.Cryptography;
//using TrackManager.Animation;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;

//namespace TrackManager.BodyTracking
//{
//    public class BodyTrackingEffect : ControlledObjectEffect
//    {
//        [DoNotSerialize] private VRBodyTracker bodyTracker;

//        //[DoNotSerialize] private List<HeadHandPose> temporaryBodyMoves;
//        [SerializeField] private Dictionary<int, HeadHandPose> bodyMovesStorage;
//        [SerializeField] private Dictionary<int, AvatarAnimationControl.AnimationState> animationStatesStorage;
//        //private bool pressedKeyFrame = false;
//        [DoNotSerialize] public bool captureMoves = false;
//        [DoNotSerialize] private AvatarAnimationControl avatar;

//        [DoNotSerialize] private int startFrameOffset;

//        //private int recordingStartFrame;
//        [DoNotSerialize] private AvatarAnimationControl.AnimationState lastAnimationState;


//        //Experimental
//        private AnimationClip animClip;
//        private new void Start()
//        {
//            base.Start();
//            bodyTracker = VRBodyTracker.instance;
//            captureMoves = false;
//            //pressedKeyFrame = false;
//            //temporaryBodyMoves ??= new List<HeadHandPose>();
//            //bodyMovesStorage ??= new List<HeadHandPose>();
//            bodyMovesStorage = new Dictionary<int, HeadHandPose>();
//            animationStatesStorage = new Dictionary<int, AvatarAnimationControl.AnimationState>();
//            lastAnimationState = AvatarAnimationControl.AnimationState.Jumping;
//        }

//        public override void PlayRelativeFrame(int relativeFrame)
//        {
//            if (AnimationRecorder.instance.GetCurrentEffect() == this) return; // do not play the effect if it is the same being recorded
//            if (target == null) return;
//            if (captureMoves) CaptureMoves(false);
//            if (relativeFrame > 0 && bodyMovesStorage.Count > relativeFrame)
//            {
//                if (bodyMovesStorage.TryGetValue(relativeFrame, out HeadHandPose bodyPose))
//                {
//                    if (animationStatesStorage.TryGetValue(relativeFrame, out AvatarAnimationControl.AnimationState animState))
//                    {
//                        lastAnimationState = animState;
//                        avatar.NewAnimData(lastAnimationState);
//                    }
//                    avatar.SetAnimate(true);
//                    avatar.NewPoseData(bodyPose);
//                    //avatar.UpdateHeadHandPose(bodyPose, lastAnimationState); // do not use update and fix version
//                }
//            }
//            else
//            {
//                avatar.SetAnimate(false);
//            }
//        }

//        public override void RecordKeyFrame(int absoluteFrameNum)
//        {
//            if (AnimationRecorder.instance.isRecording)
//            {
//                startFrameOffset = 0;

//                if (GetRelativeFrameNum(absoluteFrameNum) < 0)
//                {
//                    startFrameOffset = -GetRelativeFrameNum(absoluteFrameNum);
//                }
//            }
//            else
//            {
//                CaptureMoves(false);
//                // Experimental
//                //StoreAnimationClip();
//            }
//            //Debug.Log("Body Tracking Effect: Record Key Frame");
//            //if (target != null && captureMoves)
//            //{
//            //    if (!pressedKeyFrame)
//            //    {
//            //        Debug.Log("Body tracking, key pressed and storing");
//            //        pressedKeyFrame = true;
//            //        recordingStartFrame = absoluteFrameNum;
//            //        // start recording
//            //        AnimationRecorder.instance.PlayProgrammatically();
//            //    }
//            //    else
//            //    {
//            //        pressedKeyFrame = false;
//            //        AnimationRecorder.instance.PauseProgrammatically();
//            //        Debug.Log("Body tracking, key pressed and stroing permanently, removing temporary.");
//            //        // end recording
//            //        Debug.Log(temporaryBodyMoves.Count + "frames were stored in bodyMovesStorage, count: " + bodyMovesStorage.Count);
//            //        int relativeFrame = GetRelativeFrameNum(recordingStartFrame);
//            //        if (relativeFrame < 0) relativeFrame = bodyMovesStorage.Count;
//            //        bodyMovesStorage.InsertRange(relativeFrame, temporaryBodyMoves);
//            //        temporaryBodyMoves.Clear();
//            //        Debug.Log("Body moves storage count : " + bodyMovesStorage.Count);
//            //        if (GetSlot().framesCount < bodyMovesStorage.Count)
//            //        {
//            //            GetSlot().SetFrameInterval(GetSlot().startFrame, GetSlot().startFrame + bodyMovesStorage.Count);
//            //        }
//            //    }
//            //}
//            //else if (temporaryBodyMoves != null && temporaryBodyMoves.Count > 0)
//            //{
//            //    temporaryBodyMoves.Clear();
//            //    Debug.Log("temporary more than 0 elements.");
//            //}
//        }

//        public override bool SetFieldValue(string fieldName, object value)
//        {
//            switch (fieldName)
//            {
//                case "captureMoves":
//                    CaptureMoves((bool)value);
//                    break;
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

//        public override void SetReferenceSpace(Transform space)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override bool SetTarget(ControlledObject target)
//        {
//            //Check if it is a right object to be recorded
//            if (target == null || target.GetComponent<AvatarAnimationControl>() == null) return false;

//            this.target = target;
//            avatar = target.GetComponent<AvatarAnimationControl>();

//            return true;
//        }

//        public override void SetTargets(ControlledObject[] targets)
//        {
//            throw new System.NotImplementedException();
//        }

//        private void CaptureMoves(bool enable)
//        {
//            this.captureMoves = enable;
//            if (enable)
//            {
//                this.bodyTracker.SetAvatar(avatar);
//                bodyTracker.EnableTracking(true);
//                avatar.SetAnimate(true);
//            }
//            else
//            {
//                this.bodyTracker.SetAvatar(null);
//                bodyTracker.EnableTracking(false);
//                avatar.SetAnimate(false);
//                //temporaryBodyMoves.Clear();
//                //pressedKeyFrame = false;
//            }
//        }

//        //
//        //private void Update()
//        //{
//        //    if (captureMoves && pressedKeyFrame)
//        //    {
//        //        var bodyPose = bodyTracker.GetBodyPose();
//        //        if (bodyPose != null)
//        //        {
//        //            if (temporaryBodyMoves == null) temporaryBodyMoves = new List<HeadHandPose>();
//        //            temporaryBodyMoves.Add(bodyPose);
//        //        }
//        //    }
//        //}

//        private void CalcStartFrameOffset(int absoluteFrameNum)
//        {
//            if (GetRelativeFrameNum(absoluteFrameNum) < 0 && startFrameOffset <= 0)
//            {
//                startFrameOffset = -1 * GetRelativeFrameNum(absoluteFrameNum);
//            }
//        }

//        public override void RecordEachFrame(int absoluteFrameNum)
//        {
//            if (captureMoves)
//            {
//                int relativeFrame = GetRelativeFrameNum(absoluteFrameNum);
//                if (relativeFrame < 0 && startFrameOffset == 0)
//                {
//                    CalcStartFrameOffset(absoluteFrameNum);
//                }

//                var bodyPose = bodyTracker.GetBodyPose();
//                if (bodyPose != null)
//                {
//                    // should call update first, as it fixes the offset
//                    //avatar.UpdateHeadHandPose(bodyPose);
//                    avatar.NewPoseData(bodyPose);
//                    bodyMovesStorage[relativeFrame + startFrameOffset] = bodyPose;
//                }

//                if (avatar.animationState != lastAnimationState)
//                {
//                    lastAnimationState = avatar.animationState;
//                    animationStatesStorage[relativeFrame + startFrameOffset] = lastAnimationState;
//                }


//                if (GetSlot().FramesCount < bodyMovesStorage.Count)
//                {
//                    GetSlot().SetFrameInterval(GetSlot().startFrame, GetSlot().startFrame + relativeFrame + startFrameOffset);//bodyMovesStorage.Count);
//                }
//            }
//        }
//        private bool StoreAnimationClip()
//        {
//            AnimationClip animClip = ConvertToAnimationClip();

//            if (animClip == null)
//                return false;

//            string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
//            string path = $"Assets/Animations/GeneratedAnimation_{timestamp}.anim";

//            AssetDatabase.CreateAsset(animClip, path);
//            AssetDatabase.SaveAssets();

//            return true;
//        }
//        private AnimationClip ConvertToAnimationClip()
//        {
//            AnimationClip animClip = new AnimationClip();

//            if (bodyMovesStorage==null || bodyMovesStorage.Count == 0)
//                return null;

//            foreach (KeyValuePair<int, HeadHandPose> entry in bodyMovesStorage)
//            {
//                int frame = entry.Key;
//                HeadHandPose pose = entry.Value;

//                float time = frame / 30f; // assuming 30 frames per second

//                // Head position and forward direction
//                animClip.SetCurve("Head", typeof(Transform), "localPosition.x", AnimationCurve.Linear(time, pose.head.x, time, pose.head.x));
//                animClip.SetCurve("Head", typeof(Transform), "localPosition.y", AnimationCurve.Linear(time, pose.head.y, time, pose.head.y));
//                animClip.SetCurve("Head", typeof(Transform), "localPosition.z", AnimationCurve.Linear(time, pose.head.z, time, pose.head.z));

//                animClip.SetCurve("Head", typeof(Transform), "localRotation.x", AnimationCurve.Linear(time, pose.headForward.x, time, pose.headForward.x));
//                animClip.SetCurve("Head", typeof(Transform), "localRotation.y", AnimationCurve.Linear(time, pose.headForward.y, time, pose.headForward.y));
//                animClip.SetCurve("Head", typeof(Transform), "localRotation.z", AnimationCurve.Linear(time, pose.headForward.z, time, pose.headForward.z));

//                // Left wrist position and rotation
//                string leftWristPath = avatar.GetBonePath(HumanBodyBones.LeftHand);
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localPosition.x", AnimationCurve.Linear(time, pose.leftWristLocPos.x, time, pose.leftWristLocPos.x));
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localPosition.y", AnimationCurve.Linear(time, pose.leftWristLocPos.y, time, pose.leftWristLocPos.y));
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localPosition.z", AnimationCurve.Linear(time, pose.leftWristLocPos.z, time, pose.leftWristLocPos.z));

//                animClip.SetCurve(leftWristPath, typeof(Transform), "localRotation.x", AnimationCurve.Linear(time, pose.leftWristLocRot.x, time, pose.leftWristLocRot.x));
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localRotation.y", AnimationCurve.Linear(time, pose.leftWristLocRot.y, time, pose.leftWristLocRot.y));
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localRotation.z", AnimationCurve.Linear(time, pose.leftWristLocRot.z, time, pose.leftWristLocRot.z));
//                animClip.SetCurve(leftWristPath, typeof(Transform), "localRotation.w", AnimationCurve.Linear(time, pose.leftWristLocRot.w, time, pose.leftWristLocRot.w));

//                // Right wrist position and rotation
//                string rightWristPath = avatar.GetBonePath(HumanBodyBones.RightHand);
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localPosition.x", AnimationCurve.Linear(time, pose.rightWristLocPos.x, time, pose.rightWristLocPos.x));
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localPosition.y", AnimationCurve.Linear(time, pose.rightWristLocPos.y, time, pose.rightWristLocPos.y));
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localPosition.z", AnimationCurve.Linear(time, pose.rightWristLocPos.z, time, pose.rightWristLocPos.z));

//                animClip.SetCurve(rightWristPath, typeof(Transform), "localRotation.x", AnimationCurve.Linear(time, pose.rightWristLocRot.x, time, pose.rightWristLocRot.x));
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localRotation.y", AnimationCurve.Linear(time, pose.rightWristLocRot.y, time, pose.rightWristLocRot.y));
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localRotation.z", AnimationCurve.Linear(time, pose.rightWristLocRot.z, time, pose.rightWristLocRot.z));
//                animClip.SetCurve(rightWristPath, typeof(Transform), "localRotation.w", AnimationCurve.Linear(time, pose.rightWristLocRot.w, time, pose.rightWristLocRot.w));
//            }

//            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClip);
//            settings.loopTime = true;
//            AnimationUtility.SetAnimationClipSettings(animClip, settings);

//            return animClip;
//        }

//        public override bool SaveAsFile(string filePath)
//        {
//            using (FileStream filestream = File.Create(filePath))
//            {
//                BinaryFormatter formatter = new BinaryFormatter();
//                byte[] bytes;
//                using (MemoryStream stream = new MemoryStream())
//                {
//                    formatter.Serialize(stream, this);
//                    bytes = stream.ToArray();
//                }
//                filestream.Write(bytes, 0, bytes.Length);
//            }
//            return true;
//        }

//        public override ControlledObjectEffect LoadFromFile(string filePath)
//        {
//            using (FileStream filestream = File.OpenRead(filePath))
//            {
//                byte[] bytes = new byte[filestream.Length];
//                BinaryFormatter formatter = new BinaryFormatter();
//                using (MemoryStream stream = new MemoryStream(bytes))
//                {
//                    return (BodyTrackingEffect)formatter.Deserialize(stream);
//                }
//            }
//        }
//    }
//}