using UnityEngine;


namespace TrackManager.Animation
{
    public abstract class EnvironmentInteractionState : BaseState<EnviornmentInteractionStateManager.EEnvironmentInteractionState>
    {
        protected EnvironmentInteractionContext context;

        public EnvironmentInteractionState(EnvironmentInteractionContext context, EnviornmentInteractionStateManager.EEnvironmentInteractionState stateKey) : base(stateKey)
        {
            this.context = context;
        }
    }
}
