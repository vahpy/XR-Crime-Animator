using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TrackManager.Animation
{
    public class FootCollisionDetection : MonoBehaviour
    {
        private TwoBoneIKConstraint footIKConstraint;
        private MultiRotationConstraint footRotConstraint;
        private Transform targetObj;
        private Transform sourceJoint;
        //bool isTriggered = false;
        Vector3 onGroundTargetPos;

        private void Awake()
        {
            footIKConstraint = transform.parent.GetComponent<TwoBoneIKConstraint>();
            footRotConstraint = transform.parent.GetComponent<MultiRotationConstraint>();
            targetObj = transform;
            sourceJoint = footIKConstraint.data.tip;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Ground")
            {
                //isTriggered = true;
                onGroundTargetPos = transform.position;
                onGroundTargetPos.y = other.transform.position.y;
                targetObj.transform.position = onGroundTargetPos;
                StartCoroutine(SmoothTransition(footIKConstraint.weight, 1, 0.5f));
            }
        }
        private void OnTriggerExit(Collider other)
        {
            StartCoroutine(SmoothTransition(footIKConstraint.weight, 0, 0.5f));
            //isTriggered = false;
        }

        private IEnumerator SmoothTransition(float fromWeight, float toWeight, float duration)
        {
            float elapsedTime = 0;

            // While the duration is not yet reached
            while (elapsedTime < duration)
            {
                // Calculate the current time step's progress
                elapsedTime += Time.deltaTime; // Increment elapsed time
                float currentWeight = Mathf.Lerp(fromWeight, toWeight, elapsedTime / duration); // Linearly interpolate from fromWeight to toWeight over 'duration'

                // Apply the interpolated weight to constraints

                footIKConstraint.weight = currentWeight;
                footRotConstraint.weight = currentWeight;

                yield return null; // Wait until the next frame before continuing the loop

            }

            footIKConstraint.weight = toWeight;
            footRotConstraint.weight = toWeight;
        }
        private void Update()
        {
            //if (isTriggered)
            //{
            //    targetObj.transform.position = onGroundTargetPos;
            //}
            //else
            //{
            //    targetObj.SetWorldPose(sourceJoint.GetWorldPose());
            //}
            
        }
    }
}
