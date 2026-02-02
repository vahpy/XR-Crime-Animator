using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace TrackManager.Animation
{
    public class ObjectMovementEffect2 : EffectsWithAnimation
    {
        private GameObjectRecorder recorder;
        public new static string effectTypeName => "Rigid Transform";
        private int lastRecordedFrame = -1;
        private void LateUpdate()
        {
            if (target == null) return;
            if (AnimationRecorder.instance.isRecording) // not to rewrite on previous animation
            {
                var curRelFrame = GetRelativeFrameNum(AnimationRecorder.instance.absoluteFrameNum);
                CaptureNewChanges(curRelFrame);
            }
            else if (lastRecordedFrame != -1 && animClip == null) // save the recording
            {
                OnStopRecording();
            }
        }
        public override void RecordKeyFrame(int absoluteFrameNum)
        {
            if (GetSlot() != TracksManager.instance.currentSlot) return;
            CaptureNewChanges(GetRelativeFrameNum(absoluteFrameNum));
        }

        public override bool SetFieldValue(string fieldName, object value)
        {
            switch (fieldName)
            {
                case "target":
                    if (value is ControllableObject)
                    {
                        SetTarget((ControllableObject)value);
                    }
                    else
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        private AnimationClip SaveAnimation()
        {
            lastRecordedFrame = -1;
            if (recorder == null || animClip != null) return null;
            var tempAnimClip = new AnimationClip();
            recorder.SaveToClip(tempAnimClip, AnimationRecorder.instance.framePerSecond);
            AssetDatabase.CreateAsset(tempAnimClip, AnimationRecorder.instance.GetUniqueClipPath());
            return tempAnimClip;
        }
        internal virtual void CaptureNewChanges(int currentRelativeFrame)
        {
            if (target == null || animClip != null) return;
            if (recorder == null)
            {
                recorder = new GameObjectRecorder(target.gameObject);
                recorder.BindComponentsOfType<Transform>(target.gameObject, false);
            }


            if (currentRelativeFrame > lastRecordedFrame)
            {
                float timePassed;
                if (lastRecordedFrame == -1)
                {
                    lastRecordedFrame = 0;
                    recorder.TakeSnapshot(0);
                    OnStartRecording();
                }

                timePassed = ((float)(currentRelativeFrame - lastRecordedFrame)) / AnimationRecorder.instance.framePerSecond;

                recorder.TakeSnapshot(timePassed);

                lastRecordedFrame = currentRelativeFrame;
            }
        }

        internal override void OnStopRecording()
        {
            base.OnStopRecording();
            animClip = SaveAnimation();
            AssignAnimationToTarget();
            AdjustSlotDuration(animClip.length);
        }

        public override string GetEffectName()
        {
            return effectTypeName;
        }
    }
}