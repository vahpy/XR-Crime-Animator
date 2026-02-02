using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using TrackManager.Animation;

namespace TrackManager.BodyTracking.Meta
{

    public class BodyTrackingManager : MonoBehaviour
    {


        // Single instance
        private static BodyTrackingManager _instance;
        public static BodyTrackingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BodyTrackingManager>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(BodyTrackingManager).Name);
                        _instance = singletonObject.AddComponent<BodyTrackingManager>();
                    }
                }
                return _instance;
            }
        }

        // Functionality
        [SerializeField] private OVRBody avatarOVRBody;
        [SerializeField] private OVRBody[] characters;

        public bool isRecording { get; private set; }

        private GameObjectRecorder recorder;

        //private float timePassed;
        private int lastRecordedFrame = -1;
        private MetaBodyTrackingEffect currentEffect;
        void Start()
        {
            isRecording = false;

            avatarOVRBody.enabled = false;
            avatarOVRBody.gameObject.SetActive(false);
            foreach (var character in characters)
            {
                character.enabled = false;
                character.gameObject.SetActive(false);
            }
            //timePassed = 0;
            lastRecordedFrame = -1;
        }

        public void StartRecording( MetaBodyTrackingEffect effect)
        {
            if (isRecording)
            {
                Debug.LogWarning("Already recording. Stop recording first.");
                return;
            }
            if (effect.target.name.ToLower().Contains("soldier"))
            {
                foreach(var character in characters)
                {
                    if (character.name.ToLower().Contains("soldier"))
                    {
                        avatarOVRBody = character;
                    }
                }
            }
            else
            {
                foreach (var character in characters)
                {
                    if (!character.name.ToLower().Contains("soldier"))
                    {
                        avatarOVRBody = character;
                    }
                }
            }
            recorder = new GameObjectRecorder(avatarOVRBody.gameObject);
            recorder.BindComponentsOfType<Transform>(avatarOVRBody.gameObject, true);
            avatarOVRBody.gameObject.SetActive(true);
            avatarOVRBody.enabled = true;
            //timePassed = 0;
            currentEffect = effect;
            lastRecordedFrame = -1;
            isRecording = true;
        }

        public AnimationClip StopRecordingAndExport()
        {
            currentEffect = null;
            if (!isRecording)
            {
                Debug.LogWarning("Not recording. Start recording first.");
            }

            avatarOVRBody.enabled = false;
            avatarOVRBody.gameObject.SetActive(false);
            isRecording = false;

            if (recorder == null)
            {
                Debug.LogError("Game Object recorder is null. Recording failed.");
                return null;
            }
            else if (recorder.isRecording)
            {
                Debug.Log("Recording in progress...");
                var recordingAnimClip = new AnimationClip();
                //recorder.SaveToClip(recordingAnimClip);
                recorder.SaveToClip(recordingAnimClip, AnimationRecorder.instance.framePerSecond);

                avatarOVRBody.enabled = false;
                avatarOVRBody.gameObject.SetActive(false);
                isRecording = false;
                AssetDatabase.CreateAsset(recordingAnimClip, AnimationRecorder.instance.GetUniqueClipPath());

                return recordingAnimClip;
            }
            else
            {
                Debug.LogError("The recorder wasn't recording.");
                return null;
            }
        }

        private void LateUpdate()
        {
            if (!isRecording || recorder == null || currentEffect == null) return;
            //timePassed += Time.deltaTime;
            var currentFrame = currentEffect.GetRelativeFrameNum(AnimationRecorder.instance.absoluteFrameNum);
            if (currentFrame > lastRecordedFrame)
            {
                float timePassed;
                if (lastRecordedFrame == -1)
                {
                    lastRecordedFrame = 0;
                    recorder.TakeSnapshot(0);
                }

                timePassed = ((float)(currentFrame - lastRecordedFrame)) / AnimationRecorder.instance.framePerSecond;

                recorder.TakeSnapshot(timePassed);

                lastRecordedFrame = currentFrame;
            }
        }

        public Transform GetTrackedAvatarTransform()
        {
            return avatarOVRBody.transform;
        }
    }
}