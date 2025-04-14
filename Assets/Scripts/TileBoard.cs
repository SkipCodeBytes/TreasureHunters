using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;
using UnityEditor;
using Unity.Collections;

[Serializable]
public class TileBoard : MonoBehaviour
{
    [SerializeField] private Vector2Int _order;
    [SerializeField] private TileType _type = TileType.None;

    public Vector2Int Order { get => _order; set => _order = value; }
    public TileType Type { get => _type; set => _type = value; }


    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), 2f, EventType.Repaint);
    }
}
