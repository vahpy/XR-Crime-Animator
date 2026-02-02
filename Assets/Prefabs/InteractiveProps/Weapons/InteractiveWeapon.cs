using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractiveWeapon : MonoBehaviour
{
    [SerializeField] private GameObject knifePrefab = default;
    [SerializeField] private GameObject pistolPrefab = default;
    public enum WeaponType
    {
        None,
        Knife,
        Pistol
    }
    [SerializeField] private WeaponType weaponType;

    public void Initialize(WeaponType type)
    {
        weaponType = type;
        switch(weaponType)
        {
            case WeaponType.Knife:
                Instantiate(knifePrefab, this.transform, false);
                break;
            case WeaponType.Pistol:
                Instantiate(pistolPrefab, this.transform, false);
                break;
        }
    }


}
