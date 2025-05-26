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

    private GameBoardManager _gameBoardManager;
    private TileBehavior _tileBehavior;

    private Light _activeHighlighGuide;
    private MeshRenderer _activeSignGuide;
    private bool _isQuestionGuide = false;

    public Vector2Int Order { get => _order; set => _order = value; }
    public TileType Type { get => _type; set => _type = value; }
    public List<TileBoard> PreviusTiles { get => previusTiles; set => previusTiles = value; }
    public List<TileBoard> NextTiles { get => nextTiles; set => nextTiles = value; }
    public TileBehavior TileBehavior { get => _tileBehavior; set => _tileBehavior = value; }
    public GameBoardManager GameBoardManager { get => _gameBoardManager; set => _gameBoardManager = value; }

    private void Awake()
    {
        _tileBehavior = transform.GetComponentInChildren<TileBehavior>();
        _gameBoardManager = transform.GetComponentInParent<GameBoardManager>();
    }


    public void TurnOnGuide(Color highlightColor)
    {
        if (_activeHighlighGuide != null) return;
        GameObject highlighter = InstanceManager.Instance.GetObject(_gameBoardManager.TileHighlighter);
        _activeHighlighGuide = highlighter.GetComponent<Light>();
        _activeHighlighGuide.color = highlightColor;
        highlighter.transform.position = transform.position + _gameBoardManager.TileHighlighterOffset;
        _gameBoardManager.ActiveHighlighsTiles.Add(this);

        GameObject arrowGuide;

        _isQuestionGuide = false;
        if (nextTiles.Count == 1) { arrowGuide = InstanceManager.Instance.GetObject(_gameBoardManager.DirectionArrow); }
        else { 
            arrowGuide = InstanceManager.Instance.GetObject(_gameBoardManager.DirectionQuestion);
            _isQuestionGuide = true;
        }

        _activeSignGuide = arrowGuide.GetComponent<MeshRenderer>();
        arrowGuide.transform.position = transform.position + _gameBoardManager.DirectionGuideOffset;
        Vector3 direction = nextTiles[0].transform.position - _activeSignGuide.transform.position;
        Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);
        _activeSignGuide.transform.rotation = rotation * Quaternion.Euler(_gameBoardManager.RotationGuideOffset);

        GuideBasicView();
    }

    public void GuideFocusView()
    {
        if (_activeSignGuide == null) return;
        _activeHighlighGuide.intensity = _gameBoardManager.HighlighterFocusIntensity;
        if(!_isQuestionGuide) { _activeSignGuide.material = _gameBoardManager.DirectionGuideFocusMaterial; }
        else { _activeSignGuide.material = _gameBoardManager.QuestionGuideFocusMaterial; }
    }

    public void GuideBasicView()
    {
        if (_activeSignGuide == null) return;
        _activeHighlighGuide.intensity = _gameBoardManager.HighlighterBaseIntensity;
        if (!_isQuestionGuide) { _activeSignGuide.material = _gameBoardManager.DirectionGuideBaseMaterial; }
        else { _activeSignGuide.material = _gameBoardManager.QuestionGuideBaseMaterial; }
    }

    //TurnOffGuide
    public void TurnOffGuide()
    {
        _gameBoardManager.ActiveHighlighsTiles.Remove(this);
        if (_activeHighlighGuide != null)
        {
            _activeHighlighGuide.gameObject.SetActive(false);
            _activeHighlighGuide = null;
        }
        if (_activeSignGuide != null)
        {
            _activeSignGuide.gameObject.SetActive(false);
            _activeSignGuide = null;
        }
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
