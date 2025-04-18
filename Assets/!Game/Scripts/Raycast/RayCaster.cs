using System.Net;
using System.Transactions;
using UnityEngine;

public class RayCaster
{
    #region Fields
    private float _rayLength = 10f;
    private LayerMask _raycastedMasks = 0;
    #endregion

    #region Methods
    public RayCaster(float RayLength, LayerMask RaycastedLayers)
    {
        _rayLength = RayLength;
        _raycastedMasks = RaycastedLayers;
    }

    public bool RaycastAndReturnHit(Vector3 GlobalOriginPosition, Vector3 RaycastDirection, out RaycastHit hit)
    {
        if(Physics.Raycast(GlobalOriginPosition, RaycastDirection, out RaycastHit raycastHit, _rayLength, _raycastedMasks))
        {
            hit = raycastHit;
            return true;
        }
        else
        {
            hit = new RaycastHit();
            return false;
        }
    }
    #endregion
}