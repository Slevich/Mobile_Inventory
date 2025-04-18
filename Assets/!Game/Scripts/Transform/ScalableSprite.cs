using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableSprite : MonoBehaviour
{
    #region Fields
    [Header("Scale bases on this sprite."), SerializeField] private SpriteRenderer _renderer;
    [Header("Transform to scale"), SerializeField] private BoxCollider2D _scaledCollider;

    private Vector2 _startBoxSize = Vector2.zero;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (!_renderer && TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
            _renderer = renderer;

        if (_scaledCollider)
            _startBoxSize = _scaledCollider.size;
    }

    public void Scale()
    {
        if(!_scaledCollider)
        {
            Debug.LogError("Scalable sprite has no collider!");
            return;
        }

        if(!_renderer)
        {
            Debug.LogError("Scalable sprite has no sprite renderer!");
            return;
        }

        Bounds globalBounds = GlobalValues.Instance.GraphicImagesBounds;
        _scaledCollider.size = _scaledCollider.transform.parent.InverseTransformPoint(globalBounds.size);
        float xScaleModifier = MathF.Round(_scaledCollider.size.x / _startBoxSize.x, 2);
        float yScaleModifier = MathF.Round(_scaledCollider.size.y / _startBoxSize.y, 2);
        Vector3 rendererGlobalScale = _renderer.transform.lossyScale;
        Vector3 rendererModifiedScale = new Vector3(rendererGlobalScale.x * xScaleModifier, rendererGlobalScale.y * yScaleModifier, rendererGlobalScale.z);
        _renderer.transform.localScale = _renderer.transform.parent.InverseTransformPoint(rendererModifiedScale);
        Debug.Log($"Local scale of the {_renderer.gameObject.name} is {_renderer.transform.localScale}");
    }
    #endregion
}
