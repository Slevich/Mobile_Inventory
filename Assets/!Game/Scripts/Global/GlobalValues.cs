using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    private static GlobalValues _instance;

    public static GlobalValues Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalValues>();
            }

            return _instance;
        }
    }

    public Bounds GraphicImagesBounds { get; set; } = new Bounds();
}
