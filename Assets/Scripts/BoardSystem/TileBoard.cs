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

    private TileBehavior _tileBehavior;

    public Vector2Int Order { get => _order; set => _order = value; }
    public TileType Type { get => _type; set => _type = value; }
    public List<TileBoard> PreviusTiles { get => previusTiles; set => previusTiles = value; }
    public List<TileBoard> NextTiles { get => nextTiles; set => nextTiles = value; }
    public TileBehavior TileBehavior { get => _tileBehavior; set => _tileBehavior = value; }

    private void Awake()
    {
        _tileBehavior = transform.GetComponentInChildren<TileBehavior>();
    }

#if UNITY_EDITOR
    public void GenerateTileType()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        GameBoardManager _gameBoard = transform.parent.GetComponent<GameBoardManager>();
        if (Enum.GetValues(typeof(TileType)).Length > 0)
        {
            if (_gameBoard.TilesPrefab[(int)Type] != null)
            {
                GameObject newTile = (GameObject)PrefabUtility.InstantiatePrefab(_gameBoard.TilesPrefab[(int)Type], transform);
                newTile.transform.position = transform.position;
                newTile.transform.rotation = transform.rotation;
            }
        }
    }

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
