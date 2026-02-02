using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class LookRayHitCollector : MonoBehaviour
{
    [SerializeField] private string taskMainDirectory = default;
    [SerializeField] private string resultDirectory = default;
    [SerializeField] private Camera participantView;
    [SerializeField] private GameObject hitpointPrefab;
    [SerializeField] private GameObject hitpointCollection;

    [SerializeField] private int taskNumber;
    [SerializeField] private bool save;
    [SerializeField] private bool visualise;
    private List<Vector3> setOfHits;
    private Dictionary<int, string> files;
    private int currentParticipant;


    private bool isLoaded;
    private Pose[] poses;
    int frameNum = 0;
    void Start()
    {
        setOfHits = new();
        files = GetFiles(taskNumber);
        foreach (var file in files)
        {
            Debug.Log(file.Key + " : " + file.Value);
        }
        currentParticipant = 1;
        isLoaded = false;
    }

    private void Update()
    {
        if (!isLoaded)
        {
            if (!files.ContainsKey(currentParticipant))
            {
                currentParticipant++;
                return;
            }
            Debug.Log("Participant to be played: " + currentParticipant);
            poses = ReadPoses(files[currentParticipant]).ToArray();
            frameNum = 0;
            setOfHits.Clear();
            setOfHits = new();
            isLoaded = true;
        }
        if (frameNum < poses.Length)
        {
            var pose = poses[frameNum];
            this.transform.SetPositionAndRotation(pose.position, pose.rotation);
            participantView.transform.SetPositionAndRotation(pose.position, pose.rotation);
            var hitPos = LookRayHitPosition();
            if (hitPos != Vector3.zero)
            {
                if (taskNumber != 2 || this.transform.position.y > 5)
                    setOfHits.Add(hitPos);
            }
            frameNum++;
        }
        else
        {
            isLoaded = false;
            if (save) SaveParticipantResult(setOfHits, taskNumber, currentParticipant);
            if (visualise) VisualiseResults(setOfHits);
            currentParticipant++;
        }
    }

    private void VisualiseResults(List<Vector3> points)
    {
        if (hitpointPrefab == null || hitpointCollection == null) return;
        foreach (var hit in points)
        {
            var hitpoint = Instantiate(hitpointPrefab, hitpointCollection.transform, true);
            hitpoint.transform.position = hit;
        }
    }

    private Vector3 LookRayHitPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private List<Pose> ReadPoses(string filePath)
    {
        var poses = new List<Pose>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                poses.Add(ExtractPose(line));
            }
        }
        return poses;
    }

    private Pose ExtractPose(string line)
    {
        // Regex to extract content inside parentheses
        Regex regex = new Regex(@"\(([^\)]+)\)");
        var matches = regex.Matches(line);

        // Ensure we have at least two sets of vector data (position and rotation)
        if (matches.Count < 2)
        {
            throw new FormatException("Invalid data format: Expected position and rotation.");
        }

        // Helper function to parse a Vector3 from a string like "-24.31, 0.02, 3.61"
        Vector3 ParseVector3(string vectorString)
        {
            string[] parts = vectorString.Split(',');
            if (parts.Length != 3)
                throw new FormatException("Invalid vector format.");

            return new Vector3(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture)
            );
        }

        // Convert extracted text into Unity types
        Vector3 position = ParseVector3(matches[0].Groups[1].Value);
        Vector3 forward = ParseVector3(matches[1].Groups[1].Value);
        Quaternion rotation = Quaternion.LookRotation(forward);
        //Quaternion rotation = Quaternion.Euler(forward);

        return new Pose(position, rotation);
    }

    private Dictionary<int, string> GetFiles(int taskNumber)
    {
        string folderPath = taskMainDirectory + "\\task" + taskNumber.ToString();

        var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
        var filesList = new Dictionary<int, string>();


        foreach (var file in files)
        {
            // extract participant id

            string fileName = Path.GetFileName(file); // Extract only the file name
            string[] parts = fileName.Split('_');
            filesList.Add(int.Parse(parts[0]), file.ToString());
        }

        return filesList;
    }

    private void SaveParticipantResult(List<Vector3> points, int taskNumber, int participantID)
    {
        string lookHitFile = resultDirectory + "/task" + taskNumber + "/look_hits_" + participantID + ".txt";
        // save world position of each point in the file
        using (StreamWriter writer = new StreamWriter(lookHitFile))
        {
            foreach (Vector3 t in points)
            {
                writer.WriteLine(t.ToString());
            }
        }
        Debug.Log("Hit points for " + participantID + " stored at " + lookHitFile);
    }
}
