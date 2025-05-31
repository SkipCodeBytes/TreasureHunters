using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    [SerializeField] private int adicionalDice = 0;
    [SerializeField] private float coinsBonusScale = 1f;

    [SerializeField] private int lifeInHomeAdded = 1;

    private GameManager _gm;

    [SerializeField, HideInInspector] List<int> dicesQuantityForAction = new List<int>();
    public List<int> DicesQuantityForAction { get => dicesQuantityForAction; set => dicesQuantityForAction = value; }
    public float CoinsBonusScale { get => coinsBonusScale; set => coinsBonusScale = value; }
    public int LifeInHomeAdded { get => lifeInHomeAdded; set => lifeInHomeAdded = value; }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    public int GetDiceQuantityUse(int actionValue)
    {
        return dicesQuantityForAction[actionValue] + adicionalDice;
    }

    public int GetLifeInHomeAdded()
    {
        return lifeInHomeAdded;
    }

    public void ResetValues()
    {
        adicionalDice = 0;
        coinsBonusScale = 1f;
    }

    public int GetAttackValuePlayer(int playerIndex)
    {
        int atk = _gm.PlayersArray[playerIndex].SelectedCharacter.attackStat;
        return atk;
    }


    public int GetDefenseValuePlayer(int playerIndex)
    {
        int def = _gm.PlayersArray[playerIndex].SelectedCharacter.defenseStat;
        return def;
    }

    public int GetEvasionValuePlayer(int playerIndex)
    {
        int eva = _gm.PlayersArray[playerIndex].SelectedCharacter.evadeStat;
        return eva;
    }
}
