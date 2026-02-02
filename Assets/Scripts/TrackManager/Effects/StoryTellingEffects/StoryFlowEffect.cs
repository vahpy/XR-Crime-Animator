using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrackManager;
using TrackManager.Animation;
using static Microsoft.MixedReality.GraphicsTools.MeshInstancer;

namespace TrackManager.StoryTelling
{
    public class StoryFlowEffect : ControllableObjectEffect
    {
        public new static string effectTypeName => "Floating Arrows";
        [SerializeField] public ControllableObject source;
        private ChainOfArrows arrows;

        public override void PlayRelativeFrame(int relativeFrame)
        {

        }

        public override void RecordEachFrame(int absoluteFrameNum)
        {

        }

        public override void RecordKeyFrame(int absoluteFrameNum)
        {

        }

        public override bool SetFieldValue(string fieldName, object value)
        {
            switch (fieldName)
            {
                case "source":
                    if (value is ControllableObject)
                    {
                        return SetSource((ControllableObject)value);
                    }
                    else
                    {
                        return false;
                    }
                case "target":
                    if (value is ControllableObject)
                    {
                        return SetTarget((ControllableObject)value);
                    }
                    else
                    {
                        return false;
                    }
            }
            return true;
        }
        private void CreateArrows()
        {
            if (source != null && target != null)
            {

                if (arrows == null)
                {
                    var newArrows = new GameObject();
                    newArrows.transform.parent = AnimationRecorder.instance.GetAnimationSpace();
                    arrows = newArrows.AddComponent<ChainOfArrows>();
                    arrows.arrowPrefab = GlobalResourceFinder.instance.FindPrefabWithName(GlobalResourceFinder.STR_CHEVRON);
                }
                arrows.target = target.transform;
                arrows.source = source.transform;
            }
        }

        public override bool SetTarget(ControllableObject target)
        {
            this.target = target;
            CreateArrows();
            return true;
        }

        public bool SetSource(ControllableObject source)
        {
            this.source = source;
            CreateArrows();
            return true;
        }

        private void Update()
        {
            if (arrows == null) return;
            int relFrame = GetRelativeFrameNum(AnimationRecorder.instance.absoluteFrameNum);
            
            if (relFrame>=0 && relFrame<=GetSlot().FramesCount)
            {
                arrows.gameObject.SetActive(true);
            }
            else
            {
                arrows.gameObject.SetActive(false);
            }
        }

        public override string GetEffectName()
        {
            return effectTypeName;
        }
    }
}