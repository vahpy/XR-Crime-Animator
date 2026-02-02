using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

//[ExecuteAlways]
public class GuidingLabelController : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private bool rightSide;
    [SerializeField] private bool alwaysLookAtCamera;
    [SerializeField] private TextMeshPro labelText;
    [SerializeField] private Transform labelTransform;
    [SerializeField] private GameObject targetAnchor;
    [SerializeField] private Transform sourceLeftAnchor;
    [SerializeField] private Transform sourceRightAnchor;
    private LineRenderer lineRenderer;

    public void SetLabelText(string text)
    {
        this.text = text;
        labelText.text = text;
    }

    private void Start()
    {
        labelText.text = text;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        if (targetAnchor == null) return;
        var targetLocalPos = transform.InverseTransformPoint(targetAnchor.transform.position);
        var sourceLocalPos = transform.InverseTransformPoint((rightSide?sourceRightAnchor.position: sourceLeftAnchor.position));
        lineRenderer.SetPosition(0, sourceLocalPos);
        lineRenderer.SetPosition(1, targetLocalPos);
    }

    private void Update()
    {
        if (alwaysLookAtCamera)
        {
            LookAtCameraAroundLocalXAxis();
        }
        //if(targetAnchor== null || !targetAnchor.activeInHierarchy) { 
        //    this.labelTransform.gameObject.SetActive(false);
        //}
        //else
        //{
        //    if(!this.labelTransform.gameObject.activeSelf)
        //    {
        //        this.labelTransform.gameObject.SetActive(true);
        //    }
        //}
    }
    //void LookAtCameraAroundLocalXAxis()
    //{
    //    //Vector3 direction = Camera.main.transform.position - transform.position;
    //    //direction.x = 0; // Keep only horizontal rotation
    //    //Quaternion rotation = Quaternion.LookRotation(direction);
    //    //transform.rotation = rotation;

    //    Vector3 direction = labelTransform.position - Camera.main.transform.position;
    //    direction.x = 0;
    //    direction.y = 0;
    //    Quaternion rotation = Quaternion.LookRotation(direction);
    //    labelTransform.rotation = rotation;
    //}

    void LookAtCameraAroundLocalXAxis()
    {
        var camFwd = Camera.main.transform.forward;
        var labelFwd = labelTransform.forward;
        // A matrix that rotates labelFwd to camFwd
        var rotation = Quaternion.FromToRotation(labelFwd, camFwd);
        labelTransform.rotation = rotation * labelTransform.rotation;
    }

}
