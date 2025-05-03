using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomsManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Button btnClosePanel;
    [SerializeField] private Text roomNameText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Text debugText;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messagePanelTxt;

    [SerializeField] private List<Button> slotButtonList;

    [Header("Game config")]
    [SerializeField] private int maxPlayers = 4;

    [Header("Check Values")]
    [SerializeField] private bool isMasterPlayer = false;

    private Player _hostPlayer;
    private Dictionary<Button, Player> playerSlotDicc;



    private void Start()
    {
        playerSlotDicc = new Dictionary<Button, Player>();
    }


    //OPERACIONES CON LOS ESPACIOS DE JUGADOR
    private void generateSlotList()
    {
        for (int i = 0; i < slotButtonList.Count; i++)
        {
            if(slotButtonList[i] != null) { 
                playerSlotDicc[slotButtonList[i]] = null;
            }
        }
    }

    private void clearSlots()
    {
        playerSlotDicc.Clear();
        generateSlotList();
        foreach (var key in playerSlotDicc.Keys) {
            key.transform.GetChild(0).GetComponent<Text>().text = "<VACÍO>";
        }
    }

    private void asingSlots()
    {
        Debug.Log("Asing: " + PhotonNetwork.PlayerList.Length);
        Debug.Log("Slots: " + playerSlotDicc.Keys.Count);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            foreach (Button key in playerSlotDicc.Keys)
            {
                if (playerSlotDicc[key] == null)
                {
                    addPlayerToSlot(key, PhotonNetwork.PlayerList[i]);
                    break;
                }
            }
        }
    }

    private void addPlayerToSlot(Button slot, Player player)
    {
        if (playerSlotDicc.ContainsKey(slot))
        {
            playerSlotDicc[slot] = player;
            slot.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
        } else
        {
            Debug.Log("Slot not found: " + slot);
        }
        playerCountText.text = "JUGADORES: " + PhotonNetwork.PlayerList.Length + "/4";
    }

    private void refreshPlayerSlots()
    {
        foreach(Button slot in playerSlotDicc.Keys)
        {
            if (playerSlotDicc[slot] == null) slot.transform.GetChild(0).GetComponent<Text>().text = "<VACÍO>";
            else slot.transform.GetChild(0).GetComponent<Text>().text = playerSlotDicc[slot].NickName;
        }
    }

    private void removePlayer(Player player)
    {
        foreach(Button slot in playerSlotDicc.Keys)
        {
            if (playerSlotDicc[slot] == player)
            {
                playerSlotDicc[slot] = null;
                slot.transform.GetChild(0).GetComponent<Text>().text = "<VACÍO>";
                break;
            }
        }
        playerCountText.text = "JUGADORES: " + PhotonNetwork.PlayerList.Length + "/4";
    }


    //OPERACIONES CON LAS SALA
    public void CreateRoom(Text txtCreateRoom)
    {
        try
        {
            if (txtCreateRoom.text != string.Empty)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayers;
                isMasterPlayer = true;
                PhotonNetwork.CreateRoom(txtCreateRoom.text, roomOptions);
            }
            else
            {
                messagePanel.SetActive(true);
                messagePanelTxt.text = "Name of room empty";
            }
        }
        catch (System.Exception e)
        {
            messagePanel.SetActive(true);
            messagePanelTxt.text = e.Message;
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
                messagePanel.transform.GetChild(0).GetComponent<Text>().text = "Name of room empty";
            }
        }
        catch (System.Exception e)
        {
            messagePanel.SetActive(true);
            messagePanel.transform.GetChild(0).GetComponent<Text>().text = e.Message;
        }
    }

    public void LeaveRoom()
    {
        isMasterPlayer = false;
        PhotonNetwork.LeaveRoom();
        clearSlots();
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        messagePanel.SetActive(true);
        messagePanelTxt.text = message;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        messagePanel.SetActive(true);
        messagePanelTxt.text = message;
    }




    public override void OnJoinedRoom()
    {
        Debug.Log("Ingresando a sala");
        if (isMasterPlayer)
        {
            photonView.RPC("SetMasterPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        Debug.Log("RPC check");
        
        roomPanel.SetActive(true);
        debugText.text = "You are joined to the room";
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
        clearSlots();
        asingSlots();
        photonView.RPC("EnterPlayerRoom", RpcTarget.Others, PhotonNetwork.LocalPlayer);
    }


    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        debugText.text = "The player " + otherPlayer.NickName + " has joined the room";
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == _hostPlayer)
        {
            btnClosePanel.onClick.Invoke();
            return;
        } else
        {
            removePlayer(otherPlayer);
            debugText.text = "The player " + otherPlayer.NickName + " has left the room";
        }
    }


    [PunRPC]
    public void SetMasterPlayer(Player player)
    {
        _hostPlayer = player;
    }


    [PunRPC]
    public void EnterPlayerRoom(Player player)
    {
        foreach (Button key in playerSlotDicc.Keys)
        {
            if (playerSlotDicc[key] == null)
            {
                addPlayerToSlot(key, player);
                break;
            }
        }
    }
}
