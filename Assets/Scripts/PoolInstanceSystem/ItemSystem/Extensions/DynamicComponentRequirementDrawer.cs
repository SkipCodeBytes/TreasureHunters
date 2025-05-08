using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(DynamicComponentRequirementAttribute))]
public class DynamicComponentRequirementDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DynamicComponentRequirementAttribute dynamicAttr = (DynamicComponentRequirementAttribute)attribute;

        // Obtener el objeto padre
        SerializedProperty parent = property.serializedObject.FindProperty(property.propertyPath.Replace(property.name, dynamicAttr.controllingFieldName));

        if (parent == null)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.HelpBox(position, $"No se encontró el campo '{dynamicAttr.controllingFieldName}'", MessageType.Warning);
            return;
        }

        // Obtener el tipo requerido basado en el enum
        ItemType type = (ItemType)parent.enumValueIndex;
        Type requiredComponentType = GetRequiredComponentType(type);

        EditorGUI.BeginProperty(position, label, property);

        property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(GameObject), false);

        GameObject go = property.objectReferenceValue as GameObject;

        if (go != null && requiredComponentType != null && go.GetComponent(requiredComponentType) == null)
        {
            Rect helpBox = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.HelpBox(helpBox, $"Este GameObject debe tener un componente del tipo '{requiredComponentType.Name}'", MessageType.Error);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        DynamicComponentRequirementAttribute dynamicAttr = (DynamicComponentRequirementAttribute)attribute;
        SerializedProperty parent = property.serializedObject.FindProperty(property.propertyPath.Replace(property.name, dynamicAttr.controllingFieldName));

        if (parent == null) return EditorGUIUtility.singleLineHeight;

        ItemType type = (ItemType)parent.enumValueIndex;
        Type requiredComponentType = GetRequiredComponentType(type);

        GameObject go = property.objectReferenceValue as GameObject;
        bool showWarning = go != null && requiredComponentType != null && go.GetComponent(requiredComponentType) == null;

        float baseHeight = EditorGUIUtility.singleLineHeight;
        return showWarning ? baseHeight + EditorGUIUtility.singleLineHeight * 2 + 2 : baseHeight;
    }

    private Type GetRequiredComponentType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Card:
                return typeof(CardScript);
            case ItemType.Gem:
                return typeof(GemScript);
            default:
                return null;
        }
    }
}