using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordXZPlaneTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransform1;
    [SerializeField] private Transform targetTransform2;
    [SerializeField] private Transform targetTransform3;
    [SerializeField] private string currentFolderPath = "C:\\user study\\analysis\\visualisation\\locations\\";
    private bool startedSaving = false;
    private StreamWriter _streamWriter1;
    private StreamWriter _streamWriter2;
    private StreamWriter _streamWriter3;
    void Update()
    {
        if (AnimationRecorder.instance.isPlaying)
        {
            FrameRecord();
        }
        else if (startedSaving)
        {
            // stop saving
            StopSaving();
        }
    }
    private void StopSaving()
    {
        if (startedSaving)
        {
            _streamWriter1.Close();
            _streamWriter2.Close();
            _streamWriter3.Close();
            _streamWriter1 = null;
            _streamWriter2 = null;
            _streamWriter3 = null;
            startedSaving = false;
        }
    }

    private void StartSaving()
    {
        if (startedSaving) return;
        try
        {
            _streamWriter1 = new StreamWriter(currentFolderPath + "character1.txt", true);
            _streamWriter2 = new StreamWriter(currentFolderPath + "character2.txt", true);
            _streamWriter3 = new StreamWriter(currentFolderPath + "character3.txt", true);
            startedSaving = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    private void FrameRecord()
    {
        if (!startedSaving)
        {
            StartSaving();
        }
        else
        {
            _streamWriter1.WriteLine(Time.time + "," + targetTransform1.position + "," + targetTransform1.forward);
            _streamWriter2.WriteLine(Time.time + "," + targetTransform2.position + "," + targetTransform2.forward);
            _streamWriter3.WriteLine(Time.time + "," + targetTransform3.position + "," + targetTransform3.forward);
        }
    }
}
