using System.Collections.Generic;
using UnityEngine;

public class BattleTile : TileBehavior
{
    private GameManager _gm;


    [SerializeField] private PlayerManager ofensivePlayer;
    [SerializeField] private PlayerManager defensivePlayer;
    [SerializeField] private BannerScript bannerScript;


    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;

        for (int i = 0; i < HideableProps.Count; i++)
        {
            HideableProps[i].SetActive(false);
        }
    }

    public override void UnhideProps()
    {
        base.UnhideProps();
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("MonsterLaught"));
    }


    public override void StartTileEvent()
    {
        SettingTileEvent();
    }


    public override void SettingTileEvent()
    {
        List<int> availablePlayersIndex = new List<int>();
        for (int i = 0; i < _gm.PlayersArray.Length; i++)
        {
            if (_gm.PlayersArray[i] != null && _gm.PlayersArray[i] != _gm.PlayersArray[_gm.CurrentPlayerTurnIndex])
            {
                if(_gm.PlayersArray[i].Rules.Life > 0) availablePlayersIndex.Add(i);
            }
        }
        int randomPlayerIndex = -1;
        if (availablePlayersIndex.Count > 0)
        {
            randomPlayerIndex = availablePlayersIndex[Random.Range(0, availablePlayersIndex.Count)];

            StartCoroutine(CinematicAnimation.WaitTime(1.5f, () => 
            _gm.GmView.RPC("SyncroBattleTile", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex, randomPlayerIndex)
            ));
        }
        else
        {
            StartCoroutine(CinematicAnimation.WaitTime(1.5f, () => EventManager.TriggerEvent("EndEvent")));
        }
    }

    public override void PlayTileEvent()
    {
        ofensivePlayer = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex];
        defensivePlayer = _gm.PlayersArray[_gm.SecondaryPlayerTurn];

        _gm.OfensivePlayerValue = 0;
        _gm.DefensivePlayerValue = 0;

        _gm.GuiManager.BattlePanelGui.gameObject.SetActive(true);
        _gm.GuiManager.BattlePanelGui.OpenPanel(ofensivePlayer, defensivePlayer);
    }

    public override void HideProps()
    {
        //base.HideProps();
        bannerScript.PlayEnd();
    }
}
