using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsIdentificationManager : MonoBehaviour
{
    private static ItemsIdentificationManager _instance;
    public static ItemsIdentificationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemsIdentificationManager>();
                _instance.SystematizeItems();
            }

            return _instance;
        }
    }

    [field: Header("Items prefabs."), SerializeField] public GameObject[] ItemsPrefabs { get; set; }

    private void SystematizeItems()
    {
        
        for(int i = 0; i < ItemsPrefabs.Length; i++)
        {
            DraggableItem item = ItemsPrefabs[i].GetComponent<DraggableItem>();
            if(item != null)
            {
                item.ID = i;
            }
        }
    }
}
