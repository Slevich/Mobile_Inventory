using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Fields
    [Header("Parent for slots."), SerializeField] private Transform _slotsParent;
    [Header("Inventory slot prefab."), SerializeField] private GameObject _slotPrefab;
    [Header("Collider for grid."), SerializeField] private BoxCollider2D _boxCollider;
    [Header("Slot local scale offset percantage."), Range(0, 1000),SerializeField] private int _scalePercentage = 100;
    [Space(25), Header("Inventory items."), SerializeField, ReadOnly] private List<InventoryItem> _items = new List<InventoryItem>();

    private GridUpdater _gridUpdater;
    private SlotSpawner _slotSpawner;
    private List<InventorySlot> _slots = new List<InventorySlot>();
    #endregion

    #region Methods
    private void Awake ()
    {
        _gridUpdater = SystemReferencesContainer.Instance.GridUpdater;
        _slotSpawner = new SlotSpawner(_slotPrefab, (float)_scalePercentage / 100);
    }

    private void Start ()
    {
        SpawnSlots();
    }

    private void SpawnSlots()
    {
        Vector3[] slotsPositions = _gridUpdater.CellsWorldPositions;
        GameObject[] spawnedSlotsObjects = _slotSpawner.SpawnIntoPoints(_boxCollider, slotsPositions, _slotsParent);

        GlobalValues.Instance.GraphicImagesBounds = spawnedSlotsObjects.FirstOrDefault().GetComponent<SpriteRenderer>().bounds;

        foreach(GameObject slotObject in spawnedSlotsObjects)
        {
            InventorySlot slot;

            if(slotObject.TryGetComponent<InventorySlot>(out InventorySlot inventorySlot))
                slot = inventorySlot;
            else
                slot = slotObject.AddComponent<InventorySlot>();

            _slots.Add(slot);
        }
    }

    public void PutItemInsideInventory(DraggableItem NewItem)
    {

    }

    public void RemoveItemFromInventory(DraggableItem ItemInInventory)
    {

    }

    public void StackItemsInInventory(DraggableItem ItemToStack)
    {

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