using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class BoardPlayer : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    [SerializeField] private float speed = 0.5f;
    private TileBoard _nextTile = null;

    [Header("General Info")]
    [SerializeField] private Player player;
    [SerializeField] private PhotonView view;
    [SerializeField] private UnitData selectedCharacter;
    [SerializeField] private TileBoard homeTile;

    [Header("Check Values")]
    [SerializeField] private TileBoard currentTilePosition;
    [SerializeField] private bool isPlayerTurn = false;

    private PlayerGraphics _playerGraphics;

    public Player Player { get => player; set => player = value; }
    public PhotonView View { get => view; set => view = value; }
    public UnitData SelectedCharacter { get => selectedCharacter; set => selectedCharacter = value; }
    public TileBoard HomeTile { get => homeTile; set => homeTile = value; }
    public TileBoard CurrentTilePosition { get => currentTilePosition; set => currentTilePosition = value; }
    public bool IsPlayerTurn { get => isPlayerTurn; set => isPlayerTurn = value; }

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        _playerGraphics = GetComponent<PlayerGraphics>();
    }

    [PunRPC]
    public void SetPlayerInfo(int tileOrderX, int tileOrderY)
    {
        TileBoard tile = GameManager.Instance.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];
        SetTilePosition(tile);
        homeTile = tile;
    }


    public void SetTilePosition(TileBoard tile)
    {
        currentTilePosition = tile;
        transform.position = tile.transform.position;
    }

    public void MoveNextTile()
    {
        int numOfRoutes = currentTilePosition.NextTiles.Count;
        if (numOfRoutes == 0) return;
        if (numOfRoutes == 1)
        {
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToTile(_nextTile);
        }
        if (numOfRoutes > 1) {
            //A elecci�n del jugador
        }
    }

    private void DisplaceToTile(TileBoard tileTarget)
    {
        Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
        if (tileTarget != null)
        {
            _playerGraphics.MovePlayerAtPoint(newPos, true, finishMove);
        }
    }

    private void finishMove()
    {
        currentTilePosition = _nextTile;
        _nextTile = null;
        //isMoving = false;
        EventManager.TriggerEvent("EndPlayerMovent", true);
    }

}
