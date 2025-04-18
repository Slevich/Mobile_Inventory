using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemReferencesContainer : MonoBehaviour
{
    private static SystemReferencesContainer _instance;
    public static SystemReferencesContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SystemReferencesContainer>();
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
}
