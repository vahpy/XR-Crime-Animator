using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TrackManager;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;

public class EffectMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject effectMenuItemPrefab = default;
    [SerializeField] private Transform effectsMenuPanel = default;

    void Start()
    {
        List<Type> effectTypes = GetAllDerivedTypes(typeof(ControllableObjectEffect));
        List<Transform> gObjTransfroms = new List<Transform>();
        foreach (Type effect in effectTypes)
        {
            GameObject g = Instantiate(effectMenuItemPrefab, effectsMenuPanel, false);
            
            var effectTypeName = effect.GetProperty("effectTypeName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            string name;
            Debug.Log(effectTypeName);
            if (effectTypeName!=null)
            {
                name = (string)effectTypeName.GetValue(null);
            }
            else
            {
                name = Utils.EffectNameBeautifier(effect.Name);
            }
                g.GetComponent<EffectMenuItemScript>().Initialize(name, effect);
            g.transform.localPosition = Vector3.zero;
            gObjTransfroms.Add(g.transform);
        }
        var gObjsArr = gObjTransfroms.ToArray();
        var locPositions = Utils.SortUIVertically(gObjsArr, 0.1f);
        Utils.MoveObjects(gObjsArr, locPositions, false);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        TracksManager.instance.ShowHideEffectItemsUI(true);

    }

    List<Type> GetAllDerivedTypes(Type baseType)
    {
        List<Type> derivedTypes = new List<Type>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                {
                    derivedTypes.Add(type);
                }
            }
        }

        return derivedTypes;
    }
}
