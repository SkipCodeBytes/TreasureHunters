using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRules : MonoBehaviourPunCallbacks
{
    [Header("Config Values")]
    [SerializeField] private List<CharacterData> availableCharacters;

    [Header("Player Info")]
    [SerializeField] private int life;
    //[SerializeField]

    //Lista de efectos
    //Lista de gemas
    //Lista de cartas
    private BoardPlayer _boardPlayer;
    private PlayerGraphics _playerGraphics;

    public int Life { get => life; }

    private void Awake()
    {
        _boardPlayer = GetComponent<BoardPlayer>();
        _playerGraphics = GetComponent<PlayerGraphics>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("AssingPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
    }

    [PunRPC]
    public void AssingPlayer(Player player) {
        for (int i = 0; i < GameManager.Instance.BoardPlayers.Length; i++) {
            if (GameManager.Instance.BoardPlayers[i] == null){ 
                GameManager.Instance.BoardPlayers[i] = _boardPlayer;
                break;
            }
        }
        _boardPlayer.Player = player;
        _boardPlayer.SelectedCharacter = findCharacterData(player);

        if (_boardPlayer.SelectedCharacter == null) {
            Debug.Log("Personaje no registrado");
            return;
        }
        else
        {
            _playerGraphics.GeneratePlayerModel();
        }

        Debug.Log("Jugador " + player.NickName + " listo");
    }

    private CharacterData findCharacterData(Player player)
    {
        string selectedCharacter = (string)player.CustomProperties["characterSelected"];
        for (int i = 0; i < availableCharacters.Count; i++)
        {
            if (availableCharacters[i].characterName == selectedCharacter)
            {
                return availableCharacters[i];
            }
        }
        return null;
    }

}
