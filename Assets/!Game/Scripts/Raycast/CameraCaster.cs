using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;
using static UnityEngine.UI.Image;

public class CameraCaster : MonoBehaviour
{
    #region Fields
    [Header("Pointer prefab."), SerializeField] private GameObject _pointer;
    [Header("Ray length."), SerializeField, Range(0.1f, 10f)] private float _rayLength = 5f;
    [Header("Raycasted masks."), SerializeField] private LayerMask _masks = 0;

    private ActionInterval _raycastingInverval;
    private CameraPointerPosition _pointerPosition;
    private GameObject _pointerInstance;
    private PositionFollower _positionFollower;
    private ItemHolder _itemHolder;
    private bool _firstCast = false;
    private static readonly float _raycastingUpdateTime = 0.05f;
    private Vector3 _origin;
    #endregion

    #region Properties
    public Action<DraggableItem> OnFirstCastItem { get; set; }
    public Action<DraggableItem[]> OnHitItems { get; set; }
    #endregion

    #region Methods
    private void Awake ()
    {
        _pointerPosition = SystemReferencesContainer.Instance.PointerPosition;
        _raycastingInverval = new ActionInterval();

        if(_pointerPosition)
            _pointerPosition.OnWorldPointerPosition += (hasPointer, pos) => TryToRaycast(hasPointer, pos);
    }

    private void TryToRaycast(bool needToRaycast, Vector3 origin)
    {
        _origin = origin;

        if(needToRaycast && !_raycastingInverval.Busy)
        {
            Action raycastAction = delegate
            {
                Transform cameraTransform = Camera.main.transform;
                Vector3 cameraLocalDirection = cameraTransform.forward;
                Vector3 cameraWorldDirection = cameraTransform.parent.TransformDirection(cameraLocalDirection);

                RaycastHit2D[] hits = Physics2D.RaycastAll(_origin, cameraWorldDirection, _rayLength, _masks);

                if (hits != null && hits.Length > 0)
                {
                    ReactToHitObjects(hits);
                }

                Debug.DrawLine(origin, origin + cameraWorldDirection, Color.green);
            };

            _raycastingInverval.StartInterval(_raycastingUpdateTime, raycastAction);
        }
        else if(!needToRaycast && _raycastingInverval.Busy)
        {
            _raycastingInverval.Stop();

            if (_pointerInstance != null)
            {
                Destroy(_pointerInstance);
                _pointerInstance = null;
            }
        }
    }

    private void ReactToHitObjects (RaycastHit2D[] hits)
    {
        GameObject[] raycastedObjects = hits.Select(hit => hit.collider.gameObject).ToArray();

        GameObject surfaceObject = raycastedObjects.Where(raycasted => raycasted.GetComponent<RaycastSurface>() != null).FirstOrDefault();

        if (surfaceObject != null)
        {
            RaycastHit2D surfaceHit = hits.Where(hit => hit.collider.gameObject == surfaceObject).FirstOrDefault();
            Vector3 hitPosition = new Vector3(surfaceHit.point.x, surfaceHit.point.y, surfaceObject.transform.position.z);

            if (_pointerInstance == null)
            {
                _firstCast = true;
                _pointerInstance = Instantiate(_pointer, hitPosition, Quaternion.identity, transform);
                _positionFollower = _pointerInstance.AddComponent<PositionFollower>();
                _itemHolder = _pointerInstance.GetComponent<ItemHolder>();
            }
            else
            {
                _firstCast = false;
                _positionFollower.UpdatePosition(hitPosition);
            }
        }
        else
            return;

        IEnumerable<GameObject> dropZonesObjects = raycastedObjects.Where(raycasted => raycasted.GetComponent<IDropZone>() != null);
        if(dropZonesObjects != null && dropZonesObjects.Count() > 0)
        {
            GameObject dropZoneObject = raycastedObjects.Where(raycasted => raycasted.GetComponent<IDropZone>() != null).First();
            IDropZone dropZone = dropZoneObject.GetComponent<IDropZone>();

            if (dropZone != null)
                Debug.Log("ƒроп«она!");

            _itemHolder.SetDropZone(dropZone);
        }

        IEnumerable<GameObject> draggableItems = raycastedObjects.Where(raycasted => raycasted.GetComponent<DraggableItem>() != null);
        if(draggableItems != null && draggableItems.Count() > 0 && _itemHolder != null)
        {
            if (_firstCast)
            {
                _itemHolder.Drag(draggableItems.FirstOrDefault().GetComponent<DraggableItem>());
            }
            else
            {
                _itemHolder.DetectedStackItems(draggableItems.Select(item => item.GetComponent<DraggableItem>()).ToArray());
            }
        }
    }
    #endregion
}
