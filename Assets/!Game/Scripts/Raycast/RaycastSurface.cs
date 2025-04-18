using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSurface : MonoBehaviour
{
    #region Fields
    private BoxCollider2D _surfaceCollider;
    #endregion

    #region Methods
    private void Awake ()
    {
        if(_surfaceCollider == null && TryGetComponent<BoxCollider2D>(out BoxCollider2D boxCollider))
            _surfaceCollider = boxCollider;
    }
    #endregion
}
