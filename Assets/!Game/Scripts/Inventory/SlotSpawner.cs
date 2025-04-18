using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SlotSpawner
{
    [SerializeField] private GameObject _originalPrefab;
    [SerializeField] private float _scaleModifier = 1f;

    public SlotSpawner(GameObject Prefab, float ScaleModifier)
    {
        _originalPrefab = Prefab;
        _scaleModifier = ScaleModifier;
    }

    public GameObject[] SpawnIntoPoints(BoxCollider2D SizeBaseBox, Vector3[] Points, Transform Parent)
    {
        Vector3 colliderSize = SizeBaseBox.size;
        Vector3[] slotsPositions = Points;
        Vector3 slotSize = (colliderSize / slotsPositions.Length);
        slotSize *= _scaleModifier;

        List<GameObject> spawnedObjects = new List<GameObject>();

        foreach (Vector3 slot in slotsPositions)
        {
            GameObject slotClone = GameObject.Instantiate(_originalPrefab, slot, Quaternion.identity, Parent);
            BoxCollider2D slotBoxCollider = slotClone.GetComponent<BoxCollider2D>();
            Vector3 currentSlotSize = slotBoxCollider.bounds.size;
            float xDifferencePercentage = MathF.Round(slotSize.x / currentSlotSize.x, 2);
            float yDifferencePercentage = MathF.Round(slotSize.y / currentSlotSize.y, 2);

            if (xDifferencePercentage > yDifferencePercentage)
                yDifferencePercentage = xDifferencePercentage;
            else if (yDifferencePercentage > xDifferencePercentage)
                xDifferencePercentage = yDifferencePercentage;

            Transform slotTransform = slotClone.transform;
            Vector3 slotLocalScale = new Vector3(slotTransform.localScale.x * xDifferencePercentage, slotTransform.localScale.y * yDifferencePercentage, 1);
            slotTransform.localScale = slotLocalScale;
            spawnedObjects.Add(slotClone);
        }

        return spawnedObjects.ToArray();
    }
}
