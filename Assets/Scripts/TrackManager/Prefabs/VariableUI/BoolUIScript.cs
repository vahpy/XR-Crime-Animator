using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TrackManager;
using TrackManager.Animation;
using UnityEngine;

public class BoolUIScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro label;
    [SerializeField]
    private PressableButton button;
    private EffectSettingUI parentUI;
    private ControllableObjectEffect effectObj;
    private string fieldName;
    public void Initialize(string name, bool defaultValue, EffectSettingUI settingUI)
    {
        parentUI = settingUI;
        effectObj = parentUI.currentEffect;
        fieldName = name;
        label.text = name;
        SetCheckBoxUI(defaultValue);
    }

    public void SetCheckBoxUI(bool value)
    {
        button.ForceSetToggled(value);
    }

    public void AssignValue(bool value)
    {
        effectObj.SetFieldValue(fieldName, value);
    }
}