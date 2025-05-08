using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameItem))]
public class GameItemDrawner : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //position = EditorGUI.PrefixLabel(position, label);

        float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        int currentLine = 0;

        SerializedProperty itemTypeProp = property.FindPropertyRelative("itemType");
        SerializedProperty prefabProp = property.FindPropertyRelative("itemPrefab");
        SerializedProperty iconProp = property.FindPropertyRelative("icon");
        SerializedProperty cardDataProp = property.FindPropertyRelative("cardData");
        SerializedProperty gemFormProp = property.FindPropertyRelative("gemForm");
        SerializedProperty gemColorProp = property.FindPropertyRelative("gemColor");

        ItemType itemType = (ItemType)itemTypeProp.enumValueIndex;

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;

        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
        position.y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.indentLevel = 1;

        Rect itemTypeRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
        EditorGUI.PropertyField(itemTypeRect, itemTypeProp);

        bool showWarning = false;
        string warningText = "";

        if (itemType != ItemType.None)
        {
            Rect prefabRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
            EditorGUI.PropertyField(prefabRect, prefabProp);

            GameObject go = prefabProp.objectReferenceValue as GameObject;

            // Verificación de componente según tipo
            if (go != null)
            {
                switch (itemType)
                {
                    case ItemType.Card:
                        if (go.GetComponent<CardScript>() == null)
                        {
                            showWarning = true;
                            warningText = "El prefab debe tener el componente CardScript.";
                        }
                        break;
                    case ItemType.Gem:
                        if (go.GetComponent<GemScript>() == null)
                        {
                            showWarning = true;
                            warningText = "El prefab debe tener el componente GemScript.";
                        }
                        break;
                }
            }

            if (showWarning)
            {
                Rect warningRect = new Rect(position.x, position.y + (lineHeight * currentLine) + 2, position.width, EditorGUIUtility.singleLineHeight * 2);
                EditorGUI.HelpBox(warningRect, warningText, MessageType.Error);
                currentLine += 2; // Aumentamos 2 líneas para dejar espacio suficiente
            }

            Rect iconRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
            EditorGUI.PropertyField(iconRect, iconProp);
        }

        switch (itemType)
        {
            case ItemType.Card:
                Rect cardDataRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
                EditorGUI.PropertyField(cardDataRect, cardDataProp);
                break;

            case ItemType.Gem:
                Rect gemFormRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
                EditorGUI.PropertyField(gemFormRect, gemFormProp);

                Rect gemColorRect = new Rect(position.x, position.y + (lineHeight * currentLine++), position.width, lineHeight);
                EditorGUI.PropertyField(gemColorRect, gemColorProp);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 2; // itemType
        var itemTypeProp = property.FindPropertyRelative("itemType");
        ItemType itemType = (ItemType)itemTypeProp.enumValueIndex;

        if (itemType != ItemType.None)
        {
            lines += 2; // prefab + icon

            // Verificamos si se necesita HelpBox
            var prefabProp = property.FindPropertyRelative("itemPrefab");
            GameObject go = prefabProp.objectReferenceValue as GameObject;

            if (go != null)
            {
                if ((itemType == ItemType.Card && go.GetComponent<CardScript>() == null) ||
                    (itemType == ItemType.Gem && go.GetComponent<GemScript>() == null))
                {
                    lines += 2; // espacio para HelpBox
                }
            }
        }

        if (itemType == ItemType.Card)
            lines += 1;
        else if (itemType == ItemType.Gem)
            lines += 2;

        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
