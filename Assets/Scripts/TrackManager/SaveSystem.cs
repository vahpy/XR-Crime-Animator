using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private readonly string Path = Application.persistentDataPath + "/scenedata";
    private void SaveTracks()
    {

    }

    private void LoadData()
    {

    }


    private void OnApplicationQuit()
    {
        //save data
    }

    private void Awake()
    {
        //load data
    }
}
