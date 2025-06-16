using System.Collections.Generic;
using UnityEngine;

public class RuinsTile : TileBehavior
{
    //Debe ser asignado por el Host con el GameManager
    public static GemItemData[] GemsNeeded = new GemItemData[4];
    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }

    public override void StartTileEvent()
    {
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
        //_gm.GuiManager.RuinPanelUI.gameObject.SetActive(true);
    }

    public override void SettingTileEvent()
    {

    }

    public override void PlayTileEvent()
    {
        if(_gm.PlayerIndex == _gm.CurrentPlayerTurnIndex)
        {
            _gm.GuiManager.RuinPanelUI.StartRuinPanel();
        } else
        {
            //Panel de esperando
        }
    }
}
