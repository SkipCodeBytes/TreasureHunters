using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomsManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Text roomNameText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Text debugText;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messagePanelTxt;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button btnClosePanel;

    [SerializeField] private RoomMapSelector mapSelector;
    [SerializeField] private RoomCharacterSelector characterSelector;

    [SerializeField] private Material slotDefaultMaterial;
    [SerializeField] private Material slotReadyMaterial;
    [SerializeField] private Material slotNonReadyMaterial;

    [SerializeField] private List<Button> slotButtonList;

    [Header("Game config")]
    [SerializeField] private int maxPlayers = 4;

    [Header("Check Values")]
    [SerializeField] private static bool isMasterPlayer = false;

    private static Player _hostPlayer;

    private Dictionary<Button, Player> playerSlotDicc;
    private bool _isPlayerReady = false;
    private Hashtable _customProperty = new Hashtable();

    public static bool IsMasterPlayer { get => isMasterPlayer;}
    public static Player HostPlayer { get => _hostPlayer; set => _hostPlayer = value; }

    private void Awake()
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
                slot.image.material = slotDefaultMaterial;
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
        readyButton.GetComponent<Image>().material = slotDefaultMaterial;
        readyButton.transform.GetChild(0).GetComponent<Text>().text = "LISTO";

        Debug.Log("Ingresando a sala");

        if (isMasterPlayer)
        {
            debugText.color = Color.yellow;
            debugText.text = "You are the Host";
            photonView.RPC("SetMasterPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
            startButton.transform.GetChild(0).GetComponent<Text>().text = "INICIAR";
            startButton.interactable = true;
            startButton.GetComponent<Image>().material = slotDefaultMaterial;
        }
        else
        {
            debugText.color = Color.yellow;
            debugText.text = "You are joined to the room";
            startButton.interactable = false;
            startButton.transform.GetChild(0).GetComponent<Text>().text = "Esperando al host...";
            startButton.GetComponent<Image>().material = slotDefaultMaterial;
        }

        roomPanel.SetActive(true);
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
        clearSlots();
        asingSlots();
        photonView.RPC("EnterPlayerRoom", RpcTarget.Others, PhotonNetwork.LocalPlayer);

        _isPlayerReady = false;
        _customProperty["isReady"] = _isPlayerReady;
        _customProperty["luckyNumber"] = Random.Range(0,100);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperty);
        photonView.RPC("SetReadyStatus", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, _isPlayerReady);
    }


    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        debugText.color = Color.green;
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
            debugText.color = Color.yellow;
            debugText.text = "The player " + otherPlayer.NickName + " has left the room";
        }
    }

    public void btnReadyUnready()
    {
        _isPlayerReady = !_isPlayerReady;
        _customProperty["isReady"] = _isPlayerReady;
        _customProperty["characterSelected"] = characterSelector.PlayableCharacters[characterSelector.SelectedIndex].CharacterData.characterName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperty);
        photonView.RPC("SetReadyStatus", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, _isPlayerReady);
    }


    public void btnStartGame()
    {
        if (isMasterPlayer)
        {
            foreach(Button key in playerSlotDicc.Keys)
            {
                if(playerSlotDicc[key] != null)
                {
                    if (!(bool)playerSlotDicc[key].CustomProperties["isReady"])
                    {
                        debugText.color = Color.red;
                        debugText.text = "All players must be ready to start";
                        return;
                    }
                }
                else
                {
                    /* *--------------------- RETIRAR ESTO DESPUES DE LOS TESTS -------------------------------------------
                    debugText.color = Color.red;
                    debugText.text = "4 players are needed to start";
                    return;*/
                }
            }
        }
        else
        {
            debugText.color = Color.red;
            debugText.text = "Only the host can start the game";
            return;
        }
        photonView.RPC("StarGameAll", RpcTarget.All, mapSelector.PlayableMapList[mapSelector.SelectedMapIndex].SceneName);
    }

    [PunRPC]
    private void SetMasterPlayer(Player player)
    {
        _hostPlayer = player;
    }

    [PunRPC]
    private void EnterPlayerRoom(Player player)
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

    [PunRPC]
    private void SetReadyStatus(Player player, bool isReady)
    {
        foreach(Button key in playerSlotDicc.Keys)
        {
            if(playerSlotDicc[key] == player)
            {
                if (isReady)
                {
                    key.image.material = slotReadyMaterial;
                    if (player == PhotonNetwork.LocalPlayer)
                    {
                        readyButton.GetComponent<Image>().material = slotNonReadyMaterial;
                        readyButton.transform.GetChild(0).GetComponent<Text>().text = "X CANCELAR";
                        mapSelector.DisableButtons();
                        characterSelector.DisableButtons();
                        if(isMasterPlayer) startButton.GetComponent<Image>().material = slotReadyMaterial;
                    }
                } else
                {
                    key.image.material = slotNonReadyMaterial;
                    if (player == PhotonNetwork.LocalPlayer)
                    {
                        readyButton.GetComponent<Image>().material = slotReadyMaterial;
                        readyButton.transform.GetChild(0).GetComponent<Text>().text = "LISTO";
                        if (isMasterPlayer) { 
                            mapSelector.EnableButtons();
                            startButton.GetComponent<Image>().material = slotDefaultMaterial;
                        }
                        else mapSelector.DisableButtons();
                        characterSelector.EnableButtons();
                    }
                }
                break;
            }

        }
    }

    [PunRPC]
    private void StarGameAll(string gameSceneName)
    {
        Debug.Log("JUEGO EN: " + gameSceneName);
        PhotonNetwork.LoadLevel(gameSceneName);
    }
}
