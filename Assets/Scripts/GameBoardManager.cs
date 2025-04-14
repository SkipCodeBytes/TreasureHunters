using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameBoardManager : MonoBehaviour
{
    [SerializeField] private TileBoard _baseTilePrefab;
    [SerializeField] private Vector2 _separationOffset = new Vector2(2.5f,2.5f);

    private Dictionary<Vector2Int, TileBoard> _tileDicc = new Dictionary<Vector2Int, TileBoard>();

    public TileBoard BaseTilePrefab { get => _baseTilePrefab; set => _baseTilePrefab = value; }
    public Vector2 SeparationOffset { get => _separationOffset; set => _separationOffset = value; }
    public Dictionary<Vector2Int, TileBoard> TileDicc { get => _tileDicc; set => _tileDicc = value; }

    private void Awake()
    {
        recoverGameBoard();
    }

    public void recoverGameBoard()
    {
        //Reposicionamos tiles, eliminamos posibles duplicados y elementos que no pertenecen a la lógica del GameBoard
        _tileDicc.Clear();
        foreach (Transform child in transform)
        {
            TileBoard tile = child.GetComponent<TileBoard>();
            if (tile == null)
            {
                DestroyImmediate(child.gameObject);
                continue;
            } else if (_tileDicc.ContainsKey(tile.Order))
            {
                DestroyImmediate(child.gameObject);
                continue;
            }
            child.localPosition = new Vector3(tile.Order.x * _separationOffset.x, 0f, tile.Order.y * _separationOffset.y);
            _tileDicc.Add(tile.Order, tile);
        }
        Debug.Log("Tiles actualizados: " + _tileDicc.Count);
    }
}
