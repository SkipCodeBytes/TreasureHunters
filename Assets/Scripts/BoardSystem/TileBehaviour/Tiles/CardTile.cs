using Photon.Pun;
using UnityEngine;

public class CardTile : TileBehavior
{
    [SerializeField] private PedestalScript pedestalScript;
    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }

    //Llama al evento inicial - Si no la hay, 
    public override void StartTileEvent()
    {
        //StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => SettingTileEvent()));
    }

    //Solo lo ejecuta el invitado
    public override void SettingTileEvent()
    {
        int cardReward = ItemManager.Instance.GetRandomItemIndexOfType<CardItemData>();
        _gm.GmView.RPC("SyncroCardTileEffect", RpcTarget.All, _gm.CurrentPlayerTurnIndex, cardReward);
    }


    //Este es un visual para todos los jugadores
    public override void PlayTileEvent()
    {
        pedestalScript.PresentAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);


        //_pedestalScript.PresentAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);
        //_pedestalScript.SpawnObjectsAnimation(_gm.CurrentPlayerTurnIndex, _gm.LastRewards);
    }

    public override void HideProps()
    {
        base.HideProps();
        pedestalScript.StopParticles();
    }
}
