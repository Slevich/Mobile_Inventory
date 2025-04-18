using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using UniRx;

public static class InputHandler
{
    #region Fields
    private static InputActions _inputActions;
    private static ActionUpdate _update;
    private static Action _updateAction;
    private static bool _pressDetected = false;
    private static Vector2[] _pointerPositions = new Vector2[] { };
    #endregion

    #region Properties
    public static Subject<bool> PressDetectionSender { get; private set; }
    public static Subject<Vector2[]> PointerPositionsSender { get; private set; }
    #endregion

    #region Constructor
    static InputHandler ()
    {
        if(Application.isPlaying)
            Initialize();
    }
    #endregion

    #region Methods
    public static void Initialize()
    {
        _inputActions = new InputActions();
        PressDetectionSender = new Subject<bool>();
        PointerPositionsSender = new Subject<Vector2[]>();
        _update = new ActionUpdate();
        _updateAction = delegate 
        {
            _pressDetected = _inputActions.Buttons.Drag.phase == InputActionPhase.Performed;
            PressDetectionSender.OnNext(_pressDetected);

            _pointerPositions = new Vector2[]
            {
                _inputActions.Positions.PointerTouch.ReadValue<Vector2>(),
                _inputActions.Positions.PointerMouse.ReadValue<Vector2>()
            };

            PointerPositionsSender.OnNext(_pointerPositions);
        };
    }

    public static void EnableInput()
    {
        _inputActions.Enable();
        _update.StartUpdate(_updateAction);
    }

    public static void DisableInput()
    {
        _inputActions.Disable();
        _update.StopUpdate();
    }
    #endregion
}