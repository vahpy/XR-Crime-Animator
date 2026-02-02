using System.Collections.Generic;
using UnityEngine;
using static TrackManager.Animation.InteractiveProp;

namespace TrackManager.Animation
{
    public class IntPropInteractionEffect : ObjectMovementEffect2
    {
        [SerializeField] private Dictionary<int, InteractionState> recordedInteractionStates; // only records change states
        private InteractionState lastRecordedInteractionState;
        public new static string effectTypeName => "Interactive Transform";
        public override void PlayRelativeFrame(int relativeFrame)
        {
            base.PlayRelativeFrame(relativeFrame);
            if (recordedInteractionStates == null) return;
            var state = InteractionState.Idle;
            if (relativeFrame < 0)
            {
                state = recordedInteractionStates[0];
            }
            else if (recordedInteractionStates.TryGetValue(relativeFrame, out InteractionState s))
            {
                state = s;
            }
            switch (state)
            {
                case InteractionState.InteractionStarted:
                    (this.target as InteractiveProp).StartTriggerInteraction();
                    break;
                case InteractionState.InteractionEnded:
                    (this.target as InteractiveProp).StopTriggerInteraction();
                    break;
                case InteractionState.InteractionOnGoing:
                    (this.target as InteractiveProp).OnTriggeringInteraction();
                    break;
            }
        }

        public override bool SetTarget(ControllableObject target)
        {
            if (target is not InteractiveProp) return false;
            base.SetTarget(target);
            return true;
        }

        internal override void CaptureNewChanges(int currentRelativeFrame)
        {
            //Debug.Log("CaptureNewChanges - IntPropEffect");
            base.CaptureNewChanges(currentRelativeFrame); // records animations (transform)
            if (recordedInteractionStates == null)
            {
                recordedInteractionStates = new Dictionary<int, InteractionState>
                {
                    { 0, InteractionState.Idle }
                };
                lastRecordedInteractionState = InteractionState.Idle;
            }

            if (((InteractiveProp)target).interactionState != lastRecordedInteractionState)
            {
                lastRecordedInteractionState = ((InteractiveProp)target).interactionState;
                recordedInteractionStates.Add(currentRelativeFrame, lastRecordedInteractionState);
            }
        }

        public Dictionary<int, InteractionState> GetRecordedInteractionStates()
        {
            return recordedInteractionStates;
        }

        public void LoadInteractionStates(Dictionary<int, InteractionState> interactionStates)
        {
            if (interactionStates == null) return;
            if (this.target == null)
            {
                Debug.Log("LoadInteractionStates function should be called after SetTarget");
                return;
            }
            recordedInteractionStates = interactionStates;
        }

        public override string GetEffectName()
        {
            return effectTypeName;
        }
    }
}