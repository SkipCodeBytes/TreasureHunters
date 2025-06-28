using System.Collections.Generic;
using UnityEngine;

public class RuinsTile : TileBehavior
{
    //Debe ser asignado por el Host con el GameManager
    public static int[] GemsNeededID = new int[4];
    public static GemItemData[] GemsNeeded = new GemItemData[4];
    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }

    public override void StartTileEvent()
    {
        //StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
        _gm.GuiManager.RuinPanelUI.gameObject.SetActive(true);
        SettingTileEvent();
    }

    public override void SettingTileEvent()
    {
        _gm.GmView.RPC("SyncroRuinEvent", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex);
    }

    public override void PlayTileEvent()
    {
        if(_gm.PlayerIndex == _gm.CurrentPlayerTurnIndex)
        {
            _gm.GuiManager.RuinPanelUI.StartRuinPanel();
        } else
        {
            _gm.GuiManager.WaitIfoUI.SetActive(true);
        }
    }

    public static void GenerateGemsNeeded()
    {
        int[] gemsID = new int[4];
        for (int i = 0; i < gemsID.Length; i++)
        {
            gemsID[i] = ItemManager.Instance.GetRandomItemIndexOfType<GemItemData>();
        }
        GameManager.Instance.GmView.RPC("SyncroGameGemsNeeded", Photon.Pun.RpcTarget.All, gemsID[0], gemsID[1], gemsID[2], gemsID[3]);
    }

    public static void SetGemsNeeded(int[] gemsID)
    {
        GemsNeededID = gemsID;
        for (int i = 0; i < gemsID.Length; i++)
        {
            GemsNeeded[i] = ItemManager.Instance.GetItemData(gemsID[i]) as GemItemData;
        }
    }
}
