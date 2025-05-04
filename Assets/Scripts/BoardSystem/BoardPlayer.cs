using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]
public class BoardPlayer : MonoBehaviourPunCallbacks
{
    [Header("Config")]
    [SerializeField] private float travelTime = 0.5f;
    private TileBoard _nextTile = null;

    [Header("General Info")]
    [SerializeField] private Player player;
    [SerializeField] private PhotonView view;
    [SerializeField] private UnitData selectedCharacter;
    [SerializeField] private TileBoard homeTile;

    [Header("Game Info")]
    [SerializeField] private TileBoard currentTilePosition;


    public Player Player { get => player; set => player = value; }
    public PhotonView View { get => view; set => view = value; }
    public UnitData SelectedCharacter { get => selectedCharacter; set => selectedCharacter = value; }
    public TileBoard HomeTile { get => homeTile; set => homeTile = value; }

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    [PunRPC]
    public void SetPlayerInfo(int tileOrderX, int tileOrderY)
    {
        TileBoard tile = GameManager.Instance.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];
        currentTilePosition = tile;
        transform.position = tile.transform.position;
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
            DisplaceToTile(_nextTile, travelTime);
        }
        if (numOfRoutes > 1) {
            //A elecci�n del jugador
        }
    }

    private void DisplaceToTile(TileBoard tileTarget, float duration)
    {
        Vector3 newPos = new Vector3(tileTarget.transform.position.x, transform.position.y, tileTarget.transform.position.z);
        if (tileTarget != null)
        {
            StartCoroutine(CinematicAnimation.MoveTo(this.gameObject, newPos, duration, finishMove));
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
