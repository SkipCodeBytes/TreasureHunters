using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Text welcomeTxt;

    public void RefreshDataUserName()
    {
        welcomeTxt.text = "¡Bienvenido " + PhotonNetwork.NickName + "!";
    }
}
