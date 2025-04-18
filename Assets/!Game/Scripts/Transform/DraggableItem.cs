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
    [Header("Amount of item."), SerializeField, ReadOnly] private int _amount = 1;
    [field: Header("Event on drag."), SerializeField] public UnityEvent OnDrag { get; set; }
    [field: Header("Event on drop."), SerializeField] public UnityEvent OnDrop { get; set; }
    [field: Header("Event when item receive stack."), SerializeField] public UnityEvent<string> OnStackReceiver { get; set; }
    [field: Header("Event when item send stack."), SerializeField] public UnityEvent OnStackSender { get; set; }
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Stack()
    {
        _amount++;
        Debug.Log("бвннннЪ!!!");
        OnStackReceiver?.Invoke(_amount.ToString());
    }
    #endregion
}
