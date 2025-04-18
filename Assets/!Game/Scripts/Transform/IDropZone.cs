using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropZone
{
    public Vector3 ReturnDropPoint ();
    public Transform ReturnDropParent ();
}
