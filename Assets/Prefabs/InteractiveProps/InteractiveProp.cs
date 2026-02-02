using System;
using UnityEngine;

namespace TrackManager.Animation
{
    public abstract class InteractiveProp : ControllableObject
    {
        [Serializable]
        public enum InteractionState
        {
            Idle,
            InteractionStarted,
            InteractionEnded,
            InteractionOnGoing
        }

        public bool isGrabbed { get; protected set; } = false;
        [SerializeField, HideInInspector] public IntPropInteractionEffect interactionRecorderEffect { internal set; get; }
        [SerializeField, HideInInspector] public InteractionState interactionState { get; protected set; } = InteractionState.Idle;

        public abstract void StartTriggerInteraction();
        public abstract void OnTriggeringInteraction();
        public abstract void StopTriggerInteraction();
        public void OnGrabStarted()
        {
            AnimationRecorder.instance.SetCurrentInteractiveProp(this);
            isGrabbed = true;
        }
        public void OnGrabEnded()
        {
            AnimationRecorder.instance.ClearCurrentInteractiveProp(this);
            isGrabbed = false;
        }
    }
}