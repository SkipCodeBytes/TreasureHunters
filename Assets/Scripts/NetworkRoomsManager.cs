using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomsManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private int maxPlayers = 4;

    [Header("Check Values")]
    [SerializeField] private bool isMasterPlayer;
    [SerializeField] private Player hostPlayer;


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
}
