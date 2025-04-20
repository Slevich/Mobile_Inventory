using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerReferencesContainer : MonoBehaviour
{
    private static PlayerReferencesContainer _instance;
    public static PlayerReferencesContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerReferencesContainer>();
            }

            return _instance;
        }
    }

    [field: Header("Touch detector."), SerializeField] public TouchDetector TouchDetector { get; set; }
    [field: Header("Camera ray cast for interaction."), SerializeField] public CameraCaster CameraRaycaster { get; set; }
    [field: Header("Camera pointer position handler."), SerializeField] public CameraPointerPosition PointerPosition { get; set; }

    [field: Header("Inventory management.")]
    [field: SerializeField] public InventoryManager InventoryManager { get; set; }
    [field: SerializeField] public GridUpdater GridUpdater { get; set; }


    private DraggableItem _currentHoldingItem;
    public DraggableItem CurrentHoldingItem
    {
        get
        {
            return _currentHoldingItem;
        }
        set
        {
            OnSetHoldingItem?.Invoke(value);
            _currentHoldingItem = value;
        }
    }
    public Action<DraggableItem> OnSetHoldingItem;
}
