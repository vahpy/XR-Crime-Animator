using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationConvertor : MonoBehaviour
{
    [SerializeField] private Transform sourceHip;
    [SerializeField] private Transform targetHip;

    [SerializeField] private List<Transform> leftCenterBonesOfSource;
    [SerializeField] private List<Transform> leftCenterBonesOfTarget;
    [SerializeField] private List<Transform> rightBonesOfSource;
    [SerializeField] private List<Transform> rightBonesOfTarget;
    //[SerializeField] private Transform[] bonesOfSource;
    //[SerializeField] private Transform[] bonesOfTarget;

    private void Start()
    {
        if(sourceHip!=null && targetHip!=null)
        {
            leftCenterBonesOfSource= new List<Transform>();
            leftCenterBonesOfTarget = new List<Transform>();
            rightBonesOfSource = new List<Transform>();
            rightBonesOfTarget = new List<Transform>();
            AddBonesAutomatically();
        }
    }
    private void Update()
    {
        for (int i = 0; i < leftCenterBonesOfTarget.Count; i++)
        {
            leftCenterBonesOfTarget[i].position = leftCenterBonesOfSource[i].position;
            leftCenterBonesOfTarget[i].rotation = leftCenterBonesOfSource[i].rotation;
        }
        for(int i=0; i < rightBonesOfTarget.Count; i++)
        {
            rightBonesOfTarget[i].position = rightBonesOfSource[i].position;
            rightBonesOfTarget[i].rotation = rightBonesOfSource[i].rotation * Quaternion.Euler(180, 0, 0);
        }
    }
    private void AddBonesAutomatically()
    {
        string[] keywords = { "right","left" };
        List<Transform> leftsrc = new List<Transform>();
        List<Transform> rightsrc = new List<Transform>();
        List<Transform> lefttar = new List<Transform>();
        List<Transform> righttar = new List<Transform>();

        // match based on depth from hip
        leftsrc.Add(sourceHip);
        lefttar.Add(targetHip);
    }
    private void AddBonesRecursively(Transform srcBone, Transform tarBone)
    {
        if (srcBone == null || tarBone == null) return;
        if (srcBone.name.ToLower().Contains("right"))
        {
            rightBonesOfSource.Add(srcBone);
            rightBonesOfTarget.Add(tarBone);
        }
        else
        {
            leftCenterBonesOfSource.Add(srcBone);
            leftCenterBonesOfTarget.Add(tarBone);
        }
        var tr = srcBone.GetComponentsInChildren<Transform>();
        
    }
}
