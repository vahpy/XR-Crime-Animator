using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TrackManager;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ControlledObjectFieldUIScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro label;
    [SerializeField]
    private PressableButton button;
    private EffectSettingUI parentUI;
    private ControllableObjectEffect effectObj;
    private string fieldName;

    private UnityAction<Object> action;
    public void Initialize(string name, Component defaultValue, EffectSettingUI settingUI)
    {
        parentUI = settingUI;
        effectObj = parentUI.currentEffect;
        fieldName = name;
        if (defaultValue == null)
        {
            label.text = fieldName + ": None";
        }
        else
        {
            label.text = fieldName + ": " + defaultValue.gameObject.name;
        }
    }

    public void TriggerSelectingObject()
    {
        if (action != null) CancelSelecting();
        action = this.ObjectSelectedUI;
        
        AnimationRecorder.instance.AddObjectSelectorHandler(action);
    }

    public void CancelSelecting()
    {
        if (action != null) AnimationRecorder.instance.RemoveObjectSelectorHandler(action);
        action = null;
    }
    public void ObjectSelectedUI(Object value)
    {
        Debug.Log("Object selected: " + (((GameObject)value).GetComponent<Transform>().gameObject.name));
        Component comp = value.GetComponent<ControllableObject>();
        if (value != null)
        {
            if (AssignValue(comp))
            {
                CancelSelecting();
                button.ForceSetToggled(false);
                label.text = fieldName + ": " + comp.gameObject.name;
            }
        }
    }

    private bool AssignValue(Component value)
    {
        return effectObj.SetFieldValue(fieldName, value);
    }
}