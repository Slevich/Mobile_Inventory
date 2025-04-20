using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class InventorySlot : MonoBehaviour
{
    #region Fields
    [Header("SpriteRenderer."), SerializeField] private SpriteRenderer _renderer;
    [Header("Event on select slot."), SerializeField] private UnityEvent OnSelection;
    [Header("Event on wrong select slot."), SerializeField] private UnityEvent OnWrongSelection;
    [Header("Event on deselect slot."), SerializeField] private UnityEvent OnDeselection;
    private Bounds _bounds => _renderer.bounds;
    private bool _selected = false;
    private bool _wrongSelected = false;
    #endregion

    #region Properties
    [field: Header("Slot index in inventory."), SerializeField, ReadOnly] public int Index { get; set; } = 0;
    [field: Header("Grid position."), SerializeField, ReadOnly] public Vector2 GridPosition { get; set; } = Vector2.zero;
    [field: Header("Occupied item."), SerializeField, ReadOnly] public DraggableItem OccupiedItem { get; set; } = null;

    public bool IsFree => OccupiedItem == null;
    public bool Selected 
    { 
        get
        {
            return _selected;
        } 
        set
        {
            if(_selected != value)
            {
                if(!_selected && value)
                    OnSelection?.Invoke();
                else if(_selected && !value)
                    OnDeselection?.Invoke();
            }

            _selected = value;
        }
    }

    public bool WrongSelected
    {
        get
        {
            return _wrongSelected;
        }
        set
        {
            if (_wrongSelected != value)
            {
                if (!_wrongSelected && value)
                    OnWrongSelection?.Invoke();
                else if (_wrongSelected && !value)
                    OnDeselection?.Invoke();
            }

            _wrongSelected = value;
        }
    }

    public Vector2 Extents => _renderer.bounds.extents;
    public Vector2 Size => _renderer.bounds.size;
    #endregion

    #region Methods
    public PointerDirection CalculatePointerDirection(Vector3 PointerPosition)
    {
        Vector2 pointerPosition2D = PointerPosition;
        Vector2 boundsCenter = _bounds.center;

        if (pointerPosition2D == boundsCenter)
            return PointerDirection.Center;

        Vector2 upperLeftPoint = new Vector2(boundsCenter.x - Extents.x, boundsCenter.y + Extents.y);
        Vector2 upperRightPoint = new Vector2(boundsCenter.x + Extents.x, boundsCenter.y + Extents.y);
        Vector2 lowerLeftPoint = new Vector2(boundsCenter.x - Extents.x, boundsCenter.y - Extents.y);
        Vector2 lowerRightPoint = new Vector2(boundsCenter.x + Extents.x, boundsCenter.y - Extents.y);

        //left triangle
        Vector2[] leftTrianglePoints = new Vector2[3] { upperLeftPoint, boundsCenter, lowerLeftPoint};
        bool isPointInLeftTriangle = PointInTriangle(leftTrianglePoints, pointerPosition2D);
        if(isPointInLeftTriangle)
            return PointerDirection.Left;

        //right triangle
        Vector2[] rightTrianglePoints = new Vector2[3] { upperRightPoint, lowerRightPoint, boundsCenter};
        bool isPointInRightTriangle = PointInTriangle(rightTrianglePoints, pointerPosition2D);
        if(isPointInRightTriangle)
            return PointerDirection.Right;

        //upper triangle
        Vector2[] upperTrianglePoints = new Vector2[3] { boundsCenter, upperLeftPoint, upperRightPoint };
        bool isPointInUpperTriangle = PointInTriangle(upperTrianglePoints, pointerPosition2D);
        if (isPointInUpperTriangle)
            return PointerDirection.Up;

        //lower triangle
        Vector2[] lowerTrianglePoints = new Vector2[3] { lowerLeftPoint, boundsCenter, lowerRightPoint };
        bool isPointInLowerTriangle = PointInTriangle(lowerTrianglePoints, pointerPosition2D);
        if (isPointInLowerTriangle)
            return PointerDirection.Down;

        return PointerDirection.Undefined;
    }

    private bool PointInTriangle (Vector2[] TrianglePoints, Vector2 Point)
    {
        TriangleCalculation triangle = new TriangleCalculation(TrianglePoints);
        return triangle.IsPointInsideTriangle(Point);
    }
    #endregion
}

public enum PointerDirection
{
    Undefined,
    Center,
    Up,
    Down,
    Left,
    Right
}

public class TriangleCalculation
{
    private float[] xCoordinates;
    private float[] yCoordinates;
    private int npol;

    public TriangleCalculation(Vector2[] TrianglePoints)
    {
        xCoordinates = TrianglePoints.Select(point => point.x).ToArray();
        yCoordinates = TrianglePoints.Select(point => point.y).ToArray();
    }

    public int TrianglePoint (int npol, float[] xp, float[] yp, float x, float y)
    {
        int c = 0;
        for (int i = 0, j = npol - 1; i < npol; j = i++)
        {
            if ((((yp[i] <= y) && (y < yp[j])) || ((yp[j] <= y) && (y < yp[i]))) &&
              (x > (xp[j] - xp[i]) * (y - yp[i]) / (yp[j] - yp[i]) + xp[i]))
                c = 1 - c;
        }
        return c;
    }

    public bool IsPointInsideTriangle (Vector2 Point)
    {
        npol = xCoordinates.Length;
        int res = TrianglePoint(npol, xCoordinates, yCoordinates, Point.x, Point.y);
        return res == 1;
    }
}