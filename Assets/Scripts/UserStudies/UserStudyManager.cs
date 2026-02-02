using System;
using System.IO;
using TrackManager;
using TrackManager.Animation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UserStudyManager : MonoBehaviour
{
    // SINGLETON
    public static UserStudyManager instance;
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

    public enum StudyStep
    {
        None,
        ViewOnly,
        Act
    }
    private int taskNumber = -1;
    private int[] taskAttempt = new int[] { 0, 0, 0, 0, 0 };
    public string participantID = "";
    public StudyStep currentStep = StudyStep.None;
    public GameObject participantHeadObject;
    public LocomotionSystem system;
    private string currentFileName;
    private string currentFilePathDir;
    private string currentFilePath;
    private StreamWriter _streamWriter;
    private StreamReader _streamReader;
    private bool isReviewing = false;

    
    void Update()
    {
        if (isReviewing)
        {
            if (_streamReader != null && !_streamReader.EndOfStream)
            {
                string line = _streamReader.ReadLine();
                ApplyRecordedDataToHeadsetPose(line);
            }
            else isReviewing = false;
        }
        else if (currentStep != StudyStep.None && taskNumber > -1)
        {
            RecordHeadsetPose();
            if (participantHeadObject != null && participantHeadObject.activeSelf) participantHeadObject.SetActive(false);
        }
        if (!isReviewing && _streamReader != null)
        {
            CloseReaderStream();
        }
    }

    private void OnApplicationQuit()
    {
        CloseWriting();
    }

    /// <summary>
    /// task number is 1 based, 1 to 4
    /// </summary>
    /// <param name="number"></param>
    public void StartTask(int number)
    {
        isReviewing = false;
        Debug.Log("Trigger task " + number + " for " + participantID + "current step:" + currentStep);
        if (taskNumber != -1)
        {
            if (taskNumber == number) return;
            else StopTask();
        }
        taskNumber = -1;
        if (
              (number <= 0 || number >= 5) ||
              (number <= 3 && currentStep != StudyStep.ViewOnly) ||
              (number >= 3 && currentStep != StudyStep.Act)
           )
        {
            Debug.LogWarning("Wrong condition has been chosen for Task " + number);
        }
        
        taskNumber = number;
        taskAttempt[taskNumber]++;
        
        StartWriting();
    }
    public void StopTask()
    {
        isReviewing = false;
        taskNumber = -1;
        CloseWriting();
    }
    public void LoadViewOnlyAnimation()
    {
        if (TracksManager.instance.AllTracks == null || TracksManager.instance.AllTracks.Length == 0)
        {
            
        string relativePath = Path.Combine("Assets", "Recordings", "AnimationClips", "Clips_04-02-2025_4", "Tracks_1.json");
        SaveReloadTracksManager.instance.ReloadByFilePath(relativePath);
        }
        
        currentStep = StudyStep.ViewOnly;
        // Move the headset to the end of hallway
        if (system != null)
        {
            var tran = system.xrOrigin.Origin.transform;
            tran.position = new Vector3(-25.38f, tran.position.y, 3.46f);
            tran.rotation = Quaternion.Euler(0, 94.38f, 0);
        }
        //if (alignment != null) alignment.EnabeBatControleld(true);
    }

    public void PrepareSceneForUserToAct()
    {
        //if (taskNumber <= 0 || taskNumber >= taskAttempt.Length) return;
        //taskAttempt[taskNumber]++;
        
        if (TracksManager.instance.AllTracks == null || TracksManager.instance.AllTracks.Length == 0)
        {
            string relativePath = Path.Combine("Assets", "Recordings", "AnimationClips", "Clips_15-10-2024_2", "Tracks_2.json");
            SaveReloadTracksManager.instance.ReloadByFilePath(relativePath);
        }
        if (TracksManager.instance.AllTracks != null && TracksManager.instance.AllTracks.Length > 0)
        {
            var track0 = TracksManager.instance.GetTrack("Track 0");
            if (track0 != null)
                TracksManager.instance.DeleteTrack(track0, true);
        }

        currentStep = StudyStep.Act;
        //if(alignment!=null) alignment.EnabeBatControleld(false);
    }

    private void RecordHeadsetPose()
    {
        try
        {
            _streamWriter.WriteLine(Time.time + "," + Camera.main.transform.position + "," + Camera.main.transform.forward);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void StartWriting()
    {
        if (!UpdateFileName()) return;
        try
        {
            if (!Directory.Exists(currentFilePathDir))
            {
                Directory.CreateDirectory(currentFilePathDir);
            }
            _streamWriter = new StreamWriter(currentFilePath, true);
            Debug.Log("** USER STUDY => " + participantID + " started " + taskNumber + " is recorded in " + currentFilePath); ;
            //_streamWriter = new StreamWriter(currentFilePath, true);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void CloseWriting()
    {
        if (_streamWriter == null) return;
        Debug.Log("** USER STUDY => " + currentFilePath + "is being saved.");
        try
        {
            _streamWriter.Close();
            _streamWriter = null;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private bool UpdateFileName()
    {
        if (participantID == null || participantID.Length == 0) return false;
        currentFilePathDir = Application.dataPath + "/userstudy/" + participantID + "/";
        try
        {
            currentFileName = participantID + "_" + taskNumber + "_" + taskAttempt[taskNumber];
            currentFilePath = Path.Combine(currentFilePathDir, currentFileName);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
    private void CloseReaderStream()
    {
        try
        {
            if (_streamReader == null) return;
            _streamReader.Close();
            _streamReader = null;
            isReviewing = false;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    public void PlayReview(int taskNumber, int taskAttempt)
    {
        if (isReviewing)
        {
            CloseReaderStream();
        }

        if (participantID == null || participantID.Length == 0) return;
        Debug.Log("** USER STUDY Reviewing interaction for " + participantID + ", task " + taskNumber + ", attempt " + taskAttempt);
        //check if file exists
        UpdateFileName();
        string reviewFileName = participantID + "_" + taskNumber + "_" + taskAttempt;
        string reviewFilePath = Path.Combine(currentFilePathDir, reviewFileName);
        if (!File.Exists(reviewFilePath))
        {
            Debug.Log("** USER STUDY such task or attempt doesn't exist for this participant.");
            return;
        }
        // read file line by line
        try
        {
            _streamReader = new StreamReader(reviewFilePath);
            isReviewing = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

    }

    private void ApplyRecordedDataToHeadsetPose(string line)
    {
        if (line == null || line.Length <= 0) return;
        if (participantHeadObject != null && !participantHeadObject.activeSelf) participantHeadObject.SetActive(true);
        // parse line
        string[] parts = line.Replace("(", "").Replace(")", "").Split(',');
        float time = float.Parse(parts[0]);
        Vector3 position = new Vector3(
            float.Parse(parts[1]),
            float.Parse(parts[2]),
            float.Parse(parts[3])
        );
        Vector3 forward = new Vector3(
            float.Parse(parts[4]),
            float.Parse(parts[5]),
            float.Parse(parts[6])
        );

        participantHeadObject.transform.position = position;
        participantHeadObject.transform.forward = forward;
    }

    public  void CreateTrackSlotEffect()
    {
        var tracks = TracksManager.instance.AllTracks;
        if(tracks == null || tracks.Length<2) return;
        var track0 = TracksManager.instance.GetTrack("Track 0");
        if(track0!=null) TracksManager.instance.DeleteTrack(track0);
        var track2 = TracksManager.instance.GetTrack("Track 2");


        var newTrack0 = TracksManager.instance.CreateTrack();
        newTrack0.trackName = "Track 0";
        var slot = newTrack0.CreateSlot(0,10*AnimationRecorder.instance.framePerSecond);
        if(track2!= null)
        {
            slot.SetFrameInterval(0, track2.LastFrame);
        }
        var effComp = (MetaBodyTrackingEffect) slot.AddEffect(typeof(MetaBodyTrackingEffect));
        var character = Utils.FindDeepChildNameLike(GlobalResourceFinder.instance.animationSpace.transform,"Blue");

        effComp.SetTarget(character.GetComponent<ControllableObject>());
        TracksManager.instance.UpdateAllTracksSlotsUI();
    }
}
