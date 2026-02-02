
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using TMPro;
using TrackManager;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class AnimationRecorder : MonoBehaviour
{
    [SerializeField] private ViewLevelProvider viewProvider = default;
    public static AnimationRecorder instance { get; private set; }
    [SerializeField]
    private GameObject mainAnimationSpace;
    [SerializeField]
    private EffectSettingUI effectSettingUI = default;
    [SerializeField] private GameObject trackEditorPanel = default;
    [SerializeField]
    private Slider timeSlider = default;
    [SerializeField]
    private PressableButton playBtn = default;
    [SerializeField]
    private UnityAction<UnityEngine.Object> objectSelectorHandlers;
    [SerializeField] private TMP_Text recordingSign = default;
    [SerializeField] private bool triggerRecord = false;
    private bool lastTriggerRecord = false;
    public bool isPlaying { private set; get; }
    public bool isRecording { private set; get; }
    public bool isPlayingForwardDir { private set; get; }
    [DoNotSerialize] public int absoluteFrameNum;// { private set; get; }
    [DoNotSerialize, Range(1, 30)] public int framePerSecond = 10;
    public bool UpdateTick;

    private float startCountingTime;
    private int startCountingFrame;
    private int lastFramePlayed;
    public float eachFrameDuration { get; private set; }

    private InteractiveProp currentInteractiveProp = null;
    private string relativeDirPath;

    private int clipCounter = 1;
    private int tracksCounter = 1;

    // controller buttons
    public bool IsAnyGripTriggerPressing { private set; get; }

    private void Awake()
    {
        absoluteFrameNum = 0;
        startCountingTime = -1f;
        eachFrameDuration = 1f / framePerSecond;
        isPlayingForwardDir = false;
        IsAnyGripTriggerPressing = false;
        //lastSliderMoveTime = 0;
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        triggerRecord = false;
        lastTriggerRecord = triggerRecord;
    }

    private void CreateClipsDir()
    {
        // Set a parent directory 
        string parentDirectory = Application.dataPath + "/Recordings/AnimationClips/" + "Clips_" + DateTime.Now.ToString("dd-MM-yyyy") + "_";
        var counter = 1;
        while (counter < 1000)
        {
            var dirPath = parentDirectory + counter;
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
                relativeDirPath = "Assets/Recordings/AnimationClips/" + "Clips_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + counter;
                break;
            }
            counter++;
        }
    }
    public string[] GetRecentSavedTracks()
    {
        string parentDir = Application.dataPath + "/Recordings/AnimationClips/";
        // in each folder in the parentDir, find all files with .json extension
        var dirs = Directory.GetDirectories(parentDir);
        List<string> jsonFiles = new();
        foreach(var dir in dirs )
        {
            jsonFiles.AddRange( Directory.GetFiles(dir, "*.json"));
        }
        var sortedFiles = jsonFiles.OrderByDescending(f => File.GetLastWriteTime(f)).ToArray();
        return sortedFiles;
    }
    public string GetMainRelativePath()
    {
        if (relativeDirPath == null) CreateClipsDir();
        return relativeDirPath;
    }
    public string GetUniqueJSONFilePath()
    {
        if (relativeDirPath == null) CreateClipsDir();
        return relativeDirPath + "/Tracks_" + (tracksCounter++) + ".json";
    }
    public string GetUniqueClipPath()
    {
        if (relativeDirPath == null) CreateClipsDir();
        return relativeDirPath + "/Clip_" + (clipCounter++) + ".anim";
    }
    public string GetUniqueAnimControllerPath()
    {
        if (relativeDirPath == null) CreateClipsDir();
        var objs = FindObjectsByType<AvatarInitializer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        return relativeDirPath + "/AnimController_" + objs.Length + ".controller";
    }
    public void RecordKeyFrame()
    {
        if (viewProvider != null && viewProvider.isEagleView) return;
        isRecording = !isRecording;
        absoluteFrameNum = (int)Mathf.Lerp(TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame, timeSlider.Value);
        if (isRecording)
        {
            PlayProgrammatically();
        }
        else
        {
            PauseProgrammatically();
        }
        if (effectSettingUI != null && effectSettingUI.currentEffect != null)
        {
            effectSettingUI.currentEffect.RecordKeyFrame(absoluteFrameNum);
        }
        if (currentInteractiveProp != null)
        {
            SetupInteractionRecorderEffect(currentInteractiveProp);
            currentInteractiveProp.interactionRecorderEffect.RecordKeyFrame(absoluteFrameNum);
        }
    }

    private void Update()
    {
        UpdateTick = false;
        if (recordingSign.isActiveAndEnabled != isRecording)
        {
            recordingSign.gameObject.SetActive(isRecording);
        }
        if (triggerRecord != lastTriggerRecord)
        {
            if (triggerRecord)
            {
                RecordKeyFrame();
            }
            triggerRecord = false;
            lastTriggerRecord = triggerRecord;
        }


        // adjust the frame rate
        CalculatePreciseAbsFrame();
        if (lastFramePlayed != absoluteFrameNum)
        {
            UpdateTick = true;
            lastFramePlayed = absoluteFrameNum;
            if (isPlaying)
            {
                UpdateTimeSlider();
                TracksManager.instance.PlayTracks(absoluteFrameNum);
            }
            if (isRecording)
            {
                if (effectSettingUI != null && effectSettingUI.currentEffect != null)
                {
                    effectSettingUI.currentEffect.RecordEachFrame(absoluteFrameNum);
                }
                if (currentInteractiveProp != null)
                {
                    SetupInteractionRecorderEffect(currentInteractiveProp);
                    currentInteractiveProp.interactionRecorderEffect.RecordEachFrame(absoluteFrameNum);
                }
            }
        }
    }


    private void CalculatePreciseAbsFrame()
    {
        if (!isPlaying && !isRecording)
        {
            startCountingTime = -1f;
            startCountingFrame = -1;
            return;
        }
        if (startCountingFrame == -1)
        {
            startCountingTime = Time.time;
            startCountingFrame = absoluteFrameNum;
        }
        absoluteFrameNum = ((int)((Time.time - startCountingTime) * framePerSecond)) + startCountingFrame;
    }
    private void SetupInteractionRecorderEffect(InteractiveProp prop)
    {
        if (prop == null) return;
        //setup an effect for this prop
        Slot currentSlot = TracksManager.instance.currentSlot;
        if (currentSlot == null) return;
        if (prop.interactionRecorderEffect == null || prop.interactionRecorderEffect.GetSlot() != currentSlot)
        {
            prop.interactionRecorderEffect = null; // not effective for first condition, but used for second one to get the current slot's effect
            foreach (ControllableObjectEffect effect in currentSlot.Effects)
            {
                if (effect.target == prop && effect is IntPropInteractionEffect)
                {
                    prop.interactionRecorderEffect = effect as IntPropInteractionEffect;
                }
            }
        }
        if (prop.interactionRecorderEffect == null)
        {
            IntPropInteractionEffect effect = currentSlot.AddComponent<IntPropInteractionEffect>();
            prop.interactionRecorderEffect = effect;
            if (!effect.SetTarget(prop))
            {
                Debug.LogError("Prop not assigned as target.");
            }
        }
    }


    public ControllableObjectEffect GetCurrentEffect()
    {
        return effectSettingUI.currentEffect;
    }
    public void ObjectSelected(UnityEngine.Object obj)
    {
        this.objectSelectorHandlers?.Invoke(obj);
    }

    public void AddObjectSelectorHandler(UnityAction<UnityEngine.Object> objectSelectorHandler)
    {
        this.objectSelectorHandlers += objectSelectorHandler;
    }
    public void RemoveObjectSelectorHandler(UnityAction<UnityEngine.Object> objectSelectorHandler)
    {
        this.objectSelectorHandlers -= objectSelectorHandler;
    }

    public void PlayByUI()
    {
        isPlaying = true;
        absoluteFrameNum = (int)Mathf.Lerp(TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame, timeSlider.Value);
        TracksManager.instance.PlayReverseFirstRelativeFrame();
    }
    public void PauseByUI()
    {
        isPlaying = false;
        StopAllAnimations();
    }
    public void UpdateFrameNumBySlider(SliderEventData data)
    {
        if (isRecording || data.NewValue == data.OldValue) return;
        var newAbsFrameNum  = (int)Mathf.Lerp(TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame, data.NewValue);
        if (isPlaying && newAbsFrameNum < absoluteFrameNum)
        {
            PauseByUI();
            UpdatePlayBtnUI(false);
        }
        absoluteFrameNum = newAbsFrameNum;// (int)Mathf.Lerp(TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame, data.NewValue);
    }
    public void PlayProgrammatically()
    {
        isPlaying = true;
        absoluteFrameNum = (int)Mathf.Lerp(TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame, timeSlider.Value);
        TracksManager.instance.PlayReverseFirstRelativeFrame();
        UpdatePlayBtnUI(true);
    }
    public void PauseProgrammatically()
    {
        isPlaying = false;
        UpdatePlayBtnUI(false);
        StopAllAnimations();
    }


    public void StopAllAnimations()
    {
        var anims = FindObjectsByType<AvatarAnimationControl>(FindObjectsSortMode.None);
        foreach (var anim in anims)
        {
            anim.SetAnimate(false);
        }
    }


    public void UpdatePlayBtnUI(bool pressed)
    {
        if (playBtn.IsToggled != pressed)
        {
            playBtn.ForceSetToggled(pressed);
        }
    }

    private void UpdateTimeSlider()
    {
        var value = (absoluteFrameNum - TracksManager.instance.minVisibleFrame) / (float)(TracksManager.instance.maxVisibleFrame - TracksManager.instance.minVisibleFrame);
        if (value < 0) value = 0;
        if (value > 1f)
        {
            value = 0f;
            PauseByUI();
            UpdatePlayBtnUI(false);
        }
        timeSlider.Value = value;
    }

    public Transform GetAnimationSpace()
    {
        return mainAnimationSpace.transform;
    }

    public void GripTriggerPressed()
    {
        IsAnyGripTriggerPressing = true;
        TriggerPropInteraction();
    }

    public void GripTiggerReleased()
    {
        IsAnyGripTriggerPressing = false;
        EndTriggerPropInteraction();
    }

    public void TriggerPropInteraction()
    {
        if (currentInteractiveProp == null) return;
        currentInteractiveProp.StartTriggerInteraction();
    }

    public void EndTriggerPropInteraction()
    {
        if (currentInteractiveProp == null) return;
        currentInteractiveProp.StopTriggerInteraction();
    }

    public void SetCurrentInteractiveProp(InteractiveProp prop)
    {
        currentInteractiveProp = prop;
    }

    public void ClearCurrentInteractiveProp(InteractiveProp prop)
    {
        if (currentInteractiveProp == prop) currentInteractiveProp = null;
    }
}
