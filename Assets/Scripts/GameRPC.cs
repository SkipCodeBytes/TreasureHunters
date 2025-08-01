
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        for (int i = 0; i < _gm.PlayersArray.Length; i++) 
        {
            if (_gm.PlayersArray[i] != null)
            {
                _gm.PlayerNumericIcons[i].SetActive(true);
                _gm.PlayerNumericIcons[i].transform.SetParent(_gm.PlayersArray[i].transform);
                _gm.PlayerNumericIcons[i].transform.localPosition = _gm.NumericIconsOffset;
            }
        }

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


        //Esto lo ejecuta solo el due�o de los dados
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

        if(coins > 0) _gm.PlayersArray[playerId].Inventory.AddCoins(coins);
        _gm.PlayersArray[playerId].Inventory.AddGem(gem);
        _gm.PlayersArray[playerId].Inventory.AddCard(card);

        if(_gm.PlayersArray[playerId].Inventory.RelicItemData == null)
        {
            _gm.PlayersArray[playerId].Inventory.AddRelic(relic);
            _gm.PlayersArray[playerId].Inventory.availableToReceiveRelic = true;
        }

        _gm.LastRewards = rewards;
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();
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


    //TransmutationTile.StartTileEvent() /All
    [PunRPC]
    public void SyncroTransmutationTileEffect(int playerId, int gemReward)
    {
        int[] rewards = { 0, gemReward, 0, 0 };
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.AddGem(gemReward);
        _gm.LastRewards = rewards;
        _gm.LastRewards = new int[] { 0, gemReward, 0, 0 };
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

                if (_gm.PlayersArray[playerId].Inventory.RelicItemData == null)
                {
                    _gm.PlayersArray[playerId].Inventory.AddRelic(itemID);
                    _gm.PlayersArray[playerId].Inventory.availableToReceiveRelic = true;
                }
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

    //ShopPanelGUI.SkipPanel() //All
    [PunRPC]
    public void SyncroSkipShop(int tileX, int tileY)
    {
        ShopScript shopScript = (_gm.BoardManager.TileDicc[new Vector2Int(tileX, tileY)].TileBehavior as ShopTile).shopScript;
        shopScript.continousParticle.Play();
        StartCoroutine(CinematicAnimation.WaitTime(0.3f, () => {
            _gm.BoardManager.TileDicc[new Vector2Int(tileX, tileY)].TileBehavior.HideProps();
        }));
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

        //A�adir un momento de batalla
        if (_gm.IsHostPlayer)
        {
            _gm.HostManager.mtBattleUseCardElection();
            EventManager.TriggerEvent("EndEvent");
        } else if(_gm.CurrentPlayerTurnIndex == _gm.PlayerIndex)
        {
            EventManager.TriggerEvent("EndEvent");
        }
    }

    //RuinsTile.GenerateGemsNeeded() //All
    [PunRPC]
    public void SyncroGameGemsNeeded(int gem0, int gem1, int gem2, int gem3)
    {
        int[] gems = new int[4];
        gems[0] = gem0;
        gems[1] = gem1;
        gems[2] = gem2;
        gems[3] = gem3;

        RuinsTile.SetGemsNeeded(gems);
    }

    //RuinsTile.PlayTileEvent() //All
    [PunRPC]
    public void SyncroRuinEvent(int playerId)
    {
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.PlayTileEvent();
    }

    [PunRPC]
    public void CloseWaitPanel()
    {
        _gm.GuiManager.WaitIfoUI.SetActive(false);
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

    //HostManager.ReverseBattle() / All
    //HostManager.ReverseBattle() / All
    [PunRPC]
    public void EndBattle()
    {


        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.TileBehavior.HideProps();
        _gm.GuiManager.BattlePanelGui.gameObject.SetActive(false);

        _gm.GuiManager.BattlePanelGui.battleCamera_1.gameObject.SetActive(false);
        _gm.GuiManager.BattlePanelGui.battleCamera_2.gameObject.SetActive(false);

        _gm.PlayersArray[_gm.SecondaryPlayerTurn].IsPlayerSubTurn = false;
        EventManager.TriggerEvent("EndEvent");


        if (_gm.IsHostPlayer)
        {
            if (_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.Life <= 0) _gm.GmView.RPC("PlayerDropObject", RpcTarget.All, _gm.CurrentPlayerTurnIndex, _gm.HostManager.AttackerPosibleDrop);
            if (_gm.PlayersArray[_gm.SecondaryPlayerTurn].Rules.Life <= 0) _gm.GmView.RPC("PlayerDropObject", RpcTarget.All, _gm.SecondaryPlayerTurn, _gm.HostManager.DefenderPosibleDrop);
        }

        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.AttackStatMod = 0;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.DefenseStatMod = 0;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.EvasionStatMod = 0;

        _gm.PlayersArray[_gm.SecondaryPlayerTurn].Rules.AttackStatMod = 0;
        _gm.PlayersArray[_gm.SecondaryPlayerTurn].Rules.DefenseStatMod = 0;
        _gm.PlayersArray[_gm.SecondaryPlayerTurn].Rules.EvasionStatMod = 0;

        _gm.SecondaryPlayerTurn = -1;
    }

    //GameRPC.EndBattle() / All
    //GameRPC.EndBattle() / All
    [PunRPC]
    public void PlayerDropObject(int playerId, int[] itemsId)
    {
        _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior.AddTileRewards(itemsId, _gm.PlayersArray[playerId].Inventory.DropObjects(itemsId));
        _gm.GuiManager.SlotInfoUIList[playerId].SetPlayerInfo();
    }

    //TileBehabior.GetTileRewards() //All
    [PunRPC]
    public void PlayerGetTileReward(int playerId, int tileX, int tileY)
    {
        _gm.BoardManager.TileDicc[new Vector2Int(tileX, tileY)].TileBehavior.SyncroGetTileRewards(playerId);
    }


    //RuinPanelUI.btnSubmit() //All
    [PunRPC]
    public void SyncroSubmitRuins(int playerId,int gem0, int gem1, int gem2, int gem3)
    {
        List<int> dropsId = new List<int>();
        dropsId.Add(0);
        dropsId.Add(gem0);
        dropsId.Add(gem1);
        dropsId.Add(gem2);
        dropsId.Add(gem3);

        ItemObject[] objects = _gm.PlayersArray[playerId].Inventory.DropObjects(dropsId.ToArray());
        RuinsTile ruinTile = _gm.PlayersArray[playerId].BoardPlayer.CurrentTilePosition.TileBehavior as RuinsTile;
        //ruinTile.RuinPedestal.PlayRuinEvent(playerId, objects);

        StartCoroutine(CinematicAnimation.WaitTime(_gm.PlayersArray[playerId].Inventory.itemTimeDrop + 0.1f, () => ruinTile.RuinPedestal.PlayRuinEvent(playerId, objects)));
        CloseWaitPanel();
        //_gm.GuiManager.SlotInfoUIList[playerId].SetPlayerInfo();
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
        if(_gm.GameRound % 3 == 0)
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
        _gm.PlayersArray[playerIndex].BoardPlayer.CurrentTilePosition.TileBehavior.GetTileRewards(GameManager.Instance.CurrentPlayerTurnIndex);
        _gm.GuiManager.SlotInfoUIList[playerIndex].SetPlayerInfo();
    }

    [PunRPC]
    public void WinGame()
    {
        _gm.GuiManager.WinPanelGUI.gameObject.SetActive(true);
        _gm.GuiManager.WinPanelGUI.StartWinPanel();
    }



    //CardPanelUI.UseCard() //All
    [PunRPC]
    public void UseCard(int playerCasterIndex, int cardIndex) 
    {
        CardItemData cardData = ItemManager.Instance.GetItemData(cardIndex) as CardItemData;
        if (cardData == null) { Debug.LogError("Card not exist ID: " + cardIndex); return; }

        _gm.PlayersArray[playerCasterIndex].Inventory.CardItems.Remove(cardData);

        switch (cardData.CardType)
        {
            case CardType.Battle:
                if (playerCasterIndex == _gm.CurrentPlayerTurnIndex) _gm.PrimaryCardUsed = cardData;
                if (playerCasterIndex == _gm.SecondaryPlayerTurn) _gm.SecondaryCardUsed = cardData;

                if (_gm.PlayerIndex == playerCasterIndex) EventManager.TriggerEvent("EndEvent");
                //Se debe esperar a que ambos decidan usar o no cartas antes de mostrar
                //Actualizar los valores de los jugadores
                break;

            case CardType.Tramp:
                _gm.PlayersArray[playerCasterIndex].BoardPlayer.CurrentTilePosition.TileBehavior.AddTrampCard(cardData);
                _gm.GuiManager.SlotInfoUIList[playerCasterIndex].SetPlayerInfo();
                if(_gm.CurrentPlayerTurnIndex == _gm.PlayerIndex)
                {
                    StartCoroutine(CinematicAnimation.WaitTime(0.7f, () => _gm.GuiManager.CardPanelUI.closeCallback?.Invoke()));
                }
                break;

            default:
                //_gm.PrimaryCardUsed = cardData;
                _gm.GuiManager.SlotInfoUIList[playerCasterIndex].SetPlayerInfo();
                PlayCardEffect(playerCasterIndex, cardIndex);
                break;
        }
    }

    [PunRPC]
    public void PlayCardEffect(int playerTargetIndex, int cardIndex)
    {
        CardItemData cardData = ItemManager.Instance.GetItemData(cardIndex) as CardItemData;
        if (cardData == null) { Debug.LogError("Card not exist ID: " + cardIndex); return; }

        //Muestra la carta y aplica efecto

        switch (cardData.CardType)
        {
            case CardType.Battle:
                

                _gm.GuiManager.CardViewUI.gameObject.SetActive(true);
                _gm.GuiManager.CardViewUI.StartCardView(() => EventManager.TriggerEvent("EndEvent"));
                _gm.GuiManager.SlotInfoUIList[_gm.CurrentPlayerTurnIndex].SetPlayerInfo();

                if (_gm.PrimaryCardUsed != null) { 
                    _gm.GuiManager.CardViewUI.SetCardView(_gm.PrimaryCardUsed, 2);
                    _gm.PlayCardEffect(_gm.CurrentPlayerTurnIndex, _gm.PrimaryCardUsed);
                }

                if (_gm.SecondaryCardUsed != null) { 
                    _gm.GuiManager.CardViewUI.SetCardView(_gm.SecondaryCardUsed, 1);
                    _gm.PlayCardEffect(_gm.SecondaryPlayerTurn, _gm.SecondaryCardUsed);
                }
                _gm.GuiManager.CardViewUI.PlayAnimation();

                _gm.PrimaryCardUsed = null;
                _gm.SecondaryCardUsed = null;
                break;

            case CardType.Tramp:
                _gm.PlayersArray[playerTargetIndex].BoardPlayer.CurrentTilePosition.TileBehavior.RemoveTrampCard();
                _gm.GuiManager.CardViewUI.gameObject.SetActive(true);
                _gm.GuiManager.CardViewUI.StartCardView(() => _gm.PlayCardEffect(playerTargetIndex, cardData));
                _gm.GuiManager.CardViewUI.SetCardView(cardData);
                _gm.GuiManager.CardViewUI.PlayAnimation();
                break;

            default:
                _gm.GuiManager.CardViewUI.gameObject.SetActive(true);
                _gm.GuiManager.CardViewUI.StartCardView(() => _gm.PlayCardEffect(playerTargetIndex, cardData));
                _gm.GuiManager.CardViewUI.SetCardView(cardData);
                _gm.GuiManager.CardViewUI.PlayAnimation();
                break;
        }
    }

    //CardMethods.SaltarTurno() // All
    [PunRPC]
    public void AddPlayerEffect(int playerTagetIndex, int effectID)
    {
        _gm.PlayersArray[playerTagetIndex].PlayerEffects.SetEffect(EffectManager.Instance.GetGameEffect(effectID));
    }

    //EffectManager.SkipTurn() // Others
    [PunRPC]
    public void DropPlayerEffect(int playerTagetIndex, int effectID)
    {
        _gm.PlayersArray[playerTagetIndex].PlayerEffects.DropEffect(EffectManager.Instance.GetGameEffect(effectID).EffectName);
    }
}
