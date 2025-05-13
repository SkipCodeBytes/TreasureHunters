using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameRPC : MonoBehaviourPunCallbacks
{
    private GameManager _gm;

    private Player _hostPlayer;

    public Player HostPlayer { get => _hostPlayer; set => _hostPlayer = value; }

    private void Awake()
    {
        _gm = GameManager.Instance;
        _hostPlayer = NetworkRoomsManager.HostPlayer;
    }



    [PunRPC]
    public void FirstSyncGameData(int playerId1, int playerId2, int playerId3, int playerId4)
    {
        List<Player> playerOrder = new List<Player>();
        BoardPlayer[] array = new BoardPlayer[4];

        if (playerId1 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId1)); else playerOrder.Add(null);
        if (playerId2 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId2)); else playerOrder.Add(null);
        if (playerId3 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId3)); else playerOrder.Add(null);
        if (playerId4 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId4)); else playerOrder.Add(null);

        for (int i = 0; i < playerOrder.Count; i++) {
            for(int j = 0; j < array.Length; j++)
            {
                if (playerOrder[i] == null) continue;
                if (playerOrder[i] == _gm.BoardPlayers[j].Player)
                {
                    array[i] = _gm.BoardPlayers[j];
                    break;
                }
            }
        }
        _gm.BoardPlayers = array;
        _gm.GeneratePlayerIndex();
    }

    [PunRPC]
    public void SetSyncroPlayer(int playerIndex)
    {
        _gm.HostManager.SetSyncroPlayer(playerIndex);
    }

    [PunRPC]
    public void PlayPresentationPanel() {
        _gm.TurnOrderUi.StartPresentation();
    }

    [PunRPC]
    public void ShowPlayerInfoUI()
    {
        _gm.SlotInfoUIList = new List<PlayerSlotInfoUi>();
        for (int i = 0; i < _gm.PlayerInfoPanel.transform.childCount; i++)
        {
            PlayerSlotInfoUi plySlotInfo = _gm.PlayerInfoPanel.transform.GetChild(i).GetComponent<PlayerSlotInfoUi>();
            if (plySlotInfo == null) continue;
            plySlotInfo.StartChargingPlayerInfo();
            _gm.SlotInfoUIList.Add(plySlotInfo);
        }
        _gm.PlayerInfoPanel.SetActive(true);

        CamFocusTarget(-1);
    }


    [PunRPC]
    public void CamFocusTarget(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4)
        {
            _gm.Cameraman.FocusPanoramicView();
            return;
        }
        if (_gm.BoardPlayers[playerIndex] == null)
        {
            _gm.Cameraman.FocusPanoramicView();
            return;
        }
        _gm.Cameraman.FocusTarget(_gm.BoardPlayers[playerIndex].gameObject);
    }

    [PunRPC]
    public void NewRound()
    {
        _gm.GameRound++;
        _gm.RoundInfoPanel.gameObject.SetActive(true);
        _gm.RoundInfoPanel.StartPresentation();
    }

    [PunRPC]
    public void NewTurn()
    {
        while (true)
        {
            _gm.CurrentPlayerTurnIndex++;
            if (_gm.CurrentPlayerTurnIndex >= _gm.BoardPlayers.Length) { 
                _gm.CurrentPlayerTurnIndex = -1;
                return;
            }
            
            if (_gm.BoardPlayers[_gm.CurrentPlayerTurnIndex] != null) break;
        }
        CamFocusTarget(_gm.CurrentPlayerTurnIndex);
    }

    [PunRPC]
    public void OpenPlayerActionPanel(int playerIndex)
    {
        _gm.PlayerActionPanel.gameObject.SetActive(true);
        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.PlayerActionPanel.OpenActionPanel();
        } else
        {
            _gm.PlayerActionPanel.OpenInfoPanel();
        }
    }

    [PunRPC]
    public void btnOpenDice(int actionDice)
    {
        /* Id | Action      | Default dices quantity
         * 0    Move            1
         * 
         */
        _gm.DiceAction = (PlayerDiceAction)actionDice;
        _gm.HostManager.Ply_ThrowDicesAction();
    }

    [PunRPC]
    public void OpenDicePanel(int playerIndex, int dicesQuantity) 
    {
        _gm.PlayerActionPanel.CloseAll();
        _gm.DicePanelUI.gameObject.SetActive(true);
        _gm.DiceResult = 0;
        _gm.DiceManager.UseDice(playerIndex, dicesQuantity);

        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.DiceManager.DicePanel.OpenTurnPanel();
        }
        else
        {
            _gm.DiceManager.DicePanel.OpenNoTurnPanel();
        }
    }

    [PunRPC]
    public void SentDiceResults(int result) 
    {
        _gm.DiceResult = result;
        _gm.DiceManager.EndAnimationCamera();
    }


}
