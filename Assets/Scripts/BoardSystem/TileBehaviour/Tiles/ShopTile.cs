using UnityEngine;

public class ShopTile : TileBehavior
{
    public ShopScript shopScript;

    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }
    public override void StartTileEvent()
    {
        _gm.GuiManager.ShopPanelGUI.gameObject.SetActive(true);
        _gm.GuiManager.ShopPanelGUI.StartShop();
    }

    public override void SettingTileEvent()
    {
    }

    public override void PlayTileEvent()
    {
        shopScript.PresentAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);
    }
}
