using Photon.Pun;
using Photon.Realtime;
using System.Collections;
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


    //[PunRPC]
    public void CamFocusTarget(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= 4)
        {
            _gm.CameraController.FocusPanoramicView();
            return;
        }
        if (_gm.PlayersArray[playerIndex] == null)
        {
            _gm.CameraController.FocusPanoramicView();
            return;
        }
        _gm.CameraController.FocusTarget(_gm.PlayersArray[playerIndex].gameObject);
    }



    //GuestManager.GenericEndTask() / Host
    [PunRPC]
    public void SetSyncroPlayer(int playerIndex)
    {
        _gm.HostManager.SetSyncroPlayer(playerIndex);
    }



    //********************************************************************************************************************//
    //*******************************************  ACCIONES INICIO DE CICLO  *********************************************//
    //********************************************************************************************************************//

    //HostManager.PrepareScene() / All
    [PunRPC]
    public void FirstSyncGameData(int playerId1, int playerId2, int playerId3, int playerId4)
    {
        List<Player> playerOrder = new List<Player>();
        PlayerManager[] array = new PlayerManager[4];

        if (playerId1 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId1)); else playerOrder.Add(null);
        if (playerId2 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId2)); else playerOrder.Add(null);
        if (playerId3 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId3)); else playerOrder.Add(null);
        if (playerId4 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId4)); else playerOrder.Add(null);

        for (int i = 0; i < playerOrder.Count; i++) {
            for(int j = 0; j < array.Length; j++)
            {
                if (playerOrder[i] == null) continue;
                if (playerOrder[i] == _gm.PlayersArray[j].Player)
                {
                    array[i] = _gm.PlayersArray[j];
                    break;
                }
            }
        }
        _gm.PlayersArray = array;
        _gm.GeneratePlayerIndex();
    }


    //HostManager.NewGame() / All
    [PunRPC]
    public void PlayPresentationPanel()
    {
        _gm.GuiManager.TurnOrderUi.StartPresentation();
    }

    //HostManager.ShowPlayerInfoUI() / All
    [PunRPC]
    public void ShowPlayerInfoUI()
    {
        _gm.GuiManager.SlotInfoUIList = new List<PlayerSlotInfoUi>();
        for (int i = 0; i < _gm.GuiManager.PlayerInfoPanel.transform.childCount; i++)
        {
            PlayerSlotInfoUi plySlotInfo = _gm.GuiManager.PlayerInfoPanel.transform.GetChild(i).GetComponent<PlayerSlotInfoUi>();
            if (plySlotInfo == null) continue;
            plySlotInfo.StartChargingPlayerInfo();
            _gm.GuiManager.SlotInfoUIList.Add(plySlotInfo);
        }
        _gm.GuiManager.PlayerInfoPanel.SetActive(true);

        CamFocusTarget(-1);
    }




    //GUIManager.btnMovePlayer() / HostPlayer
    [PunRPC]
    public void OpenDiceForAction(int actionDice)
    {
        // Id | Action 
        // 0    Move    
        // 1    Attack 
        // 2    Defend
        // 3    Evade
        // 4    UseChest
        // 5    UseTramp
        _gm.DiceAction = (PlayerDiceAction)actionDice;
        _gm.HostManager.StartDicePanel();
    }

    //HostManager.OpenDicePanel() / All
    [PunRPC]
    public void OpenDicePanel(int playerIndex, int dicesQuantity)
    {
        _gm.GuiManager.PlayerActionPanel.CloseAll();
        _gm.GuiManager.DicePanelUI.gameObject.SetActive(true);
        _gm.DiceResult = 0;
        _gm.DiceManager.UseDice(playerIndex, dicesQuantity);

        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.DiceManager.DiceCanvas.OpenTurnPanel();
        }
        else
        {
            _gm.DiceManager.DiceCanvas.OpenNoTurnPanel();
        }
    }


    //HostManager.CloseDicePanel() / All
    [PunRPC]
    public void CloseDicePanel(int playerIndex)
    {
        _gm.GuiManager.DicePanelUI.gameObject.SetActive(false);
        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.MomentManager.IsWaitingForEvent = false;
            switch (_gm.DiceAction)
            {
                case PlayerDiceAction.Move:
                    _gm.GameMoments.InitMoventPlayer();
                    break;
                case PlayerDiceAction.UseChest:
                    //_gm.BoardPlayers[_gm.CurrentPlayerTurnIndex]
                    //EXPULSAR RECOMPENSAS
                    break;
                default:
                    Debug.LogError("No implementado");
                    break;
            }
        }
        else
        {
            EventManager.TriggerEvent("EndEvent");
        }
    }

    //DiceManager.CheckDiceStatus() / All
    [PunRPC]
    public void SentDiceResults(int result)
    {
        _gm.DiceResult = result;
        _gm.DiceManager.EndAnimationFocusDices();
    }














    //********************************************************************************************************************//
    //**********************************************  ACCIONES DE CICLO  *************************************************//
    //********************************************************************************************************************//



    //HostManager.Round() / All
    [PunRPC]
    public void NewRound()
    {
        _gm.GameRound++;
        _gm.GuiManager.RoundInfoPanel.gameObject.SetActive(true);
        _gm.GuiManager.RoundInfoPanel.StartPresentation();
    }

    //HostManager.NewTurn() / All
    [PunRPC]
    public void NewTurn()
    {
        while (true)
        {
            _gm.CurrentPlayerTurnIndex++;
            if (_gm.CurrentPlayerTurnIndex >= _gm.PlayersArray.Length)
            {
                _gm.CurrentPlayerTurnIndex = -1;
                return;
            }

            if (_gm.PlayersArray[_gm.CurrentPlayerTurnIndex] != null) break;
        }
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].IsPlayerTurn = true;
        _gm.CameraController.FocusTarget(_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].gameObject);
    }

    //HostManager.OpenPlayerActionPanel() / All
    [PunRPC]
    public void OpenPlayerActionPanel(int playerIndex)
    {
        _gm.GuiManager.PlayerActionPanel.gameObject.SetActive(true);
        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.GuiManager.PlayerActionPanel.OpenActionPanel();
        }
        else
        {
            _gm.GuiManager.PlayerActionPanel.OpenInfoPanel();
        }
    }







    /*


    [PunRPC]
    public void SentItemsIDs(int[] ids)
    {
        Debug.Log("Recibidos: " + string.Join(", ", ids));
    }*/
}
