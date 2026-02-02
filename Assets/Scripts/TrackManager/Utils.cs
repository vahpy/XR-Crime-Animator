using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TrackManager.Animation;
using UnityEngine;

namespace TrackManager
{
    public class Utils
    {
        private static readonly float eachFrameTransitionDelay = 0.2f;

        public static (int? lower, int? upper) FindClosestNumbers(List<int> sortedNumbers, int target)
        {
            int index = sortedNumbers.BinarySearch(target);

            if (index >= 0)
            {
                // Exact match found, handle edge cases for first and last elements
                int? lower = index == 0 ? (int?)null : sortedNumbers[index - 1];
                int? upper = index == sortedNumbers.Count - 1 ? (int?)null : sortedNumbers[index + 1];
                return (lower, upper);
            }
            else
            {
                int insertionPoint = ~index; // Bitwise complement gives the insertion point

                int? lower = insertionPoint == 0 ? (int?)null : sortedNumbers[insertionPoint - 1];
                int? upper = insertionPoint == sortedNumbers.Count ? (int?)null : sortedNumbers[insertionPoint];

                return (lower, upper);
            }
        }

        public static Vector3[] SortUIVertically(Transform[] objs, float gapRatio)
        {
            if (objs == null || objs.Length == 0) return null;
            Vector3[] locPos = new Vector3[objs.Length];
            float moveBlock = -(1 + gapRatio);
            //bool thereIsCollider = ;
            var blockSize = objs[0].localScale.y;
            if (objs[0].GetComponent<BoxCollider>() != null)
            {
                blockSize = Mathf.Max(blockSize, objs[0].GetComponent<BoxCollider>().size.y);
            }
            for (int i = 0; i < objs.Length; i++)
            {
                Transform tr = objs[i];
                if (tr != null)
                {
                    var pos = tr.localPosition;
                    pos.y = blockSize * i * moveBlock;
                    locPos[i] = pos;
                }
                else
                {
                    locPos[i] = Vector3.zero;
                }
            }
            return locPos;
        }

        public static void MoveObjects(Transform[] transforms, Vector3[] finalLocalPos, bool smoothTransition)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                MoveObject(transforms[i], finalLocalPos[i], smoothTransition);
            }
        }
        public static void MoveObject(Transform tr, Vector3 finalLocalPos, bool smoothTransition)
        {
            if (tr == null) return;
            if (!smoothTransition) tr.localPosition = finalLocalPos;
            else
            {
                var mono = tr.GetComponent<MonoBehaviour>();
                if (mono != null)
                {
                    mono.StartCoroutine(SmoothTransition(tr, tr.localPosition, finalLocalPos, eachFrameTransitionDelay));
                }
            }
        }
        private static IEnumerator SmoothTransition(Transform tr, Vector3 initPos, Vector3 tarPos, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                tr.transform.localPosition = Vector3.Lerp(initPos, tarPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            tr.transform.localPosition = tarPos;
        }

        public static Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }

                Transform found = FindDeepChild(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static Transform FindDeepChildNameLike(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Contains( name))
                {
                    return child;
                }

                Transform found = FindDeepChildNameLike(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static T[] GetComponentsInDirectChildren<T>(Transform parent) where T : Component
        {
            List<T> components = new List<T>();

            // Loop through each direct child of the parent GameObject.
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);

                // Attempt to get the component of type T in the direct child.
                T component = child.GetComponent<T>();

                // If the component is found, add it to the list.
                if (component != null)
                {
                    components.Add(component);
                }
            }
            return components.ToArray();
        }

        public static string EffectNameBeautifier(string effectName)
        {
            if (effectName != null && effectName.Length > 0)
            {
                string formatted = Regex.Replace(effectName, "Effect$", ""); // Remove "Effect" at the end
                formatted = Regex.Replace(formatted, "(?<!^)([A-Z])", " $1");
                return formatted;
            }
            return effectName;
        }
    }
}