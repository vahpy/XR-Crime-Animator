using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackManager.Animation
{
    public class ResetState : EnvironmentInteractionState
    {
        public ResetState(EnvironmentInteractionContext context, EnviornmentInteractionStateManager.EEnvironmentInteractionState estate) : base(context, estate)
        {

        }

        public override void EnterState()
        {
            context.LeftFootIKConstraint.weight = 0.0f;
            context.LeftFootRotConstraint.weight = 0.0f;
            context.RighFootIKConstraint.weight = 0.0f;
            context.RightFootRotConstraint.weight = 0.0f;
        }

        public override void ExitState()
        {
            
        }

        public override EnviornmentInteractionStateManager.EEnvironmentInteractionState GetNextState()
        {
            return StateKey;
        }

        public override void OnTriggerEnter(Collider other)
        {

        }

        public override void OnTriggerExit(Collider other)
        {
            
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            
        }
    }
}