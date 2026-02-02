using UnityEngine;
using System.Collections.Generic;

namespace TrackManager.StoryTelling
{
    public class ChainOfArrows : MonoBehaviour
    {
        public Transform source;
        public Transform target;
        public GameObject arrowPrefab;
        public float gap = 0.25f;
        public float speed = 1f;

        private List<GameObject> arrows = new List<GameObject>();
        private float spawnTimer = 0f;

        private Vector3 lastSourcePos;
        private Vector3 lastTargetPos;
        private Vector3 sourceCenter;
        private Vector3 targetCenter;
        private Vector3 direction;
        private float maxDistance;
        private bool initialized = false;

        void Update()
        {
            if (source == null || target == null || arrowPrefab == null) return;

            if (!initialized || lastSourcePos != sourceCenter || lastTargetPos != targetCenter)
            {
                initialized = true;
                sourceCenter = GetCenter(source);
                targetCenter = GetCenter(target);
                lastSourcePos= sourceCenter;
                lastTargetPos= targetCenter;
                direction = (GetCenter(target) - GetCenter(source)).normalized;
                maxDistance = Vector3.Distance(sourceCenter, targetCenter);
            }

            spawnTimer += Time.deltaTime;

            if (spawnTimer >= gap / speed)
            {
                SpawnArrow();
                spawnTimer = 0f;
            }

            MoveArrows();
        }

        void SpawnArrow()
        {
            Vector3 spawnPosition = sourceCenter + direction * gap;

            if (arrows.Count == 0 || Vector3.Distance(spawnPosition, arrows[arrows.Count - 1].transform.position) >= gap)
            {
                GameObject arrow = GetPooledArrow();
                if (arrow == null)
                {
                    arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.LookRotation(direction));
                    arrow.transform.parent = this.transform;
                    arrow.transform.localScale = Vector3.one * 0.4f; // Set initial scale to 0.4
                    arrows.Add(arrow);
                }
                else
                {
                    arrow.transform.position = spawnPosition;
                    arrow.transform.rotation = Quaternion.LookRotation(direction);
                    arrow.transform.localScale = Vector3.one * 0.4f; // Set initial scale to 0.4
                    arrow.SetActive(true);
                }
            }
        }

        void MoveArrows()
        {
            

            foreach (GameObject arrow in arrows)
            {
                if (arrow.activeSelf)
                {
                    arrow.transform.position += direction * speed * Time.deltaTime;

                    float currentDistance = Vector3.Distance(arrow.transform.position, sourceCenter);
                    float scale = Mathf.Lerp(0.4f, 1f, currentDistance / maxDistance);
                    arrow.transform.localScale = Vector3.one * scale;

                    if (Vector3.Distance(arrow.transform.position, targetCenter) <= gap)
                    {
                        arrow.SetActive(false);
                    }
                }
            }
        }

        Vector3 GetCenter(Transform obj)
        {
            // check if the object or any of the children have collider, return the center of one of the colliders
            Collider collider = obj.GetComponentInChildren<Collider>();
            if (collider != null)
            {
                return collider.bounds.center;
            }

            // if not, check if the object, or any of its children have a mesh, then return the center of the mesh
            MeshFilter meshFilter = obj.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
            {
                return meshFilter.mesh.bounds.center;
            }

            // if not then return the position of this obj transform
            return obj.position;
        }

        GameObject GetPooledArrow()
        {
            foreach (GameObject arrow in arrows)
            {
                if (!arrow.activeSelf)
                {
                    return arrow;
                }
            }
            return null;
        }
    }
}