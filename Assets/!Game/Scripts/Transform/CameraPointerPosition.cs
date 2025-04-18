using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class CameraPointerPosition : MonoBehaviour
{
    #region Fields
    private TouchDetector _touchDetector;
    #endregion

    #region Properties
    public Action<bool, Vector3> OnWorldPointerPosition { get; set; }
    public Action<bool, Vector3> OnViewportPointerPosition { get; set; }
    public Action<bool, Ray> OnPointerRay { get; set; }
    #endregion

    #region Methods
    private void Awake ()
    {
        _touchDetector = SystemReferencesContainer.Instance.TouchDetector;
    }

    private void OnEnable ()
    {
        _touchDetector.OnNoActiveTouches += DropEvents;
        _touchDetector.OnActiveTouchPosition += pos => UpdateEvents(pos);
    }

    private void OnDisable ()
    {
        _touchDetector.OnNoActiveTouches -= DropEvents;
        _touchDetector.OnActiveTouchPosition -= pos => UpdateEvents(pos);
    }

    private void UpdateEvents(Vector2 PointerScreenPosition)
    {
        Vector3 worldPointerPosition = Camera.main.ScreenToWorldPoint(PointerScreenPosition);
        OnWorldPointerPosition?.Invoke(true, worldPointerPosition);

        Vector3 viewportPointerPosition = Camera.main.ScreenToViewportPoint(PointerScreenPosition);
        OnViewportPointerPosition?.Invoke(true, viewportPointerPosition);

        Ray cameraPointerRay = Camera.main.ScreenPointToRay(PointerScreenPosition);
        OnPointerRay?.Invoke(true, cameraPointerRay);
    }

    private void DropEvents()
    {
        OnWorldPointerPosition?.Invoke(false, Vector3.zero);
        OnViewportPointerPosition?.Invoke(false, Vector3.zero);
        OnPointerRay?.Invoke(false, new Ray());
    }
    #endregion
}
