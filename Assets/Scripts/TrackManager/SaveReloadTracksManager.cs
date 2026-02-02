
using MixedReality.Toolkit.UX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;

namespace TrackManager
{
    public class SaveReloadTracksManager : MonoBehaviour
    {
        public static SaveReloadTracksManager instance;

        [SerializeField] private Transform characterEnvironment;
        [SerializeField] private Transform roomEnvironment;
        [DoNotSerialize] public bool save = false;
        [DoNotSerialize] public bool reload = false;
        [SerializeField] private DialogPool dialogPool;

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
        

        public void AskAndSave()
        {
            var dialog = dialogPool.Get().SetHeader("Save")
                .SetBody("Press \"Yes\" if you are sure that you want to save the current animated scene.")
            .SetPositive("Yes", SaveAllTracks)
            .SetNegative("No")
            .Show();
        }

        public void AskAndReload()
        {
            dialogPool.Get().SetHeader("Reload")
            .SetBody("Press \"Yes\" if you are sure that you want to reload the saved animated scene.")
            .SetPositive("Yes", ReloadAllTracks)
            .SetNegative("No")
            .Show();
        }
        public void ReloadByFilePath(string filepath)
        {
            if (filepath == null || filepath.Length == 0) return;
            if (!File.Exists(filepath))
            {
                Debug.LogWarning("No saved tracks data found to reload.");
                dialogPool.Get().SetHeader("Failed!")
                .SetBody("No saved data found in recording directory.")
                .SetNeutral("Ok")
                .Show();
            }
            else
            {
                string jsonInput = File.ReadAllText(filepath);
                TracksManagerData tracksData = JsonUtility.FromJson<TracksManagerData>(jsonInput);

                // Assuming TracksManager has a method to load data from TracksManagerData
                TracksManager.instance.LoadTracksData(tracksData);
                TracksManager.instance.UpdateTotalFrames();
                TracksManager.instance.UpdateAllTracksSlotsUI();
                Debug.Log("Tracks loaded from " + filepath);
                dialogPool.Get().SetHeader("Success")
                .SetBody("File successfully loaded from " + filepath)
                .SetNeutral("Ok")
                .Show();
            }
        }
        private void SaveAllTracks(DialogButtonEventArgs arg)
        {
            TracksManagerData tracksData = new TracksManagerData(TracksManager.instance);
            string output = JsonUtility.ToJson(tracksData, true);

            var _path = AnimationRecorder.instance.GetUniqueJSONFilePath();

            File.WriteAllText(_path, output);
            Debug.Log("Tracks saved to " + _path);
            dialogPool.Get().SetHeader("Success")
                .SetBody("File successfully saved at " + _path)
                .SetNeutral("Ok")
                .Show();
        }

        private void ReloadAllTracks(DialogButtonEventArgs arg)
        {
            // find latest file in directory of _path
            var files = AnimationRecorder.instance.GetRecentSavedTracks();

            if (files == null || files.Length == 0 || !File.Exists(files[0]))
            {
                Debug.LogWarning("No saved tracks data found to reload.");
                dialogPool.Get().SetHeader("Failed!")
                .SetBody("No saved data found in recording directory.")
                .SetNeutral("Ok")
                .Show();
            }
            else
            {
                string jsonInput = File.ReadAllText(files[0]);
                TracksManagerData tracksData = JsonUtility.FromJson<TracksManagerData>(jsonInput);

                // Assuming TracksManager has a method to load data from TracksManagerData
                TracksManager.instance.LoadTracksData(tracksData);
                TracksManager.instance.UpdateTotalFrames();
                TracksManager.instance.UpdateAllTracksSlotsUI();
                Debug.Log("Tracks loaded from " + files[0]);
                dialogPool.Get().SetHeader("Success")
                .SetBody("File successfully loaded from " + files[0])
                .SetNeutral("Ok")
                .Show();
            }
        }

        private void Update()
        {
            if (save)
            {
                save = false;
                SaveAllTracks(null);
            }
            if (reload)
            {
                reload = false;
                ReloadAllTracks(null);
            }
        }

        public ControllableObject FindGameObject(string name)
        {
            var sortedGameObjects = GetGameObjectsAndDepth();
            foreach (ControllableObject t in sortedGameObjects)
            {
                if (t.name == name)
                {
                    return t;
                }
            }

            return null;
        }

        private List<ControllableObject> GetGameObjectsAndDepth()
        {

            if (characterEnvironment == null && roomEnvironment) return null;
            var characterChildren = characterEnvironment.GetComponentsInChildren<ControllableObject>();
            var roomChildren = roomEnvironment.GetComponentsInChildren<ControllableObject>();
            var objectsDepth = new Dictionary<ControllableObject, int>();

            foreach (ControllableObject child in characterChildren)
            {
                if (child == characterEnvironment) continue;
                objectsDepth.Add(child, GetDepth(child.transform));
            }
            foreach (ControllableObject child in roomChildren)
            {
                if (child == roomEnvironment) continue;
                objectsDepth.Add(child, GetDepth(child.transform));
            }

            return objectsDepth.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
        }

        private static int GetDepth(Transform current)
        {
            int depth = 0;

            while (current.parent != null)
            {
                depth++;
                current = current.parent;
            }

            return depth;
        }
    }
}