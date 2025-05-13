using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityGameBoard.Tiles;

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
        EditorGUILayout.LabelField("Tile Prefabs", EditorStyles.boldLabel);

        for (int i = 0; i < System.Enum.GetValues(typeof(TileType)).Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.Label(System.Enum.GetNames(typeof(TileType))[i]);
            GUILayout.FlexibleSpace();
            if (_myGameBoard.TilesPrefab.Count < i + 1) _myGameBoard.TilesPrefab.Add(null);
            _myGameBoard.TilesPrefab[i] = (GameObject)EditorGUILayout.ObjectField(_myGameBoard.TilesPrefab[i], typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Tile Creation", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.green;
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
            newObj.GetComponent<TileBoard>().GenerateTileType();
            Undo.RegisterCreatedObjectUndo(newObj, "Tile created");
            Selection.activeGameObject = newObj;

            Undo.RecordObject(_myGameBoard, "Añade tile al diccionario");
            _myGameBoard.TileDicc[Vector2Int.zero] = newObj.GetComponent<TileBoard>();
            EditorUtility.SetDirty(_myGameBoard);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh Data", GUILayout.Width(200), GUILayout.Height(40)))
        {
            _myGameBoard.recoverGameBoard();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete All", GUILayout.Width(200), GUILayout.Height(40)))
        {
            _myGameBoard.DeleteAllTiles();
            _myGameBoard.TileDicc.Clear();
            Undo.RegisterCreatedObjectUndo(_myGameBoard, "Destroy All");
        }
        GUI.backgroundColor = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }

}
