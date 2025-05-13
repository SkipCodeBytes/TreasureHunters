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
    [SerializeField] private CharacterData selectedCharacter;
    [SerializeField] private TileBoard homeTile;

    [Header("Check Values")]
    [SerializeField] private TileBoard currentTilePosition;
    [SerializeField] private bool isPlayerTurn = false;

    private PlayerGraphics _playerGraphics;
    private PlayerRules _playerRules;
    private PlayerInventory _playerInventory;

    public Player Player { get => player; set => player = value; }
    public PhotonView View { get => view; set => view = value; }
    public CharacterData SelectedCharacter { get => selectedCharacter; set => selectedCharacter = value; }
    public TileBoard HomeTile { get => homeTile; set => homeTile = value; }
    public TileBoard CurrentTilePosition { get => currentTilePosition; set => currentTilePosition = value; }

    public PlayerGraphics PlayerGraphics { get => _playerGraphics; set => _playerGraphics = value; }
    public PlayerRules PlayerRules { get => _playerRules; set => _playerRules = value; }
    public PlayerInventory PlayerInventory { get => _playerInventory; set => _playerInventory = value; }
    public bool IsPlayerTurn { get => isPlayerTurn; set => isPlayerTurn = value; }

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        _playerGraphics = GetComponent<PlayerGraphics>();
        _playerRules = GetComponent<PlayerRules>();
        _playerInventory = GetComponent<PlayerInventory>();
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
        if (numOfRoutes > 1) {
            //A elecci�n del jugador
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToTile(_nextTile);
        }
    }

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
        }
        if (numOfRoutes > 1)
        {
            //A elecci�n del jugador
            _nextTile = currentTilePosition.NextTiles[0];
            DisplaceToStandTile(_nextTile);
        }
    }


    private void DisplaceToTile(TileBoard tileTarget)
    {
        if (tileTarget != null)
        {
            Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
            _playerGraphics.MovePlayerAtPoint(newPos, true, finishMove);
        }
    }

    private void DisplaceToStandTile(TileBoard tileTarget)
    {
        if (tileTarget != null)
        {
            Vector3 iteractionPosition = tileTarget.BehaviorScript.GetInteractionPosition();
            Debug.Log(iteractionPosition);
            Vector3 newPos = new Vector3(iteractionPosition.x, transform.position.y, iteractionPosition.z);
            _playerGraphics.MovePlayerAtPoint(newPos, true, finishMove);
            _playerGraphics.RotatePlayerAtPoint(tileTarget.BehaviorScript.GetIteractionViewPoint(), _playerGraphics.ClearAnimationStatus);
        }
    }

    private void finishMove()
    {
        currentTilePosition = _nextTile;
        _nextTile = null;
        EventManager.TriggerEvent("EndPlayerMovent", true);
    }

}
