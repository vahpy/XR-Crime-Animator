using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TrackManager
{
    public class Track : MonoBehaviour
    {
        [SerializeField]
        private Transform trackContainer;
        [SerializeField]
        private GameObject slotPrefab = default;
        [SerializeField]
        private int defaultSlotDurationSec = 20;
        [SerializeField]
        private PressableButton selectBoxBtn = default;
        [SerializeField]
        private PressableButton lockBtn = default;
        [SerializeField]
        private PressableButton hideBtn = default;
        [SerializeField]
        public bool hide
        {
            get
            {
                return hideBtn.IsToggled;
            }
            set
            {
                hideBtn.ForceSetToggled(value);
            }
        }
        [SerializeField]
        public bool locked
        {
            get
            {
                return lockBtn.IsToggled;
            }
            set
            {
                lockBtn.ForceSetToggled(value);
            }
        }
        public string trackName
        {
            get
            {
                return name;
            }
            internal set
            {
                SetName(value);
            }
        }

        public bool selected
        {
            get
            {
                return selectBoxBtn.IsToggled;
            }
            set
            {
                selectBoxBtn.ForceSetToggled(value);
            }
        }
        /// <summary>
        /// Last slot's end frame, if no slot exists, returns -1
        /// </summary>
        public int LastFrame
        {
            get
            {
                return GetLastFrame();
            }
        }
        
        public Slot[] allSlots
        {
            get
            {
                return  trackContainer.GetComponentsInChildren<Slot>();
            }
        }
        //
        private Coroutine movingCoroutine;

        //
        public void Initialize(string name)
        {
            trackName = name;
        }
        public void SetHide(bool hide)
        {
            this.hide = hide;
            foreach (var slot in allSlots)
            {
                slot.SetDisabled(hide);
            }
        }

        public void SetLock(bool locked)
        {
            this.locked = locked;
            //
            
            foreach(var slot in allSlots)
            {
                if (locked)
                {
                    if (TracksManager.instance.currentSlot == slot)
                    {
                        TracksManager.instance.OpenInitializeEffectItemsUI(null);
                    }
                    slot.DeselectWhenLocked();
                }
                slot.EnableTrimming(!locked);
            }
            UpdateSlotsUI();
        }

        private void SetName(string newName)
        {
            if (newName == null) return;
            if (newName.Length == 0 || newName.Length > 10)
            {
                throw new System.ArgumentOutOfRangeException("newName", "A valid name is between 1 and 10 charachters.");
            }
            name = newName;
            // Updae UI Names
            var nameLabel = this.transform.Find("TrackNameLabel")?.GetComponent<TextMeshPro>();
            if (nameLabel != null)
            {
                nameLabel.text = newName;
            }
        }

        public void OrderSwapStarted()
        {
            if (movingCoroutine != null) StopCoroutine(movingCoroutine);
            movingCoroutine = StartCoroutine(MovingTrack());
        }

        IEnumerator MovingTrack()
        {
            while (true)
            {
                TracksManager.instance.UpdateOrder(this, true);
                yield return null;
            }
        }

        public void OrderSwapEnded()
        {
            if (movingCoroutine != null) StopCoroutine(movingCoroutine);
            TracksManager.instance.UpdateOrder(this, false);
        }

        internal void Play(int frame)
        {
            var slots = allSlots;
            if (slots == null) return;
            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot == null) continue;
                if (slot.startFrame > frame) break;
                //run this slot
                slot.PlayOneFrame(frame);
            }
        }

        public void DeleteThisTrack()
        {
            if (locked) return;
            TracksManager.instance.DeleteTrack(this);
        }


        // Slots management
        public Slot CreateSlot()
        {
            if(locked) return null;
            int initialLastFrame = this.LastFrame + 1;
            return CreateSlot(initialLastFrame, defaultSlotDurationSec * AnimationRecorder.instance.framePerSecond);
        }
        public Slot CreateSlot(int startFrame, int duration)
        {
            if (locked) return null;
            if (startFrame < 0) startFrame = 0;
            GameObject obj = Instantiate(slotPrefab, trackContainer);

            Slot slotObj = obj.GetComponent<Slot>();
            slotObj.Initialize(this, startFrame, startFrame + duration - 1);
            SortSlots();
            TracksManager.instance.UpdateTotalFrames();
            TracksManager.instance.UpdateAllTracksSlotsUI();
            return slotObj;
        }
        public bool InsertExistingSlot(Slot slot)
        {
            if (locked || slot == null) return false;
            if (slot.startFrame < 0) return false;
            if (!CheckNoOverlap(slot))
            {
                return false;
            }
            //L
            slot.transform.SetParent(trackContainer);
            slot.SetParentTrack(this);
            SortSlots();

            //UI
            var locPos = slot.transform.localPosition;
            locPos.y = 0;
            slot.transform.localPosition = locPos;

            //UI
            TracksManager.instance.UpdateTotalFrames();
            TracksManager.instance.UpdateAllTracksSlotsUI();
            return true;
        }
        public bool RemoveSlot(Slot slot)
        {
            if (locked) return false;
            return RemoveSlot(slot, false);
        }
        public bool RemoveSlot(Slot slot, bool destroyGameObject)
        {
            if (locked) return false;
            Slot[] slots = allSlots;
            if (slot != null && slots.Contains(slot))
            {
                //Remove
                slot.transform.parent = null;

                //Destroy
                if (destroyGameObject)
                {
                    DestroyImmediate(slot);
                }
                return true;
            }
            return false;
        }
        private void SortSlots()
        {
            Slot[] slots = allSlots;
            slots.OrderBy(slot => slot.startFrame);
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].transform.SetSiblingIndex(i);
            }
        }
        private bool CheckNoOverlap(Slot slot)
        {
            Slot[] slots = allSlots;
            foreach (Slot sl in slots)
            {
                if (sl != slot && slot.endFrame > sl.startFrame && slot.startFrame < sl.endFrame)
                {
                    return false;
                }
            }
            return true;
        }
        private int GetLastFrame()
        {
            Slot[] slots = trackContainer.transform.GetComponentsInChildren<Slot>();
            if (slots == null || slots.Length == 0) return -1;
            return slots[slots.Length - 1].endFrame;
        }
        // Slots UI Management
        public void ContainerClicked(SelectEnterEventArgs arg)
        {
            Vector3 interactionPoint = arg.interactorObject.transform.position;
            var interObj = arg.interactableObject;

            var localPos = interObj.transform.InverseTransformPoint(interactionPoint);
            if (TracksManager.instance.createSlotPressed)
            {
                //Debug.Log("Creating a slot on " + trackName + " at " + localPos + ", converted to frame:" + TracksManager.instance.ConvertXPosToFrame(localPos.x));
                var middleFrame = TracksManager.instance.ConvertXPosToFrame(localPos.x);
                CreateSlot((int)(middleFrame - defaultSlotDurationSec * AnimationRecorder.instance.framePerSecond / 2.0f), defaultSlotDurationSec * AnimationRecorder.instance.framePerSecond);
                TracksManager.instance.createSlotPressed = false;
            }
        }
        public void UpdateSlotsUI()
        {
            int minF = TracksManager.instance.minVisibleFrame;
            int maxF = TracksManager.instance.maxVisibleFrame;
            if (minF == maxF || minF < 0 || maxF < 0) return;
            Slot[] slots = allSlots;

            // Update Position and Scale
            foreach (var sl in slots)
            {
                sl.UpdateSlotUISizePos(true);
                sl.UpdateMaterial();
            }
        }
        public (int, int) GetFrameWindow()
        {
            return (TracksManager.instance.minVisibleFrame, TracksManager.instance.maxVisibleFrame);
        }

        public void SlotMoveStart(Slot slot)
        {
            if (locked) return;
            var locPos = slot.transform.localPosition;
            locPos.z -= 0.001f;
            slot.transform.localPosition = locPos;
        }
        public void SlotMoveEnd(Slot slot, Vector3 prePos, Vector3 preScale)
        {
            if (locked) return;
            var locPos = slot.transform.localPosition;
            locPos.z = 0;
            slot.transform.localPosition = locPos;
            int preStartFrame = slot.startFrame, preEndFrame = slot.endFrame;
            // Dst track
            var slot_trManSpace = TracksManager.instance.transform.InverseTransformPoint(slot.transform.position);
            Track dstTrack = TracksManager.instance.ClosestTrackYAxis(slot_trManSpace);
            if (dstTrack == null) dstTrack = this;
            //
            dstTrack.MoveSlotFrames(slot);
            if (dstTrack.InsertExistingSlot(slot))
            {
                dstTrack.UpdateSlotsUI();
            }
            else
            {
                slot.transform.localPosition = prePos;
                slot.transform.localScale = preScale;
                slot.startFrame = preStartFrame;
                slot.endFrame = preEndFrame;
            }
        }

        public void MoveSlotFrames(Slot slot)
        {
            if (locked) return;
            var sLocPos = slot.transform.localPosition;
            var newMiddle = TracksManager.instance.ConvertXPosToFrame(sLocPos.x);
            var preMiddle = (int)((slot.endFrame - slot.startFrame) / 2.0f + slot.startFrame);
            //string debugStr = "[x:" + sLocPos.x + "] S: " + slot.startFrame + ", E: " + slot.endFrame + " --- " + preMiddle + "=>" + newMiddle + " ---> ";
            slot.startFrame += newMiddle - preMiddle;
            slot.endFrame += newMiddle - preMiddle;
            if (slot.startFrame < 0)
            {
                slot.endFrame = slot.endFrame - slot.startFrame;
                slot.startFrame = 0;
            }
            //debugStr += "S: " + slot.startFrame + ", E: " + slot.endFrame;
            //Debug.Log(debugStr);
        }

        internal void PlayReverseFirstRelativeFrame()
        {
            Slot[] slots = allSlots;
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                var allEffects = allSlots[i].Effects;
                for (int j = 0; j < allEffects.Length; j++)
                {
                    allEffects[j].PlayRelativeFrame(0);
                }
            }
        }

        // Serialization and Deserialization
        internal TrackData GetDate()
        {
            TrackData trackData = new TrackData(this);
            return trackData;
        }
        internal void LoadTrackData(TrackData trackData)
        {
            this.name = trackData.name;
            foreach(SlotData slotData in trackData.slotsData)
            {
                Slot slot = CreateSlot();
                slot.LoadData(slotData);
            }
        }
    }

    [Serializable]
    public class TrackData
    {
        public string name;
        public SlotData[] slotsData;
        public TrackData(Track track)
        {
            if (track == null) return;
            name = track.name;
            var slots = track.allSlots;
            if (slots == null) return;
            slotsData = new SlotData[slots.Length];
            for(int i = 0; i < slotsData.Length; i++)
            {
                slotsData[i] = new SlotData(slots[i]);
            }
        }
    }
}