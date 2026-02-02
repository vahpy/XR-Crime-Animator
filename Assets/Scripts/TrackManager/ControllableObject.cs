using MixedReality.Toolkit.SpatialManipulation;
using Unity.VisualScripting;
using UnityEditor.Animations;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TrackManager.Animation
{
    [RequireComponent(typeof(Outline))]
    public class ControllableObject : MonoBehaviour
    {
        // Public and serialized fields
        [SerializeField] private bool reactToGravity = false;
        [SerializeField] private bool reactToPhysics = false;
        [SerializeField] private float originalMass = 1.0f;
        [SerializeField] private bool ignoreMovement = false;
        // private fields
        private Outline outlineUI;


        private Rigidbody body;
        private ObjectManipulator objManipulator;
        private Vector3 lastPosition = Vector3.zero;
        private Quaternion lastRotation = Quaternion.identity;
        private Vector3 lastScale = Vector3.one;

        //Animation helper fields
        private Animator animator;
        private bool isFirstFrameCurrentRecording = true;
        private int requestedEnableAnimator = 0;
        private Slot highestPriorityAnimSlot;


        public bool IsSelected
        {
            get; private set;
        }
        public bool IsHovered
        {
            get; private set;
        }
        public bool IsReactToGravity
        {
            get { return reactToGravity; }
        }
        public bool IsReactToPhysics
        {
            get { return reactToPhysics; }
        }
        public void SetReactToGravity(bool react)
        {
            reactToGravity = react;
            
        }
        public void SetReactToPhysics(bool react)
        {
            reactToPhysics = react;
            
        }

        protected virtual void Awake()
        {
            outlineUI = this.GetOrAddComponent<Outline>();
            outlineUI.OutlineMode = Outline.Mode.OutlineAll;
            outlineUI.enabled = false;
        }
        protected virtual void Start()
        {
            InitializeRigidBody();
            objManipulator = this.GetComponent<ObjectManipulator>();
            lastPosition = this.transform.localPosition;
            lastRotation = this.transform.localRotation;
            lastScale = this.transform.localScale;
        }

        private void InitializeRigidBody()
        {
            if (reactToPhysics || reactToGravity)
            {
                body = this.GetComponent<Rigidbody>();
                if (body == null)
                {
                    body = this.AddComponent<Rigidbody>();
                }
                //Debug.Log(body.gameObject.name);
                var colliders = this.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    if (collider is MeshCollider meshCollider)
                    {
                        if (!meshCollider.convex) meshCollider.convex = true;
                    }
                }

                body.isKinematic = !reactToPhysics;
                body.useGravity = reactToGravity;
                body.mass = originalMass;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        public void TriggerSelect(bool select)
        {
            IsSelected = select;
            if (IsSelected)
            {
                //outlineUI.enabled = true;
                AnimationRecorder.instance.ObjectSelected(this.gameObject);
            }
            else if (!IsHovered)
            {
                //outlineUI.enabled = false;
            }
        }

        public void OnHover(bool hover)
        {
            IsHovered = hover;
            if (IsHovered)
            {
                outlineUI.enabled = true;
            }
            else //if (!isSelected)
            {
                outlineUI.enabled = false;
            }
        }

        public void Selected(SelectEnterEventArgs args)
        {
            TriggerSelect(true);
        }

        public void HoverEntered(HoverEnterEventArgs args)
        {
            OnHover(true);
        }
        public void HoverExited(HoverExitEventArgs args)
        {
            OnHover(false);
        }

        protected virtual void Update()
        {
            if (body == null)
            {
                InitializeRigidBody();
            }
            if (objManipulator == null)
            {
                objManipulator = this.GetComponent<ObjectManipulator>();
            }
            bool overridePhysics = body != null && objManipulator != null && (objManipulator.IsGrabSelected || objManipulator.IsRaySelected);
            if (overridePhysics)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }
            else if (body != null)
            {
                body.useGravity = reactToGravity;
                body.isKinematic = !reactToPhysics;
                if (reactToGravity == false) Debug.Log("Gravity settings applied:" + reactToGravity);
            }


            CreateMovementRecorder();


            if (animator != null)
            {
                if (requestedEnableAnimator > 0)
                {
                    if (!animator.enabled) animator.enabled = true;
                    requestedEnableAnimator = 0;
                    highestPriorityAnimSlot = null;
                }
                else
                {
                    if (animator.enabled) animator.enabled = false;
                }
            }
            if (!isFirstFrameCurrentRecording && !AnimationRecorder.instance.isRecording)
            {
                isFirstFrameCurrentRecording = true; // for next time
            }
        }
        private void CreateMovementRecorder()
        {
            if (ignoreMovement) return;
            // only create ObjectMovementEffect for those that had a change in transform in current recording
            if (AnimationRecorder.instance.isRecording
                && isFirstFrameCurrentRecording
                && (lastPosition != this.transform.localPosition || lastRotation != this.transform.localRotation || lastScale != this.transform.localScale))
            {
                var currSlot = TracksManager.instance.currentSlot;
                if (currSlot != null)
                {
                    var alreadyExists = false;
                    foreach (ControllableObjectEffect eff in currSlot.Effects)
                    {
                        if (eff is ObjectMovementEffect2 && eff.target == this)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                    if (!alreadyExists)
                    {
                        var newEffectComp = currSlot.AddEffect(typeof(ObjectMovementEffect2));
                        if (newEffectComp != null)
                        {
                            ((ObjectMovementEffect2)newEffectComp).SetTarget(this);
                            ((ObjectMovementEffect2)newEffectComp).RecordKeyFrame(AnimationRecorder.instance.absoluteFrameNum);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("You should select a slot to start recording.");
                }
                isFirstFrameCurrentRecording = false;
                // update transform
                lastPosition = this.transform.localPosition;
                lastRotation = this.transform.localRotation;
                lastScale = this.transform.localScale;
            }
            else
            {
                isFirstFrameCurrentRecording = true;
            }
            if (isFirstFrameCurrentRecording)
            {
                // update transform
                lastPosition = this.transform.localPosition;
                lastRotation = this.transform.localRotation;
                lastScale = this.transform.localScale;
            }
        }

        internal Animator GetOrAddAnimator()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = this.AddComponent<Animator>();
                var controller = new AnimatorController();
                if (controller.layers.Length == 0)
                {
                    controller.AddLayer("Default");
                }
                animator.runtimeAnimatorController = controller;
            }
            return animator;
        }

        internal void RequestEnableAnimator(bool enable, Slot requestingSlot)
        {
            if (enable)
            {
                requestedEnableAnimator++;
                var siblingId = requestingSlot.transform.parent.GetSiblingIndex();
                if (highestPriorityAnimSlot == null)
                {
                    highestPriorityAnimSlot = requestingSlot;
                }
                else
                {
                    var highPrioritySiblingId = highestPriorityAnimSlot.transform.parent.GetSiblingIndex();
                    if (highPrioritySiblingId < siblingId)
                    {
                        highestPriorityAnimSlot = requestingSlot;
                    }
                }
            }
        }
    }
}
