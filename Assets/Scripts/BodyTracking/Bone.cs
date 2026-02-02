using UnityEngine;

namespace TrackManager.BodyTracking
{
    //
    // Summary:
    //     Represents an avatar bone.
    public class Bone
    {
        private Quaternion _previous;

        //
        // Summary:
        //     The transform of the bone.
        public Transform Transform { get; }

        //
        // Summary:
        //     The original rotation of the bone.
        public Quaternion OriginalRotation { get; private set; }

        //
        // Summary:
        //     The original position of the bone.
        public Vector3 OriginalPosition { get; private set; }

        internal Bone(Transform transform)
        {
            Transform = transform;
            CalibrateOriginalRotation();
        }

        //
        // Summary:
        //     Calibrate the original bone rotation.
        public void CalibrateOriginalRotation()
        {
            OriginalRotation = Transform.rotation;
            OriginalPosition = Transform.position;
            _previous = Transform.rotation;
        }

        //
        // Summary:
        //     Updates the rotation of the current bone.
        //
        // Parameters:
        //   newRotation:
        //     The new rotation.
        //
        //   smoothDelta:
        //     The motion smoothing factor (0.0 - 1.0)
        public void UpdateRotation(Quaternion newRotation, float smoothDelta)
        {
            smoothDelta = Mathf.Clamp01(smoothDelta);
            Quaternion quaternion = newRotation * OriginalRotation;
            Quaternion quaternion2 = (_previous = UnityEngine.Quaternion.Lerp(_previous, quaternion, smoothDelta));
            Transform.rotation = quaternion2;
        }

        //
        // Summary:
        //     Updates the position of the current bone.
        //
        // Parameters:
        //   newPosition:
        //     The new bone position.
        public void UpdatePosition(Vector3 newPosition)
        {
            Transform.position = newPosition;
        }

        //
        // Summary:
        //     Resets the rotation of the current bone.
        public void Reset()
        {
            Transform.rotation = OriginalRotation;
        }
    }
}