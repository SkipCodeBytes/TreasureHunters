
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


    //[PunRPC]
    public void CamFocusTarget(int playerIndex, bool endFocusEvent = true)
    {
        if (playerIndex < 0 || playerIndex >= 4)
        {
            _gm.CameraController.FocusPanoramicView(false, endFocusEvent);
            return;
        }
        if (_gm.PlayersArray[playerIndex] == null)
        {
            _gm.CameraController.FocusPanoramicView(false, endFocusEvent);
            return;
        }
        _gm.CameraController.FocusTarget(_gm.PlayersArray[playerIndex].gameObject, endFocusEvent);
    }



    //GuestManager.GenericEndTask() / Host
    [PunRPC]
    public void SetSyncroPlayerCheck(int playerIndex)
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
        EventManager.TriggerEvent("EndEvent");
    }





    //BattlePanelGui.btnEvadeAction() / HostPlayer
    //BattlePanelGui.btnDefenseAction() / HostPlayer
    //GUIManager.btnMovePlayer() / HostPlayer
    //ChestTile.StartTileEvent() / HostPlayer
    [PunRPC]
    public void OpenDiceForAction(int newDiceOwner, int actionDice)
    {
        // Id | Action 
        // 0    Move    
        // 1    Attack 
        // 2    Defend
        // 3    Evade
        // 4    UseChest
        // 5    UseTramp
        _gm.CurrentDiceOwnerIndex = newDiceOwner;
        _gm.DiceAction = (PlayerDiceAction)actionDice;

        Debug.Log("Set to: " + _gm.DiceAction);
        _gm.HostManager.StartDicePanel();
    }

    //HostManager.OpenDicePanel() / All
    [PunRPC]
    public void OpenDicePanel(int playerIndex, int dicesQuantity, int actionDice)
    {
        _gm.DiceAction = (PlayerDiceAction)actionDice;
        _gm.GuiManager.PlayerActionPanel.CloseAll();
        _gm.GuiManager.RevivePanelUI.ClosePanel();
        _gm.GuiManager.DicePanelUI.gameObject.SetActive(true);
        _gm.LastDiceResult = 0;
        _gm.DiceManager.UseDice(playerIndex, dicesQuantity);
        if (playerIndex == _gm.PlayerIndex) _gm.DiceManager.DiceCanvas.OpenTurnPanel();
        else _gm.DiceManager.DiceCanvas.OpenNoTurnPanel();

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("OpenPanel"));
    }


    //HostManager.CloseDicePanel() / All
    [PunRPC]
    public void CloseDicePanel(int playerIndex)
    {
        //_gm.GuiManager.DicePanelUI.gameObject.SetActive(false);
        Debug.Log("Jugador " + playerIndex + " con resultado " + _gm.LastDiceResult + " PlyrAction: " + _gm.DiceAction);


        //Esto lo ejecuta solo el dueño de los dados
        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.MomentManager.IsWaitingForEvent = false;

            switch (_gm.DiceAction)
            {
                case PlayerDiceAction.Move:
                    _gm.GameMoments.InitMoventPlayer();
                    break;
                case PlayerDiceAction.Attack:
                    Debug.Log("Attack");
                    _gm.GmView.RPC("BattleAttackAction", RpcTarget.All, playerIndex, _gm.LastDiceResult);
                    break;

                case PlayerDiceAction.Defend:
                    Debug.Log("Defend");
                    _gm.GmView.RPC("DefenderElectionAction", RpcTarget.All, playerIndex, _gm.LastDiceResult, false);
                    break;

                case PlayerDiceAction.Evade:
                    Debug.Log("Evade");
                    _gm.GmView.RPC("DefenderElectionAction", RpcTarget.All, playerIndex, _gm.LastDiceResult, true);
                    break;

                case PlayerDiceAction.UseChest:
                    _gm.GameMoments.InitChestTileReward();
                    break;

                case PlayerDiceAction.Revive:
                    _gm.GameMoments.CheckToRevivePlayer();
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
        _gm.LastDiceResult = result;
        _gm.DiceManager.EndAnimationFocusDices();

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("DiceResult"));
    }

    //ChestTile.SettingTileEvent() / All
    [PunRPC]
    public void SyncroAddChestReward(int playerId, int coins, int gem, int card, int relic)
    {
        int[] rewards = { coins, gem, card, relic };
        if(coins > 0) _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddCoins(coins);
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddGem(gem);
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddCard(card);
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddRelic(relic);
        _gm.LastRewards = rewards;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();
    }


    //HomeTile.StartTileEvent() / All
    [PunRPC]
    public void SyncroHomeTileEffect(int playerId, int addLife)
    {
        _gm.PlayersArray[playerId].Rules.AddLife(addLife);
        _gm.PlayersArray[playerId].Inventory.SaveRelic();
        _gm.GuiManager.SlotInfoUIList[playerId].SetPlayerInfo();
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();

    }


    //CardTile.StartTileEvent() /All
    [PunRPC]
    public void SyncroCardTileEffect(int playerId, int cardReward)
    {
        int[] rewards = { 0, 0, cardReward, 0 };
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddCard(cardReward);
        _gm.LastRewards = rewards;
        _gm.LastRewards = new int[] { 0, 0, cardReward, 0 };
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();

    }

    //ShopPanelGUI.btnBuyItem(); /All
    [PunRPC]
    public void SyncroAddShopReward(int playerId, int itemID, int coinCost)
    {
        ItemType itemType = ItemManager.Instance.GetItemType(itemID);
        Debug.Log(itemID);

        switch (itemType)
        {
            case ItemType.Card:
                _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddCard(itemID);
                _gm.LastRewards = new int[] { 0, 0, itemID, 0 };
                break;

            case ItemType.Gem:
                _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddGem(itemID);
                _gm.LastRewards = new int[] { 0, itemID, 0, 0 };
                break;

            case ItemType.Relic:
                _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddRelic(itemID);
                _gm.LastRewards = new int[] { 0, 0, 0, itemID };
                break;

            default:
                _gm.LastRewards = new int[] { 0, 0, 0, 0 };
                Debug.LogError("Item no asignado");
                break;
        }
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.CoinsQuantity -= coinCost;
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();

    }

    [PunRPC]
    public void SyncroPortalEffect(int playerId, int tileX, int tileY)
    {
        Vector2Int newPosOrder = new Vector2Int(tileX, tileY);
        ShorcutRoadTile.currentTileOrder = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.Order;
        ShorcutRoadTile.nextTileOrder = newPosOrder;

        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();
        //_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition = _gm.BoardManager.TileDicc[newPosOrder];

    }

    [PunRPC]
    public void SyncroEndPortal(int playerId)
    {
        _gm.BoardManager.TileDicc[ShorcutRoadTile.currentTileOrder].TileBehavior.HideProps();
        _gm.BoardManager.TileDicc[ShorcutRoadTile.nextTileOrder].TileBehavior.HideProps();
    }


    //BattleTile.StartTileEvent() //All
    [PunRPC]
    public void SyncroBattleTile(int ofesivePlayerId, int defensiveId)
    {
        _gm.SecondaryPlayerTurn = defensiveId;
        _gm.PlayersArray[defensiveId].IsPlayerSubTurn = true;
        _gm.ReverseBattle = false;
        _gm.PlayersArray[ofesivePlayerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();

        //Añadir un momento de batalla
        if (_gm.IsHostPlayer)
        {
            _gm.HostManager.mtBattleUseCardElection();
            EventManager.TriggerEvent("EndEvent");
        } else if(_gm.CurrentPlayerTurnIndex == _gm.PlayerIndex)
        {
            EventManager.TriggerEvent("EndEvent");
        }
    }

    //HostManager.CardElection() // All
    [PunRPC]
    public void OpenCardActions() { _gm.GuiManager.BattlePanelGui.OpenCardActions(); }

    //GameRPC.CloseDicePanel() / All
    [PunRPC]
    public void BattleAttackAction(int atackerIndex, int ofenseValue)
    {
        Debug.Log("Attaker Index " + atackerIndex + "ofenseValue " + ofenseValue);
        _gm.OfensivePlayerValue = ofenseValue + _gm.GameRules.GetAttackValuePlayer(atackerIndex);
        if (_gm.IsHostPlayer)
        {
            _gm.HostManager.mtDefenderElection();
        }
        _gm.GuiManager.BattlePanelGui.ShowOfenseValue();
        if(atackerIndex == _gm.PlayerIndex)
        {
            EventManager.TriggerEvent("EndEvent");
        }
    }

    //HostManager.DefenderElection() / All
    [PunRPC]
    public void ShowDefenderElection() { _gm.GuiManager.BattlePanelGui.ShowDefensiveOptions(); }


    //HostManager.DefenderElection() / All
    [PunRPC]
    public void DefenderElectionAction(int defenderIndex, int defenseValue, bool isEvade)
    {
        Debug.Log("defender Index " + defenderIndex + "defend value: " + defenseValue);
        _gm.IsEvadeAction = isEvade;
        if (isEvade)
        {
            _gm.DefensivePlayerValue = defenseValue + _gm.GameRules.GetEvasionValuePlayer(defenderIndex);
        } 
        else
        {
            _gm.DefensivePlayerValue = defenseValue + _gm.GameRules.GetDefenseValuePlayer(defenderIndex);
        }

        if (_gm.IsHostPlayer)
        {
            _gm.HostManager.mtShowResults();
        }

        _gm.GuiManager.BattlePanelGui.ShowDefenseValue(isEvade);
    }

    //HostManager.ShowResults() / All
    [PunRPC]
    public void ShowBattleResults(int damage)
    {
        if (_gm.ReverseBattle)
        {
            _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.GetDamage(damage);
        }
        else
        {
            _gm.PlayersArray[_gm.SecondaryPlayerTurn].Rules.GetDamage(damage);
        }
        _gm.GuiManager.BattlePanelGui.ShowResults(damage);


        if (_gm.IsHostPlayer)
        {
            _gm.HostManager.mtReverseBattle();
        }
    }

    //HostManager.ReverseBattle() / All
    [PunRPC]
    public void SetToReverseBattle()
    {
        _gm.ReverseBattle = true;
        _gm.GuiManager.BattlePanelGui.ResetInfoValues();
        EventManager.TriggerEvent("EndEvent");
    }

    [PunRPC]
    public void EndBattle()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.HideProps();
        _gm.GuiManager.BattlePanelGui.gameObject.SetActive(false);
        _gm.PlayersArray[_gm.SecondaryPlayerTurn].IsPlayerSubTurn = false;
        _gm.SecondaryPlayerTurn = -1;
        EventManager.TriggerEvent("EndEvent");
    }


    //********************************************************************************************************************//
    //**********************************************  ACCIONES DE CICLO  *************************************************//
    //********************************************************************************************************************//



    //HostManager.Round() / All
    [PunRPC]
    public void NewRound()
    {
        CamFocusTarget(-1, false);
        _gm.GameRound++;
        _gm.GuiManager.RoundInfoPanel.gameObject.SetActive(true);
        _gm.GuiManager.RoundInfoPanel.StartPresentation();
        if(_gm.GameRound % 4 == 0)
        {
            _gm.MusicManager.NextMusic();
        }
    }

    //HostManager.NewTurn() / All
    [PunRPC]
    public void NewTurn(int playerIndex)
    {
        _gm.CurrentPlayerTurnIndex = playerIndex;

        for(int i  = 0; i < _gm.PlayersArray.Length; i++)
        {
            if(i == playerIndex) _gm.PlayersArray[i].IsPlayerTurn = true;
            else if (_gm.PlayersArray[i] != null) _gm.PlayersArray[i].IsPlayerTurn = false;
        }
        
        _gm.CameraController.FocusTarget(_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].gameObject);
        SoundController.Instance.PlaySound(_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].SelectedCharacter.turnAudio);
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


    //HostManager.OpenPlayerRevivePanel() / All
    [PunRPC]
    public void OpenPlayerRevivePanel(int playerIndex)
    {
        _gm.GuiManager.RevivePanelUI.gameObject.SetActive(true);
        if (playerIndex == _gm.PlayerIndex)
        {
            _gm.GuiManager.RevivePanelUI.OpenActionPanel();
            _gm.GuiManager.RevivePanelUI.StartPanel();
        }
        else
        {
            _gm.GuiManager.RevivePanelUI.OpenInfoPanel();
        }
    }

    [PunRPC]
    public void RevivePlayer(int playerIndex)
    {
        _gm.PlayersArray[playerIndex].Rules.Life = _gm.PlayersArray[playerIndex].SelectedCharacter.lifeStat;
        _gm.PlayersArray[playerIndex].Graphics.reviveAnimation();
        _gm.GuiManager.SlotInfoUIList[playerIndex].SetPlayerInfo();
    }

}
