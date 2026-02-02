using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPicker : MonoBehaviour
{
    private readonly string[] predefined_texts = {"Annotation",
    "Annotation 1",
    "Annotation 2",
    "Annotation 3",
    "Annotation 4",
    "Annotation 5"};
    [SerializeField] private TMPro.TextMeshPro textComp = default;
    [SerializeField] private int annotationID;
    [SerializeField] private string text;
    private int previousAnnotationID;
    private void Start()
    {
        previousAnnotationID = annotationID;
        textComp.text = predefined_texts[annotationID];
    }
    private void Update()
    {
        if (textComp == null) return;
        if (text != null && textComp.text != text && text.Trim().Length > 0)
        {
            text = text.Trim();
            textComp.text = text;
        }else if (annotationID != previousAnnotationID) textComp.text = predefined_texts[annotationID];
    }
}
