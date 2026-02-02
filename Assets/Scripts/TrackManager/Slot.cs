using MixedReality.Toolkit.SpatialManipulation;
using System;
using TMPro;
using TrackManager.Animation;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;

namespace TrackManager
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Track parentTrack;
        [SerializeField] private Material defaultMat = default;
        [SerializeField] private Material highlightMat = default;
        [SerializeField] private Material disabledMat = default;
        [SerializeField] private Material selectedMat = default;
        [SerializeField] private TextMeshPro slotLabel = default;
        [SerializeField] private GameObject trimStart = default;
        [SerializeField] private GameObject trimEnd = default;
        [SerializeField] private GameObject interactablePanel = default;

        private Vector3 prePos;
        private Vector3 preScale;
        private bool isTrimming = false;

        public bool isHovered { get; private set; }
        public bool isSelected { get; private set; }
        public bool isDisabled { get; private set; }
        /// <summary>
        /// Start Frame (Inclusive)
        /// </summary>
        [SerializeField, HideInInspector]
        public int startFrame;// { private set; get; }
        /// <summary>
        /// End Frame (Inclusive)
        /// </summary>
        [SerializeField, HideInInspector]
        public int endFrame;// { private set; get; }
        [SerializeField, HideInInspector]
        public ControllableObjectEffect[] Effects
        {
            get
            {
                return this.GetComponents<ControllableObjectEffect>();
            }
            private set
            {
            }
        }
        public int FramesCount
        {
            get
            {
                return endFrame - startFrame + 1;
            }
        }

        public bool isMoving;
        public ObjectManipulator manipulator;

        public void Initialize(Track parent, int startFrame, int endFrame)
        {
            parentTrack = parent;
            this.startFrame = startFrame;
            this.endFrame = endFrame;

            //initialScale = interactablePanel.transform.localScale;

            var temp = parentTrack.transform.localScale;
            temp.y = 1;
            transform.localScale = temp;

            isMoving = false;
            manipulator = interactablePanel.GetComponent<ObjectManipulator>();
            //slotLabel.transform.localScale = new Vector3(1 / (parentTrack.transform.localScale.y * transform.localScale.x),1, 1);
            //Debug.Log("Start:" + startFrame + ", End: " + endFrame);
            //this.AddComponent<LocalTransformEffect>();
            //this.AddComponent<FallingObjectEffect>();
            ////this.AddComponent<FireEffect>();
            //this.AddComponent<BodyTrackingEffect>();
            //this.AddComponent<StoryFlowEffect>();
            UpdateText();
        }

        private void Update()
        {
            if (isTrimming)
            {
                UpdateAllTrimmedEffects();
            }
            if (isMoving && AnimationRecorder.instance.IsAnyGripTriggerPressing)
            {
                manipulator.AllowedManipulations = MixedReality.Toolkit.TransformFlags.Move;
            }
            else
            {
                manipulator.AllowedManipulations = MixedReality.Toolkit.TransformFlags.None;
            }
        }

        /// <summary>
        /// Change the slot's duration
        /// </summary>
        /// <param name="start">Absolute start frame number</param>
        /// <param name="end">Absolute end frame number</param>
        /// <returns></returns>
        public bool SetFrameInterval(int start, int end)
        {
            if (parentTrack.locked) return false;
            if (end >= start && start >= 0)
            {
                this.startFrame = start;
                this.endFrame = end;
                parentTrack.UpdateSlotsUI();
                Debug.Log("Slot new start:" + this.startFrame + ", end:" + this.endFrame);
                return true;
            }
            return false;
        }

        public Component AddEffect(Type effect)
        {
            if (parentTrack.locked) return null;
            if (effect != null && effect.IsSubclassOf(typeof(ControllableObjectEffect)))
            {
                return gameObject.AddComponent(effect);
            }
            return null;
        }

        public void PlayOneFrame(int frame)
        {
            if (frame >= startFrame && frame <= endFrame)
            {
                foreach (ControllableObjectEffect effect in Effects)
                {
                    effect.PlayRelativeFrame(frame - startFrame);
                }
            }
        }

        //UI
        public void MoveStart()
        {
            if (parentTrack.locked) return;
            isMoving = true;
            //
            if (TracksManager.instance.deleteSlotPressed)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            //
            prePos = this.transform.localPosition;
            preScale = this.transform.localScale;
            parentTrack.SlotMoveStart(this);
            TracksManager.instance.OpenInitializeEffectItemsUI(this);
            UpdateMaterial();
        }
        public void MoveEnd()
        {
            isMoving = false;
            parentTrack.SlotMoveEnd(this, prePos, preScale);
            UpdateMaterial();
        }

        private void UpdateText()
        {
            //Debug.Log("Updating text for " + this.gameObject.name);
            var text = "";
            if (Effects == null || Effects.Length == 0) text = "Empty";
            else
            {
                foreach (var effect in Effects)
                {
                    text += (effect.target == null ? "" : effect.target.name) + " (" + effect.GetEffectName() + ")" + ", ";
                }
            }
            slotLabel.text = text;
        }

        public void UpdateMaterial()
        {

            if (isSelected)
            {
                interactablePanel.gameObject.GetComponent<Renderer>().sharedMaterial = selectedMat;
                return;
            }

            if (isDisabled)
            {
                interactablePanel.gameObject.GetComponent<Renderer>().sharedMaterial = disabledMat;
            }
            else if (isHovered)
            {
                interactablePanel.gameObject.GetComponent<Renderer>().sharedMaterial = highlightMat;
            }
            else
            {
                interactablePanel.gameObject.GetComponent<Renderer>().sharedMaterial = defaultMat;
            }
            UpdateText();
        }
        public void SetHover(bool hovered)
        {
            if (parentTrack.locked) return;
            isHovered = hovered;
            UpdateMaterial();
        }
        public void SetSelected(bool selected)
        {
            if (parentTrack.locked) return;
            isSelected = selected;
            UpdateMaterial();
        }

        public void DeselectWhenLocked()
        {

            isSelected = false;
            isHovered = false;
            UpdateMaterial();
        }

        public void EnableTrimming(bool enabled)
        {
            var startManipulator = trimStart.GetComponent<ObjectManipulator>();
            var endManipulator = trimEnd.GetComponent<ObjectManipulator>();
            if (startManipulator != null) startManipulator.enabled = enabled;
            if (endManipulator != null) endManipulator.enabled = enabled;
        }
        public void SetDisabled(bool disabled)
        {
            //disable targets
            foreach (ControllableObjectEffect effect in Effects)
            {
                if (effect.target != null)
                    effect.target.gameObject.SetActive(!disabled);
            }
            isDisabled = disabled;
            UpdateMaterial();
        }

        public void SetParentTrack(Track track)
        {
            this.parentTrack = track;
        }

        public void DeleteAllEffects()
        {
            foreach (ControllableObjectEffect effect in Effects)
            {
                Destroy(effect);
            }
        }

        public void StartTrimming(SelectEnterEventArgs args)
        {
            if (parentTrack.locked) return;
            isTrimming = true;
        }
        public void EndTrimming(SelectExitEventArgs args)
        {
            isTrimming = false;
        }

        public Transform GetInteractablePanel()
        {
            return interactablePanel.transform;
        }

        /// <summary>
        /// not implemented completely, only updates the start and end frame, but not the effects
        /// </summary>
        public void UpdateAllTrimmedEffects()
        {
            startFrame = TracksManager.instance.ConvertXPosToFrame(transform.parent.InverseTransformPoint(trimStart.transform.position).x);
            endFrame = TracksManager.instance.ConvertXPosToFrame(transform.parent.InverseTransformPoint(trimEnd.transform.position).x);

            UpdateSlotUISizePos(false); // first and second can be anything, because it is not used in this case
        }

        /// <summary>
        /// Moves the trim start and end trims to the given positions in track space, if updateTrimmerPos is set (only affects the x position)
        /// </summary>
        /// <param name="startUIPosx"></param>
        /// <param name="endUIPosx"></param>
        /// <param name="updateTrimmersPos"></param>
        public void UpdateSlotUISizePos(bool updateTrimmersPos)
        {
            var startTrimWPos = trimStart.transform.position;
            var endTrimWPos = trimEnd.transform.position;
            if (updateTrimmersPos)
            {
                var convertedToWorld = TracksManager.instance.ConvertFrameToXPos(startFrame);
                var tempx = transform.parent.TransformPoint(convertedToWorld, 0, 0);
                var startUIPosx = transform.InverseTransformPoint(tempx).x;

                var endUIPosx = transform.InverseTransformPoint(transform.parent.TransformPoint(TracksManager.instance.ConvertFrameToXPos(endFrame), 0, 0)).x;
                var tempPos = trimStart.transform.localPosition;
                tempPos.x = startUIPosx;
                trimStart.transform.localPosition = tempPos;
                tempPos = trimEnd.transform.localPosition;
                tempPos.x = endUIPosx;
                trimEnd.transform.localPosition = tempPos;
            }
            var slotScale = interactablePanel.transform.localScale;
            slotScale.x = Mathf.Abs(trimEnd.transform.localPosition.x - trimStart.transform.localPosition.x);
            interactablePanel.transform.localScale = slotScale;

            var size = slotLabel.rectTransform.sizeDelta;
            size.x = slotScale.x * 19.5f;
            slotLabel.rectTransform.sizeDelta = size;

            var temp = (trimStart.transform.position + trimEnd.transform.position) / 2.0f;
            transform.localPosition = new Vector3(transform.parent.InverseTransformPoint(temp).x, 0, -0.001f);
            if (!updateTrimmersPos)
            {
                trimStart.transform.position = startTrimWPos;
                trimEnd.transform.position = endTrimWPos;
            }
        }

        internal void LoadData(SlotData slotData)
        {
            this.startFrame = slotData.startFrame;
            this.endFrame = slotData.endFrame;
            foreach (ControlledObjectEffectData effectData in slotData.effectsData)
            {
                //Debug.Log("Effect type:" + Type.GetType(effectData.effectType));
                ControllableObjectEffect effect = (ControllableObjectEffect)AddEffect(Type.GetType(effectData.effectType));

                effect.LoadData(effectData);
            }
            UpdateText();
        }
    }
    [Serializable]
    public class SlotData
    {
        public int startFrame;
        public int endFrame;
        public ControlledObjectEffectData[] effectsData;
        public SlotData(Slot slot)
        {
            startFrame = slot.startFrame;
            endFrame = slot.endFrame;
            var effects = slot.Effects;
            effectsData = new ControlledObjectEffectData[effects.Length];
            for (int i = 0; i < effectsData.Length; i++)
            {
                effectsData[i] = new ControlledObjectEffectData(effects[i]);
            }
        }
    }
}