using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanelGui : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerManager target_1;
    [SerializeField] private PlayerManager target_2;

    [SerializeField] private Transform battleCamera_1;
    [SerializeField] private Transform battleCamera_2;

    [SerializeField] private GameObject cardButtons;
    [SerializeField] private GameObject defenderButtons;
    [SerializeField] private GameObject waitInfo;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultTxt;

    [Header("Target_1")]
    [SerializeField] private Text atkTxt_1;
    [SerializeField] private Text defTxt_1;
    [SerializeField] private Text evaTxt_1;
    [SerializeField] private Text vitTxt_1;
    [SerializeField] private GameObject resultPanel_1;
    [SerializeField] private Text actionTxt_1;
    [SerializeField] private Text diceValTxt_1;

    [Header("Target_2")]
    [SerializeField] private Text atkTxt_2;
    [SerializeField] private Text defTxt_2;
    [SerializeField] private Text evaTxt_2;
    [SerializeField] private Text vitTxt_2;
    [SerializeField] private GameObject resultPanel_2;
    [SerializeField] private Text actionTxt_2;
    [SerializeField] private Text diceValTxt_2;

    private GameManager _gm;

    public void OpenPanel(PlayerManager offensePlayer, PlayerManager defensePlayer)
    {
        _gm = GameManager.Instance;
        target_1 = offensePlayer;
        target_2 = defensePlayer;
        ResetInfoValues();

        defenderButtons.SetActive(false);
    }

    public void ResetInfoValues()
    {
        resultPanel.SetActive(false);

        battleCamera_1.position = target_1.transform.position;
        battleCamera_1.rotation = target_1.transform.rotation;
        atkTxt_1.text = target_1.SelectedCharacter.attackStat.ToString();
        defTxt_1.text = target_1.SelectedCharacter.defenseStat.ToString();
        evaTxt_1.text = target_1.SelectedCharacter.evadeStat.ToString();
        vitTxt_1.text = target_1.Rules.Life + " / " + target_1.SelectedCharacter.lifeStat;
        actionTxt_1.text = "ATK";
        resultPanel_1.SetActive(false);

        battleCamera_2.position = target_2.transform.position;
        battleCamera_2.rotation = target_2.transform.rotation;
        atkTxt_2.text = target_2.SelectedCharacter.attackStat.ToString();
        defTxt_2.text = target_2.SelectedCharacter.defenseStat.ToString();
        evaTxt_2.text = target_2.SelectedCharacter.evadeStat.ToString();
        vitTxt_2.text = target_2.Rules.Life + " / " + target_2.SelectedCharacter.lifeStat;
        actionTxt_2.text = "ATK";
        resultPanel_2.SetActive(false);


        target_2.Graphics.StopDefenseAnim();
        target_2.Graphics.StopDefenseAnim();
    }

    public void OpenCardActions()
    {
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("OpenPanel"));

        if (target_1 == _gm.PlayersArray[_gm.PlayerIndex] || target_2 == _gm.PlayersArray[_gm.PlayerIndex])
        {
            waitInfo.SetActive(false);
            cardButtons.SetActive(true);
        }
        else
        {
            waitInfo.SetActive(true);
            cardButtons.SetActive(false);
            EventManager.TriggerEvent("EndEvent");
        }
    }

    public void btnUseCard()
    {
        //Añade evento de carta y aumenta stats
        Debug.LogError("No implementado");
        cardButtons.SetActive(false);
        btnSkipCard();
    }

    public void btnSkipCard()
    {
        EventManager.TriggerEvent("EndEvent");
        cardButtons.SetActive(false);
    }


    public void ShowOfenseValue()
    {
        if (_gm.ReverseBattle)
        {
            resultPanel_2.SetActive(true);
            actionTxt_2.text = "ATK";
            diceValTxt_2.text = "" + _gm.OfensivePlayerValue;
            ShowDefensiveOptions();
        }
        else
        {
            resultPanel_1.SetActive(true);
            actionTxt_1.text = "ATK";
            diceValTxt_1.text = "" + _gm.OfensivePlayerValue;
            //ShowDefensiveOptions();
        }
        EventManager.TriggerEvent("EndEvent");
    }

    public void ShowDefensiveOptions()
    {
        if (_gm.ReverseBattle)
        {
            if (target_1 == _gm.PlayersArray[_gm.PlayerIndex])
            {
                StartCoroutine(CinematicAnimation.WaitTime(0.5f, () => defenderButtons.SetActive(true)));
            }
            else
            {
                EventManager.TriggerEvent("EndEvent");
            }
        }
        else
        {
            if (target_2 == _gm.PlayersArray[_gm.PlayerIndex])
            {
                StartCoroutine(CinematicAnimation.WaitTime(0.5f, () => defenderButtons.SetActive(true)));
            }
            else
            {
                EventManager.TriggerEvent("EndEvent");
            }
        }
    }

    public void btnEvadeAction()
    {
        _gm.DiceAction = PlayerDiceAction.Evade;
        if(_gm.ReverseBattle) _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.CurrentPlayerTurnIndex, (int)PlayerDiceAction.Evade);
        else _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.SecondaryPlayerTurn, (int)PlayerDiceAction.Evade);
        defenderButtons.SetActive(false);
    }

    public void btnDefenseAction()
    {
        _gm.DiceAction = PlayerDiceAction.Defend;
        if (_gm.ReverseBattle) _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.CurrentPlayerTurnIndex, (int)PlayerDiceAction.Defend);
        else _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.SecondaryPlayerTurn, (int)PlayerDiceAction.Defend);
        defenderButtons.SetActive(false);
    }


    public void ShowDefenseValue(bool isEvade)
    {
        if (_gm.ReverseBattle)
        {
            resultPanel_1.SetActive(true);
            diceValTxt_1.text = "" + _gm.DefensivePlayerValue;

            if (isEvade)
            {
                actionTxt_1.text = "EVA";
                //target_1.Graphics.PlayEvadeAnim();
            }
            else
            {
                actionTxt_1.text = "DEF";
                target_1.Graphics.PlayDefenseAnim();
            }
        }
        else
        {
            resultPanel_2.SetActive(true);
            diceValTxt_2.text = "" + _gm.DefensivePlayerValue;

            if (isEvade)
            {
                actionTxt_2.text = "EVA";
                //target_2.Graphics.PlayEvadeAnim();
            }
            else
            {
                actionTxt_2.text = "DEF";
                target_2.Graphics.PlayDefenseAnim();
            }
        }

        EventManager.TriggerEvent("EndEvent");
    }

    public void ShowResults(int damage)
    {
        resultPanel.SetActive(true);
        resultTxt.text = "" + damage;

        if (_gm.ReverseBattle)
        {
            target_2.Graphics.PlayAttackAnim();
            if (target_1.Rules.Life <= 0) target_1.Graphics.setDieAnimation();
            else if (damage == 0 ) target_1.Graphics.PlayEvadeAnim();
            else target_1.Graphics.PlayHitAnim();
        }
        else
        {
            target_1.Graphics.PlayAttackAnim();
            if (target_2.Rules.Life <= 0) target_2.Graphics.setDieAnimation();
            else if (damage == 0) target_2.Graphics.PlayEvadeAnim();
            else target_2.Graphics.PlayHitAnim();
        }
        ReloadValues();
        EventManager.TriggerEvent("EndEvent");
    }

    private void ReloadValues()
    {
        _gm.GuiManager.SlotInfoUIList[0].SetPlayerInfo();
        _gm.GuiManager.SlotInfoUIList[1].SetPlayerInfo();
        _gm.GuiManager.SlotInfoUIList[2].SetPlayerInfo();
        _gm.GuiManager.SlotInfoUIList[3].SetPlayerInfo();

        vitTxt_1.text = target_1.Rules.Life + " / " + target_1.SelectedCharacter.lifeStat;
        vitTxt_2.text = target_2.Rules.Life + " / " + target_2.SelectedCharacter.lifeStat;
    }

}
