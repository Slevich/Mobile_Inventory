using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DraggableItem : MonoBehaviour
{
    #region Fields
    [field: Header("Item ID."), SerializeField,ReadOnly] public int ID { get; set; } = -1; 
    [Header("Front local axis."), SerializeField] private Axes _frontAxis = Axes.Z;
    [field: Header("Amount of the item."), SerializeField, ReadOnly] public int Amount { get; set; } = 1;
    [Header("Inventory size in number of slots."), SerializeField, Range(1, 5)] public int _size = 1;
    [Header("Shape of the item."), SerializeField] private ItemShape _shape = ItemShape.Dot;
    [field: Header("Event on drag."), SerializeField] public UnityEvent OnDrag { get; set; }
    [field: Header("Event on drop."), SerializeField] public UnityEvent OnDrop { get; set; }
    [field: Header("Event when item receive stack."), SerializeField] public UnityEvent<string> OnStackReceiver { get; set; }
    [field: Header("Event when item send stack."), SerializeField] public UnityEvent OnStackSender { get; set; }
    #endregion

    #region Properties
    public int Size
    {
        get
        {
            UpdateShapeValues();

            return _size;
        }
    }

    public ItemShape Shape => _shape;
    #endregion

    #region Methods
    private void OnValidate () => UpdateShapeValues();

    private void UpdateShapeValues()
    {
        if (_shape == ItemShape.Dot && _size != 1)
            _size = 1;
        else if (_shape == ItemShape.Linear && _size < 2)
            _size = 2;
    }

    public void Stack(int StackAmount = 1)
    {
        Amount += StackAmount;
        OnStackReceiver?.Invoke(Amount.ToString());
    }
    #endregion
}

public enum ItemShape
{
    Dot,
    Linear,
    Extended
}