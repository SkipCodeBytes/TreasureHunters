using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    [SerializeField] private int adicionalDice = 0;
    [SerializeField] private float coinsBonusScale = 1f;


    [SerializeField, HideInInspector] List<int> dicesQuantityForAction = new List<int>();
    public List<int> DicesQuantityForAction { get => dicesQuantityForAction; set => dicesQuantityForAction = value; }
    public float CoinsBonusScale { get => coinsBonusScale; set => coinsBonusScale = value; }

    public int GetDiceQuantityUse(int actionValue)
    {
        return dicesQuantityForAction[actionValue] + adicionalDice;
    }

    public void ResetValues()
    {
        adicionalDice = 0;
        coinsBonusScale = 1f;
    }
}
