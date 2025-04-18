using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMProTextChanger : MonoBehaviour
{
    [SerializeField] private TextMeshPro _tmPro;
    
    public void ChangeText(string NewText)
    {
        _tmPro.text = NewText;
    }
}
