using TrackManager.BodyTracking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TrackManager.Animation
{
    public class AvatarAnimationControl : MonoBehaviour
    {
        private readonly float startWalkingThreshold = 0.2f; // 10 cm/sec
        private readonly float startJumpHeightThreshold = 0.3f; // 10 cm
        private readonly float startSittingThreshold = 1.2f; // 10 cm
        private readonly float startSleepingThreshold = 0.3f; // 10 cm


        [SerializeField] private Transform avatarRoot = default;
        [SerializeField] private Transform head = default;
        [SerializeField] private MultiAimConstraint headConstraint = default;
        [SerializeField] private TwoBoneIKConstraint leftWristConstraint = default;
        [SerializeField] private TwoBoneIKConstraint rightWristConstraint = default;
        [SerializeField] private float rotationSpeed = default;
        private bool animate;

        Animator animator;
        public enum AnimationState { Idle, Walking, Jumping, Sitting, Sleeping }
        public AnimationState animationState { private set; get; }
        int animationStateHash;

        private MultiAimConstraintData headSource;
        private Transform leftWristTarget;
        private Transform rightWristTarget;
        private Transform headTarget;

        private Vector3 lastAvatarRootPos;
        private float lastAvatarRootUpdateTime;
        private float lastUpdateTickTime;

        private bool updateHeadHandPoseInUpdate = false;
        private HeadHandPose lastCapturedPose;
        private bool updateAnimation;
        private AvatarAnimationControl.AnimationState lastCapturedAnim;
        void Start()
        {
            animator = GetComponent<Animator>();
            animationState = AnimationState.Idle;
            animationStateHash = Animator.StringToHash("animationState");

            //defaultAvatarRotation = avatarRoot.localRotation;

            lastAvatarRootPos = avatarRoot.position;
            headSource = headConstraint.data;
            headTarget = headSource.sourceObjects.GetTransform(0);
            leftWristTarget = leftWristConstraint.data.target;
            rightWristTarget = rightWristConstraint.data.target;
            animate = false;
            animator.enabled = false;
            lastUpdateTickTime = Time.time;
        }

        public void NewPoseData(HeadHandPose pose)
        {
            lastCapturedPose = pose;
            updateHeadHandPoseInUpdate = true;
        }

        public void NewAnimData(AnimationState state)
        {
            lastCapturedAnim = state;
            updateAnimation = true;
        }

        public void UpdateHeadHandPose(HeadHandPose pose, AnimationState? state = null)
        {
            headTarget.position = head.position + pose.headForward;

            if (pose.leftWristLocPos != Vector3.zero)
            {
                leftWristTarget.localPosition = pose.leftWristLocPos;
                leftWristTarget.localRotation = pose.leftWristLocRot;
                //leftWristTarget.Rotate(90f, 0f, 0f, Space.Self);
                pose.leftWristLocRot = leftWristTarget.localRotation;
            }
            if (pose.rightWristLocPos != Vector3.zero)
            {
                rightWristTarget.localPosition = pose.rightWristLocPos;
                rightWristTarget.localRotation = pose.rightWristLocRot;
                //rightWristTarget.Rotate(90f, 0f, 0f, Space.Self);
                pose.rightWristLocRot = rightWristTarget.localRotation;
            }
            //RotateBody(pose);
            avatarRoot.position = pose.head + avatarRoot.position - head.position;
            RunAnimation(state);
            lastAvatarRootPos = avatarRoot.position;
            lastAvatarRootUpdateTime = Time.time;
        }

        private void Update()
        {
            if (avatarRoot != null && animate && AnimationRecorder.instance.UpdateTick)
            {
                var headTargetPos = headTarget.position;
                // rotate the avatar root to face the head direction only on the xz plane
                Vector3 targetDirection = head.forward;
                targetDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                if (Quaternion.Angle(avatarRoot.rotation, targetRotation) > 5f)
                {
                    avatarRoot.rotation = Quaternion.RotateTowards(avatarRoot.rotation, targetRotation, rotationSpeed * (Time.time - lastUpdateTickTime));
                }
                else
                {
                    avatarRoot.rotation = targetRotation;
                }
                headTarget.position = headTargetPos;
                if (updateAnimation)
                {
                    UpdateHeadHandPose(lastCapturedPose, lastCapturedAnim);
                }
                else if (updateHeadHandPoseInUpdate)
                {
                    UpdateHeadHandPose(lastCapturedPose);
                }
                updateHeadHandPoseInUpdate = false;
                updateAnimation = false;
                lastUpdateTickTime = Time.time;
            }
        }

        private void RunAnimation(AnimationState? state)
        {
            if (state != null && ((AnimationState)animator.GetInteger(animationStateHash)) != state) animator.SetInteger(animationStateHash, (int)state);
            else RunAnimation();
        }

        private void RunAnimation()
        {
            if (!animate) return;

            animationState = AnimationState.Idle;
            if (avatarRoot.position.y > startJumpHeightThreshold)
            {
                animationState = AnimationState.Jumping;
            }
            else if (head.position.y < startSleepingThreshold)
            {
                animationState = AnimationState.Sleeping;
            }
            else if (head.position.y < startSittingThreshold)
            {
                animationState = AnimationState.Sitting;
            }
            else if (Vector3.Distance(avatarRoot.position, lastAvatarRootPos) / (Time.time - lastAvatarRootUpdateTime) > startWalkingThreshold)
            {
                animationState = AnimationState.Walking;
            }

            animator.SetInteger(animationStateHash, (int)animationState);
        }

        public void SetAnimate(bool enable)
        {
            if (animate != enable)
            {
                animate = enable;
                animator.enabled = enable;
                animationState = AnimationState.Idle;
            }
        }

        public string GetBonePath(Transform boneTransform)
        {
            string path = boneTransform.name;
            Transform currentTransform = boneTransform.parent;
            while (currentTransform != null && currentTransform != avatarRoot)
            {
                path = currentTransform.name + "/" + path;
                currentTransform = currentTransform.parent;
            }
            return path;
        }

        public string GetBonePath(HumanBodyBones bone)
        {
            var boneTransform = GetBoneTransform(bone);
            string path = boneTransform.name;
            Transform currentTransform = boneTransform.parent;
            while (currentTransform != null && currentTransform != avatarRoot)
            {
                path = currentTransform.name + "/" + path;
                currentTransform = currentTransform.parent;
            }
            return path;
        }

        private Transform GetBoneTransform(HumanBodyBones bone)
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator.GetBoneTransform(bone);
        }
    }
}