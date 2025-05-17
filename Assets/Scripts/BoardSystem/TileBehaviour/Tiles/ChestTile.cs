using System.Collections.Generic;
using UnityEngine;

public class ChestTile : TileBehavior
{
    private GameManager _gm;
    [SerializeField] private List<RewardGroup> _rewardGroups;

    protected override void Awake()
    {
        base.Awake();
        _gm = GameManager.Instance;
    }

    protected override void Start()
    {
        base.Start();
    }
    public override void StartTileEvent()
    {
        base.StartTileEvent();
        _gm.GmView.RPC("btnOpenDice", _gm.HostPlayer, 5);
        //OpenChest();
    }

    public override void ApplyTileEvent()
    {
        List<int> rewards = GetRewardList();
    }



    public void OpenChest()
    {
        //Panel de lanzar dados
        //Recompensas correspondientes a los dados
        //Termina el turno

    }

    private List<int> GetRewardList()
    {
        int diceResult = _gm.DiceResult;
        List<int> rewardList = new List<int>();
        //rewardList[0] siempre es la cantidad de monedas, el resto son los IDs de objetos

        for (int i = 0; i < _rewardGroups.Count; i++)
        {
            int quantity = 0;

            if (_rewardGroups[i].ProbabilityRange.x <= diceResult && _rewardGroups[i].ProbabilityRange.y >= diceResult)
            {
                for (int j = 0; j < _rewardGroups[i].Rewards.Count; j++)
                {
                    switch (_rewardGroups[i].Rewards[j])
                    {
                        case ItemType.Coin:
                            quantity = (int)(diceResult * (0.25f * (4f - i)) * _gm.GameRound);
                            rewardList.Add(quantity);
                            break;

                        case ItemType.Relic:
                            rewardList.Add(ItemManager.Instance.GetRandomItemIndexOfType<RelicItemData>());
                            break;

                        case ItemType.Gem:
                            rewardList.Add(ItemManager.Instance.GetRandomItemIndexOfType<GemItemData>());
                            break;

                        case ItemType.Card:
                            rewardList.Add(ItemManager.Instance.GetRandomItemIndexOfType<CardItemData>());
                            break;
                    }
                }
                break;
            }
        }
        return rewardList;
    }
}
