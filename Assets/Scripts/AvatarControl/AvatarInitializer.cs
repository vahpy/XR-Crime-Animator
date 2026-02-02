using UnityEditor.Animations;
using UnityEngine;

namespace TrackManager.Animation
{
    public class AvatarInitializer : MonoBehaviour
    {
        void Start()
        {
            if (GetComponent<Animator>().runtimeAnimatorController == null)
            {
                var controller = AnimatorController.CreateAnimatorControllerAtPath(
                    AnimationRecorder.instance.GetUniqueAnimControllerPath());
                GetComponent<Animator>().runtimeAnimatorController = controller;
            }
            GetComponent<Animator>().StartPlayback();
        }
    }
}