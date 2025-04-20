using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class InventoryManager : MonoBehaviour, IDropZone
{
    #region Fields
    [Header("Parent for slots."), SerializeField] private Transform _slotsParent;
    [Header("Parent for items."), SerializeField] private Transform _itemsParent;
    [Header("Inventory slot prefab."), SerializeField] private GameObject _slotPrefab;
    [Header("Collider for grid."), SerializeField] private BoxCollider2D _boxCollider;
    [Space(25), Header("Inventory items."), SerializeField, ReadOnly] private List<InventoryItem> _items = new List<InventoryItem>();

    private GridUpdater _gridUpdater;
    private SlotSpawner _slotSpawner;
    private List<InventorySlot> _slots = new List<InventorySlot>();
    private List<InventorySlot> _selectedSlots = new List<InventorySlot>();
    #endregion

    #region Methods

#if UNITY_EDITOR
    private void OnValidate ()
    {
        if(!Application.IsPlaying(this) && _items.Count > 0)
            _items.Clear();

    }
#endif

    private void Awake ()
    {
        _gridUpdater = PlayerReferencesContainer.Instance.GridUpdater;
        _slotSpawner = new SlotSpawner(_slotPrefab);
        _items = new List<InventoryItem>();
    }

    private void Start ()
    {
        SpawnSlots();
    }

    private void OnEnable () => PlayerReferencesContainer.Instance.OnSetHoldingItem += (item) => RemoveItemFromInventory(item);
    private void OnDisable () => PlayerReferencesContainer.Instance.OnSetHoldingItem -= (item) => RemoveItemFromInventory(item);

    private void SpawnSlots()
    {
        GridSlot[] gridSlots = _gridUpdater.GridSlots.ToArray();
        Vector3[] slotsPositions = gridSlots.Select(slot => slot.GlobalPosition).ToArray();
        Vector3[] slotsScales = gridSlots.Select(slot => slot.GlobalScale).ToArray();
        GameObject[] spawnedSlotsObjects = _slotSpawner.SpawnIntoPoints(_boxCollider, slotsPositions, _slotsParent);

        int currentSlotIndex = 0;
        foreach(GameObject slotObject in spawnedSlotsObjects)
        {
            InventorySlot slot;

            if(slotObject.TryGetComponent<InventorySlot>(out InventorySlot inventorySlot))
                slot = inventorySlot;
            else
                slot = slotObject.AddComponent<InventorySlot>();

            _slots.Add(slot);
            slot.Index = currentSlotIndex;
            slot.GridPosition = gridSlots[currentSlotIndex].GridPosition;
            slotObject.transform.parent = _slotsParent;

            Vector3 reachGlobalScale = slotsScales[currentSlotIndex];
            Vector3 slotLocalScale = slotObject.transform.parent.InverseTransformVector(reachGlobalScale);
            slot.transform.localScale = slotLocalScale;
            currentSlotIndex++;
        }

        GlobalValues.Instance.GraphicImagesBounds = spawnedSlotsObjects.FirstOrDefault().GetComponent<SpriteRenderer>().bounds;
    }

    public void SlotsSelection(Vector3 PointerPosition)
    {
        DraggableItem holdingItem = PlayerReferencesContainer.Instance.CurrentHoldingItem;

        if (holdingItem == null)
            return;

        InventorySlot closestSlot = ReturnClosestSlot(PointerPosition);
        SelectSlots(holdingItem, closestSlot, PointerPosition);
    }

    private void SelectSlot(InventorySlot slot, bool asWrong = false)
    {
        if (asWrong)
        {
            slot.Selected = false;
            slot.WrongSelected = true;
        }
        else
        {
            slot.WrongSelected = false;
            slot.Selected = true;
        }
    }

    private void DeselectSlot(InventorySlot slot, bool removeFromList = false)
    {
        slot.Selected = false;
        slot.WrongSelected = false;

        if (!removeFromList)
            return;

        if (_selectedSlots.Contains(slot))
            _selectedSlots.Remove(slot);
    }

    private void SelectSlots(DraggableItem item, InventorySlot anchorSlot, Vector3 PointerPosition)
    {
        ItemShape itemShape = item.Shape;
        int slotsSize = item.Size;
        InventorySlot[] neighborSlots = null;

        switch (itemShape)
        {
            case ItemShape.Dot:
                break;

            case ItemShape.Linear:
                PointerDirection direction = anchorSlot.CalculatePointerDirection(PointerPosition);
                Vector2 gridPosition = anchorSlot.GridPosition;

                switch(direction)
                {
                    case PointerDirection.Left:
                        neighborSlots = _slots.Where(slot => (slot.GridPosition.x < anchorSlot.GridPosition.x) && (slot.GridPosition.y == anchorSlot.GridPosition.y)).OrderByDescending(slot => slot.GridPosition.x).Take(slotsSize - 1).ToArray();
                        break;

                    case PointerDirection.Right:
                        neighborSlots = _slots.Where(slot => (slot.GridPosition.x > anchorSlot.GridPosition.x) && (slot.GridPosition.y == anchorSlot.GridPosition.y)).OrderBy(slot => slot.GridPosition.x).Take(slotsSize - 1).ToArray();
                        break;

                    case PointerDirection.Down:
                        neighborSlots = _slots.Where(slot => (slot.GridPosition.y > anchorSlot.GridPosition.y) && (slot.GridPosition.x == anchorSlot.GridPosition.x)).Take(slotsSize - 1).ToArray();
                        break;

                    case PointerDirection.Up:
                        neighborSlots = _slots.Where(slot => (slot.GridPosition.y < anchorSlot.GridPosition.y) && (slot.GridPosition.x == anchorSlot.GridPosition.x)).Take(slotsSize - 1).ToArray();
                        break;

                    default:
                        break;

                }

                switch(direction)
                {
                    case PointerDirection.Left or PointerDirection.Right:
                        item.GetComponent<RotationAnimation>().RotateTo(90);
                        break;

                    case PointerDirection.Up or PointerDirection.Down:
                        item.GetComponent<RotationAnimation>().RotateTo(0);
                        break;
                }

                break;

            case ItemShape.Extended:
                break;

            default:
                return;
        }

        List<InventorySlot> newSelectedSlots = new List<InventorySlot>();
        newSelectedSlots.Add(anchorSlot);

        if (neighborSlots != null && neighborSlots.Length > 0)
            newSelectedSlots.AddRange(neighborSlots);

        bool wrongSelected = false;

        if (newSelectedSlots.Count < slotsSize || newSelectedSlots.Any(slot => !slot.IsFree))
            wrongSelected = true;

        if(_selectedSlots.Count > 0)
        {
            IEnumerable<InventorySlot> deselectedSlots = _selectedSlots.Where(slot => !newSelectedSlots.Contains(slot));
            List<InventorySlot> slotsToDeselect = new List<InventorySlot>();
            slotsToDeselect.AddRange(deselectedSlots);
            
            if(deselectedSlots != null && deselectedSlots.Count() > 0)
            {
                foreach (InventorySlot slot in slotsToDeselect)
                {
                    _selectedSlots.Remove(slot);
                    DeselectSlot(slot);
                }
            }
        }

        foreach (InventorySlot slot in newSelectedSlots)
        {
            SelectSlot(slot, wrongSelected);

            if(!_selectedSlots.Contains(slot))
                _selectedSlots.Add(slot);
        }
    }

    private InventorySlot ReturnClosestSlot(Vector3 GlobalPosition)
    {
        Dictionary<InventorySlot, float> slotsDistances = new Dictionary<InventorySlot, float>();

        foreach (InventorySlot slot in _slots)
        {
            float distance = Vector3.Distance(slot.transform.position, GlobalPosition);
            slotsDistances.Add(slot, distance);
        }

        float minDistance = slotsDistances.OrderBy(distance => distance.Value).First().Value;

        if (minDistance > GlobalValues.Instance.GraphicImagesBounds.size.x)
            return null;

        InventorySlot closestSlot = slotsDistances.Where(pair => pair.Value == minDistance).Single().Key;
        return closestSlot;
    }

    public Vector3 ReturnDropPoint()
    {
        if(_selectedSlots.Count > 0)
        {
            PutItemInsideInventory();
            Vector3 slotPosition = Vector3.zero;

            if (_selectedSlots.Count == 1)
            {
                slotPosition = _selectedSlots.First().transform.position;
            }
            else
            {
                Vector3 minXPosition = _selectedSlots.OrderBy(slot => slot.transform.position.x).First().transform.position;
                Vector3 maxXPosition = _selectedSlots.OrderByDescending(slot => slot.transform.position.x).First().transform.position;
                Vector3 minYPosition = _selectedSlots.OrderBy(slot => slot.transform.position.y).First().transform.position;
                Vector3 maxYPosition = _selectedSlots.OrderByDescending(slot => slot.transform.position.y).First().transform.position;
                Vector3 xMidPoint = (minXPosition + maxXPosition) * 0.5f;
                Vector3 yMidPoint = (minYPosition + maxYPosition) * 0.5f;
                slotPosition = new Vector3(xMidPoint.x, yMidPoint.y, 0);
            }

            DeselectSlots();
            return slotPosition;
        }
        else
            return Vector3.zero;
    }

    public Transform ReturnDropParent () => _itemsParent;

    public bool ResponseOnDrop ()
    {
        bool canDrop = _selectedSlots.All(slot => slot.Selected);

        return canDrop;
    }

    public void DeselectSlots()
    {
        if (_selectedSlots.Count == 0)
            return;

        foreach(InventorySlot slot in _selectedSlots)
        {
            DeselectSlot(slot);
        }

        _selectedSlots.Clear();
    }

    public void PutItemInsideInventory()
    {
        bool allSlotsIsFree = _selectedSlots.Any(slot => slot.IsFree);

        if(!allSlotsIsFree)
            return;

        DraggableItem item = PlayerReferencesContainer.Instance.CurrentHoldingItem;

        if(item == null)
            return;

        foreach (InventorySlot slot in _selectedSlots)
        {
            slot.OccupiedItem = item;
        }

        List<InventorySlot> itemSlots = new List<InventorySlot>();
        itemSlots.AddRange(_selectedSlots);
        InventoryItem newInventoryItem = new InventoryItem(itemSlots ,item, item.Amount);
        Debug.Log("Кладет, слоты: " + _selectedSlots.Count);
        _items.Add(newInventoryItem);
    }

    public void RemoveItemFromInventory(DraggableItem holdingItem)
    {
        if(_items.Count == 0)
            return;

        if(holdingItem == null)
            return;

        IEnumerable<InventoryItem> inventoryItems = _items.Where(item => item.Item == holdingItem);

        if (inventoryItems != null && inventoryItems.Count() > 0)
        {
            InventoryItem removingItem = inventoryItems.First();

            Debug.Log("Забирает, слоты: " + removingItem.OccupiedSlots.Count);

            foreach (InventorySlot slot in removingItem.OccupiedSlots)
            {
                slot.OccupiedItem = null;
            }

            _items.Remove(removingItem);
        }
        DeselectSlots();
    }
    #endregion
}

[Serializable]
public class InventoryItem
{
    [field: Header("Slots occupied by item."), SerializeField, ReadOnly] public List<InventorySlot> OccupiedSlots { get; set; } = new List<InventorySlot>();
    [field: Header("Item in slots."), SerializeField, ReadOnly] public DraggableItem Item { get; set; }
    [field: Header("Amount of items."), SerializeField, ReadOnly] public int Amount { get; set; }

    public InventoryItem(List<InventorySlot> Slots = null, DraggableItem InventoryItem = null, int ItemsAmount = -1)
    {
        if (Slots != null)
            OccupiedSlots = Slots;

        if(InventoryItem != null)
            Item = InventoryItem;

        if(ItemsAmount > -1)
            Amount = ItemsAmount;
    }
}