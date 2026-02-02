using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MarkAnnotation : MonoBehaviour
{

    [SerializeField] private TextMeshPro markNumberTxt1;
    [SerializeField] private TextMeshPro markNumberTxt2;
    [SerializeField] private int markNumber;

    private void Start()
    {
        SetMarkNumber(GenerateUniqueNumber());
    }
    public int GetNumber()
    {
        return markNumber;
    }

    public void SetMarkNumber(int number)
    {
        if (number <= 0 || number >= 100) markNumber = GenerateUniqueNumber();
        else markNumber = number;
        UpdateMarkText();
    }

    private void UpdateMarkText()
    {
        if (markNumberTxt1 != null) markNumberTxt1.text = markNumber.ToString();
        if (markNumberTxt2 != null) markNumberTxt2.text = markNumber.ToString();
    }

    private int GenerateUniqueNumber()
    {
        var markAnnotObjs = FindObjectsOfType<MarkAnnotation>();
        var existNumbers = markAnnotObjs.Select(m => m.GetNumber()).ToHashSet();
        for(int i=1;i<100;i++)
        {
            if(!existNumbers.Contains(i)) return i;
        }
        return 0;
    }
}
