
using System;
using TrackManager.BodyTracking.Meta;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

namespace TrackManager.Animation
{
    public class MetaBodyTrackingEffect : EffectsWithAnimation
    {
        public new static string effectTypeName => "Full Body Tracking";
        public override void RecordKeyFrame(int absoluteFrameNum)
        {
            // initialise and start recording
            var currEff = AnimationRecorder.instance.GetCurrentEffect();
            if (target == null || currEff != this) return;


            if (AnimationRecorder.instance.isRecording)
            {
                var tr = BodyTrackingManager.Instance.GetTrackedAvatarTransform();
                target.transform.localPosition = tr.localPosition;
                target.transform.localRotation = tr.localRotation;
                target.transform.localScale = tr.localScale; 
                OnStartRecording();
            }
            else
            {
                // stop recording and close all initialised parameters
                OnStopRecording();
            }

        }
        internal override void OnStopRecording()
        {
            base.OnStopRecording();
            target.gameObject.SetActive(true);
            animClip = BodyTrackingManager.Instance.StopRecordingAndExport();
            AssignAnimationToTarget();
            AdjustSlotDuration(animClip.length);
        }

        internal override void OnStartRecording()
        {
            if (target == null) return;
            base.OnStartRecording();

            initialWorldPosition = BodyTrackingManager.Instance.transform.position;
            initialWorldRotation = BodyTrackingManager.Instance.transform.rotation;

            target.gameObject.SetActive(false);
            //var timeOffset = GetRelativeFrameNum(AnimationRecorder.instance.absoluteFrameNum) /
            //((float)AnimationRecorder.instance.framePerSecond);
            BodyTrackingManager.Instance.StartRecording(this);
        }

        public override bool SetFieldValue(string fieldName, object value)
        {
            switch (fieldName)
            {
                case "target":
                    if (value is ControllableObject)
                        return SetTarget((ControllableObject)value);
                    break;
            }
            return false;
        }

        public override string GetEffectName()
        {
            return effectTypeName;
        }
    }
}