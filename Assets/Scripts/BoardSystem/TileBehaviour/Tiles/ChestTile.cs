using System.Collections.Generic;
using UnityEngine;

public class ChestTile : TileBehavior
{
    [SerializeField] private List<RewardGroup> _rewardGroups;
    [SerializeField] private ChestScript chestObj;

    private GameManager _gm;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }

    //Llama al evento inicial de lanzar dado
    public override void StartTileEvent()
    {
        Debug.Log("Tile Event");
        Debug.Log(_gm);
        Debug.Log(_gm.HostPlayer);
        Debug.Log((int)_gm.DiceAction);
        _gm.DiceAction = PlayerDiceAction.UseChest;
        _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, (int)_gm.DiceAction);
    }

    //Realiza las operaciones y comparte los resultados con todos
    //Solo lo ejecuta el invitado
    public override void SettingTileEvent()
    {
        int[] rewards = GetRewardList(4);
        _gm.GmView.RPC("SyncroAddChestReward", Photon.Pun.RpcTarget.All, _gm.PlayerIndex, rewards[0], rewards[1], rewards[2], rewards[3]);
        //Sincronizar recompensas con los demás jugadores
        //Sincronizar animación

    }

    //Este es un visual para todos los jugadores
    public override void PlayTileEvent()
    {
        chestObj.OpenChestAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);
    }


    private int[] GetRewardList(int listSize)
    {
        int diceResult = _gm.LastDiceResult;
        //rewardList[0] siempre es la cantidad de monedas, el resto son los IDs de objetos

        int[] rewardArray = new int[4];

        for (int i = 0; i < listSize; i++)
        {
            if (_rewardGroups.Count < i + 1) rewardArray[i] = -1;

            if (_rewardGroups[i].ProbabilityRange.x <= diceResult && _rewardGroups[i].ProbabilityRange.y >= diceResult)
            {
                for (int j = 0; j < _rewardGroups[i].Rewards.Count; j++)
                {
                    switch (_rewardGroups[i].Rewards[j])
                    {
                        case ItemType.Coin:
                            int quantity = (int)(diceResult * (0.25f * (4f - i)) * _gm.GameRound * _gm.GameRules.CoinsBonusScale);
                            rewardArray[0] = quantity;
                            break;

                        case ItemType.Gem:
                            rewardArray[1] = (ItemManager.Instance.GetRandomItemIndexOfType<GemItemData>());
                            break;

                        case ItemType.Relic:
                            rewardArray[3] = (ItemManager.Instance.GetRandomItemIndexOfType<RelicItemData>());
                            break;


                        case ItemType.Card:
                            rewardArray[2] = (ItemManager.Instance.GetRandomItemIndexOfType<CardItemData>());
                            break;
                    }
                }
                break;
            }
        }
        return rewardArray;
    }

}
