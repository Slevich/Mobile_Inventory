using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteImageAnimation : MonoBehaviour
{
    #region Fields
    [Header("Sprite renderer to animate."), SerializeField] private SpriteRenderer _renderer;
    [Header("Time in seconds to interpolate color."), SerializeField] private float _colorDuration;
    [Header("Color change duration."), SerializeField] private Color[] _colors; 
    private Tween _spriteTween;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (!_renderer && TryGetComponent<SpriteRenderer>(out SpriteRenderer Renderer))
            _renderer = Renderer;
    }

    public void SetColorByID(int ColorIndex)
    {
        if (!_renderer)
            return;


        if (ColorIndex < 0 || ColorIndex >= _colors.Length)
        {
            Debug.LogError($"Colors doesn't have index {ColorIndex} !");
            return;
        }

        _renderer.color = _colors[ColorIndex];
    }

    public void PlayAnimationAtIndex(int ColorIndex)
    {
        if(!_renderer)
            return;

        if (ColorIndex < 0 || ColorIndex >= _colors.Length)
        {
            Debug.LogError($"Colors doesn't have index {ColorIndex} !");
            return;
        }

        Color targetColor = _colors[ColorIndex];
        _spriteTween = _renderer.DOColor(targetColor, _colorDuration);
        _spriteTween.Play();
    }
    #endregion
}
