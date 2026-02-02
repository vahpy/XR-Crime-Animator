using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class ControllerButtonsHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionReference controllerPrimaryButtonActionReference;
    [SerializeField]
    private InputActionReference controllerSecondaryButtonActionReference;
    [SerializeField]
    private InputActionReference controllerGripButtonActionReference;
    [SerializeField]
    private InputActionReference controllerChangeViewActionReference;
    [SerializeField]
    private InputActionReference controllerPanZoomActionReference;
    [SerializeField]
    private InputActionReference menuActionReference;

    public UnityEvent onPrimaryButtonPressed;
    public UnityEvent onPrimaryButtonReleased;
    public UnityEvent onPrimaryButtonDoubleClick;
    public UnityEvent onSecondaryButtonPressed;
    public UnityEvent onSecondaryButtonReleased;
    public UnityEvent onGripButtonPressed;
    public UnityEvent onGripButtonReleased;
    public UnityEvent<Vector2> onTeleportationPressed;
    public UnityEvent onTeleportationReleased;
    public UnityEvent onChangeViewPressed;
    public UnityEvent onChangeViewReleased;
    public UnityEvent onPanZoomStarted;
    public UnityEvent onPanZoomEnded;
    public UnityEvent onMenuPressed;
    public UnityEvent onMenuReleased;

    [SerializeField, Range(0, 3f)]
    private float doubleClickThreshold = 0.75f;
    private float lastPrimaryBtnPressTime = 0;

    private void OnEnable()
    {
        controllerPrimaryButtonActionReference.action.performed += OnPrimaryButtonPerformed;
        controllerPrimaryButtonActionReference.action.canceled += OnPrimaryButtonCanceled;

        controllerSecondaryButtonActionReference.action.performed += OnSecondaryButtonPerformed;
        controllerSecondaryButtonActionReference.action.canceled += OnSecondaryButtonCanceled;

        controllerGripButtonActionReference.action.performed += OnGripButtonPerformed;
        controllerGripButtonActionReference.action.canceled += OnGripButtonCanceled;

        controllerChangeViewActionReference.action.performed += OnChangeViewPerformed;
        controllerChangeViewActionReference.action.canceled += OnChangeViewCanceled;

        controllerPanZoomActionReference.action.performed += OnPanZoomStarted;
        controllerPanZoomActionReference.action.canceled += OnPanZoomEnded;

        menuActionReference.action.performed += OnMenuPressed;
        menuActionReference.action.canceled += OnMenuCanceled;

    }

    private void OnDisable()
    {
        controllerPrimaryButtonActionReference.action.performed -= OnPrimaryButtonPerformed;
        controllerPrimaryButtonActionReference.action.canceled -= OnPrimaryButtonCanceled;

        controllerSecondaryButtonActionReference.action.performed -= OnSecondaryButtonPerformed;
        controllerSecondaryButtonActionReference.action.canceled -= OnSecondaryButtonCanceled;

        controllerGripButtonActionReference.action.performed -= OnGripButtonPerformed;
        controllerGripButtonActionReference.action.canceled -= OnGripButtonCanceled;

        controllerChangeViewActionReference.action.performed -= OnChangeViewPerformed;
        controllerChangeViewActionReference.action.canceled -= OnChangeViewCanceled;

        controllerPanZoomActionReference.action.performed -= OnPanZoomStarted;
        controllerPanZoomActionReference.action.canceled -= OnPanZoomEnded;

        menuActionReference.action.performed -= OnMenuPressed;
        menuActionReference.action.canceled -= OnMenuCanceled;
    }

    private void OnSecondaryButtonPerformed(InputAction.CallbackContext context)
    {
        onSecondaryButtonPressed.Invoke();
    }
    private void OnSecondaryButtonCanceled(InputAction.CallbackContext context)
    {
        onSecondaryButtonReleased.Invoke();
    }
    private void OnPrimaryButtonPerformed(InputAction.CallbackContext context)
    {
        var time = Time.unscaledTime;
        if (time - lastPrimaryBtnPressTime < doubleClickThreshold)
        {
            onPrimaryButtonDoubleClick.Invoke();
            time = 0;
        }
        lastPrimaryBtnPressTime = time;
        onPrimaryButtonPressed.Invoke();
    }

    private void OnPrimaryButtonCanceled(InputAction.CallbackContext context)
    {
        onPrimaryButtonReleased.Invoke();
    }

    private void OnGripButtonPerformed(InputAction.CallbackContext context)
    {
        onGripButtonPressed.Invoke();
    }
    private void OnGripButtonCanceled(InputAction.CallbackContext context)
    {
        onGripButtonReleased.Invoke();
    }

    private void OnChangeViewPerformed(InputAction.CallbackContext context)
    {
        onChangeViewPressed.Invoke();
    }

    private void OnChangeViewCanceled(InputAction.CallbackContext context)
    {
        onChangeViewReleased.Invoke();
    }

    private void OnPanZoomStarted(InputAction.CallbackContext context)
    {
        onPanZoomStarted.Invoke();
    }
    private void OnPanZoomEnded(InputAction.CallbackContext context)
    {
        onPanZoomEnded.Invoke();
    }

    private void OnMenuPressed(InputAction.CallbackContext context)
    {
        onMenuPressed.Invoke();
    }
    private void OnMenuCanceled(InputAction.CallbackContext context)
    {
        onMenuReleased.Invoke();
    }
}
