using Photon.Pun;
using UnityEngine;

public class TransmutationTile : TileBehavior
{
    [SerializeField] private FontainScript fontainScript;
    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }
    public override void StartTileEvent()
    {
        //StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => SettingTileEvent()));
    }
    public override void SettingTileEvent()
    {
        int gemReward = ItemManager.Instance.GetRandomItemIndexOfType<GemItemData>();
        _gm.GmView.RPC("SyncroTransmutationTileEffect", RpcTarget.All, _gm.CurrentPlayerTurnIndex, gemReward);
    }
    public override void PlayTileEvent()
    {
        fontainScript.PresentAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);
        //throw new System.NotImplementedException();
    }

}
