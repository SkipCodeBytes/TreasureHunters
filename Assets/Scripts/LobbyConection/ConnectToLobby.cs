using Photon.Pun;
using UnityEngine;

public class ConnectToLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private string LobbySceneName;

    void Start() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby() => PhotonNetwork.LoadLevel(LobbySceneName);
}
