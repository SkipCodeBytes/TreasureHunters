using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardGroup
{
    [SerializeField] private Vector2 _probabilityRange;
    [SerializeField] private List<ItemType> _rewards;

    public Vector2 ProbabilityRange { get => _probabilityRange; set => _probabilityRange = value; }
    public List<ItemType> Rewards { get => _rewards; set => _rewards = value; }
}

/*
[System.Serializable]
public class Reward
{
    [SerializeField] private int _quantity = 1;
    [SerializeField] private List<ItemData> _rndRewardObj;

    public int Quantity { get => _quantity; set => _quantity = value; }

    public ItemData GetReward()
    {
        if (_rndRewardObj.Count == 0) return null;
        if (_rndRewardObj.Count == 1) return _rndRewardObj[0];
        int indexRnd = Random.Range(0, _rndRewardObj.Count);
        return _rndRewardObj[indexRnd];
    }
}*/