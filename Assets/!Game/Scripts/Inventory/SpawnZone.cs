using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    #region Fields
    [Header("Collider zone to spawn inside."), SerializeField] private BoxCollider2D _colliderZone;
    [Header("Parent for spawned objects."), SerializeField] private Transform _objectsParent;
    private GameObject[] _prefabsToSpawn;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (!_colliderZone && TryGetComponent<BoxCollider2D>(out BoxCollider2D collider))
            _colliderZone = collider;

        _prefabsToSpawn = ItemsIdentificationManager.Instance.ItemsPrefabs;
    }

    public void SpawnRandomObject()
    {
        if(!_colliderZone)
        {
            Debug.LogError("Collider zone has no collider!");
            return;
        }

        Bounds bounds = _colliderZone.bounds;
        float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetY = Random.Range(-bounds.extents.y, bounds.extents.y);
        float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);
        Vector3 randomPosition = bounds.center + new Vector3(offsetX, offsetY, offsetZ);

        int randomIndex = Random.Range(0, _prefabsToSpawn.Length - 1);
        GameObject randomPrefab = _prefabsToSpawn[randomIndex];
        GameObject spawnedObject = Instantiate(randomPrefab, _objectsParent);
        spawnedObject.transform.position = randomPosition;
    }
    #endregion
}
