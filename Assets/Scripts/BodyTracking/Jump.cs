//using LightBuzz.Kinect4Azure;
//using LightBuzz;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace TrackManager.BodyTracking
//{
//    public class Jump
//    {
//        //
//        // Summary:
//        //     Offset for the lowest joint from the floor (represents the minimum height of
//        //     the joint).
//        public const float Tolerance = 0.1f;

//        //
//        // Summary:
//        //     True if the body is jumping. False if not.
//        public bool IsJumping { get; protected set; }

//        //
//        // Summary:
//        //     Jump height in meters.
//        public float JumpHeight { get; protected set; }

//        //
//        // Summary:
//        //     Indicates the lowest joint in the body.
//        public Joint LowestJoint { get; protected set; }

//        //
//        // Summary:
//        //     Finds if the body has jump and how much. Updates the variable LowestJoint.
//        //
//        // Parameters:
//        //   body:
//        //     The body to calculate if it jumped.
//        //
//        //   floor:
//        //     The floor to calculate the jump.
//        //
//        // Returns:
//        //     True if the body is jumping. False if not.
//        public void Update(Body body, Floor floor)
//        {
//            UpdateLowestJoint(body);
//            if (floor != null)
//            {
//                Vector3D position = LowestJoint.Position;
//                position.Y *= -1f;
//                JumpHeight = floor.Distance(position);
//            }

//            IsJumping = JumpHeight - 0.1f > 0f;
//        }

//        //
//        // Summary:
//        //     Updates the variable LowestJoint.
//        //
//        // Parameters:
//        //   body:
//        public void UpdateLowestJoint(Body body)
//        {
//            float num = float.MinValue;
//            LowestJoint = body.Joints[JointType.Pelvis];
//            foreach (KeyValuePair<JointType, Joint> joint in body.Joints)
//            {
//                if (joint.Value.TrackingState != 0 && num < joint.Value.Position.Y)
//                {
//                    LowestJoint = joint.Value;
//                    num = LowestJoint.Position.Y;
//                }
//            }
//        }

//        //
//        // Summary:
//        //     Finds the lowest Y of the feet.
//        //
//        // Parameters:
//        //   body:
//        //     Position in y.
//        protected Vector3D FindLowestAnkle(Body body)
//        {
//            if (!(body.Joints[JointType.AnkleLeft].Position.Y < body.Joints[JointType.AnkleRight].Position.Y))
//            {
//                return body.Joints[JointType.AnkleRight].Position;
//            }

//            return body.Joints[JointType.AnkleLeft].Position;
//        }

//        //
//        // Summary:
//        //     Finds the lowest Y of the feet.
//        //
//        // Parameters:
//        //   body:
//        //     Position in y.
//        protected Vector3D FindLowestFoot(Body body)
//        {
//            if (!(body.Joints[JointType.FootLeft].Position.Y < body.Joints[JointType.FootRight].Position.Y))
//            {
//                return body.Joints[JointType.FootRight].Position;
//            }

//            return body.Joints[JointType.FootLeft].Position;
//        }
//    }
//}
