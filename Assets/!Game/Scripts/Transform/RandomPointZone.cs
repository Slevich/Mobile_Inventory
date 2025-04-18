using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RandomPointZone : MonoBehaviour, IDropZone
{
    #region Fields
    [Header("Transform parent for drop objects"), SerializeField] private Transform _dropParent;
    [Header("Box collider zone."), SerializeField] private BoxCollider2D _boxZone;
    [Header("Spawn bounds center offset on X-axis."), SerializeField, Range(-1, 1)] private float _centerXOffset = 0f;
    [Header("Spawn bounds center offset on Y-axis."), SerializeField, Range(-1, 1)] private float _centerYOffset = 0f;
    [Header("Zone size modifier on X-Axis."), SerializeField, Range(0, 10)] private float _sizeXModifier = 1f;
    [Header("Zone size modifier on Y-Axis."), SerializeField, Range(0, 10)] private float _sizeYModifier = 1f;

    [SerializeField, HideInInspector] private Bounds _zoneBounds;
    [SerializeField, HideInInspector] private Vector3 _zoneCenter = Vector3.one;
    [SerializeField, HideInInspector] private Vector3 _zoneSize = Vector3.one;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (!_boxZone && TryGetComponent<BoxCollider2D>(out BoxCollider2D box))
            _boxZone = box;

        if (!_boxZone)
            return;

        UpdateZone();
    }

    public void UpdateZone()
    {
        if (!_boxZone)
            return;

        _zoneBounds = _boxZone.bounds;

        float zoneCenterXCoordinate = _zoneBounds.center.x + (_zoneBounds.extents.x * _centerXOffset);
        float zoneCenterYCoordinate = _zoneBounds.center.y + (_zoneBounds.extents.y * _centerYOffset);
        float zoneCenterZCoordinate = _zoneBounds.center.z;
        _zoneCenter = new Vector3(zoneCenterXCoordinate, zoneCenterYCoordinate, zoneCenterZCoordinate);

        float zoneXSize = _zoneBounds.size.x * _sizeXModifier;
        float zoneYSize = _zoneBounds.size.y * _sizeYModifier;
        float zoneZSize = _zoneBounds.size.z;
        _zoneSize = new Vector3(zoneXSize, zoneYSize, zoneZSize);
    }

    public Vector3 ReturnDropPoint()
    {
        if (!_boxZone)
            return Vector3.zero;

        float xExtents = _zoneSize.x / 2;
        float xMin = _zoneCenter.x - xExtents;
        float xMax = _zoneCenter.x + xExtents;
        float offsetX = Random.Range(xMin, xMax);

        float yExtents = _zoneSize.y / 2;
        float yMin = _zoneCenter.y - yExtents;
        float yMax = _zoneCenter.y + yExtents;
        float offsetY = Random.Range(yMin, yMax);

        float offsetZ = Random.Range(-_zoneSize.z, _zoneSize.z);
        Vector3 randomPosition = new Vector3(offsetX, offsetY, offsetZ);
        return randomPosition;
    }

    public Transform ReturnDropParent () => _dropParent;

    private void OnDrawGizmos ()
    {
        if (!_boxZone || Application.IsPlaying(this))
            return;

        if (!Selection.objects.Contains(gameObject))
            return;

        Color gizmosColor = Color.blue;
        gizmosColor.a = 0.3f;
        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(_zoneCenter, _zoneSize);
    }
    #endregion
}

[CustomEditor(typeof(RandomPointZone))]
public class RandomPointZoneEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        RandomPointZone zone = (RandomPointZone)target;
        zone.UpdateZone();

        serializedObject.ApplyModifiedProperties();
    }
}