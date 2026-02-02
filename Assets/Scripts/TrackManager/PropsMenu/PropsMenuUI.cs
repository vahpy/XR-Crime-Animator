using MixedReality.Toolkit.SpatialManipulation;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TrackManager.Animation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PropsMenuUI : MonoBehaviour
{
    private static int propUID = 0;
    [SerializeField] private AttachObjectToHand attachObjectToHandManager;

    private static PropsMenuUI _instance;

    public static PropsMenuUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PropsMenuUI>(true);

                if (_instance == null)
                {
                    Debug.LogError("No instance of PropsMenuUI found in the scene.");
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }



    public void ShowHide()
    {
        if (gameObject.activeSelf || AnimationRecorder.instance.isRecording)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void CloseBtnPressed()
    {
        HidePropsManagerUI();
    }

    public void PropSelected(SelectEnterEventArgs args)
    {
        var propName = args.interactableObject.GetAttachTransform(args.interactorObject).gameObject.name;
        try
        {
            CreateProp(propName);
        }
        catch (Exception e)
        {

        }
    }
    public void PropSelectionExit(SelectExitEventArgs args)
    {
        HidePropsManagerUI();
    }

    private void HidePropsManagerUI()
    {
        UIManager.Instance.ShowHidePanels();
    }
    public ControllableObject RecreatePropNonStandardName(string nonStandardName)
    {
        string possibleName = Regex.Match(nonStandardName, "[a-zA-Z]+").Value;
        var prop = CreateProp(possibleName);
        if (prop != null) prop.name = nonStandardName;
        return prop;
    }
    public ControllableObject CreateProp(string name)
    {
        var selectedPrefab = GlobalResourceFinder.instance.FindPrefabWithName(name);
        if (selectedPrefab == null)
        {
            Debug.LogWarning("couldn't find " + name + " prefab in the list of GlobalResourceFinder.");
            return null;
        }

        //
        GameObject newObj;
        if (name.Contains("photo") || name.Contains("text"))
        {
            newObj = Instantiate(selectedPrefab, GlobalResourceFinder.instance.animationSpace.transform, false);
        }
        else
        {
            newObj = Instantiate(selectedPrefab, GlobalResourceFinder.instance.characterEnv.transform, false);
        }
        Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward;
        string[] charactersKeywords = { "blue", "green", "red" };
        string prefabName = selectedPrefab.name.ToLower();
        if (charactersKeywords.Any(keyword => prefabName.Contains(keyword)))
        {
            pos.y -= 1.5f;
            GameObject characterAnimSpace = new GameObject("CharacterSpace" + propUID);
            characterAnimSpace.transform.SetParent(newObj.transform.parent, false);
            newObj.transform.SetParent(characterAnimSpace.transform, false);
        }

        // Remove this part later, from here
        string[] keywords = { "baseball", "knife" };
        if (keywords.Any(keyword => prefabName.Contains(keyword)))
        {
            newObj.GetComponent<ObjectManipulator>().selectEntered.AddListener(attachObjectToHandManager.GrabStarted);
            newObj.GetComponent<ObjectManipulator>().selectExited.AddListener(attachObjectToHandManager.GrabEnded);
        }
        // Until here

        newObj.transform.position = pos;
        var rot = newObj.transform.localRotation;
        rot.y = Camera.main.transform.localRotation.y;
        newObj.transform.localRotation = rot;
        var extString = Regex.Match(newObj.name, "[a-zA-Z]+").Value;
        newObj.name = extString + "_" + propUID++;
        ApplySelectableProps.MakeSelectable(newObj);

        //UIManager.Instance.HidePropsMenu();

        return newObj.GetComponent<ControllableObject>();
    }
}
