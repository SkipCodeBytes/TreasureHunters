using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(GameBoardManager))]
public class GameBoardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameBoardManager _myGameBoard = (GameBoardManager)target;

        EditorGUILayout.LabelField("Config", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        DrawDefaultInspector();
        EditorGUI.indentLevel--;

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create First Tile", GUILayout.Width(200), GUILayout.Height(40)))
        {
            if (_myGameBoard.BaseTilePrefab == null) {
                Debug.LogError("Debes tener como referencia un prefab con el componente TileBoard");
                return; 
            }

            _myGameBoard.recoverGameBoard();

            if (_myGameBoard.transform.childCount > 0) {
                Selection.activeGameObject = _myGameBoard.transform.GetChild(0).gameObject;
                return; 
            }

            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(_myGameBoard.BaseTilePrefab.gameObject, _myGameBoard.transform);
            newObj.transform.localPosition = Vector3.zero;
            newObj.name = "0_0 Tile";

            Undo.RegisterCreatedObjectUndo(newObj, "Tile created");
            Selection.activeGameObject = newObj;

            Undo.RecordObject(_myGameBoard, "Añade tile al diccionario");
            _myGameBoard.TileDicc[Vector2Int.zero] = newObj.GetComponent<TileBoard>();
            EditorUtility.SetDirty(_myGameBoard);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh Data", GUILayout.Width(200), GUILayout.Height(40)))
        {
            _myGameBoard.recoverGameBoard();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

}
