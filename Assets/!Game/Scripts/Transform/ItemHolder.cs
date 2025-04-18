using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemHolder : MonoBehaviour
{
    #region Fields
    [Header("Speed modifier for item snap."), SerializeField, Range(0, 100)] private float _speedModifier = 1f;

    private DraggableItem _holdingItem = null;
    private ObjectMover _holdingItemMover = null;
    private UnityAction<DraggableItem> _stackAction;
    private DraggableItem _stackItem = null;
    private Vector3 _snapPosition = Vector3.zero;
    private IDropZone _currentDropZone;
    #endregion

    #region Methods
    public void Drag (DraggableItem Item)
    {
        if (_holdingItem != null)
            return;

        _holdingItem = Item;
        _holdingItemMover = _holdingItem.GetComponent<ObjectMover>();
        _holdingItem.OnDrag?.Invoke();

        if (_holdingItemMover)
        {
            _holdingItemMover.StopLerping();
            _holdingItemMover.StartLerpingToTransform(transform, _speedModifier);
        }

        _stackAction = (item) =>
        {
            _holdingItem.OnStackSender?.Invoke();
            item.Stack();
            _holdingItem.OnDrop.RemoveAllListeners();
        };
    }

    public void SetDropZone(IDropZone DropZone) => _currentDropZone = DropZone; 

    private void Drop()
    {
        if(_holdingItem == null)
            return;

        if(_stackItem == null && _currentDropZone != null)
        {
            if (_holdingItemMover)
            {
                _holdingItemMover.StopLerping();
                _holdingItem.transform.parent = _currentDropZone.ReturnDropParent();
                _holdingItemMover.StartLerpingToPosition(_currentDropZone.ReturnDropPoint(), _speedModifier);
            }

            _holdingItem.OnDrop?.Invoke();
            _holdingItem = null;
        }
        else if(_stackItem != null)
        {
            _holdingItem.OnDrop.AddListener(ActOnStack);
            _holdingItem.OnDrop?.Invoke();
            _holdingItem = null;
        }
    }

    public void DetectedStackItems(DraggableItem[] StackItems)
    {
        if (_holdingItem == null)
            return;

        List<DraggableItem> items = StackItems.ToList();

        if(items.Contains(_holdingItem))
            items.Remove(_holdingItem);

        if (items.Count == 0)
        {
            if(_stackItem != null)
            {
                _stackItem = null;
                _holdingItem.OnDrop.RemoveListener(ActOnStack);
            }

            return;
        }

        IEnumerable<DraggableItem> stackItems = items.Where(item => item.ID == _holdingItem.ID);

        if(stackItems != null && stackItems.Count() > 0)
        {
            DraggableItem item = stackItems.FirstOrDefault();

            if(item != _stackItem)
            {
                _stackItem = item;
            }
        }
    }

    private void ActOnStack()
    {
        _holdingItem.OnStackSender?.Invoke();
        _stackItem.Stack(_holdingItem.Amount);
        _holdingItem.OnDrop.RemoveAllListeners();
    }

    private void OnDisable () => Drop();
    #endregion
}
