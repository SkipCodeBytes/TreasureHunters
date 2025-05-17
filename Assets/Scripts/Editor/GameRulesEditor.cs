using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameRules))]
public class GameRulesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameRules _myGameRules = (GameRules)target;
        base.OnInspectorGUI();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Default Dices quantity for action", EditorStyles.boldLabel);

        for (int i = 0; i < System.Enum.GetValues(typeof(PlayerDiceAction)).Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label(System.Enum.GetNames(typeof(PlayerDiceAction))[i]);
            GUILayout.FlexibleSpace();
            if (_myGameRules.DicesQuantityForAction.Count < i + 1) _myGameRules.DicesQuantityForAction.Add(0);
            _myGameRules.DicesQuantityForAction[i] = EditorGUILayout.IntField(_myGameRules.DicesQuantityForAction[i]);
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_myGameRules);
            }
        }
    }
}
