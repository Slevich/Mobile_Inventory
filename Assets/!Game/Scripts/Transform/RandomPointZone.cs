using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomPointZone : MonoBehaviour
{
    [Header("Box collider zone."), SerializeField] private BoxCollider2D _boxZone;
    [Header("Spawn bounds center offset on X-axis."), SerializeField, Range(-1, 1)] private float _centerXOffset = 0f;
    [Header("Spawn bounds center offset on Y-axis."), SerializeField, Range(-1, 1)] private float _centerYOffset = 0f;
    [Header("Zone size modifier on X-Axis."), SerializeField, Range(0, 10)] private float _sizeXModifier = 1f;
    [Header("Zone size modifier on Y-Axis."), SerializeField, Range(0, 10)] private float _sizeYModifier = 1f;

    [SerializeField, HideInInspector] private Bounds _zoneBounds;
    [SerializeField, HideInInspector] private Vector3 _zoneCenter = Vector3.one;
    [SerializeField, HideInInspector] private Vector3 _zoneSize = Vector3.one;

    private void Awake ()
    {
        if (!_boxZone && TryGetComponent<BoxCollider2D>(out BoxCollider2D box))
            _boxZone = box;

        _zoneBounds = _boxZone.bounds;
    }

    private void Start () => UpdateZone();

    public void UpdateZone()
    {
        if (!_boxZone)
            return;

        float zoneCenterXCoordinate = _zoneBounds.
    }

    public void ReturnRandomPoint()
    {
        if (!_boxZone)
            return;



        float offsetX = Random.Range(-_zoneBounds.extents.x, _zoneBounds.extents.x);
        float offsetY = Random.Range(-_zoneBounds.extents.y, _zoneBounds.extents.y);
        float offsetZ = Random.Range(-_zoneBounds.extents.z, _zoneBounds.extents.z);
        Vector3 randomPosition = _zoneBounds.center + new Vector3(offsetX, offsetY, offsetZ);
    }

    private void OnDrawGizmos ()
    {
        if (!_boxZone)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_zoneCenter, _zoneSize);
    }
}
