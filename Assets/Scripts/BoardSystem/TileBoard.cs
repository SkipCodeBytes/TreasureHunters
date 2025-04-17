using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;
using UnityEditor;
using Unity.Collections;

[Serializable]
public class TileBoard : MonoBehaviour
{
    [SerializeField] private Vector2Int _order = Vector2Int.zero;
    [SerializeField] private TileType _type = TileType.None;

    [SerializeField] private List<TileBoard> previusTiles = new List<TileBoard>();
    [SerializeField] private List<TileBoard> nextTiles = new List<TileBoard>();

    public Vector2Int Order { get => _order; set => _order = value; }
    public TileType Type { get => _type; set => _type = value; }
    public List<TileBoard> PreviusTiles { get => previusTiles; set => previusTiles = value; }
    public List<TileBoard> NextTiles { get => nextTiles; set => nextTiles = value; }

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
}
