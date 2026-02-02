using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace TrackManager
{
    //[RequireComponent(typeof(XRSimpleInteractable))]
    public class SlotUIController : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent slotAreaUIUpdated = default;
        private Vector3 _lastPosition;
        private Vector3 _lastScale;
        private bool grab;



        void Update()
        {
            if (grab && (_lastPosition != this.transform.localPosition || _lastScale != this.transform.localScale))
            {
                _lastPosition = this.transform.localPosition;
                _lastScale = this.transform.localScale;
                slotAreaUIUpdated.Invoke();
            }
        }

        public void HandleSelectStart(SelectEnterEventArgs arg)
        {
            grab = true;
        }
        public void HandleSelectEnd(SelectExitEventArgs arg)
        {
            grab = false;
        }
    }

}