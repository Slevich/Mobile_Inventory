using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [Header("Transform to rotate."), SerializeField] private Transform _rotatableTransform;
    [Header("Rotation duration in seconds."), SerializeField] private float _rotationDuration = 0.4f;
    [Header("Global rotation axis."), SerializeField] private Axes _axes = Axes.Z;

    private Tween _currentTween;
    private Vector3 _startScale = Vector3.one;

    private void Awake ()
    {
        if(_rotatableTransform == null)
            _rotatableTransform = transform;
    }

    public void PlayRotationAnimation(float PlusDegrees)
    {
        if(_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Kill();
            _currentTween = null;
        }

        Vector3 rotationEndValue = AxesSelector.ReturnVectorWithPlusPositionByAxis(_axes, transform.rotation.eulerAngles, PlusDegrees);

        _currentTween = _rotatableTransform.DORotate(rotationEndValue, _rotationDuration);
        _currentTween.Play();
    }

    public void RotateTo(float TargetDegrees)
    {
        if (_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Kill();
            _currentTween = null;
        }
        Vector3 targetRotation = AxesSelector.ReturnVectorWithPositionByAxis(_axes, transform.rotation.eulerAngles, new Vector3(TargetDegrees, TargetDegrees, TargetDegrees));

        if (transform.rotation.eulerAngles == targetRotation)
            return;

        _currentTween = _rotatableTransform.DORotate(targetRotation, _rotationDuration);
        _currentTween.Play();
    }
}
