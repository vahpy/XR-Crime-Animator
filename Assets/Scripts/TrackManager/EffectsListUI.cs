using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TrackManager;
using TrackManager.Animation;
using UnityEngine;

public class EffectsListUI : MonoBehaviour
{
    [SerializeField]
    private Transform effectsPanel = default;
    [SerializeField]
    private GameObject effectItemUIPrefab;
    [SerializeField]
    private EffectSettingUI effectSettingPanel;
    [SerializeField]
    private GameObject effectMenuUI;
    [SerializeField]
    private PressableButton deleteBtn;
    private Slot slot;

    public bool isDeletingModeOn
    {
        get
        {
            if (deleteBtn == null) return false;
            return deleteBtn.IsToggled;
        }
    }
    private void Start()
    {
        deleteBtn.ForceSetToggled(false);
    }
    public void Initialize(Slot slot)
    {
        if(slot==null) return;
        if (isDeletingModeOn) deleteBtn.ForceSetToggled(false);
        if (this.slot == slot) return;
        effectSettingPanel.ClosePanel();
        effectMenuUI.SetActive(false);
        this.slot = slot;
        //Destroy previous ones
        EffectListItemUI[] effectUIs = GetComponentsInChildren<EffectListItemUI>();
        for (int i = 0; i < effectUIs.Length; i++)
        {
            if (effectUIs[i] != null)
            {
                Destroy(effectUIs[i].gameObject);
            }
        }
        //
        List<Transform> gObjTransfroms = new List<Transform>();

        if (slot.Effects != null)
        {
            foreach (ControllableObjectEffect effect in slot.Effects)
            {
                GameObject g = Instantiate(effectItemUIPrefab, effectsPanel, false);
                g.GetComponent<EffectListItemUI>().Initialize(effect, effectSettingPanel);
                g.transform.localPosition = Vector3.zero;
                gObjTransfroms.Add(g.transform);
            }
            var gObjsArr = gObjTransfroms.ToArray();
            var locPositions = Utils.SortUIVertically(gObjsArr, 0.1f);
            Utils.MoveObjects(gObjsArr, locPositions, false);
        }
        this.gameObject.SetActive(true);
        //
    }
    public void Close()
    {
        if (isDeletingModeOn) deleteBtn.ForceSetToggled(false);
        if (slot != null)
        {
            slot.SetSelected(false);
            slot = null;
        }
        gameObject.SetActive(false);
    }

    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
        if (isDeletingModeOn) deleteBtn.ForceSetToggled(false);
    }

    public void CancelDeletingEffect()
    {
        deleteBtn.ForceSetToggled(false);
    }

    public void OpenEffectMenuPanel()
    {
        ShowHide(false);
        effectMenuUI.SetActive(true);
    }

    void OnEnable()
    {
        if (slot == null) this.gameObject.SetActive(false);
    }
}
