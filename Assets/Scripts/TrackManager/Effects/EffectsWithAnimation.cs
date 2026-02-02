using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace TrackManager.Animation
{
    public abstract class EffectsWithAnimation : ControllableObjectEffect
    {
        // helper fields
        private bool isPlayingAnimation = false;
        private AnimatorController controller;
        internal bool RecordedDefaulPose = false;
        public new static string effectTypeName => "Animted Transform";
        internal Animator animator
        {
            get
            {
                return target.GetOrAddAnimator();
            }
        }

        // data fields
        [SerializeField] internal AnimationClip animClip;
        internal Vector3 initialLocalPosition = Vector3.zero;
        internal Quaternion initialLocalRotation = Quaternion.identity;
        internal Vector3 initialLocalScale = Vector3.one;
        internal Vector3 initialWorldPosition = Vector3.zero;
        internal Quaternion initialWorldRotation = Quaternion.identity;



        internal bool PlayAnimationFrom(int relFrame)
        {
            isPlayingAnimation = true;
            if (target == null)
            {
                return false;
            }

            ApplyOneAnimationFrame(relFrame);
            animator.speed = 1;
            return true;
        }
        public override void PlayRelativeFrame(int relativeFrame)
        {
            if (!isPlayingAnimation
                && !AnimationRecorder.instance.isPlaying && !AnimationRecorder.instance.isRecording)
            {
                ApplyOneAnimationFrame(relativeFrame);
            }
        }
        internal void ApplyOneAnimationFrame(int relFrame)
        {
            if (target == null || animClip == null || animator == null || controller == null) return;
            
            if (this is MetaBodyTrackingEffect)
            {
                target.transform.parent.SetPositionAndRotation(initialWorldPosition, initialWorldRotation);
            }

            target.transform.localPosition = initialLocalPosition;
            target.transform.localRotation = initialLocalRotation;
            target.transform.localScale = initialLocalScale;


            float normTime = Mathf.Clamp01(((float)relFrame) / AnimationRecorder.instance.framePerSecond / animClip.length);
            animator.Play(animClip.name, 0, normTime);
            animator.speed = 0;
        }

        internal bool PauseAnimationClip()
        {
            if (animator == null) return false;
            animator.speed = 0;
            return true;
        }

        internal void AssignAnimationToTarget()
        {
            if (target == null || animClip == null) return;

            controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null || controller.name.Length == 0) // // By default, there is always an unnamed controller. We can identify it using this information.
            {
                controller = new AnimatorController
                {
                    name = target.name + "_AnimControl"
                };
                controller.AddLayer("Default");
                animator.runtimeAnimatorController = controller;
            }
            animator.speed = 0;

            AnimatorState state;
            foreach (var st in controller.layers[0].stateMachine.states)
            {
                if (st.state.name == animClip.name)
                {
                    // state with the same name found, so just set the motion
                    st.state.motion = animClip;
                    return;
                }
            }
            // no state with the same name found
            state = controller.layers[0].stateMachine.AddState(animClip.name);
            state.motion = animClip;
        }
        public void LoadAnimationClip(string path)
        {
            if (path != null && path.Length > 0)
            {
                animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                AssignAnimationToTarget();
                if (animClip!) AdjustSlotDuration(animClip.length);
            }
        }

        public string GetAnimClipPath()
        {
            if (animClip == null) return null;
            return AssetDatabase.GetAssetPath(animClip);
        }

        internal void Update()
        {
            if (target == null) return;

            // If it is in slot, animator should take control if there is a animClip
            if (animClip != null && IsInSlot())
                target.RequestEnableAnimator(true, GetSlot());


            // If in slot and playing, continuous play
            var relFrame = GetRelativeFrameNum(AnimationRecorder.instance.absoluteFrameNum);
            if ((AnimationRecorder.instance.isPlaying
                || AnimationRecorder.instance.isRecording)
                && IsInSlot())
            {
                if (!isPlayingAnimation)
                    isPlayingAnimation = PlayAnimationFrom(relFrame);
            }
            else if (isPlayingAnimation)
            {
                PauseAnimationClip();
                isPlayingAnimation = false;
            }
        }

        public override bool SetTarget(ControllableObject target)
        {
            if (target == null) return false;
            this.target = target;
            animator.speed = 0;

            return true;
        }
        public override void RecordEachFrame(int absoluteFrameNum)
        {

        }
        internal virtual void OnStartRecording()
        {
            if (target == null) return;
            if (!RecordedDefaulPose)
            {
                RecordedDefaulPose = true;
                initialLocalPosition = target.transform.localPosition;
                initialLocalRotation = target.transform.localRotation;
                initialLocalScale = target.transform.localScale;
            }
        }
        internal virtual void OnStopRecording()
        {
            RecordedDefaulPose = false;
        }
    }
}
