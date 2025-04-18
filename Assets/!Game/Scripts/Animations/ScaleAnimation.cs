using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    #region Fields
    [Header("Scaling transform."), SerializeField] private Transform _scalingTransform;
    [Header("Animation duration in seconds."), SerializeField, Range(0f, 5f)] private float _duration = 1.0f;
    [Header("Animation end scale modifier."), SerializeField, Range(0f, 10f)] private float _scaleModifier = 1.4f;

    private Tween _currentTween;
    private Vector3 _startScale = Vector3.one;
    private bool _vectorUpdated = false;
    #endregion

    #region Properties
    private Vector3 StartScale
    {
        get 
        { 
            if(!_vectorUpdated)
            {
                _vectorUpdated = true;
                _startScale = _scalingTransform.localScale;
            }

            return _startScale; 
        }
    }
    private Vector3 targetAnimationScale => _startScale * _scaleModifier;

    #endregion

    #region Methods
    private void Awake ()
    {
        if(_scalingTransform == null)
            _scalingTransform = GetComponent<Transform>();
    }

    public void PlayAnimationForward () => SetCurrentTweenAndPlay(targetAnimationScale, _scalingTransform.localScale);

    public void PlayAnimationReversed () => SetCurrentTweenAndPlay(StartScale, _scalingTransform.localScale);

    private void SetCurrentTweenAndPlay(Vector3 targetScale, Vector3 currentScale, Action OnComplete = null)
    {
        if (_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Kill();
            _currentTween = null;
        }

        _currentTween = _scalingTransform.DOScale(targetScale, _duration).SetEase(Ease.Linear);
        _currentTween.onComplete += delegate { _currentTween = null; };
        _scalingTransform.localScale = currentScale;

        if (OnComplete != null)
            _currentTween.onComplete += delegate { OnComplete?.Invoke(); };

        _currentTween.Play();
    }

    public void PlayAnimationFromZero() => SetCurrentTweenAndPlay(StartScale, Vector3.zero);
    public void PlayAnimationToZero() => SetCurrentTweenAndPlay(Vector3.zero, _scalingTransform.localScale, delegate { Destroy(transform.parent.gameObject); });
    #endregion
}
