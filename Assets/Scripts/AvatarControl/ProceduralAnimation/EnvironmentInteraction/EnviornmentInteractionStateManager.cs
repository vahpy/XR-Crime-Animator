using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

namespace TrackManager.Animation
{
    public class EnviornmentInteractionStateManager : StateManager<EnviornmentInteractionStateManager.EEnvironmentInteractionState>
    {
        public enum EEnvironmentInteractionState
        {
            Search,
            Approach,
            Rise,
            Touch,
            Reset
        }

        private EnvironmentInteractionContext context;

        [SerializeField] private TwoBoneIKConstraint leftFootIKConstraint;
        [SerializeField] private TwoBoneIKConstraint righFootIKConstraint;
        [SerializeField] private MultiRotationConstraint leftFootRotConstraint;
        [SerializeField] private MultiRotationConstraint rightFootRotConstraint;
        [SerializeField] private BoxCollider leftFootCollider;
        [SerializeField] private BoxCollider rightFootCollider;
        //[SerializeField] private Rigidbody _rigidbody;
        //[SerializeField] private CapsuleCollider capsuleCollider;

        private void Awake()
        {
            ValidateConstraints();

            context = new EnvironmentInteractionContext(leftFootIKConstraint, righFootIKConstraint, leftFootRotConstraint, rightFootRotConstraint, leftFootCollider, rightFootCollider);
            InitializeStates();
        }
        private void ValidateConstraints()
        {
            // assert all fields
            Assert.IsNotNull(leftFootIKConstraint, nameof(leftFootIKConstraint) + " is not assigned.");
            Assert.IsNotNull(righFootIKConstraint, nameof(righFootIKConstraint) + " is not assigned.");
            Assert.IsNotNull(leftFootRotConstraint, nameof(leftFootRotConstraint) + " is not assigned.");
            Assert.IsNotNull(rightFootRotConstraint, nameof(rightFootRotConstraint) + " is not assigned.");
            Assert.IsNotNull(leftFootCollider, nameof(leftFootCollider) + " is not assigned.");
            Assert.IsNotNull(rightFootCollider, nameof(rightFootCollider) + " is not assigned.");
            //Assert.IsNotNull(_rigidbody, nameof(_rigidbody) + " is not assigned.");
            //Assert.IsNotNull(capsuleCollider, nameof(capsuleCollider) + " is not assigned.");

        }

        private void InitializeStates()
        {
            States.Add(EEnvironmentInteractionState.Reset, new ResetState(context, EEnvironmentInteractionState.Reset));
            States.Add(EEnvironmentInteractionState.Touch, new TouchState(context, EEnvironmentInteractionState.Touch));
            States.Add(EEnvironmentInteractionState.Rise, new RiseState(context, EEnvironmentInteractionState.Rise));
            States.Add(EEnvironmentInteractionState.Approach, new ApproachState(context, EEnvironmentInteractionState.Approach));
            States.Add(EEnvironmentInteractionState.Search, new SearchState(context, EEnvironmentInteractionState.Search));
            CurrentState = States[EEnvironmentInteractionState.Reset];
        }
    }
}

