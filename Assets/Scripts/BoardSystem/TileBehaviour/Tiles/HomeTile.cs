using Photon.Pun;
using UnityEngine;

public class HomeTile : TileBehavior
{
    [SerializeField] private BonfireScript bonfire;

    private GameManager _gm;


    protected override void Start()
    {
        _gm = GameManager.Instance;
    }

    //Lo ejecuta solo el jugador de turno
    public override void StartTileEvent()
    {
        //Esperamos a un tiempo para dar paso a la animación de inicio
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => SettingTileEvent()));
    }

    public override void SettingTileEvent()
    {
        //Además de sindronizar la info, se da al PlayTileEvent()
        _gm.GmView.RPC("SyncroHomeTileEffect", RpcTarget.All, _gm.CurrentPlayerTurnIndex, _gm.GameRules.GetLifeInHomeAdded());
    }

    //Lo ejecuta todos los jugadores
    public override void PlayTileEvent()
    {
        //AÑADIR ANIMACIÓN DE SALUD AQUI
        //AÑADIR ANIMACIÓN DE GUARDAR RELIQUIA AQUI

        if(_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.SafeRelicsQuantity >= 4)
        {
            Debug.Log("JUGADOR " + _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Player.NickName + " ha ganado la partida");
        }
        else
        {
            StartCoroutine(CinematicAnimation.WaitTime(0.8f, () => EventManager.TriggerEvent("EndEvent")));
        }
    }

    public override void HideProps()
    {
        bonfire.Animator.SetTrigger("Destroy");

        //Desde la animación "Destroy":
        //  StopFireParticles()
        //  StopSmokeParticles()
        //  HideElements()
    }
}
