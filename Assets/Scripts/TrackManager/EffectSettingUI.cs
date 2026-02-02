using TMPro;
using TrackManager.Animation;
using UnityEngine;
using System.Reflection;
using System;
using TrackManager;
using MixedReality.Toolkit.UX;

public class EffectSettingUI : MonoBehaviour
{
    public ControllableObjectEffect currentEffect { private set; get; }
    [SerializeField]
    private TextMeshPro effectLabel;
    [SerializeField]
    private EffectsListUI effectListUI;
    [SerializeField]
    private Transform settingListCanvas;

    //
    [SerializeField]
    private GameObject boolCheckboxPrefab;
    [SerializeField]
    private GameObject intCheckboxPrefab;
    [SerializeField]
    private GameObject floatCheckboxPrefab;
    [SerializeField]
    private GameObject componentFieldPrefab;
    [SerializeField]
    private GameObject controlledObjectFieldPrefab;


    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void InitializeOpen(ControllableObjectEffect effect)
    {
        currentEffect = effect;
        effectLabel.text = effect.GetEffectName();

        //print(effect.GetType().Name);

        foreach (var field in effect.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = field.GetValue(effect);
            //Debug.Log(field.Name + "[" + field.FieldType + "]" + " = " + value?.ToString());
            if (field.FieldType == typeof(int))
            {
                AddIntField(field.Name, (int)value);
            }
            else if (field.FieldType == typeof(ControllableObject) || field.FieldType == typeof(InteractiveProp))
            {
                AddControlledObjectField(field.Name, (ControllableObject)value);
            }
            else if (field.FieldType == typeof(Component))
            {
                AddComponentField(field.Name, (Component)value);
            }
            else if (field.FieldType == typeof(bool))
            {
                AddBoolField(field.Name, (bool)value);
            }
            else
            {
                Debug.LogWarning("Field " + field.Name + " is not supported.");
            }
        }

        UpdateUI();

        //
        TracksManager.instance.ShowHideEffectItemsUI(false);
        gameObject.SetActive(true);
        //effectListUI.gameObject.SetActive(false);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        var trs = settingListCanvas.GetComponentsInChildren<Transform>();
        foreach (Transform tr in trs)
        {
            if (tr != settingListCanvas)
            {
                tr.SetParent(null);
                Destroy(tr.gameObject);
            }
        }
        TracksManager.instance.ShowHideEffectItemsUI(true);
        //effectListUI.gameObject.SetActive(true);
    }
    private void UpdateUI()
    {
        var trs = Utils.GetComponentsInDirectChildren<Transform>(settingListCanvas);
        //Debug.Log(trs.Length);
        var locPos = Utils.SortUIVertically(trs, 0.1f);
        Utils.MoveObjects(trs, locPos, false);
    }
    private void AddIntField(string name, int value)
    {

    }
    private void AddControlledObjectField(string name, ControllableObject value)
    {
        //Debug.Log("Adding prefab for controlled object field, called " + name + " and object is " + value?.ToString());
        var gobj = Instantiate(controlledObjectFieldPrefab, settingListCanvas, false);
        gobj.transform.localPosition = Vector3.zero;
        gobj.transform.localRotation = Quaternion.identity;
        gobj.GetComponent<ControlledObjectFieldUIScript>().Initialize(name, value, this);
    }
    private void AddComponentField(string name, Component value)
    {

    }
    private void AddBoolField(string name, bool value)
    {
        var gobj = Instantiate(boolCheckboxPrefab, settingListCanvas, false);
        gobj.transform.localPosition = Vector3.zero;
        gobj.transform.localRotation = Quaternion.identity;
        gobj.GetComponent<BoolUIScript>().Initialize(name, value, this);
    }

}
