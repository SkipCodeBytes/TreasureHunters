using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BoardPlayer : MonoBehaviourPunCallbacks
{
    [Header("General Info")]
    [SerializeField] private Player player;
    [SerializeField] private PhotonView view;
    [SerializeField] private UnitData unitData;
    [SerializeField] private TileBoard homeTile;

    [Header("Player Info")]
    [SerializeField] private int coins;
    //Lista de efectos
    //Lista de gemas
    //Lista de cartas


    [Header("Game Info")]
    [SerializeField] private TileBoard currentTilePosition;

    [Header("Config")]
    [SerializeField] private float travelTime = 0.5f;
    private TileBoard _nextTile = null;



    private void Awake()
    {
        view = GetComponent<PhotonView>();
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
