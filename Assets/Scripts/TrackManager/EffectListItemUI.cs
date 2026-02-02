using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using TrackManager;
using TrackManager.Animation;
using UnityEngine;
using UnityEngine.Windows;

public class EffectListItemUI : MonoBehaviour
{
    [SerializeField]
    private EffectSettingUI effectSettingUI;
    [SerializeField]
    private ControllableObjectEffect effect;
    [SerializeField]
    private TextMeshPro label;
    [SerializeField]
    private Material defaultMat;
    [SerializeField]
    private Material highlightMat;

    public void Initialize(ControllableObjectEffect effect, EffectSettingUI effectSettingUI)
    {
        if (effect == null) return;
        this.effect = effect;
        this.effectSettingUI = effectSettingUI;

        var target = effect.GetTarget();
        if(target == null)
        {
            label.text = effect.GetEffectName();
        }
        else
        {
            label.text = target.name + ":" + effect.GetEffectName();
        }
    }
    
    public void EffectPressed()
    {
        var effectListUI = this.transform.parent.parent.GetComponent<EffectsListUI>();
        if (effectListUI.isDeletingModeOn)
        {
            DestroyImmediate(this.effect);
            effectListUI.CancelDeletingEffect();
            effectListUI.Close();
            if (TracksManager.instance.currentSlot != null)
            {
                TracksManager.instance.RefreshEffectItemsUI(TracksManager.instance.currentSlot);
            }
            return;
        }
        else
        {
            effectSettingUI.InitializeOpen(effect);
        }
    }
}