using MixedReality.Toolkit.SpatialManipulation;
using System.Collections.Generic;
using TrackManager.Animation;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ApplySelectableProps : MonoBehaviour
{
    [SerializeField] private List<GameObject> objs = default;
    private void Awake()
    {

        if (objs == null)
        {
            return;
        }
        foreach (var obj in objs)
        {
            MakeSelectable(obj);
        }
    }

    private void Start()
    {
    }

    public static void MakeSelectable(GameObject obj)
    {
        if (obj.GetComponent<Outline>() == null)
        {
            obj.AddComponent<Outline>();
        }
        if (obj.GetComponent<ControllableObject>() == null)
        {
            obj.AddComponent<ControllableObject>();
        }
        if (obj.GetComponent<ConstraintManager>() == null)
        {
            obj.AddComponent<ConstraintManager>();
        }
        if (obj.GetComponent<ObjectManipulator>() == null)
        {
            obj.AddComponent<ObjectManipulator>();
        }
        var colliders = obj.GetComponentsInChildren<Collider>();
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                if (col is MeshCollider) ((MeshCollider)col).convex = true;
            }
            //obj.AddComponent<BoxCollider>();
        }
        //if (obj.GetComponent<Rigidbody>() == null)
        //{
        //    var rigid = obj.AddComponent<Rigidbody>();
        //    rigid.useGravity = true;
        //}
        obj.GetComponent<ObjectManipulator>().hoverEntered.AddListener(obj.GetComponent<ControllableObject>().HoverEntered);
        obj.GetComponent<ObjectManipulator>().hoverExited.AddListener(obj.GetComponent<ControllableObject>().HoverExited);
        obj.GetComponent<ObjectManipulator>().selectEntered.AddListener(obj.GetComponent<ControllableObject>().Selected);

        if (obj.GetComponent<ControllableObject>().IsReactToPhysics)
        {
            int layerAssigned = LayerMask.NameToLayer("PhysicalInteraction");
            obj.SetLayerRecursively(layerAssigned);
        }
        else
        {
            int layerAssigned = LayerMask.NameToLayer("IgnorePhysics");
            obj.SetLayerRecursively(layerAssigned);
        }
        int layerMask = ~(1 << LayerMask.NameToLayer("IgnorePhysics"));
        foreach (Collider col in colliders)
        {
            //col.includeLayers |= layerMask;
            col.excludeLayers = ~layerMask;
        }
    }


}
