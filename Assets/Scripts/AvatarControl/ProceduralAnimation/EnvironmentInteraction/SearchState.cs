using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackManager.Animation
{
    public class SearchState : EnvironmentInteractionState
    {
        public SearchState(EnvironmentInteractionContext context, EnviornmentInteractionStateManager.EEnvironmentInteractionState estate):base(context, estate)
        {
            EnvironmentInteractionContext Context = context;
        }

        public override void EnterState()
        {
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