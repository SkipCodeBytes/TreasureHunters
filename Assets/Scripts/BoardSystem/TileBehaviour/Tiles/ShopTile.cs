using UnityEngine;

public class ShopTile : TileBehavior
{
    [SerializeField] private GameObject shopPanel;

    protected override void Start()
    {
        base.Start();
    }
    public override void StartTileEvent()
    {
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
    }

    public override void SettingTileEvent()
    {

    }
    public override void PlayTileEvent()
    {
        //throw new System.NotImplementedException();
    }
}
