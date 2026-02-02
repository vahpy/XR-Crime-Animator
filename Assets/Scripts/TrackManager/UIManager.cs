using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { private set; get; }
    [SerializeField] private GameObject trackEditorPanel;
    [SerializeField] private GameObject propMenuPanel;
    [SerializeField] private MixedReality.Toolkit.UX.PressableButton followBtn;

    // setting info
    public bool TrackEditorShown { private set; get; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (AnimationRecorder.instance.isRecording)
        {
            TrackEditorPanelVisible(false);
            PropMenuPanelVisible(false);
        }
        else if (AnimationRecorder.instance.isPlaying)
        {
            if (propMenuPanel.activeSelf)
            {
                PropMenuPanelVisible(false);
            }
            TrackEditorPanelVisible(TrackEditorShown);
        }
        
    }

    public void ShowHidePanels()
    {
        if (AnimationRecorder.instance.isRecording) return; // ignore this button while recording
        PropMenuPanelVisible(!propMenuPanel.activeSelf);
    }

    public void HideEditorPanel()
    {
        TrackEditorShown = false;
        TrackEditorPanelVisible(false);
    }

    public void HidePropsMenu()
    {
        PropMenuPanelVisible(false);
    }

    public void MenuPressed()
    {
        Debug.Log("Menu button pressed");
        TrackEditorShown = true;
        TrackEditorPanelVisible(true);
        if (!followBtn.IsToggled)
        {
            StartCoroutine(ActionWithDelay());
        }
    }

    private IEnumerator ActionWithDelay()
    {
        followBtn.ForceSetToggled(true);
        yield return new WaitForSeconds(1f);
        followBtn.ForceSetToggled(false);
    }

    private void TrackEditorPanelVisible(bool visible)
    {
        if (visible && !trackEditorPanel.activeSelf) trackEditorPanel.SetActive(visible);
        if (trackEditorPanel.GetComponentInChildren<Renderer>().isVisible == visible)
        {
            return;
        }
        var renderers = trackEditorPanel.GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in renderers)
        {
            ren.enabled = visible;
        }
        var colliders = trackEditorPanel.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = visible;
        }
    }

    private void PropMenuPanelVisible(bool visible)
    {
        if (propMenuPanel.activeSelf != visible)
        {
            propMenuPanel.SetActive(visible);
        }
    }
}
