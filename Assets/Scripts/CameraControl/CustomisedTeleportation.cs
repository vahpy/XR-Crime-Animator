using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CustomisedTeleportation : MonoBehaviour
{
    [SerializeField] private Transform[] targets = default;
    [SerializeField, Range(1, 20)] private float teleportationSpeed = 10f;
    [SerializeField] private float eagleViewHeight = 10f;
    [SerializeField] private float groundViewHeight = 1.5f;
    [SerializeField] private float viewChangeDuration = 2f;

    private bool isMoving = false;
    private Vector2 direction = Vector2.zero;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 endPosition;
    private Quaternion endRotation;

    private bool isInTransition = false;
    public void Move(Vector2 direction)
    {
        isMoving = true;
        this.direction = direction;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (isMoving && targets != null && targets.Length > 0)
        {
            var timePassed = Time.deltaTime;
            var cameraFwd = Camera.main.transform.forward;
            var cameraRight = Camera.main.transform.right;
            var moveDirection = (cameraFwd * direction.y + cameraRight * direction.x).normalized;
            var movement = moveDirection * teleportationSpeed * timePassed;
            //MoveTargets(movement);
            this.transform.position =  this.transform.position + movement;
        }
    }
    private void MoveTargets(Vector3 movement)
    {
        foreach (var target in targets)
        {
            var pos = target.position;
            pos = new Vector3(pos.x + movement.x, pos.y, pos.z + movement.z);
            target.position = pos;
        }
    }
    
    public void ChangeView()
    {
        if (isInTransition) return;
        if (transform.position.y < eagleViewHeight)
        {
            StartCoroutine(SetViewToEagleView());
        }
        else
        {
            StartCoroutine(SetViewToGround());
        }
    }

    private IEnumerator SetViewToEagleView()
    {
        isInTransition = true;
        startPosition = transform.position;
        startRotation = transform.rotation;
        endPosition = Camera.main.transform.position + Vector3.up * eagleViewHeight;
        endRotation = Quaternion.LookRotation(Camera.main.transform.position - endPosition, Vector3.forward);
        float elapsedTime = 0f;

        while (elapsedTime < viewChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / viewChangeDuration);

            // Smoothly interpolate the position and rotation
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, 2*t);

            yield return null;
        }

        // Ensure the final position and rotation are set
        transform.position = endPosition;
        transform.rotation = endRotation;
        endPosition = startPosition;
        endRotation = startRotation;
        isInTransition = false;
    }

    private IEnumerator SetViewToGround()
    {
        isInTransition = true;
        startPosition = transform.position;
        startRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < viewChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / viewChangeDuration);

            // Smoothly interpolate the position and rotation
            transform.position = Vector3.Lerp(startPosition, endPosition, 2*t);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        // Ensure the final position and rotation are set
        transform.position = endPosition;
        transform.rotation = endRotation;
        isInTransition = false;
    }
}