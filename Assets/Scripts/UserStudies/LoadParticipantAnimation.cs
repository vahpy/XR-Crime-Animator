using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TrackManager;
using UnityEngine;

public class LoadParticipantAnimation : MonoBehaviour
{
    [SerializeField] private List<string> participantTracks = default;
    [SerializeField] private int participantID = 1;

    public bool load;

    private void Start()
    {
        load = false;

        //Debug.Log("Finding animations");
        var listOfFiles = ExtractFinalAnimation("C:\\user study\\resutls\\");
        //Debug.Log("End animation");
        if (participantTracks == null) participantTracks = new List<string>();
        foreach (var animFile in listOfFiles)
        {
            participantTracks.Add(animFile.Value);
        }
    }

    private void Update()
    {
        if (load)
        {
            load = false;
            // load
            DeleteCreatedProps();
            Load(participantTracks[participantID - 1]);
        }
    }
    private bool Load(string filePath)
    {
        try
        {
            SaveReloadTracksManager.instance.ReloadByFilePath(filePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private void DeleteCreatedProps()
    {
        // delete all children of GlobalResourceFinder.instance.characterEnv
        Transform[] controlledObject = GlobalResourceFinder.instance.characterEnv.GetComponentsInChildren<Transform>();
        foreach (var prop in controlledObject)
        {
            if (prop.gameObject != GlobalResourceFinder.instance.characterEnv) DestroyImmediate(prop.gameObject);
        }
        var obj = Utils.FindDeepChildNameLike(GlobalResourceFinder.instance.animationSpace.transform, "knife_");
        if (obj != null) DestroyImmediate(obj.gameObject);
    }

    private Dictionary<int, string> ExtractFinalAnimation(string directoryPath)
    {
        var trackFilesDict = new Dictionary<int, string>();
        Regex trackFilePattern = new(@"Tracks_(\d+)\.json", RegexOptions.IgnoreCase);

        for (int i = 1; i <= 18; i++)
        {
            string subFolderPath = Path.Combine(directoryPath, i.ToString());
            if (!Directory.Exists(subFolderPath)) continue;

            var allSubFolders = Directory.GetDirectories(subFolderPath, "*", SearchOption.AllDirectories);
            int maxTrackNumber = -1;
            string maxTrackFilePath = null;

            foreach (var folder in allSubFolders.Append(subFolderPath)) // Include main folder
            {
                foreach (var file in Directory.GetFiles(folder, "Tracks_*.json"))
                {
                    Match match = trackFilePattern.Match(Path.GetFileName(file));
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int trackNumber))
                    {
                        if (trackNumber > maxTrackNumber)
                        {
                            maxTrackNumber = trackNumber;
                            maxTrackFilePath = file;
                        }
                    }
                }
            }

            if (maxTrackFilePath != null)
            {
                trackFilesDict[i] = maxTrackFilePath;
            }
        }

        //foreach (var kvp in trackFilesDict)
        //{
        //    Debug.Log($"Folder {kvp.Key}: {kvp.Value}");
        //}
        return trackFilesDict;
    }
}
