using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrackManager.Animation
{
    public class FireEffect : ControllableObjectEffect
    {
        public new static string effectTypeName => "Fire";
        public enum FireType
        {
            None,
            Explosion,
            FireWall
        }

        [SerializeField]
        private FireType fireType
        {
            get
            {
                if (explosionType && !fireWallType)
                {
                    return FireType.Explosion;
                }
                else if (!explosionType && fireWallType)
                {
                    return FireType.FireWall;
                }
                else
                {
                    return FireType.None;
                }
            }
        }
        [NonSerialized] public bool applyFire;
        [NonSerialized] public bool explosionType;
        [NonSerialized] public bool fireWallType;

        [SerializeField] public SortedList<int, bool> recordedApplyFire;
        [SerializeField] public SortedList<int, FireType> recordedFireType;


        private new void Start()
        {
            base.Start();
            recordedApplyFire = new SortedList<int, bool> { { 0, applyFire } };
            recordedFireType = new SortedList<int, FireType> { { 0, fireType } };
        }

        public override void PlayRelativeFrame(int relativeFrame)
        {
            if (recordedApplyFire.TryGetValue(relativeFrame, out bool fireOn))
            {
                SetApplyFire(fireOn);
            }
            if (recordedFireType.TryGetValue(relativeFrame, out FireType fireType))
            {
                SetFireType(fireType);
            }
        }

        public override void RecordKeyFrame(int absoluteFrameNum)
        {
            int relativeFrameNum = GetRelativeFrameNum(absoluteFrameNum);
            if (relativeFrameNum < 0 || absoluteFrameNum > GetSlot().endFrame) return;

            if (relativeFrameNum == 0)
            {
                recordedApplyFire[0] = applyFire;
                recordedFireType[0] = fireType;
            }
            recordedApplyFire[relativeFrameNum] = applyFire;
            recordedFireType[relativeFrameNum] = fireType;
        }

        public override bool SetFieldValue(string fieldName, object value)
        {

            switch (fieldName)
            {
                case "applyFire":
                    SetApplyFire((bool)value);
                    break;
                case "explosionType":
                    SetFireType("explosion", (bool)value);
                    break;
                case "fireWallType":
                    SetFireType("firewall", (bool)value);
                    break;
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


        public override bool SetTarget(ControllableObject target)
        {
            if(target == null)
            {
                return false;
            }
            this.target = target;
            return true;
            //recordedApplyFire[0]= applyFire;
            //recordedFireType.Add(0, fireType);
        }


        //private fields
        private void SetApplyFire(bool applyFire)
        {
            this.applyFire = applyFire;
            if (this.target != null)
            {
                if (fireType == FireType.Explosion)
                {
                    var explosionFireObj = Utils.FindDeepChild(this.target.transform, GlobalResourceFinder.instance.explosionFirePrefab.name+ "(Clone)");
                    if (explosionFireObj == null && applyFire)
                    {
                        Instantiate(GlobalResourceFinder.instance.explosionFirePrefab, this.target.transform, false);
                    }
                    else if (explosionFireObj != null && !applyFire)
                    {
                        Destroy(explosionFireObj.gameObject);
                    }
                }
                else if (fireType == FireType.FireWall)
                {
                    var firewallObj = Utils.FindDeepChild(this.target.transform, GlobalResourceFinder.instance.fireWallPrefab.name + "(Clone)");
                    if (firewallObj == null && applyFire)
                    {
                        Instantiate(GlobalResourceFinder.instance.fireWallPrefab, this.target.transform, false);
                    }
                    else if(firewallObj !=null && !applyFire)
                    {
                        Destroy(firewallObj.gameObject);
                    }
                }
            }
        }

        private void SetFireType(FireType type)
        {
            if (type == FireType.Explosion)
            {
                explosionType = true;
                fireWallType = false;
            } else if (type == FireType.FireWall)
            {
                explosionType = false;
                fireWallType = true;
            }
            else
            {
                explosionType = false;
                fireWallType = false;
            }
        }
        private void SetFireType(string typeName, bool enable)
        {
            switch (typeName)
            {
                case "explosion":
                    explosionType = enable;
                    break;
                case "firewall":
                    fireWallType = enable;
                    break;
            }
        }

        public override void RecordEachFrame(int absoluteFrameNum)
        {
        }

        public override string GetEffectName()
        {
            return effectTypeName;
        }
    }
}