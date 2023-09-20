using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    public delegate void StartTouchEvent(Vector2 position);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position);
    public event EndTouchEvent OnEndTouch;

    public Finger finger;
    public CinemachineSwitcher priority;

    private void Awake() =>  EnhancedTouchSupport.Enable();

    private void OnEnable()
    {
        TouchSimulation.Enable();

        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;
    }

    private void OnDisable()
    {
        TouchSimulation.Disable();

        Touch.onFingerDown -= FingerDown;
        Touch.onFingerUp -= FingerUp;
    }

    private void FingerDown(Finger finger)
    {
        foreach (Touch touch in Touch.activeTouches)
            if (touch.screenPosition.x <= Screen.width / 2)
            {
                finger = touch.finger;
                break;
            }

        this.finger = finger;
        if (finger.screenPosition.x <= Screen.width)
            OnStartTouch(finger.screenPosition);
    }

    private void FingerUp(Finger finger)
    {
        this.finger = null;
        OnEndTouch(finger.screenPosition);
    }
}
