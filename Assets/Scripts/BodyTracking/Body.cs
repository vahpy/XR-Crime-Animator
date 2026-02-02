//using LightBuzz.Kinect4Azure;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

namespace TrackManager.BodyTracking
{
    ////
    //// Summary:
    ////     Represents a human Body.
    //public class Body
    //{
    //    //
    //    // Summary:
    //    //     The total number of joints.
    //    public static readonly int JointCount = Enum.GetValues(typeof(JointType)).Length;

    //    //
    //    // Summary:
    //    //     The unique identifier of the current Body.
    //    public uint ID { get; internal set; }

    //    //
    //    // Summary:
    //    //     The human body joints.
    //    public Dictionary<JointType, Joint> Joints { get; internal set; }

    //    //
    //    // Summary:
    //    //     Creates an empty Body instance.
    //    public Body()
    //    {
    //        Joints = new Dictionary<JointType, Joint>();
    //    }

    //    public Body(LightBuzz.Kinect4Azure.Body body)
    //    {
    //        ID = body.ID;
    //        Joints = new Dictionary<JointType, Joint>();
    //        foreach(var joint in body.Joints)
    //        {
    //            var cloneJoint = new Joint(joint.Value.JointType, joint.Value.TrackingState, joint.Value.Position, joint.Value.Orientation, joint.Value.PositionColor, joint.Value.PositionDepth);
    //            Joints.Add(joint.Key, cloneJoint);
    //        }
    //    }
    //}
}