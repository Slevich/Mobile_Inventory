using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    #region Fields
    private PositionLerper _lerper;
    #endregion
    #region Methods
    public void UpdatePosition(Vector3 FollowTarget)
    {
        if(_lerper == null)
            _lerper = new PositionLerper();

        _lerper.MoveByWorldPosition(transform, FollowTarget);
    }
    #endregion
}
