using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class BoardPlayer : MonoBehaviour
{
    [Header("Config")]
    private TileBoard _nextTile = null;

    [Header("Player Values - ReadOnly")]
    [SerializeField] private TileBoard homeTile;

    [Header("Game Values - ReadOnly")]
    [SerializeField] private TileBoard previusTilePosition;
    [SerializeField] private TileBoard currentTilePosition;


    public TileBoard HomeTile { get => homeTile; set => homeTile = value; }
    public TileBoard CurrentTilePosition { get => currentTilePosition; set => currentTilePosition = value; }
    public TileBoard PreviusTilePosition { get => previusTilePosition; set => previusTilePosition = value; }

    private PlayerManager _pm;

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
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
            //A elecci�n del jugador
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToTile(_nextTile);
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
            //A elecci�n del jugador
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToStandTile(_nextTile);
            _pm.View.RPC("SyncroEnterInTile", RpcTarget.All, _nextTile.Order.x, _nextTile.Order.y);
        }
    }

    private void DisplaceToTile(TileBoard tileTarget)
    {
        if (tileTarget != null)
        {
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
