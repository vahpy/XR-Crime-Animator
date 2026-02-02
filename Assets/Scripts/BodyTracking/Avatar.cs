//using LightBuzz;
//using LightBuzz.Kinect4Azure;
//using LightBuzz.Kinect4Azure.Avateering;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Joint = LightBuzz.Kinect4Azure.Joint;
//using Quaternion = LightBuzz.Quaternion;
namespace TrackManager.BodyTracking
{
    ////
    //// Summary:
    ////     Represents an animatable avatar.
    //[Serializable]
    //public class Avatar
    //{
    //    private readonly Dictionary<HumanBodyBones, Bone> _bones = new Dictionary<HumanBodyBones, Bone>(AvateeringConstants.LightBuzzCrossAnimatorBones.Count);

    //    private JointType _highestHierarchyBone;

    //    private Jump _jump;

    //    private bool _isJumping;

    //    private bool _isInitialized;

    //    private Vector3 _originalLocalPosition;

    //    private bool _calculatedScale;

    //    [Header("Options")]
    //    [Tooltip("The 3D model root object.")]
    //    [SerializeField]
    //    private GameObject _avatarRoot;

    //    [Tooltip("Specifies whether the avatar should update its pose.")]
    //    [SerializeField]
    //    private bool _updatePose = true;

    //    [Tooltip("Scales the model to match the scale of the body.")]
    //    [SerializeField]
    //    private bool _matchScale;

    //    [Tooltip("Specifies whether the avatar will move in the 3D world space.")]
    //    [SerializeField]
    //    private bool _useWorldPosition = true;

    //    [Tooltip("The motion smoothing factor (0.0 - 1.0).")]
    //    [Range(0f, 1f)]
    //    [SerializeField]
    //    private float _smoothDelta = 0.4f;

    //    [Header("Update individual joints")]
    //    [SerializeField]
    //    private bool _updateHead = true;

    //    [SerializeField]
    //    private bool _updateNeck = true;

    //    [SerializeField]
    //    private bool _updateChest = true;

    //    [SerializeField]
    //    private bool _updateSpine = true;

    //    [SerializeField]
    //    private bool _updatePelvis = true;

    //    [SerializeField]
    //    private bool _updateShoulderRight = true;

    //    [SerializeField]
    //    private bool _updateShoulderLeft = true;

    //    [SerializeField]
    //    private bool _updateElbowRight = true;

    //    [SerializeField]
    //    private bool _updateElbowLeft = true;

    //    [SerializeField]
    //    private bool _updateWristRight;

    //    [SerializeField]
    //    private bool _updateWristLeft;

    //    [SerializeField]
    //    private bool _updateHipRight = true;

    //    [SerializeField]
    //    private bool _updateHipLeft = true;

    //    [SerializeField]
    //    private bool _updateKneeRight = true;

    //    [SerializeField]
    //    private bool _updateKneeLeft = true;

    //    [SerializeField]
    //    private bool _updateAnkleRight = true;

    //    [SerializeField]
    //    private bool _updateAnkleLeft = true;

    //    //
    //    // Summary:
    //    //     Updates the head joint.
    //    public bool UpdateHead
    //    {
    //        get
    //        {
    //            return _updateHead;
    //        }
    //        set
    //        {
    //            _updateHead = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the neck joint.
    //    public bool UpdateNeck
    //    {
    //        get
    //        {
    //            return _updateNeck;
    //        }
    //        set
    //        {
    //            _updateNeck = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the chest joint.
    //    public bool UpdateChest
    //    {
    //        get
    //        {
    //            return _updateChest;
    //        }
    //        set
    //        {
    //            _updateChest = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the spine joint.
    //    public bool UpdateSpine
    //    {
    //        get
    //        {
    //            return _updateSpine;
    //        }
    //        set
    //        {
    //            _updateSpine = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the pelvis joint.
    //    public bool UpdatePelvis
    //    {
    //        get
    //        {
    //            return _updatePelvis;
    //        }
    //        set
    //        {
    //            _updatePelvis = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left shoulder joint.
    //    public bool UpdateShoulderLeft
    //    {
    //        get
    //        {
    //            return _updateShoulderLeft;
    //        }
    //        set
    //        {
    //            _updateShoulderLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right shoulder joint.
    //    public bool UpdateShoulderRight
    //    {
    //        get
    //        {
    //            return _updateShoulderRight;
    //        }
    //        set
    //        {
    //            _updateShoulderRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left elbow joint.
    //    public bool UpdateElbowLeft
    //    {
    //        get
    //        {
    //            return _updateElbowLeft;
    //        }
    //        set
    //        {
    //            _updateElbowLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right elbow joint.
    //    public bool UpdateElbowRight
    //    {
    //        get
    //        {
    //            return _updateElbowRight;
    //        }
    //        set
    //        {
    //            _updateElbowRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left wrist joint.
    //    public bool UpdateWristLeft
    //    {
    //        get
    //        {
    //            return _updateWristLeft;
    //        }
    //        set
    //        {
    //            _updateWristLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right wrist joint.
    //    public bool UpdateWristRight
    //    {
    //        get
    //        {
    //            return _updateWristRight;
    //        }
    //        set
    //        {
    //            _updateWristRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left hip joint.
    //    public bool UpdateHipLeft
    //    {
    //        get
    //        {
    //            return _updateHipLeft;
    //        }
    //        set
    //        {
    //            _updateHipLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right hip joint.
    //    public bool UpdateHipRight
    //    {
    //        get
    //        {
    //            return _updateHipRight;
    //        }
    //        set
    //        {
    //            _updateHipRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left knee joint.
    //    public bool UpdateKneeLEft
    //    {
    //        get
    //        {
    //            return _updateKneeLeft;
    //        }
    //        set
    //        {
    //            _updateKneeLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right knee joint.
    //    public bool UpdateKneeRight
    //    {
    //        get
    //        {
    //            return _updateKneeRight;
    //        }
    //        set
    //        {
    //            _updateKneeRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the left ankle joint.
    //    public bool UpdateAnkleLeft
    //    {
    //        get
    //        {
    //            return _updateAnkleLeft;
    //        }
    //        set
    //        {
    //            _updateAnkleLeft = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Updates the right ankle joint.
    //    public bool UpdateAnkleRight
    //    {
    //        get
    //        {
    //            return _updateAnkleRight;
    //        }
    //        set
    //        {
    //            _updateAnkleRight = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     The 3D model root object.
    //    public GameObject AvatarRoot
    //    {
    //        get
    //        {
    //            return _avatarRoot;
    //        }
    //        set
    //        {
    //            _avatarRoot = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Specifies whether the avatar should update its pose.
    //    public bool UpdatePose
    //    {
    //        get
    //        {
    //            return _updatePose;
    //        }
    //        set
    //        {
    //            _updatePose = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Scales the model to match the scale of the body.
    //    public bool MatchScale
    //    {
    //        get
    //        {
    //            return _matchScale;
    //        }
    //        set
    //        {
    //            _matchScale = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Specifies whether the avatar will move in the 3D world space.
    //    public bool UseWorldPosition
    //    {
    //        get
    //        {
    //            return _useWorldPosition;
    //        }
    //        set
    //        {
    //            _useWorldPosition = value;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     The motion smoothing factor (0.0 - 1.0).
    //    public float SmoothDelta
    //    {
    //        get
    //        {
    //            return _smoothDelta;
    //        }
    //        set
    //        {
    //            _smoothDelta = Mathf.Clamp(value, 0f, 1f);
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Assigns a new root GameObject for the avatar. Caches animator bones for rotation.
    //    //
    //    // Parameters:
    //    //   newAvatarRoot:
    //    //     The new root GameObject from the avatar to use.
    //    public void Initialize(GameObject newAvatarRoot)
    //    {
    //        _avatarRoot = newAvatarRoot;
    //        Initialize();
    //    }

    //    //
    //    // Summary:
    //    //     Caches animator bones for rotation.
    //    public void Initialize()
    //    {
    //        _isInitialized = false;
    //        Animator component = _avatarRoot.GetComponent<Animator>();
    //        if (component == null || !component.isHuman)
    //        {
    //            return;
    //        }

    //        _bones.Clear();
    //        foreach (KeyValuePair<JointType, HumanBodyBones> lightBuzzCrossAnimatorBone in AvateeringConstants.LightBuzzCrossAnimatorBones)
    //        {
    //            Transform boneTransform = component.GetBoneTransform(lightBuzzCrossAnimatorBone.Value);
    //            if (boneTransform != null)
    //            {
    //                _bones.Add(lightBuzzCrossAnimatorBone.Value, new Bone(boneTransform));
    //            }
    //        }

    //        _highestHierarchyBone = FindHighestHieararchyBone();
    //        _jump = new Jump();
    //        _avatarRoot.transform.rotation = Quaternion.Identity;
    //        TPose.DoTPose(_bones);
    //        foreach (KeyValuePair<HumanBodyBones, Bone> bone in _bones)
    //        {
    //            bone.Value.CalibrateOriginalRotation();
    //        }

    //        _originalLocalPosition = _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.localPosition;
    //        _isInitialized = true;
    //    }

    //    //
    //    // Summary:
    //    //     Forces model to T-Pose.
    //    public void DoTPose()
    //    {
    //        if (!_isInitialized)
    //        {
    //            Initialize();
    //        }

    //        TPose.DoTPose(_bones);
    //    }

    //    private JointType GetJoint(JointType original)
    //    {
    //        return original;
    //    }

    //    //
    //    // Summary:
    //    //     Calculations are done by finding a normal, a destination and the offset to the
    //    //     original rotation. The normal defines the UP vector in order to rotate the joints
    //    //     based on an axis. The destination is where the joint should "look at". The hard
    //    //     coded offset of each joint rotates the joint to the correct place from its original
    //    //     rotation from the T-Pose.
    //    //
    //    // Parameters:
    //    //   body:
    //    protected void CalculateOrientations(Body body)
    //    {
    //        Vector3D position = body.Joints[GetJoint(JointType.Head)].Position;
    //        Vector3D position2 = body.Joints[GetJoint(JointType.Nose)].Position;
    //        Vector3D position3 = body.Joints[GetJoint(JointType.Neck)].Position;
    //        Vector3D position4 = body.Joints[GetJoint(JointType.Pelvis)].Position;
    //        Vector3D position5 = body.Joints[GetJoint(JointType.SpineNaval)].Position;
    //        Vector3D position6 = body.Joints[GetJoint(JointType.ShoulderLeft)].Position;
    //        Vector3D position7 = body.Joints[GetJoint(JointType.ShoulderRight)].Position;
    //        Vector3D position8 = body.Joints[GetJoint(JointType.ElbowLeft)].Position;
    //        Vector3D position9 = body.Joints[GetJoint(JointType.ElbowRight)].Position;
    //        Vector3D position10 = body.Joints[GetJoint(JointType.WristLeft)].Position;
    //        Vector3D position11 = body.Joints[GetJoint(JointType.WristRight)].Position;
    //        Vector3D position12 = body.Joints[GetJoint(JointType.HandLeft)].Position;
    //        Vector3D position13 = body.Joints[GetJoint(JointType.HandRight)].Position;
    //        Vector3D position14 = body.Joints[GetJoint(JointType.ThumbLeft)].Position;
    //        Vector3D position15 = body.Joints[GetJoint(JointType.ThumbRight)].Position;
    //        Vector3D position16 = body.Joints[GetJoint(JointType.HipLeft)].Position;
    //        Vector3D position17 = body.Joints[GetJoint(JointType.HipRight)].Position;
    //        Vector3D position18 = body.Joints[GetJoint(JointType.KneeLeft)].Position;
    //        Vector3D position19 = body.Joints[GetJoint(JointType.KneeRight)].Position;
    //        Vector3D position20 = body.Joints[GetJoint(JointType.AnkleLeft)].Position;
    //        Vector3D position21 = body.Joints[GetJoint(JointType.AnkleRight)].Position;
    //        Vector3D position22 = body.Joints[GetJoint(JointType.FootLeft)].Position;
    //        Vector3D position23 = body.Joints[GetJoint(JointType.FootRight)].Position;
    //        Vector3D vector3D = Vector3D.Lerp(position6, position7, 0.5f);
    //        Vector3D normalized = Vector3D.Normal(position4, vector3D, position7).Normalized;
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation((vector3D - position5).Normalized, normalized) * Quaternion.Euler(90f, 0f, 180f), jointType: GetJoint(JointType.SpineNaval), update: _updateSpine);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position7, position6), normalized) * Quaternion.Euler(90f, 0f, -90f), jointType: GetJoint(JointType.SpineChest), update: _updateChest);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position, position3), normalized) * Quaternion.Euler(90f, 0f, 180f), jointType: GetJoint(JointType.Neck), update: _updateNeck);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(up: GetNormal(position3, position, position2), forward: GetDirection(position2, position3)) * Quaternion.Euler(0f, 220f, 90f), jointType: GetJoint(JointType.Head), update: _updateHead);
    //        normalized = GetNormal(position7, position9, position11);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position9, position7), normalized) * Quaternion.Euler(180f, 90f, 0f), jointType: GetJoint(JointType.ShoulderRight), update: _updateShoulderRight);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position11, position9), normalized) * Quaternion.Euler(180f, 90f, 0f), jointType: GetJoint(JointType.ElbowRight), update: _updateElbowRight);
    //        normalized = GetNormal(position15, position11, position13);
    //        Vector3D direction = GetDirection(position13, position11);
    //        UpdateJointRotation(JointType.WristRight, Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(180f, 90f, 0f), _updateWristRight);
    //        normalized = GetNormal(position6, position8, position10);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position8, position6), normalized) * Quaternion.Euler(0f, -90f, 0f), jointType: GetJoint(JointType.ShoulderLeft), update: _updateShoulderLeft);
    //        UpdateJointRotation(newRotation: Quaternion.LookRotation(GetDirection(position10, position8), normalized) * Quaternion.Euler(0f, -90f, 0f), jointType: GetJoint(JointType.ElbowLeft), update: _updateElbowLeft);
    //        normalized = GetNormal(position14, position10, position12);
    //        direction = GetDirection(position12, position10);
    //        UpdateJointRotation(JointType.WristLeft, Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, -90f, 0f), _updateWristLeft);
    //        normalized = GetNormal(position5, position4, position17);
    //        direction = GetDirection(position5, position4);
    //        UpdateJointRotation(GetJoint(JointType.Pelvis), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(-90f, 0f, 0f), _updatePelvis);
    //        normalized = (((position23 - position21).Length < (position21 - position19).Length) ? GetNormal(position23, position21, position19) : GetNormal(position17, position19, position21));
    //        direction = GetDirection(position19, position17);
    //        UpdateJointRotation(GetJoint(JointType.HipRight), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 90f, 90f), _updateHipRight);
    //        direction = GetDirection(position21, position19);
    //        UpdateJointRotation(GetJoint(JointType.KneeRight), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 90f, 90f), _updateKneeRight);
    //        direction = GetDirection(position23, position21);
    //        UpdateJointRotation(GetJoint(JointType.AnkleRight), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 150f, 90f), _updateAnkleRight);
    //        normalized = (((position22 - position20).Length < (position20 - position18).Length) ? GetNormal(position22, position20, position18) : GetNormal(position16, position18, position20));
    //        direction = GetDirection(position18, position16);
    //        UpdateJointRotation(GetJoint(JointType.HipLeft), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 90f, 90f), _updateHipLeft);
    //        direction = GetDirection(position20, position18);
    //        UpdateJointRotation(GetJoint(JointType.KneeLeft), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 90f, 90f), _updateKneeLeft);
    //        direction = GetDirection(position22, position20);
    //        UpdateJointRotation(GetJoint(JointType.AnkleLeft), Quaternion.LookRotation(direction, normalized) * Quaternion.Euler(0f, 150f, 90f), _updateAnkleLeft);
    //    }

    //    //
    //    // Summary:
    //    //     Calculates and applies the scale to the avatar to meet the body.
    //    //
    //    // Parameters:
    //    //   body:
    //    //     The body object.
    //    protected void CalculateScale(Body body)
    //    {
    //        if (!_calculatedScale)
    //        {
    //            Joint joint = body.Joints[JointType.ElbowRight];
    //            Joint joint2 = body.Joints[JointType.ShoulderRight];
    //            if (joint.TrackingState != 0 && joint2.TrackingState != 0)
    //            {
    //                Vector3 vector = joint.Position - joint2.Position;
    //                Bone bone = GetBone(HumanBodyBones.RightLowerArm);
    //                Bone bone2 = GetBone(HumanBodyBones.RightUpperArm);
    //                Vector3 vector2 = bone.OriginalPosition - bone2.OriginalPosition;
    //                float newScale = vector.magnitude / vector2.magnitude;
    //                ApplyScaleAtBody(newScale);
    //                _calculatedScale = true;
    //            }
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Finds and returns the bone of the Animator.
    //    //
    //    // Parameters:
    //    //   humanBodyBone:
    //    public Bone GetBone(HumanBodyBones humanBodyBone)
    //    {
    //        if (_bones == null)
    //        {
    //            return null;
    //        }

    //        if (_bones.ContainsKey(humanBodyBone))
    //        {
    //            return _bones[humanBodyBone];
    //        }

    //        return null;
    //    }

    //    private static Vector3D GetDirection(Vector3D to, Vector3D from)
    //    {
    //        return (to - from).Normalized;
    //    }

    //    private static Vector3D GetNormal(Vector3D j1, Vector3D j2, Vector3D j3)
    //    {
    //        return Vector3D.Normal(j1, j2, j3).Normalized;
    //    }

    //    private void UpdateJointRotation(JointType jointType, UnityEngine.Quaternion newRotation, bool update)
    //    {
    //        if (update && AvateeringConstants.LightBuzzCrossAnimatorBones.ContainsKey(jointType))
    //        {
    //            HumanBodyBones key = AvateeringConstants.LightBuzzCrossAnimatorBones[jointType];
    //            if (_bones.ContainsKey(key))
    //            {
    //                _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[jointType]].UpdateRotation(newRotation, _smoothDelta);
    //            }
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Initializes the avatar if it has not been initialized. Updates joints orientation
    //    //     and position.
    //    //
    //    // Parameters:
    //    //   body:
    //    //     The body data to feed the avatar with.
    //    //
    //    //   floor:
    //    //     The floor data.
    //    public void Update(Body body, Floor floor = null)
    //    {
    //        if (_updatePose)
    //        {
    //            if (!_isInitialized)
    //            {
    //                Initialize();
    //            }

    //            CalculateOrientations(body);
    //            if (_matchScale)
    //            {
    //                CalculateScale(body);
    //            }

    //            UpdateJump(body, floor);
    //            UpdateRootPosition(body);
    //            _avatarRoot.transform.rotation = UnityEngine.Quaternion.Euler(0f, 180f, 0f);
    //        }
    //    }

    //    private void UpdateRootPosition(Body body)
    //    {
    //        if (body == null)
    //        {
    //            return;
    //        }

    //        if (_useWorldPosition)
    //        {
    //            Vector3D position = body.Joints[_highestHierarchyBone].Position;
    //            _avatarRoot.transform.position = position * -1f;
    //            return;
    //        }

    //        Vector3 originalLocalPosition = _originalLocalPosition;
    //        if (_jump.IsJumping)
    //        {
    //            originalLocalPosition.y += _jump.JumpHeight;
    //            _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.localPosition = originalLocalPosition;
    //        }
    //        else
    //        {
    //            float num = _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.position.y - _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_jump.LowestJoint.JointType]].Transform.position.y;
    //            num = (originalLocalPosition.y = num + 0.1f);
    //            _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.localPosition = originalLocalPosition;
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Calculates jump and crouch.
    //    //
    //    // Parameters:
    //    //   body:
    //    //
    //    //   floor:
    //    protected void UpdateJump(Body body, Floor floor)
    //    {
    //        _jump.Update(body, floor);
    //        if (floor != null)
    //        {
    //            if (_jump.IsJumping)
    //            {
    //                _isJumping = true;
    //            }
    //            else if (_isJumping)
    //            {
    //                _isJumping = false;
    //            }
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Sets the position of the highest hierarchy bone at the position, thus moving
    //    //     the whole avatar.
    //    //
    //    // Parameters:
    //    //   point:
    //    //     The new position of the root bone.
    //    //
    //    // Returns:
    //    //     True if the bone was positioned. False if it failed.
    //    public bool PositionBonesAtPoint(Vector3 point)
    //    {
    //        if (_bones == null)
    //        {
    //            return false;
    //        }

    //        _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.position = point;
    //        return true;
    //    }

    //    //
    //    // Summary:
    //    //     Sets the position of the avatar at point.
    //    //
    //    // Parameters:
    //    //   point:
    //    //     The new position of the avatar.
    //    public void PositionAt(Vector3 point)
    //    {
    //        _avatarRoot.transform.position = point;
    //    }

    //    //
    //    // Summary:
    //    //     Applies scale to the Avatar Root.
    //    //
    //    // Parameters:
    //    //   newScale:
    //    //     The scale factor to apply.
    //    public void ApplyScaleAtBody(float newScale)
    //    {
    //        _avatarRoot.transform.localScale = new Vector3(newScale, newScale, newScale);
    //    }

    //    //
    //    // Summary:
    //    //     Applies scale to the highest hierarchy bone.
    //    //
    //    // Parameters:
    //    //   newScale:
    //    //     The scale factor to apply.
    //    public void ApplyScaleAtBones(float newScale)
    //    {
    //        if (_bones != null)
    //        {
    //            _bones[AvateeringConstants.LightBuzzCrossAnimatorBones[_highestHierarchyBone]].Transform.localScale = new Vector3(newScale, newScale, newScale);
    //        }
    //    }

    //    //
    //    // Summary:
    //    //     Center of mass will be used to calculate offset.
    //    protected JointType FindHighestHieararchyBone()
    //    {
    //        HumanBodyBones humanBodyBones = HumanBodyBones.Hips;
    //        int num = int.MaxValue;
    //        foreach (KeyValuePair<HumanBodyBones, Bone> bone in _bones)
    //        {
    //            int num2 = CountParents(bone.Value.Transform);
    //            if (num2 < num)
    //            {
    //                humanBodyBones = bone.Key;
    //                num = num2;
    //            }
    //        }

    //        JointType result = JointType.Pelvis;
    //        foreach (KeyValuePair<JointType, HumanBodyBones> lightBuzzCrossAnimatorBone in AvateeringConstants.LightBuzzCrossAnimatorBones)
    //        {
    //            if (lightBuzzCrossAnimatorBone.Value == humanBodyBones)
    //            {
    //                return lightBuzzCrossAnimatorBone.Key;
    //            }
    //        }

    //        return result;
    //    }

    //    //
    //    // Summary:
    //    //     Counts how many parents a transform has.
    //    //
    //    // Parameters:
    //    //   transform:
    //    private int CountParents(Transform transform)
    //    {
    //        int num = 0;
    //        Transform transform2 = transform;
    //        while (transform2.parent != null)
    //        {
    //            transform2 = transform2.parent;
    //            num++;
    //        }

    //        return num;
    //    }
    //}
}