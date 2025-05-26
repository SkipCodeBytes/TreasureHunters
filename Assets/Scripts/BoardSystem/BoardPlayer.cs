using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoardPlayer : MonoBehaviour
{
    private TileBoard _nextTile = null;

    [Header("Player Values - ReadOnly")]
    [SerializeField] private TileBoard homeTile;

    [Header("Game Values - ReadOnly")]
    [SerializeField] private TileBoard previusTilePosition;
    [SerializeField] private TileBoard currentTilePosition;


    public TileBoard HomeTile { get => homeTile; set => homeTile = value; }
    public TileBoard CurrentTilePosition { get => currentTilePosition; set => currentTilePosition = value; }
    public TileBoard PreviusTilePosition { get => previusTilePosition; set => previusTilePosition = value; }
    public int NumOfMovements { get => _numOfMovements; set => _numOfMovements = value; }

    private PlayerManager _pm;

    //Multi routes selection
    private int _numOfMovements = 0;
    private bool _isGoingLastTile = false;
    private bool _isWaitingForElection = false;
    [SerializeField] private List<List<TileBoard>> _nextTiles = new List<List<TileBoard>>();

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (_isWaitingForElection)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            currentTilePosition.GameBoardManager.BaseAllHighlight();
            bool selectedList = false;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<TileBoard>(out TileBoard tile))
                {
                    for(int j = 0; j < _nextTiles.Count; j++)
                    {
                        List<TileBoard> observedList = _nextTiles[j];

                        if (observedList.Contains(tile) && !selectedList)
                        {
                            selectedList = true;
                            for (int i = 0; i < observedList.Count; i++) observedList[i].HighlightFocus();

                            if (Input.GetMouseButtonDown(0))
                            {
                                _nextTile = observedList[0];
                                _pm.Graphics.StopWaitAction();
                                _isWaitingForElection = false;
                                _nextTile.GameBoardManager.TurnOffAllHighlight();

                                if (_isGoingLastTile)
                                {
                                    DisplaceToStandTile(_nextTile);
                                    _pm.View.RPC("SyncroEnterInTile", RpcTarget.All, _nextTile.Order.x, _nextTile.Order.y);
                                    _isGoingLastTile = false;
                                } else
                                {
                                    DisplaceToTile(_nextTile);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    //SOLO SE VA A UTILIZAR UNA SOLA VEZ, AL INICIO

    public void SetTilePosition(TileBoard tile)
    {
        currentTilePosition = tile;
        transform.position = tile.transform.position;
    }


    //FUNCIONES DE CONTROL DE MOVIMIENTO

    public void MoveNextTile()
    {
        int numOfRoutes = currentTilePosition.NextTiles.Count;
        if (numOfRoutes == 0)
        {
            Debug.LogWarning("ADVERTENCIA: Movimiento sin salida");
            return;
        }
        if (numOfRoutes == 1)
        {
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToTile(_nextTile);
        }
        if (numOfRoutes > 1)
        {
            _isWaitingForElection = true;
            _nextTiles.Clear();
            _pm.Graphics.WaitForAction();
            for (int i = 0; i < numOfRoutes; i++)
            {
                _nextTiles.Add(new List<TileBoard>());
                TileBoard observedTile = currentTilePosition.NextTiles[i];
                _nextTiles[i].Add(observedTile);
                observedTile.HighlightTile(Color.white);
                for (int j = 0; j < _numOfMovements - 1; j++)
                {
                    if (observedTile.NextTiles.Count == 1) 
                    { 
                        observedTile = observedTile.NextTiles[0];
                        _nextTiles[i].Add(observedTile);
                        observedTile.HighlightTile(Color.white);
                    }
                    else
                    {
                        _nextTiles[i].Add(observedTile);
                        observedTile.HighlightTile(Color.cyan);
                        break;
                    }
                }
            }
        }
    }


    //Sincroniza la nueva posición mientras se va desplazando al último Tile
    public void MoveLastTile()
    {
        int numOfRoutes = currentTilePosition.NextTiles.Count;
        if (numOfRoutes == 0)
        {
            Debug.LogWarning("ADVERTENCIA: Movimiento sin salida");
            return;
        }
        if (numOfRoutes == 1)
        {
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToStandTile(_nextTile);
            _pm.View.RPC("SyncroEnterInTile", RpcTarget.All, _nextTile.Order.x, _nextTile.Order.y);
        }
        if (numOfRoutes > 1)
        {
            _nextTiles.Clear();
            _isGoingLastTile = true;
            _isWaitingForElection = true;
            _pm.Graphics.WaitForAction();
            for (int i = 0; i < numOfRoutes; i++)
            {
                TileBoard observedTile = currentTilePosition.NextTiles[i];
                _nextTiles.Add(new List<TileBoard>());
                _nextTiles[i].Add(observedTile);
                if(observedTile.NextTiles.Count == 1) observedTile.HighlightTile(Color.white); 
                else observedTile.HighlightTile(Color.cyan);
            }
        }
    }

    private void DisplaceToTile(TileBoard tileTarget)
    {
        if (tileTarget != null)
        {
            _numOfMovements--;
            Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
            _pm.Graphics.MovePlayerAtPoint(newPos, true, FinishMove);
        }
    }

    private void DisplaceToStandTile(TileBoard tileTarget)
    {
        if (tileTarget != null)
        {
            Vector3 iteractionPosition = tileTarget.TileBehavior.GetInteractionPosition();
            Debug.Log(iteractionPosition);
            Vector3 newPos = new Vector3(iteractionPosition.x, transform.position.y, iteractionPosition.z);
            _pm.Graphics.MovePlayerAtPoint(newPos, true, FinishMove);
            _pm.Graphics.RotatePlayerAtPoint(tileTarget.TileBehavior.GetIteractionViewPoint(), _pm.Graphics.ClearAnimationStatus);
        }
    }

    private void FinishMove()
    {
        currentTilePosition = _nextTile;
        _nextTile = null;
        EventManager.TriggerEvent("EndPlayerMovent", true);
    }


    //Se llamará a través de un RPC para sincronizar 
    /*
    public void SetNewCurrentTilePosition(int tileOrderX, int tileOrderY)
    {
    }*/

}
