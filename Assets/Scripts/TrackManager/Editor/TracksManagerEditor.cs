using System.Runtime.CompilerServices;
using TrackManager;
using UnityEditor;
using UnityEngine;

namespace TrackManagerEditor
{
    //[CustomEditor(typeof(TracksManager))]
    //public class TracksManagerEditor : Editor
    //{
    //    private int minVisible, maxVisible;
    //    public override void OnInspectorGUI()
    //    {
    //        TracksManager tracksManager = (TracksManager)target;
    //        tracksManager.trackPrefab = (GameObject)EditorGUILayout.ObjectField("Track Prefab", tracksManager.trackPrefab, typeof(GameObject), true);
    //        tracksManager.tracksRoot = (Transform)EditorGUILayout.ObjectField("Tracks Root", tracksManager.tracksRoot, typeof(Transform), true);
    //        tracksManager.slotsVisiblePanel = (Transform)EditorGUILayout.ObjectField("Slots Visible Root", tracksManager.slotsVisiblePanel, typeof(Transform), true);
    //        tracksManager.slotsEntirePanel = (SlotUIController)EditorGUILayout.ObjectField("Slots Entire Panel", tracksManager.slotsEntirePanel, typeof(SlotUIController), true);

    //        if (GUILayout.Button("Create a Track"))
    //        {
    //            tracksManager.CreateTrack();
    //        }
    //        if (GUILayout.Button("Rename a Track"))
    //        {
    //            tracksManager.RenameTrack(tracksManager.GetComponentInChildren<Track>(), "Hello Habibiytdasdsad");
    //        }
    //        if (GUILayout.Button("Delete a track"))
    //        {
    //            tracksManager.DeleteTrack(tracksManager.GetComponentInChildren<Track>());
    //        }
    //        //if (GUILayout.Button("Swap a track"))
    //        //{
    //        //    tracksManager.SwapTrack(tracksManager.GetComponentsInChildren<Track>()[2], 2);
    //        //}
    //        if (GUILayout.Button("Show a track Name"))
    //        {
    //            Debug.Log("Track Name: " + tracksManager.GetComponentInChildren<Track>().name);
    //            Debug.Log("Track Object Name: " + tracksManager.GetComponentInChildren<Track>().gameObject.name);
    //            Debug.Log("Track Transform Name: " + tracksManager.GetComponentInChildren<Track>().transform.name);

    //        }
    //        EditorGUILayout.Separator();
    //        if (GUILayout.Button("Update UI"))
    //        {
    //            tracksManager.UpdateTrackUI(false);
    //        }

    //        EditorGUILayout.Separator();
    //        if (GUILayout.Button("Create Slots"))
    //        {
    //            tracksManager.CreateSlotInEachTrack();
    //        }

    //        tracksManager.minVisibleFrame = EditorGUILayout.IntField("Min Visible Frame:", tracksManager.minVisibleFrame, GUILayout.MinWidth(0), GUILayout.MaxWidth(1000));
    //        tracksManager.maxVisibleFrame = EditorGUILayout.IntField("Max Visible Frame:", tracksManager.maxVisibleFrame, GUILayout.MinWidth(0), GUILayout.MaxWidth(1000));
    //        if (GUILayout.Button("Update Slots"))
    //        {
    //            tracksManager.UpdateAllTracksSlotsUI();
    //        }
    //    }
    //}
}