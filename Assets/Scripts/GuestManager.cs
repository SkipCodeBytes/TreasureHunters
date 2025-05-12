using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GuestManager : MonoBehaviourPunCallbacks
{
    private GameManager _gm;


    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    public void Init()
    {
        ConfigEventListeners();
    }

    private void ConfigEventListeners()
    {
        EventManager.StartListening("EndMoment", GenericEndTask);
        EventManager.StartListening("EndEvent", GenericEndTask);
    }

    //Llamada automática después de evento tras finalizar algún momento
    private void GenericEndTask()
    {
        _gm.GmView.RPC("SetSyncroPlayer", _gm.GameRPC.HostPlayer, _gm.PlayerIndex);
    }



}
