using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using UniRx;
using UnityEngine.InputSystem;

public class TouchDetector : MonoBehaviour
{
    #region Fields
    private Action _onPressDetected;
    private bool _updatePositions = false;
    #endregion

    #region Properties
    public Action<Vector2> OnActiveTouchPosition { get; set; }
    public Action OnNoActiveTouches { get; set; }
    #endregion

    #region Methods
    private void Awake ()
    {
        _onPressDetected = delegate
        {
            InputHandler.PointerPositionsSender.Where(_ => _updatePositions).Subscribe(pos => GetPositions(pos));
        };

        InputHandler.PressDetectionSender.Where(pressed => PressDetection(pressed)).Subscribe(delegate { _onPressDetected?.Invoke(); }).AddTo(this);
    }

    private bool PressDetection(bool Pressed)
    {
        if(!Pressed)
        {
            OnNoActiveTouches?.Invoke();
        }

        _updatePositions = Pressed;
        return Pressed;
    }    

    private void GetPositions(Vector2[] Positions)
    {
        Vector2 activePosition = Positions.Where(pos => pos != Vector2.zero).FirstOrDefault();
        OnActiveTouchPosition?.Invoke(activePosition);
    }
    #endregion
}
