
using UnityEditor;
using UnityEngine;
using UnityGameBoard.Tiles;

[CustomEditor(typeof(TileBoard))]
public class TileBoardEditor : Editor
{
    private TileBoard _currentTile;

    public override void OnInspectorGUI()
    {
        _currentTile = (TileBoard)target;

        EditorGUILayout.LabelField("Create and connect tile", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("▲", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_currentTile.Order + new Vector2Int(0,1)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("◄", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_currentTile.Order + new Vector2Int(-1, 0)); }

        if (GUILayout.Button("Delete", GUILayout.Width(60), GUILayout.Height(40))) {
            GameBoardManager _gameBoard = _currentTile.transform.parent.GetComponent<GameBoardManager>();
            if (_gameBoard.transform.childCount != _gameBoard.TileDicc.Count) _gameBoard.recoverGameBoard();

            Undo.RecordObject(_gameBoard, "Eliminar tile del diccionario");

            _gameBoard.TileDicc.Remove(_currentTile.Order);
            EditorUtility.SetDirty(_gameBoard);

            GameObject nextSelectionObj = _gameBoard.gameObject;

            for (int i = 0; i < _currentTile.NextTiles.Count; i++)
            {
                if(nextSelectionObj == _gameBoard.gameObject)
                {
                    if (_currentTile.NextTiles[i] != null) nextSelectionObj = _currentTile.NextTiles[i].gameObject;
                }
                if (_currentTile.NextTiles[i] == null) continue;
                _currentTile.NextTiles[i].PreviusTiles.Remove(_currentTile);
            }

            for (int i = 0; i < _currentTile.PreviusTiles.Count; i++)
            {
                if (nextSelectionObj == _gameBoard.gameObject)
                {
                    if (_currentTile.PreviusTiles[i] != null) nextSelectionObj = _currentTile.PreviusTiles[i].gameObject;
                }
                if (_currentTile.PreviusTiles[i] == null) continue;
                _currentTile.PreviusTiles[i].NextTiles.Remove(_currentTile);
            }

            Selection.activeGameObject = nextSelectionObj;
            //SceneView.lastActiveSceneView.FrameSelected();

            Undo.DestroyObjectImmediate(_currentTile.gameObject);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        if (GUILayout.Button("►", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_currentTile.Order + new Vector2Int(1, 0)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("▼", GUILayout.Width(60), GUILayout.Height(40))) { createTile(_currentTile.Order + new Vector2Int(0, -1)); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        EditorGUILayout.LabelField("Tile properties", EditorStyles.boldLabel);
        GUILayout.Space(10);
        EditorGUI.indentLevel++;
        TileType newType = (TileType)EditorGUILayout.EnumPopup("Type", _currentTile.Type);
        if (newType != _currentTile.Type)
        {
            _currentTile.Type = newType;
            _currentTile.GenerateTileType();
        }
        GUI.enabled = false;
        _currentTile.Order = EditorGUILayout.Vector2IntField("Order", _currentTile.Order);
        GUI.enabled = true;
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        if(_currentTile != null) DrawDefaultInspector();
    }


    private void createTile(Vector2Int order)
    {
        GameBoardManager _gameBoard = _currentTile.transform.parent.GetComponent<GameBoardManager>();

        if(_gameBoard.transform.childCount != _gameBoard.TileDicc.Count) _gameBoard.recoverGameBoard();

        if (_gameBoard.BaseTilePrefab == null) return;
        if (_gameBoard.TileDicc.ContainsKey(order))
        {
            if (_gameBoard.TileDicc[order] != null)
            {
                Selection.activeGameObject = _gameBoard.TileDicc[order].gameObject;
                if (!_currentTile.PreviusTiles.Contains(_gameBoard.TileDicc[order]) && !_currentTile.NextTiles.Contains(_gameBoard.TileDicc[order]))
                {
                    _currentTile.NextTiles.Add(_gameBoard.TileDicc[order]);
                    _gameBoard.TileDicc[order].PreviusTiles.Add(_currentTile);
                }
                return; 
            }
        }

        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(_gameBoard.BaseTilePrefab.gameObject, _gameBoard.transform);
        newObj.transform.localPosition = new Vector3(order.x * _gameBoard.SeparationOffset.x,0f , order.y * _gameBoard.SeparationOffset.y);
        newObj.name = $"{order.x}_{order.y} Tile";
        Undo.RegisterCreatedObjectUndo(newObj, "Tile created");

        Selection.activeGameObject = newObj;
        //SceneView.lastActiveSceneView.FrameSelected();
        TileBoard _newTile = newObj.GetComponent<TileBoard>();
        _newTile.Order = order;

        if (!_currentTile.PreviusTiles.Contains(_newTile) && !_currentTile.NextTiles.Contains(_newTile))
        {
            _currentTile.NextTiles.Add(_newTile);
            _newTile.PreviusTiles.Add(_currentTile);
        }
        _newTile.GenerateTileType();
        EditorUtility.SetDirty(_currentTile);
        EditorUtility.SetDirty(_newTile);

        Undo.RecordObject(_gameBoard, "Añade tile al diccionario");
        _gameBoard.TileDicc[order] = _newTile;
    }
}
