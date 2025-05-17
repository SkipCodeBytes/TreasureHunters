using Photon.Pun;
using UnityEngine;

public class GameMoments : MonoBehaviour
{
    private GameManager _gm;
    private void Awake()
    {
        _gm = GameManager.Instance;
    }
    private void Start()
    {
        SetListener();
    }


    public void SetListener()
    {
        EventManager.StartListening("EndPlayerMovent", EndMovePlayer);
    }
    private void EndMovePlayer()
    {
        _gm.MomentManager.IsWaitingForEvent = false;
    }






    //---------------- MOMENTOS GENERALES ----------------

    //MOMENTO DE MOVIMIENTO DE JUGADOR

    public void InitMoventPlayer()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].View.RPC("SyncroLeaveRestSpace", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        for (int i = 0; i < _gm.DiceResult; i++)
        {
            if (i == _gm.DiceResult - 1)
            {
                _gm.MomentManager.MomentList.Add(new Moment(MovePlayerLastTile));
                _gm.MomentManager.MomentList.Add(new Moment(OpenTileEvent));
            }
            else
            {
                _gm.MomentManager.MomentList.Add(new Moment(MovePlayerNextTile));
            }
        }
    }

    private void MovePlayerNextTile()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.MoveNextTile();
    }

    private void MovePlayerLastTile()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.MoveLastTile();
    }


    //MOMENTO DE EVENTO DE CASILLA
    private void OpenTileEvent()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.StartTileEvent();
    }

}
