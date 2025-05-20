using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GuestManager : MonoBehaviourPunCallbacks
{
    private GameManager _gm;

    [Header("Check Values")]
    [SerializeField] private bool isCicleEnabled = false;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    public void Init()
    {
        ConfigEventListeners();
        isCicleEnabled = true;
    }

    private void Update()
    {

        if (!isCicleEnabled) return;
        _gm.MomentManager.MomentUpdate();
    }

    private void ConfigEventListeners()
    {
        EventManager.StartListening("EndMoment", GenericMomentEnd);
        EventManager.StartListening("EndEvent", GenericEndTask);
    }

    //Llamada automática después de evento tras finalizar algún momento
    private void GenericEndTask()
    {
        Debug.Log("End Event () ... syncro check");
        _gm.GmView.RPC("SetSyncroPlayerCheck", _gm.GameRPC.HostPlayer, _gm.PlayerIndex);
    }
    private void GenericMomentEnd() => _gm.MomentManager.IsMomentRunnning = false;
}
