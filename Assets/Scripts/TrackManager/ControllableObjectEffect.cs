using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TrackManager.Animation
{
    public abstract class ControllableObjectEffect : MonoBehaviour //, IControlledObjectEffect
    {
        [DoNotSerialize] public ControllableObject target;
        public static string effectTypeName => "Effect";
        //protected string effectDisplayName;
        [DoNotSerialize] private Slot slot;

        protected void Start()
        {
            slot = this.GetComponent<Slot>();
            if (slot == null) Debug.LogError("Effect's slot is null!");
        }
        /// <summary>
        /// for continuous playing of the effect, do not include 0 or FramesCount since
        /// they are played even if it's not in the range
        /// </summary>
        /// <param name="relativeFrame"></param>
        public abstract void PlayRelativeFrame(int relativeFrame);
        public abstract void RecordKeyFrame(int absoluteFrameNum);
        public abstract void RecordEachFrame(int absoluteFrameNum);

        public abstract bool SetTarget(ControllableObject target);
        public ControllableObject GetTarget()
        {
            return target;
        }

        public abstract string GetEffectName();

        public int GetRelativeFrameNum(int absoluteFrameNum)
        {
            if (slot == null) slot = this.GetComponent<Slot>();
            return absoluteFrameNum - slot.startFrame;
        }

        public bool IsInSlot(int absoluteFrame)
        {
            if (slot != null && slot.startFrame <= absoluteFrame && slot.endFrame >= absoluteFrame) return true;
            return false;
        }

        public bool IsInSlot()
        {
            return IsInSlot(AnimationRecorder.instance.absoluteFrameNum);
        }

        public Slot GetSlot()
        {
            if (slot == null)
            {
                slot = this.GetComponent<Slot>();
            }
            return slot;
        }
        protected void SetSlot(Slot slot)
        {
            this.slot = slot;
        }
        protected void AdjustSlotDuration(float durationTime)
        {
            var duration = Mathf.CeilToInt(durationTime * AnimationRecorder.instance.framePerSecond);
            duration = Mathf.Max(duration, GetSlot().FramesCount);
            var start = GetSlot().startFrame;
            GetSlot().SetFrameInterval(start, start + duration);
        }
        public abstract bool SetFieldValue(string fieldName, object value);
        //public abstract bool SaveAsFile(string filePath);
        //public abstract ControlledObjectEffect LoadFromFile(string filePath);

        internal void LoadData(ControlledObjectEffectData effectData)
        {
            if (effectData == null) return;
            this.name = effectData.name;
            if (effectData.targetName != null)
            {
                var obj = SaveReloadTracksManager.instance.FindGameObject(effectData.targetName);

                if (obj == null)
                {
                    // probably the object has been created by the prop menu, so recreate based on the name
                    obj = PropsMenuUI.Instance.RecreatePropNonStandardName(effectData.targetName);
                }
                SetTarget(obj);
            }
            
            if (this is EffectsWithAnimation effect)
            {
                effect.LoadAnimationClip(effectData.animClipPath);
                effect.initialLocalPosition = effectData.initialPosition;
                effect.initialLocalRotation = effectData.initialRotation;
                effect.initialLocalScale = effectData.initialScale;
                effect.initialWorldPosition = effectData.initialWorldPosition;
                effect.initialWorldRotation = effectData.initialWorldRotation;
                if (this is IntPropInteractionEffect intEffect)
                {
                    intEffect.LoadInteractionStates(ControlledObjectEffectData.IntPairListToDictionaryConvertor(effectData.interactionStatePerFrame));
                }
            }
        }
    }
    [Serializable]
    public class ControlledObjectEffectData
    {
        public string name;
        public string targetName;
        public string effectType;
        public string animClipPath;
        public List<IntPair> interactionStatePerFrame;
        public Vector3 initialPosition;
        public Quaternion initialRotation;
        public Vector3 initialScale;
        public Vector3 initialWorldPosition;
        public Quaternion initialWorldRotation;

        public ControlledObjectEffectData(ControllableObjectEffect effect)
        {
            if (effect == null) return;

            name = effect.name;
            if (effect.target != null) targetName = effect.target.name;
            effectType = effect.GetType().FullName;
            if (effect is EffectsWithAnimation)
            {
                EffectsWithAnimation eff = (EffectsWithAnimation)effect;
                animClipPath = eff.GetAnimClipPath();
                initialPosition = eff.initialLocalPosition;
                initialRotation = eff.initialLocalRotation;
                initialScale = eff.initialLocalScale;
                initialWorldPosition = eff.initialWorldPosition;
                initialWorldRotation = eff.initialWorldRotation;
                if (effect is IntPropInteractionEffect)
                {
                    interactionStatePerFrame = DictionaryToIntPairListConvertor(((IntPropInteractionEffect)effect).GetRecordedInteractionStates());
                }
            }
        }

        public static List<IntPair> DictionaryToIntPairListConvertor(Dictionary<int, InteractiveProp.InteractionState> list)
        {
            List<IntPair> intList = new List<IntPair>();
            foreach (var item in list)
            {
                intList.Add(new IntPair(item.Key, (int)item.Value));
            }
            return intList;
        }

        public static Dictionary<int, InteractiveProp.InteractionState> IntPairListToDictionaryConvertor(List<IntPair> list)
        {
            Dictionary<int, InteractiveProp.InteractionState> dictionary = new Dictionary<int, InteractiveProp.InteractionState>();
            foreach (var item in list)
            {
                dictionary.Add(item.a, (InteractiveProp.InteractionState)item.b);
            }
            return dictionary;
        }

        [Serializable]
        public class IntPair
        {
            public int a;
            public int b;

            public IntPair(int item1, int item2)
            {
                a = item1;
                b = item2;
            }
        }
    }


}