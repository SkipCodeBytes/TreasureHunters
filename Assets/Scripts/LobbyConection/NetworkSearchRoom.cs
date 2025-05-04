using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSearchRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform optionsContent;
    [SerializeField] private GameObject uiOptionPrefab;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messagePanelTxt;
    private static RectTransform _cursorSelection;
    public static List<RoomInfo> _availableRooms = new List<RoomInfo>();

    public static RectTransform CursorSelection { get => _cursorSelection; set => _cursorSelection = value; }

    void Start()
    {
        foreach (Transform optionRoom in optionsContent.transform)
        {
            Destroy(optionRoom.gameObject);
        }
        _availableRooms.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            int index = _availableRooms.FindIndex(r => r.Name == room.Name);
            Debug.Log("index: " + index);
            if (index != -1) //En caso de que la sala exista en nuesta lista
            {
                if (room.RemovedFromList || room.PlayerCount == 0) //En caso de no existir el room o sin jugadores en el server
                {
                    Debug.Log("Removiendo Room " + index);
                    int n = 0;
                    for (int i = 0; i < _availableRooms.Count; i++)
                    {
                        if (i == index) { n = -1; continue; }
                        optionsContent.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -40f - (60 * (i + n)));
                    }
                    _availableRooms.RemoveAt(index);
                    Destroy(optionsContent.GetChild(index).gameObject);
                }
                else
                {
                    //Crear uno nuevo en caso de que no lo tengamos en nuestra lista
                    //Modificar en caso de que se haya actualizado

                    Debug.Log("Actualizando info Room " + index);
                    _availableRooms[index] = room;
                    optionsContent.GetChild(index).GetChild(1).GetComponent<Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}";

                    //createUiOptionInstance(_availableRooms.Count - 1, _availableRooms[index]);
                }
            }
            else //En caso de que la sala no exista en nuesta lista
            {
                if (room.IsVisible && room.IsOpen)
                {
                    Debug.Log("Nuevo Room " + index);
                    _availableRooms.Add(room);
                    createUiOptionInstance(_availableRooms.Count - 1, room);
                }
            }
        }
    }

    private void createUiOptionInstance(int index, RoomInfo room)
    {
        RectTransform uiObject = Instantiate(uiOptionPrefab, optionsContent.transform).GetComponent<RectTransform>();
        uiObject.anchoredPosition = new Vector2(0f, -40f - (60 * index));
        uiObject.GetComponent<UiRoomOptionSelector>().RoomName = room.Name;
        uiObject.GetChild(0).GetComponent<Text>().text = room.Name;
        uiObject.GetChild(2).GetComponent<Text>().text = $"{room.PlayerCount}/{room.MaxPlayers}";
    }

    public void RefreshRoomList()
    {
        foreach (Transform optionRoom in optionsContent.transform)
        {
            Destroy(optionRoom.gameObject);
        }
        _availableRooms.Clear();
        PhotonNetwork.JoinLobby();
    }

    public void JoinRoom()
    {
        try
        {
            if (_cursorSelection != null)
            {
                PhotonNetwork.JoinRoom(_cursorSelection.GetComponent<UiRoomOptionSelector>().RoomName);
            }
            else
            {
                messagePanel.SetActive(true);
                messagePanelTxt.text = "Select a room";
            }
        }
        catch (System.Exception e)
        {
            messagePanel.SetActive(true);
            messagePanelTxt.text = e.Message;
        }
    }
}
