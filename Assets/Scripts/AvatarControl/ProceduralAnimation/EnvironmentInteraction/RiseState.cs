using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackManager.Animation
{
    public class RiseState : EnvironmentInteractionState
    {
        public RiseState(EnvironmentInteractionContext context, EnviornmentInteractionStateManager.EEnvironmentInteractionState estate):base(context, estate)
        {
            EnvironmentInteractionContext Context = context;
        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
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