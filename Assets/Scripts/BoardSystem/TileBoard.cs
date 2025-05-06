using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;
using UnityEditor;

[Serializable]
public class TileBoard : MonoBehaviour
{
    [SerializeField, HideInInspector] private Vector2Int _order = Vector2Int.zero;
    [SerializeField, HideInInspector] private TileType _type = TileType.None;

    [SerializeField] private List<TileBoard> previusTiles = new List<TileBoard>();
    [SerializeField] private List<TileBoard> nextTiles = new List<TileBoard>();

    private TileBehaviorScript _behaviorScript;

    public Vector2Int Order { get => _order; set => _order = value; }
    public TileType Type { get => _type; set => _type = value; }
    public List<TileBoard> PreviusTiles { get => previusTiles; set => previusTiles = value; }
    public List<TileBoard> NextTiles { get => nextTiles; set => nextTiles = value; }
    public TileBehaviorScript BehaviorScript { get => _behaviorScript; set => _behaviorScript = value; }

    private void Awake()
    {
        _behaviorScript = transform.GetComponentInChildren<TileBehaviorScript>();
    }


    public void GenerateTileType()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(0).gameObject);
#else
            Destroy(transform.GetChild(0).gameObject);
#endif
        }
        GameBoardManager _gameBoard = transform.parent.GetComponent<GameBoardManager>();
        if (System.Enum.GetValues(typeof(TileType)).Length > 0)
        {
            if (_gameBoard.TilesPrefab[(int)Type] != null)
            {

#if UNITY_EDITOR
                GameObject newTile = (GameObject)PrefabUtility.InstantiatePrefab(_gameBoard.TilesPrefab[(int)Type], transform);
                newTile.transform.position = transform.position;
                newTile.transform.rotation = transform.rotation;
#else
                GameObject newTile = Instantiate(_gameBoard.TilesPrefab[(int)Type], transform);
                newTile.transform.position = transform.position;
                newTile.transform.rotation = transform.rotation;
#endif
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float arrowSize = 1f;
        Handles.color = Color.cyan;
        for (int i = 0; i < nextTiles.Count; i++)
        {
            if (nextTiles[i] == null) continue;
            Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(nextTiles[i].transform.position - transform.position), arrowSize, EventType.Repaint);
        }

        Handles.color = Color.white;
        for (int i = 0; i < previusTiles.Count; i++)
        {
            if (previusTiles[i] == null) continue;
            Vector3 direction = -(previusTiles[i].transform.position - transform.position).normalized;
            Handles.ArrowHandleCap(0, transform.position - (direction * arrowSize), Quaternion.LookRotation(direction), arrowSize, EventType.Repaint);
        }
    }
#endif
}
