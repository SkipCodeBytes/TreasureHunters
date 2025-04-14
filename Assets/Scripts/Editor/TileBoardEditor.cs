using UnityEditor;
using UnityEngine;
using UnityGameBoard.Tiles;

[CustomEditor(typeof(TileBoard))]
public class TileBoardEditor : Editor
{
    private TileBoard _myTile;

    public override void OnInspectorGUI()
    {
        _myTile = (TileBoard)target;

        EditorGUILayout.LabelField("Tile properties", EditorStyles.boldLabel);
        GUILayout.Space(10);
        EditorGUI.indentLevel++;
        _myTile.Type = (TileType)EditorGUILayout.EnumPopup("Type", _myTile.Type);
        GUI.enabled = false;
        _myTile.Order = EditorGUILayout.Vector2IntField("Order", _myTile.Order);
        GUI.enabled = true;
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Create new tile", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("▲", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_myTile.Order + new Vector2Int(0,1)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("◄", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_myTile.Order + new Vector2Int(-1, 0)); }

        if (GUILayout.Button("Delete", GUILayout.Width(60), GUILayout.Height(40))) {
            GameBoardManager _gameBoard = _myTile.transform.parent.GetComponent<GameBoardManager>();
            Undo.RecordObject(_gameBoard, "Eliminar tile del diccionario");
            _gameBoard.TileDicc.Remove(_myTile.Order);
            EditorUtility.SetDirty(_gameBoard);

            Undo.DestroyObjectImmediate(_myTile.gameObject);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        if (GUILayout.Button("►", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_myTile.Order + new Vector2Int(1, 0)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("▼", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_myTile.Order + new Vector2Int(0, -1)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }


    private void createTile(Vector2Int order)
    {
        GameBoardManager _gameBoard = _myTile.transform.parent.GetComponent<GameBoardManager>();

        if (_gameBoard.BaseTilePrefab == null) return;

        if (_gameBoard.TileDicc.ContainsKey(order))
        {
            GameObject tileObjInPos = _gameBoard.TileDicc[order].gameObject;
            Selection.activeGameObject = tileObjInPos;
            if (tileObjInPos != null) return;
        }

        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(_gameBoard.BaseTilePrefab.gameObject, _gameBoard.transform);
        newObj.transform.localPosition = new Vector3(order.x * _gameBoard.SeparationOffset.x,0f , order.y * _gameBoard.SeparationOffset.y);
        newObj.name = $"{order.x}_{order.y} Tile";

        Undo.RegisterCreatedObjectUndo(newObj, "Tile created");

        Selection.activeGameObject = newObj;
        TileBoard _newBoardTile = newObj.GetComponent<TileBoard>();
        _newBoardTile.Order = order;

        Undo.RecordObject(_gameBoard, "Añade tile al diccionario");
        _gameBoard.TileDicc[order] = _newBoardTile;

        EditorUtility.SetDirty(_gameBoard);
        EditorUtility.SetDirty(_newBoardTile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
