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
        EventManager.StartListening("EndPlayerMovent", EndEvent);
        //EventManager.StartListening("TestEndTileEvent", EndEvent);
    }

    private void EndEvent()
    {
        _gm.MomentManager.IsWaitingForEvent = false;
    }






    //---------------- MOMENTOS GENERALES ----------------

    //MOMENTO DE MOVIMIENTO DE JUGADOR

    public void InitMoventPlayer()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].View.RPC("SyncroLeaveRestSpace", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.NumOfMovements = _gm.LastDiceResult;
        for (int i = 0; i < _gm.LastDiceResult; i++)
        {
            if (i == _gm.LastDiceResult - 1)
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

    //REVIVIR JUGADOR
    public void CheckToRevivePlayer()
    {
        if (_gm.LastDiceResult >= _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.ReviveValue) 
        {
            _gm.MomentManager.MomentList.Add(new Moment(RevivePlayer));
        } else
        {
            _gm.MomentManager.MomentList.Add(new Moment(ReviveSkipTurn));
        }
    }

    private void RevivePlayer()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.GmView.RPC("RevivePlayer", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        StartCoroutine(CinematicAnimation.WaitTime(2.2f, () => EventManager.TriggerEvent("EndEvent")));
    }

    private void ReviveSkipTurn()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.ReviveValue--;
        StartCoroutine(CinematicAnimation.WaitTime(1.2f, () => EventManager.TriggerEvent("EndEvent")));
    }



    //MOMENTO DE EVENTO DE CASILLA
    private void OpenTileEvent()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.StartTileEvent();
    }


    //Para el Tile de Cofre
    public void InitChestTileReward()
    {
        _gm.MomentManager.MomentList.Add(new Moment(OpenChest));
    }

    private void OpenChest()
    {
        _gm.MomentManager.IsWaitingForEvent = true;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.SettingTileEvent();
    }



}
