using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TrackManager;
using Unity.VisualScripting;
using UnityEngine;

public class EffectMenuItemScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro labelObj = default;

    private Type type;

    public void Initialize(string label, Type effectType)
    {
        labelObj.text = label;
        type = effectType;
    }

    public void AddEffectToCurrentSlot()
    {
        var slot = TracksManager.instance.currentSlot;
        this.transform.parent.parent.GetComponent<EffectMenuUI>().ClosePanel();
        if (slot != null)
        {
            if (slot.AddEffect(type))
            {
                TracksManager.instance.RefreshEffectItemsUI(slot);
            }
        }
    }
}
