using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    #region Fields
    [Header("Speed modifier for item snap to holder."), SerializeField, Range(0, 100)] private float _snapSpeedModifier = 1f;
    [Header("Speed modifier for item drop into drop zone."), SerializeField, Range(0, 100)] private float _dropToZoneSpeedModifier = 1f;

    public UnityEvent OnDragItem { get; set; }
    private DraggableItem _holdingItem = null;
    private ObjectMover _holdingItemMover = null;
    private UnityAction<DraggableItem> _stackAction;
    private DraggableItem _stackItem = null;
    private IDropZone _currentDropZone;
    private IDropZone[] _dropZones;
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
            _holdingItemMover.StartLerpingToTransform(transform, _snapSpeedModifier);
        }

        _stackAction = (item) =>
        {
            _holdingItem.OnStackSender?.Invoke();
            item.Stack();
            _holdingItem.OnDrop.RemoveAllListeners();
        };

        PlayerReferencesContainer.Instance.CurrentHoldingItem = _holdingItem;
        OnDragItem?.Invoke();
    }

    public void SetDropZones (IDropZone[] DropZones)
    {
        _dropZones = DropZones;

        bool hasInventoryManager = DropZones.Any(zone => zone is InventoryManager);
        if(hasInventoryManager)
        {
            _currentDropZone = DropZones.Where(zone => zone is InventoryManager).FirstOrDefault();
            return;
        }

        _currentDropZone = DropZones.Where(zone => zone is RandomPointZone).FirstOrDefault();
    }

    private void Drop()
    {
        if(_holdingItem == null)
            return;

        if(_stackItem == null && _currentDropZone != null)
        {
            if (_holdingItemMover)
            {
                bool canDropIntoDropZone = _currentDropZone.ResponseOnDrop();

                if(!canDropIntoDropZone && _dropZones != null && _dropZones.Length > 0)
                {
                    IEnumerable<IDropZone> availableZones = _dropZones.Where(zone => zone.ResponseOnDrop() && zone != _currentDropZone);

                    if(availableZones != null && availableZones.Count() > 0)
                    {
                        _currentDropZone = availableZones.FirstOrDefault();
                    }
                    else
                        _currentDropZone = null;
                }

                if(_currentDropZone != null)
                {
                    _holdingItemMover.StopLerping();
                    _holdingItem.transform.parent = _currentDropZone.ReturnDropParent();
                    _holdingItemMover.StartLerpingToPosition(_currentDropZone.ReturnDropPoint(), _dropToZoneSpeedModifier);
                }
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

        PlayerReferencesContainer.Instance.CurrentHoldingItem = _holdingItem;
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
