using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RangeExAttribute))]
internal sealed class RangeExDrawer : PropertyDrawer
{
    private int value;

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var rangeAttribute = (RangeExAttribute)base.attribute;
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            value = EditorGUI.IntSlider(position, label, property.intValue, rangeAttribute.min, rangeAttribute.max);
            value = (value / rangeAttribute.step) * rangeAttribute.step;
            property.intValue = value;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use Range with int.");
        }
    }
}
#endif