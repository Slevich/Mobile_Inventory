using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public class GridUpdater : MonoBehaviour
{
    #region Fields
    [Header("Box collider sized to update grid."), SerializeField] private BoxCollider2D _box;
    [Header("Grid to update."), SerializeField] private Grid _grid;
    [Header("Preview grid slot prefab."), SerializeField] private GameObject _previewSlotPrefab;
    [Header("Number of grid lines."), SerializeField, Range(2, 20)] private int _gridLines = 4;
    [Header("Number of grid columns."), SerializeField, Range(2, 20)] private int _gridColumns = 4;
    [Header("Preview slots scale modifier."), SerializeField, Range(0, 5)] private float _scaleModifier = 1.0f;

    [SerializeField, HideInInspector] private int _evenGridLines = 0;
    [SerializeField, HideInInspector] private int _evenGridColumns = 0;

    private SlotSpawner _spawner;
    [SerializeField, HideInInspector] private GameObject[] _previewSlots;
    [SerializeField, HideInInspector] private List<Vector3> _cellsPositions;
    [SerializeField, HideInInspector] private Dictionary<Vector3, Vector3> _positionsAndScales = new Dictionary<Vector3, Vector3>();
    #endregion

    #region Properties
    [field: SerializeField, HideInInspector] public List<GridSlot> GridSlots { get; private set; }
    public Vector3 CellSize => _grid.cellSize;
    [field: SerializeField, HideInInspector] public bool PreviewMode { get; set; } = false;
    #endregion

    #region Methods
    private void Awake ()
    {
        if(_box == null && TryGetComponent<BoxCollider2D>(out BoxCollider2D box))
            _box = box;

        if(_grid == null && TryGetComponent<Grid>(out Grid grid))
            _grid = grid;

        if (PreviewMode)
            StopPreview();
    }

    private void UpdateGrid()
    {
        if (!_box && !_grid)
            return;

        Vector2 boxOffset = _box.offset;
        Vector2 boxSize = _box.bounds.size;

        float xCellSize = boxSize.x / _gridLines;
        float yCellSize = boxSize.y / _gridColumns;

        float gridCenterXCoordinate = _box.bounds.center.x - _box.bounds.extents.x;
        float gridCenterYCoordinate = _box.bounds.center.y - _box.bounds.extents.y;
        float gridCenterZCoordinate = _box.bounds.center.z;

        _grid.transform.position = new Vector3(gridCenterXCoordinate, gridCenterYCoordinate, gridCenterZCoordinate);
        _grid.cellSize = new Vector2(xCellSize, yCellSize);
    }

    private Vector3[] ReturnCellsWorldPositions ()
    {
        List<Vector3> cellsWorldPositions = new List<Vector3>();

        for (int i = 0; i < _gridLines; i++)
        {
            int yCoordinate = (_gridLines - i) - 1;

            //colimns
            for (int q = 0; q < _gridColumns; q++)
            {
                int xCoordinate = q;
                Vector3Int cellPosition = new Vector3Int(xCoordinate, yCoordinate, 0);
                Vector3 cellWorldPosition = _grid.GetCellCenterWorld(cellPosition);
                cellsWorldPositions.Add(cellWorldPosition);
            }
        }

        return cellsWorldPositions.ToArray();
    }

    public void PreviewGrid()
    {
        if (PreviewMode)
            return;

        UpdateGrid();

        _cellsPositions = ReturnCellsWorldPositions().ToList();

        if (_spawner == null)
            _spawner = new SlotSpawner(_previewSlotPrefab);

        _previewSlots = _spawner.SpawnIntoPoints(_box, _cellsPositions.ToArray(), transform);

        Vector3 globalCellSize = _grid.transform.parent.TransformVector(_grid.cellSize);

        if (GridSlots != null)
            GridSlots.Clear();
        else
            GridSlots = new List<GridSlot>();

        foreach (GameObject slot in _previewSlots)
        {
            BoxCollider2D box = slot.GetComponent<BoxCollider2D>();
            Vector3 slotSize = box.bounds.size;
            float distance = Vector3.Distance(globalCellSize, slotSize);
            slot.transform.localScale *= (distance * _scaleModifier);
            Vector3 position = _cellsPositions.Where(pos => pos == slot.transform.position).Single();
            int positionIndex = _cellsPositions.IndexOf(position);
            int gridPositionY = 1 + (positionIndex / _gridLines);
            int gridPositionX = 1 + (positionIndex % _gridColumns);
            Vector2 gridPosition = new Vector2(gridPositionX, gridPositionY);
            GridSlot gridSlot = new GridSlot(position, slot.transform.localScale, gridPosition);
            GridSlots.Add(gridSlot);
        }

        PreviewMode = true;
    }

    public void StopPreview()
    {
        if (!PreviewMode)
            return;

        foreach(GameObject slot in _previewSlots)
        {
            if(!slot.activeSelf)
            {
                Vector3 position = _cellsPositions.Where(pos => pos == slot.transform.position).FirstOrDefault();
                _positionsAndScales.Remove(position);
                _cellsPositions.Remove(position);
            }

            DestroyImmediate(slot);
        }

        PreviewMode = false;
    }
    #endregion
}

[Serializable, ExecuteInEditMode]
public class GridSlot
{
    [field: SerializeField, HideInInspector] public Vector3 GlobalPosition { get; private set; }
    [field: SerializeField, HideInInspector] public Vector3 GlobalScale { get; private set; }
    [field: SerializeField, HideInInspector] public Vector2 GridPosition { get; private set; }

    public GridSlot(Vector3 Position, Vector3 Size, Vector2 PositionInGrid)
    {
        GlobalPosition = Position;
        GlobalScale = Size;
        GridPosition = PositionInGrid;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridUpdater))]
public class GridUpdaterEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        GridUpdater updater = target as GridUpdater;
        Type updaterType = updater.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        GUILayout.Space(25);
        //bool updateButtonPressed = GUILayout.Button("Update!");
        //if(updateButtonPressed)
        //{
        //    MethodInfo updateMethod = updaterType.GetMethod("UpdateGrid", flags);
        //    updateMethod.Invoke(updater, null);
        //}

        GUILayout.Space(25);
        GUILayout.Label("YOU CAN MANAGE YOUR SLOTS INTO GRID.\n" +
                        "Just press button 'Preview' down below, select some slot objects active state to false.\n" +
                        "Then press button 'Stop preview' to save results.");
        GUILayout.Space(15);

        if (!updater.PreviewMode)
        {
            bool previewButtonPressed = GUILayout.Button("Preview...");
            if (previewButtonPressed)
            {
                MethodInfo previewMethod = updaterType.GetMethod("PreviewGrid", flags);
                previewMethod.Invoke(updater, null);
            }
        }
        else
        {
            bool stopPreviewButtonPressed = GUILayout.Button("Stop preview!");
            if (stopPreviewButtonPressed)
            {
                MethodInfo previewMethod = updaterType.GetMethod("StopPreview", flags);
                previewMethod.Invoke(updater, null);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif