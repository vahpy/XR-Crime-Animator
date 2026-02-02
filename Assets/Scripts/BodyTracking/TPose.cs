//using LightBuzz.Kinect4Azure;
//using System.Collections.Generic;
//using TrackManager.BodyTracking;
//using UnityEngine;

//namespace TrackManager.BodyTracking
//{
//    internal static class TPose
//    {
//        //
//        // Summary:
//        //     Sets model to T pose.
//        //
//        // Parameters:
//        //   bones:
//        public static void DoTPose(Dictionary<HumanBodyBones, Bone> bones)
//        {
//            if (bones.ContainsKey(AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ClavicleLeft]))
//            {
//                SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ClavicleLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ShoulderLeft]].Transform.position, left: true);
//            }

//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ShoulderLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowLeft]].Transform.position, left: true);
//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.WristLeft]].Transform.position, left: true);
//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.WristLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowLeft]].Transform, left: true);
//            SetVerticalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.HipLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.KneeLeft]].Transform.position);
//            SetVerticalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.KneeLeft]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.AnkleLeft]].Transform.position);
//            if (bones.ContainsKey(AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ClavicleRight]))
//            {
//                SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ClavicleRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ShoulderRight]].Transform.position, left: false);
//            }

//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ShoulderRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowRight]].Transform.position, left: false);
//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.WristRight]].Transform.position, left: false);
//            SetHorizontalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.WristRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.ElbowRight]].Transform, left: false);
//            SetVerticalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.HipRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.KneeRight]].Transform.position);
//            SetVerticalJointTPose(bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.KneeRight]].Transform, bones[AvateeringConstants.LightBuzzCrossAnimatorBones[JointType.AnkleRight]].Transform.position);
//        }

//        internal static void SetHorizontalJointTPose(Transform joint1, Vector3 joint2Position, bool left)
//        {
//            ForceHorizontalOrientation(joint1, joint2Position, left);
//            ForceDepthOrientation(joint1, joint2Position, left);
//        }

//        internal static void SetHorizontalJointTPose(Transform joint1, Transform inverseExtensionJoint, bool left)
//        {
//            SetHorizontalJointTPose(joint1, joint1.position + (joint1.position - inverseExtensionJoint.position).normalized, left);
//        }

//        private static void ForceHorizontalOrientation(Transform joint1, Vector3 joint2Position, bool left)
//        {
//            Vector3 right = joint1.right;
//            Vector3 up = joint1.up;
//            Vector3 forward = joint1.forward;
//            Vector3 normalized = (joint2Position - joint1.position).normalized;
//            Vector3 bestAxis = GetBestAxis(right, up, forward, left ? Vector3.back : Vector3.forward);
//            float num = Vector3.Angle(new Vector3(normalized.x, normalized.y, 0f), left ? Vector3.left : Vector3.right);
//            if (Vector3.Dot(new Vector3(normalized.x, normalized.y, 0f), Vector3.up) > 0f)
//            {
//                num = 0f - num;
//            }

//            Vector3 eulerAngles = joint1.rotation.eulerAngles;
//            joint1.localRotation *= UnityEngine.Quaternion.Euler(bestAxis.x * num, bestAxis.y * num, bestAxis.z * num);
//            Vector3 eulerAngles2 = joint1.rotation.eulerAngles;
//            Vector3 vector = new Vector3(GetAngleOffset(eulerAngles.x, eulerAngles2.x), GetAngleOffset(eulerAngles.y, eulerAngles2.y), GetAngleOffset(eulerAngles.z, eulerAngles2.z));
//            if (vector.x > 90f)
//            {
//                eulerAngles2.x = eulerAngles.x;
//            }

//            if (vector.y > 90f)
//            {
//                eulerAngles2.y = eulerAngles.y;
//            }

//            if (vector.z > 90f)
//            {
//                eulerAngles2.z = eulerAngles.z;
//            }

//            joint1.eulerAngles = eulerAngles2;
//        }

//        private static void ForceDepthOrientation(Transform joint1, Vector3 joint2Position, bool left)
//        {
//            Vector3 right = joint1.right;
//            Vector3 up = joint1.up;
//            Vector3 forward = joint1.forward;
//            Vector3 normalized = (joint2Position - joint1.position).normalized;
//            Vector3 bestAxis = GetBestAxis(right, up, forward, normalized);
//            Vector3 bestAxis2 = GetBestAxis(right, up, forward, left ? Vector3.back : Vector3.forward);
//            Vector3 vector = Vector3.Cross(bestAxis, bestAxis2);
//            float num = Vector3.Angle(new Vector3(normalized.x, 0f, normalized.z), left ? Vector3.left : Vector3.right);
//            if (Vector3.Dot(new Vector3(normalized.x, 0f, normalized.z), Vector3.forward) > 0f)
//            {
//                num = 0f - num;
//            }

//            if (left)
//            {
//                num = 0f - num;
//            }

//            Vector3 eulerAngles = joint1.rotation.eulerAngles;
//            joint1.localRotation *= UnityEngine.Quaternion.Euler(vector.x * num, vector.y * num, vector.z * num);
//            Vector3 eulerAngles2 = joint1.eulerAngles;
//            Vector3 vector2 = new Vector3(GetAngleOffset(eulerAngles.x, eulerAngles2.x), GetAngleOffset(eulerAngles.y, eulerAngles2.y), GetAngleOffset(eulerAngles.z, eulerAngles2.z));
//            if (vector2.x > 180f)
//            {
//                eulerAngles2.x = eulerAngles.x;
//            }

//            if (vector2.y > 180f)
//            {
//                eulerAngles2.y = eulerAngles.y;
//            }

//            if (vector2.z > 180f)
//            {
//                eulerAngles2.z = eulerAngles.z;
//            }

//            joint1.eulerAngles = eulerAngles2;
//        }

//        internal static void SetVerticalJointTPose(Transform joint1, Vector3 joint2Position)
//        {
//            Vector3 right = joint1.right;
//            Vector3 up = joint1.up;
//            Vector3 forward = joint1.forward;
//            Vector3 normalized = (joint2Position - joint1.position).normalized;
//            Vector3 bestAxis = GetBestAxis(right, up, forward, Vector3.forward);
//            float num = Vector3.Angle(new Vector3(normalized.x, normalized.y, 0f), Vector3.down);
//            if (Vector3.Dot(new Vector3(normalized.x, normalized.y, 0f), Vector3.right) > 0f)
//            {
//                num = 0f - num;
//            }

//            joint1.localRotation *= UnityEngine.Quaternion.Euler(bestAxis.x * num, bestAxis.y * num, bestAxis.z * num);
//        }

//        public static Vector3 GetBestAxis(Vector3 vX, Vector3 vY, Vector3 vZ, Vector3 value)
//        {
//            Vector3 result = new Vector3(Vector3.Dot(vX, value), Vector3.Dot(vY, value), Vector3.Dot(vZ, value));
//            if (Mathf.Abs(result.x) < Mathf.Abs(result.y))
//            {
//                result.x = 0f;
//            }
//            else
//            {
//                result.y = 0f;
//            }

//            if (Mathf.Abs(result.x) < Mathf.Abs(result.z))
//            {
//                result.x = 0f;
//            }
//            else
//            {
//                result.z = 0f;
//            }

//            if (Mathf.Abs(result.y) < Mathf.Abs(result.z))
//            {
//                result.y = 0f;
//            }
//            else
//            {
//                result.z = 0f;
//            }

//            if (Mathf.Abs(result.x) > 0f)
//            {
//                result.x = ((result.x > 0f) ? 1 : (-1));
//            }

//            if (Mathf.Abs(result.y) > 0f)
//            {
//                result.y = ((result.y > 0f) ? 1 : (-1));
//            }

//            if (Mathf.Abs(result.z) > 0f)
//            {
//                result.z = ((result.z > 0f) ? 1 : (-1));
//            }

//            return result;
//        }

//        private static Vector3 RoundEuler(Vector3 euler)
//        {
//            euler.Set(FilterAngle(euler.x), FilterAngle(euler.y), FilterAngle(euler.z));
//            return euler;
//        }

//        private static float FilterAngle(float angle)
//        {
//            return (!(angle < 45f)) ? ((angle < 135f) ? 90 : ((angle < 225f) ? 180 : ((angle < 315f) ? 270 : 0))) : 0;
//        }

//        private static float GetAngleOffset(float angle1, float angle2)
//        {
//            if (Mathf.Abs(angle1 - angle2) > 180f)
//            {
//                if (angle1 > 180f)
//                {
//                    if (angle2 > 180f)
//                    {
//                        return Mathf.Abs(360f - angle1 - (360f - angle2));
//                    }

//                    return Mathf.Abs(360f - angle1 - angle2);
//                }

//                if (angle2 > 180f)
//                {
//                    return Mathf.Abs(angle1 - (360f - angle2));
//                }

//                return Mathf.Abs(angle1 - angle2);
//            }

//            return Mathf.Abs(angle1 - angle2);
//        }
//    }
//}