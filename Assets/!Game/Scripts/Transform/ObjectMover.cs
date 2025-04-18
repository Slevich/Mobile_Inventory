using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    private ActionInterval _moveInterval;
    private PositionLerper _positionLerper;
    private static readonly float _positionUpdateTime = 0.01f;

    public void StartLerpingToPosition(Vector3 TargetWorldPosition, float SpeedModifier)
    {
        if(_moveInterval == null)
            _moveInterval = new ActionInterval();

        if (_positionLerper == null)
            _positionLerper = new PositionLerper();

        Action onMovementTick = delegate
        {
            _positionLerper.LerpToWorldPosition(transform, TargetWorldPosition, SpeedModifier);
        };

        _moveInterval.StartInterval(_positionUpdateTime, onMovementTick);
    }

    public void StartLerpingToTransform(Transform Target, float SpeedModifier)
    {
        if (_moveInterval == null)
            _moveInterval = new ActionInterval();

        if (_positionLerper == null)
            _positionLerper = new PositionLerper();

        if (Target == null)
            return;

        Action onMovementTick = delegate
        {
            _positionLerper.LerpToTransform(transform, Target, SpeedModifier);
        };

        _moveInterval.StartInterval(_positionUpdateTime, onMovementTick);
    }

    public void StopLerping()
    {
        if(_moveInterval != null && _moveInterval.Busy)
            _moveInterval.Stop();
    }

    private void OnDisable () => StopLerping();
}
