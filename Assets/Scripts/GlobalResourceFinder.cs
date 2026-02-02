using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GlobalResourceFinder : MonoBehaviour
{
    public const string STR_CHEVRON = "chevron";
    public static GlobalResourceFinder instance;
    [SerializeField] public XRInteractionManager globalInteractionManager = default;
    [SerializeField] public GameObject explosionFirePrefab = default;
    [SerializeField] public GameObject fireWallPrefab = default;
    [SerializeField] public GameObject fireBoltPrefab = default;
    [SerializeField] public GameObject animationSpace = default;
    [SerializeField] public GameObject characterEnv = default;
    //props
    [SerializeField] public List<GameObject> propPrefabs = default;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public GameObject FindPrefabWithName(string name)
    {
        foreach(GameObject go in propPrefabs)
        {
            if (go.name.ToLower().Contains(name.ToLower()))
            {
                //Debug.Log("Found: " + go.name +" for "+name);
                return go;
            }
        }
        return null;
    }
}
