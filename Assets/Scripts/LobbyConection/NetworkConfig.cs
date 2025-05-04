using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConfig : MonoBehaviour
{
    public void AsingUserName(Text data)
    {
        if (data.text == string.Empty)
        {
            string newName = "Player" + Random.Range(100, 999);
            PhotonNetwork.LocalPlayer.NickName = newName;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = data.text;
        }
    }
    public void RefreshDataUserName(Text welcomeTxt)
    {
        welcomeTxt.text = "¡Bienvenido " + PhotonNetwork.NickName + "!";
    }
}
