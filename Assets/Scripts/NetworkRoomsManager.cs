using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomsManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Text roomNameText;
    [SerializeField] private List<Button> playerSlots;
    [SerializeField] private GameObject messagePanel;

    [Header("Game config")]
    [SerializeField] private int maxPlayers = 4;

    [Header("Check Values")]
    [SerializeField] private bool isMasterPlayer;
    private Player _hostPlayer;


    public void CreateRoom(Text txtCreateRoom)
    {
        try
        {
            if (txtCreateRoom.text != string.Empty)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayers;
                PhotonNetwork.CreateRoom(txtCreateRoom.text, roomOptions);
                isMasterPlayer = true;
            }
            else
            {
                messagePanel.SetActive(true);
                messagePanel.transform.GetChild(0).GetComponent<Text>().text = "Name of room emply";
                Debug.Log("");
            }
        }
        catch (System.Exception e)
        {
            messagePanel.SetActive(true);
            messagePanel.transform.GetChild(0).GetComponent<Text>().text = e.Message;
        }
    }


    public void JoinRoom(Text txtJoinRoom)
    {
        try
        {
            if (txtJoinRoom.text != string.Empty)
            {
                PhotonNetwork.JoinRoom(txtJoinRoom.text);
                isMasterPlayer = false;
            }
            else
            {
                messagePanel.SetActive(true);
                messagePanel.transform.GetChild(0).GetComponent<Text>().text = "Name of room emply";
            }
        }
        catch (System.Exception e)
        {
            messagePanel.SetActive(true);
            messagePanel.transform.GetChild(0).GetComponent<Text>().text = e.Message;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        messagePanel.SetActive(true);
        messagePanel.transform.GetChild(0).GetComponent<Text>().text = message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        messagePanel.SetActive(true);
        messagePanel.transform.GetChild(0).GetComponent<Text>().text = message;
    }

    public void LeaveRoom()
    {
        isMasterPlayer = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        //debugText.text = "You are joined to the room";
        if (isMasterPlayer)
        {
            photonView.RPC("setMasterPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        roomPanel.SetActive(true);
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++){
            playerSlots[i].transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName;
        }

    }

    [PunRPC]
    public void setMasterPlayer(Player player) => _hostPlayer = player;
    
    [PunRPC]
    public void setEnterPlayer(Player ply)
    {
        
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        //debugText.text = "The player " + otherPlayer.NickName + " has joined the room";
    }
}
