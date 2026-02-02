using DigitalRuby.PyroParticles;
using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using TrackManager.Animation;
using Unity.VisualScripting;
using UnityEngine;

public class PistolInteraction : InteractiveProp
{

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject pistolTip;
    [SerializeField] private GameObject fireBoltPrefab;
    [DoNotSerialize] public bool shot;

    private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        shot = false;
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void OnTriggeringInteraction()
    {
        this.interactionState = InteractionState.InteractionOnGoing;
    }

    public override void StartTriggerInteraction()
    {
        this.interactionState = InteractionState.InteractionStarted;

        var fireBolt = Instantiate(fireBoltPrefab, pistolTip.transform, false);
        fireBolt.transform.localPosition = Vector3.zero;
        fireBolt.transform.localRotation = Quaternion.identity;
        fireBolt.transform.localScale = Vector3.one/2;
    }
    public override void StopTriggerInteraction()
    {
        this.interactionState = InteractionState.InteractionEnded;
    }

    private void LaserToTarget()
    {
        RaycastHit hit;
        Vector3 targetPos;
        //if (Physics.Raycast(pistolTip.transform.position, pistolTip.transform.forward, out hit, 100f))
        //{
        //    targetPos = hit.point;
        //}
        //else
        //{
            targetPos = pistolTip.transform.forward * 100f;
        //}
        lineRenderer.SetPositions(new Vector3[] { pistolTip.transform.position, targetPos });
    }

    protected override void Update()
    {
        base.Update();
        if (shot)
        {
            shot = false;
            StartTriggerInteraction();
        }
    }
    protected void LateUpdate()
    {
        LaserToTarget();
    }
}
