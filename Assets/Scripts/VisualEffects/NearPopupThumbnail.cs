using System.Collections;
using UnityEngine;

public class NearPopupThumbnail : MonoBehaviour
{
    public float transitionDuration = 1.0f;
    public float thresholdDistance = 5.0f;
    public AnimationCurve animationCurve;
    public bool onlyLookAtCamera = false;
    private Vector3 originalScale;
    private Vector3 smallScale;
    private Transform mainCamera;
    private bool isSmall = true;

    void Start()
    {
        originalScale = transform.localScale;
        smallScale = originalScale / 10;
        if (!onlyLookAtCamera)
        {
            transform.localScale = smallScale;
        }
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        if (!onlyLookAtCamera)
        {
            float distance = Vector3.Distance(mainCamera.position, transform.position);

            if (distance < thresholdDistance && isSmall)
            {
                StopAllCoroutines();
                StartCoroutine(ScaleOverTime(originalScale));
                isSmall = false;
            }
            else if (distance >= thresholdDistance && !isSmall)
            {
                StopAllCoroutines();
                StartCoroutine(ScaleOverTime(smallScale));
                isSmall = true;
            }
        }
        LookAtCamera();
    }

    IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        float currentTime = 0.0f;
        Vector3 startScale = transform.localScale;

        while (currentTime <= transitionDuration)
        {
            float curveValue = animationCurve.Evaluate(currentTime / transitionDuration);
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    void LookAtCamera()
    {
        Vector3 direction = mainCamera.position - transform.position;
        direction.y = 0; // Keep only horizontal rotation
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
    }

}