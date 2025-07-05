#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(CardItemData))]
public class CardItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Dibuja campos normales del ScriptableObject

        CardItemData cardData = (CardItemData)target;

        // Obtener métodos válidos de la clase CardMethods
        List<MethodInfo> metodosValidos = new List<MethodInfo>();
        foreach (var metodo in typeof(CardMethods).GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (metodo.GetCustomAttribute<InvocableAttribute>() != null &&
                metodo.GetParameters().Length == 0)
            {
                metodosValidos.Add(metodo);
            }
        }

        // Obtener nombres de los métodos
        string[] opciones = metodosValidos.ConvertAll(m => m.Name).ToArray();

        // Índice actual del método seleccionado
        int indexActual = Mathf.Max(0, Array.IndexOf(opciones, cardData.MethodName));

        // Mostrar combo box
        int indexSeleccionado = EditorGUILayout.Popup("Método de Ejecución", indexActual, opciones);

        // Guardar selección en el campo 'methodName'
        if (indexSeleccionado >= 0 && indexSeleccionado < opciones.Length)
        {
            // Para evitar marcar dirty innecesario
            if (cardData.MethodName != opciones[indexSeleccionado])
            {
                Undo.RecordObject(cardData, "Cambiar método");
                SerializedObject so = new SerializedObject(cardData);
                so.FindProperty("methodName").stringValue = opciones[indexSeleccionado];
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(cardData);
            }
        }
    }
}
#endif
