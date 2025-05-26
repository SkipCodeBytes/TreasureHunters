using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameBoard.Tiles;

public class GameBoardManager : MonoBehaviour
{
    [SerializeField] private TileBoard _baseTilePrefab;
    [SerializeField] private Vector2 _separationOffset = new Vector2(2.5f,2.5f);
    
    [Header("Highlighter")]
    [SerializeField] private GameObject tileHighlighter;
    [SerializeField] private Vector3 tileHighlighterOffset;
    [SerializeField] private float highlighterBaseIntensity = 20f;
    [SerializeField] private float highlighterFocusIntensity = 60f;
    private List<TileBoard> _activeHighlighsTiles = new List<TileBoard>();

    private Dictionary<Vector2Int, TileBoard> _tileDicc = new Dictionary<Vector2Int, TileBoard>();
    

    [SerializeField, HideInInspector] private List<GameObject> _tilesPrefab = new List<GameObject>();

    public TileBoard BaseTilePrefab { get => _baseTilePrefab; set => _baseTilePrefab = value; }
    public Vector2 SeparationOffset { get => _separationOffset; set => _separationOffset = value; }
    public Dictionary<Vector2Int, TileBoard> TileDicc { get => _tileDicc; set => _tileDicc = value; }
    public List<GameObject> TilesPrefab { get => _tilesPrefab; set => _tilesPrefab = value; }
    public GameObject TileHighlighter { get => tileHighlighter; set => tileHighlighter = value; }
    public Vector3 TileHighlighterOffset { get => tileHighlighterOffset; set => tileHighlighterOffset = value; }
    public List<TileBoard> ActiveHighlighsTiles { get => _activeHighlighsTiles; set => _activeHighlighsTiles = value; }
    public float HighlighterBaseIntensity { get => highlighterBaseIntensity; set => highlighterBaseIntensity = value; }
    public float HighlighterFocusIntensity { get => highlighterFocusIntensity; set => highlighterFocusIntensity = value; }

    private void Awake()
    {
        recoverGameBoard();
    }

#if UNITY_EDITOR
    public void DeleteAllTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
#endif

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

            tile.PreviusTiles.RemoveAll(item => item == null);
            tile.NextTiles.RemoveAll(item => item == null);

            #if UNITY_EDITOR
            EditorUtility.SetDirty(tile);
            #endif
        }
    }

    public List<TileBoard> GetAllTileOfType(TileType type)
    {
        List<TileBoard> tileList = new List<TileBoard>();
        foreach (Vector2Int key in _tileDicc.Keys)
        {
            if (_tileDicc[key].Type == type)
            {
                tileList.Add(_tileDicc[key]);
            }
        }
        return tileList;
    }
    

    public void TurnOffAllHighlight()
    {
        List<TileBoard> tiles = new List<TileBoard>(_activeHighlighsTiles);
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TurnOffHighlight();
        }
        _activeHighlighsTiles.Clear();
    }

    public void BaseAllHighlight()
    {
        for (int i = 0; i < _activeHighlighsTiles.Count; i++)
        {
            _activeHighlighsTiles[i].HighlightBase();
        }
    }
}
