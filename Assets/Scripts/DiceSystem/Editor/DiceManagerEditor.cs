using UnityEngine;
using UnityEditor;
using UnityGameBoard.Tiles;

[CustomEditor(typeof(DiceManager))]
public class DiceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    { 
        DiceManager _myDiceManager = (DiceManager)target;
        base.OnInspectorGUI();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Default Dices quantity for action", EditorStyles.boldLabel);

        for (int i = 0; i < System.Enum.GetValues(typeof(PlayerDiceAction)).Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label(System.Enum.GetNames(typeof(PlayerDiceAction))[i]);
            GUILayout.FlexibleSpace();
            if (_myDiceManager.DicesQuantityForAction.Count < i + 1) _myDiceManager.DicesQuantityForAction.Add(0);
            _myDiceManager.DicesQuantityForAction[i] = EditorGUILayout.IntField(_myDiceManager.DicesQuantityForAction[i]);
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_myDiceManager);
            }
        }
    }
}
