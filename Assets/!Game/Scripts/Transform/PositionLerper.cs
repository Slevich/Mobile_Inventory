using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLerper
{
    public void MoveByWorldDirection(Transform MovingTransform, Vector3 WorldDirection)
    {
        if (MovingTransform == null)
            return;

        Vector3 currentWorldPosition = MovingTransform.position;
        MovingTransform.position = currentWorldPosition + WorldDirection;
    }

    public void MoveByWorldPosition (Transform MovingTransform, Vector3 WorldPosition)
    {
        if (MovingTransform == null)
            return;

        MovingTransform.position = WorldPosition;
    }

    public void LerpByWorldDirection(Transform MovingTransform, Vector3 WorldDirection, float SpeedModifier)
    {
        if (MovingTransform == null)
            return;

        Vector3 currentWorldPosition = MovingTransform.position;
        Vector3 targetPoint = currentWorldPosition + WorldDirection;
        Vector3 objectInterpolatedPosition = Vector3.Lerp(currentWorldPosition, targetPoint, SpeedModifier);
        MovingTransform.position = objectInterpolatedPosition;
    }

    public void LerpToWorldPosition (Transform MovingTransform, Vector3 SnapPosition, float SpeedModifier)
    {
        if(MovingTransform == null)
            return;

        Vector3 currentWorldPosition = MovingTransform.position;
        Vector3 objectInterpolatedPosition = Vector3.Lerp(currentWorldPosition, SnapPosition, SpeedModifier);
        MovingTransform.position = objectInterpolatedPosition;
    }

    public void LerpToTransform (Transform MovingTransform, Transform Target, float SpeedModifier)
    {
        if (Target == null)
            return;

        LerpToWorldPosition(MovingTransform, Target.position, SpeedModifier);
    }
}
