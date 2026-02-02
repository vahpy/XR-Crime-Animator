using Codice.CM.Common.Tree;
using Codice.CM.Common;
using TrackManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(UserStudyManager))]
public class UserStudyManagerEditor : Editor
{
    private enum TaskNumber
    {
        Task1 = 1,
        Task2 = 2,
        Task3 =3 ,
        Task4 = 4
    }


    private TaskNumber taskNumber = TaskNumber.Task1;
    private int taskAttempt = 1;
    public override void OnInspectorGUI()
    {
        // Get a reference to the target class
        UserStudyManager myClass = (UserStudyManager)target;

        // Draw default inspector elements
        //DrawDefaultInspector();

        // Add custom GUI elements
        myClass.participantID = EditorGUILayout.TextField("Participant ID:", myClass.participantID);
        
        GUILayout.Label("Step 1", EditorStyles.boldLabel);

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("Load Animation"))
        {
            if (Application.isPlaying)
            {

                UserStudyManager.instance.LoadViewOnlyAnimation();
            }
        }
        if (GUILayout.Button("Start task 1"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StartTask(1);
            }
        }

        if (GUILayout.Button("Start task 2"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StartTask(2);
            }
        }

        if (GUILayout.Button("Start task 3"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StartTask(3);
            }
        }
        if (GUILayout.Button("-- STOP TASK --"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StopTask();
            }
        }




        GUILayout.Label("Step 2", EditorStyles.boldLabel);
        if (GUILayout.Button("Prepare for Act"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.PrepareSceneForUserToAct();
            }
        }
        if (GUILayout.Button("Start task 4"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StartTask(4);
            }
        }
        if (GUILayout.Button("-- STOP TASK --"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.StopTask();
            }
        }
        if (GUILayout.Button("Save Animation"))
        {
            if (Application.isPlaying)
            {
                SaveReloadTracksManager.instance.AskAndSave();
            }
        }

        // helper functions
        GUILayout.Label("Helper Functionalities (optional)", EditorStyles.boldLabel);
        if (GUILayout.Button("Play Animation"))
        {
            if (Application.isPlaying)
            {
                AnimationRecorder.instance.PlayProgrammatically();
            }
        }

        if (GUILayout.Button("Look from Witness View (not implemented)"))
        {
            if (Application.isPlaying)
            {

            }
        }
        if (GUILayout.Button("Create New Track-Slot-Effect"))
        {
            if (Application.isPlaying)
            {
                UserStudyManager.instance.CreateTrackSlotEffect();
            }
        }
        
        // REVIEW
        GUILayout.Label("Review User Interaction", EditorStyles.boldLabel);
        myClass.participantHeadObject = (GameObject)EditorGUILayout.ObjectField("Participant headset:", myClass.participantHeadObject, typeof(GameObject), true);
        myClass.system = (LocomotionSystem)EditorGUILayout.ObjectField("System", myClass.system, typeof(LocomotionSystem), true);
        taskNumber = (TaskNumber)EditorGUILayout.EnumPopup("Task Number: ", taskNumber);

        EditorGUILayout.BeginHorizontal();
        taskAttempt = EditorGUILayout.IntField("Task Attempt: ", taskAttempt);
        if (GUILayout.Button("-", GUILayout.Width(20)))
            taskAttempt = Mathf.Clamp(taskAttempt - 1, 1, 100);

        if (GUILayout.Button("+", GUILayout.Width(20)))
            taskAttempt = Mathf.Clamp(taskAttempt + 1, 1, 100);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Play User Head Pose"))
        {
            if (Application.isPlaying)
            {
                myClass.PlayReview((int)taskNumber, taskAttempt);
            }
        }
    }
}
