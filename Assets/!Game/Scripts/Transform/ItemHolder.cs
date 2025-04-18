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
    private PositionLerper _lerper;
    private Action _onItemSnapping;
    private ActionInterval _snappingInterval;
    private CameraCaster _caster;
    private float _snapPositionUpdateTime = 0.01f;
    private UnityAction<DraggableItem> _stackAction;
    private DraggableItem _stackItem = null;
    #endregion

    #region Methods
    private void Awake ()
    {
        _lerper = new PositionLerper();
        _snappingInterval = new ActionInterval();

        _caster = SystemReferencesContainer.Instance.CameraRaycaster;

        _onItemSnapping = delegate
        {
            if (this == null)
                return;

            _lerper.LerpToWorldPosition(_holdingItem.transform, transform.position, _speedModifier);
        };
    }

    public void Drag (DraggableItem Item)
    {
        if (_holdingItem != null)
            return;

        _holdingItem = Item;
        _holdingItem.OnDrag?.Invoke();
        _snappingInterval.StartInterval(_snapPositionUpdateTime, _onItemSnapping);

        _stackAction = (item) =>
        {
            _holdingItem.OnStackSender?.Invoke();
            item.Stack();
            _holdingItem.OnDrop.RemoveAllListeners();
        };
    }

    private void Drop ()
    {
        if(_holdingItem == null)
            return;

        if (_snappingInterval.Busy)
            _snappingInterval.Stop();

        _holdingItem.OnDrop?.Invoke();
        _holdingItem = null;
    }

    public void DetectedStackItems(DraggableItem[] StackItems)
    {
        List<DraggableItem> stackItems = StackItems.ToList();

        if(stackItems.Contains(_holdingItem))
            stackItems.Remove(_holdingItem);

        if (stackItems.Count == 0)
        {
            if(_stackItem != null)
            {
                _stackItem = null;
                _holdingItem.OnDrop.RemoveListener(ActOnStack);
            }

            return;
        }

        DraggableItem stackItem = stackItems.Where(item => item.ID == _holdingItem.ID).FirstOrDefault();

        if(stackItem != null && stackItem != _stackItem)
        {
            _stackItem = stackItem;
            _holdingItem.OnDrop.AddListener(ActOnStack);
        }
    }

    private void ActOnStack()
    {
        _holdingItem.OnStackSender?.Invoke();
        _stackItem.Stack();
        _holdingItem.OnDrop.RemoveAllListeners();
    }

    private void OnDisable () => Drop();
    #endregion
}
