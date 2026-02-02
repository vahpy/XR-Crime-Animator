
using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;
namespace TrackManager
{
    public class TracksManager : MonoBehaviour
    {
        public static TracksManager instance { get; private set; }

        //public readonly int framePerSecond = 10;
        public float animationDurationSec {private set; get;}
        [SerializeField]
        public GameObject trackPrefab;
        [SerializeField]
        public Transform tracksRoot = default;
        [SerializeField]
        public Transform slotsVisiblePanel = default;
        [SerializeField]
        public SlotUIController slotsEntirePanel = default;
        [SerializeField]
        private EffectsListUI effectListUI = default;
        /// <summary>
        /// Slots button, 0- Create, 1- Split, 2- Delete
        /// </summary>
        [SerializeField]
        private PressableButton[] slotOptionsBtns = default;

        public Slot currentSlot { private set; get; }


        [SerializeField]
        public int minVisibleFrame;
        [SerializeField]
        public int maxVisibleFrame;
        public int totalFrames { private set; get; }
        public bool createSlotPressed
        {
            set
            {
                if (value != createSlotPressed)
                {
                    slotOptionsBtns[0].ForceSetToggled(value);
                }
                if (value && splitSlotPressed)
                {
                    splitSlotPressed = false;
                }
            }
            get
            {
                return slotOptionsBtns[0].IsToggled;
            }
        }

        public bool deleteSlotPressed
        {
            set
            {
                if (value != deleteSlotPressed)
                {
                    slotOptionsBtns[2].ForceSetToggled(value);
                }
            }
            get
            {
                return slotOptionsBtns[2].IsToggled;
            }
        }

        public bool splitSlotPressed
        {
            set
            {
                if (value != splitSlotPressed)
                {
                    slotOptionsBtns[1].ForceSetToggled(value);
                }
                if (value && createSlotPressed)
                {
                    createSlotPressed = false;
                }
            }
            get
            {
                return slotOptionsBtns[1].IsToggled;
            }
        }
        // private fields
        private Coroutine currentCoroutine;
        [SerializeField]
        public Track[] AllTracks
        {
            get
            {
                return tracksRoot.GetComponentsInChildren<Track>();
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }
        private void Start()
        {
            animationDurationSec = 100;
            minVisibleFrame = 0;
            maxVisibleFrame = (int)(animationDurationSec * AnimationRecorder.instance.framePerSecond);
            if (slotsEntirePanel.slotAreaUIUpdated == null)
            {
                slotsEntirePanel.slotAreaUIUpdated = new UnityEngine.Events.UnityEvent();
                slotsEntirePanel.slotAreaUIUpdated.AddListener(MoveScaleSlotsUI);
            }

            // Test - initializer
            //CreateTrack();
            //CreateTrack();
            //AllTracks[0].CreateSlot(10, 50);
            //AllTracks[1].CreateSlot(30, 100);
            //AllTracks[0].allSlots[0].AddEffect(typeof(MetaBodyTrackingEffect));
            //AllTracks[1].allSlots[0].AddEffect(typeof(MetaBodyTrackingEffect));
            //var objs = FindObjectsByType<AvatarInitializer>(FindObjectsSortMode.None);
            //AllTracks[0].allSlots[0].Effects[0].SetTarget(objs[0].GetComponent<ControlledObject>());
            //AllTracks[1].allSlots[0].Effects[0].SetTarget(objs[1].GetComponent<ControlledObject>());
            //var eff0 = AllTracks[0].allSlots[0].Effects[0] as MetaBodyTrackingEffect;
            //var eff1 = AllTracks[1].allSlots[0].Effects[0] as MetaBodyTrackingEffect;
            //eff0.SetAnimationClip(eff0.target.GetComponent<Animator>().runtimeAnimatorController.animationClips[0]);
            //eff1.SetAnimationClip(eff1.target.GetComponent<Animator>().runtimeAnimatorController.animationClips[0]);
        }

        /// <summary>
        /// When a track has been moved, need a spatial sorting
        /// </summary>
        public void UpdateOrder(Track movedTrack, bool stillMoving)
        {

            Track[] tracks = AllTracks;

            tracks = tracks.OrderByDescending(track => track.transform.localPosition.y).ToArray();

            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].transform.SetSiblingIndex(i);
            }
            if (stillMoving)
            {
                UpdateTrackUI(true, new List<Track>() { movedTrack });
            }
            else
            {
                UpdateTrackUI(true);
            }
        }
        public void UpdateTrackUI(bool smoothTransition, List<Track> exceptedTracks)
        {
            Track[] tracks = AllTracks;
            Transform[] gObjTransforms = new Transform[tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                Track tr = tracks[i];
                if (tr != null && exceptedTracks == null || !exceptedTracks.Contains(tr))
                {
                    gObjTransforms[i] = tracks[i].transform;
                }
                else
                {
                    gObjTransforms[i] = null;
                }
            }
            Vector3[] locPositions = Utils.SortUIVertically(gObjTransforms, 0.1f);
            Utils.MoveObjects(gObjTransforms, locPositions, smoothTransition);
        }
        public void UpdateTrackUI(bool smoothTransition)
        {
            UpdateTrackUI(smoothTransition, null);
        }
        // Slot UI
        /// <summary>
        /// Update slot UI components on all tracks
        /// </summary>
        public void MoveScaleSlotsUI()
        {
            //Calc min, max visible range
            int range = (int)(totalFrames / slotsEntirePanel.transform.localScale.x);
            int middle = (maxVisibleFrame + minVisibleFrame) / 2;
            minVisibleFrame = middle - range / 2;
            maxVisibleFrame = middle + range / 2;

            // Update UI
            UpdateAllTracksSlotsUI();
        }
        public void UpdateAllTracksSlotsUI()
        {
            Track[] tracks = AllTracks;
            foreach (Track tr in tracks)
            {
                tr.UpdateSlotsUI();
            }
        }
        //
        public void UpdateTotalFrames()
        {
            Track[] tracks = AllTracks;
            this.totalFrames = 0;
            foreach (Track tr in tracks)
            {
                if (tr.LastFrame > this.totalFrames)
                {
                    this.totalFrames = tr.LastFrame;
                }
            }

            maxVisibleFrame = (int)Mathf.Max(totalFrames, animationDurationSec * AnimationRecorder.instance.framePerSecond); //remove later
        }

        public Track CreateTrack()
        {
            GameObject trackObj = Instantiate(trackPrefab, tracksRoot.transform, false);

            string newName = NewTrackName();
            var tr = trackObj.GetComponent<Track>();
            tr.Initialize(newName);

            UpdateTrackUI(true);
            return tr;
        }

        public void CreateTrackByUI()
        {
            CreateTrack();
        }
        public void DeleteTrack(Track track)
        {
            DestroyImmediate(track.gameObject);
            UpdateTrackUI(true);
        }

        public void DeleteTrack(Track track, bool updateUI)
        {
            if (track == null) return;
            DestroyImmediate(track.gameObject);
            if (updateUI) UpdateTrackUI(true);
        }
        public void RenameTrack(Track track, string name)
        {
            track.trackName = name;
        }

        public Track GetTrack(string name) { 
            foreach (Track track in GetComponentsInChildren<Track>())
            {
                if (track.trackName == name)
                {
                    return track;
                }
            }
            return null;
        }

        public void PlayTracks(int absoluteFrameNum)
        {
            foreach (Track track in GetComponentsInChildren<Track>())
            {
                if (!track.hide) track.Play(absoluteFrameNum);
            }
        }

        public void StopPlaying()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
        }


        private string NewTrackName()
        {
            string newName = string.Empty;
            for (int i = 0; i < int.MaxValue; i++)
            {
                newName = "Track " + i;
                bool nameExists = false;
                foreach (Track tr in GetComponentsInChildren<Track>())
                {
                    if (tr.name == newName)
                    {
                        nameExists = true;
                        break;
                    }
                }
                if (!nameExists)
                {
                    break;
                }
            }
            return newName;
        }

        public void SetSelectAllTracks(bool select)
        {
            Track[] tracks = AllTracks;
            foreach (Track track in tracks)
            {
                track.selected = select;
            }
        }

        public void SetHideAllSelectedTracks(bool hide)
        {
            Track[] tracks = AllTracks;
            foreach (Track track in tracks)
            {
                if (track.selected)
                {
                    track.hide = hide;
                }
            }
        }

        public void SetLockAllSelectedTracks(bool locked)
        {
            Track[] tracks = AllTracks;
            foreach (Track track in tracks)
            {
                if (track.selected)
                {
                    track.locked = locked;
                }
            }
        }

        public void DeleteAllSelectedTracks()
        {
            Track[] tracks = AllTracks;
            for (int i = 0; i < tracks.Length; i++)
            {
                Track tr = tracks[i];
                if (tr.selected)
                {
                    DeleteTrack(tr, false);
                }
            }
            UpdateTrackUI(true);
        }
        public Track ClosestTrackYAxis(Vector3 localPos)
        {
            Track closestTrack = null;
            float minDistance = float.MaxValue;
            Track[] tracks = instance.AllTracks;
            foreach (Track track in tracks)
            {
                float distance = Mathf.Abs(instance.transform.InverseTransformPoint(track.transform.position).y - localPos.y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTrack = track;
                }
            }

            return closestTrack;
        }

        public int ConvertXPosToFrame(float x)
        {
            return (int)(minVisibleFrame + (x + 0.5) * (maxVisibleFrame - minVisibleFrame));
        }

        public float ConvertFrameToXPos(int frame)
        {
            return ((float)frame - minVisibleFrame) / (maxVisibleFrame - minVisibleFrame) - 0.5f;
        }

        //Temporary
        public void CreateSlotInEachTrack()
        {
            Track[] tracks = AllTracks;
            foreach (Track tr in tracks)
            {
                tr.CreateSlot();
            }
        }
        public void OpenInitializeEffectItemsUI(Slot slot)
        {
            if (currentSlot == slot && effectListUI != null && effectListUI.isActiveAndEnabled) return;
            var previousSlot = currentSlot;
            effectListUI?.Close();

            if (previousSlot != null)
            {
                previousSlot.SetSelected(false);
            }
            if (slot == null) return;
            currentSlot = slot;
            effectListUI.Initialize(currentSlot);
            currentSlot.SetSelected(true);
        }

        public void RefreshEffectItemsUI(Slot slot)
        {
            currentSlot = null;
            OpenInitializeEffectItemsUI(slot);
        }

        public void ShowHideEffectItemsUI(bool show)
        {
            effectListUI?.ShowHide(show);
        }

        internal void PlayReverseFirstRelativeFrame()
        {
            for (int frame = 0; frame < 10000; frame++)
            {
                foreach (Track track in GetComponentsInChildren<Track>())
                {
                    track.PlayReverseFirstRelativeFrame();
                }
            }
        }

        public void SelfAddEffect(ControllableObjectEffect effect)
        {
            var newTrack = CreateTrack();
            var newSlot = newTrack.CreateSlot();
            effect.gameObject.transform.parent = newSlot.gameObject.transform;
            UpdateTrackUI(newTrack);
            UpdateTotalFrames();
            UpdateAllTracksSlotsUI();
        }

        // Serialization and Deserialization
        public TracksManagerData GetData()
        {
            return new TracksManagerData(this);
        }
        public void LoadTracksData(TracksManagerData data)
        {
            // Clean the track manager editor
            SetSelectAllTracks(true);
            DeleteAllSelectedTracks();

            // Load the data
            foreach (TrackData trackData in data.tracksData)
            {
                Track newTrack = CreateTrack();
                newTrack.LoadTrackData(trackData);
            }
            AnimationRecorder.instance.framePerSecond = data.framePerSecond;
            animationDurationSec = data.animationDurationSec;
            UpdateTrackUI(true);
        }
    }
    [Serializable]
    public class TracksManagerData
    {
        public TrackData[] tracksData;
        public int framePerSecond;
        public float animationDurationSec;

        public TracksManagerData(TracksManager tracksManager)
        {
            var tracks = tracksManager.AllTracks;
            tracksData = new TrackData[tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                tracksData[i] = new TrackData(tracks[i]);
            }
            framePerSecond = AnimationRecorder.instance.framePerSecond;
            animationDurationSec = TracksManager.instance.animationDurationSec;
        }
    }
}