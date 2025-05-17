using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRules : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]
    [SerializeField] private int life = 0;

    [Header("Config Values")]
    [SerializeField] private List<CharacterData> availableCharacters;

    //Lista de efectos

    private PlayerManager _pm;

    public int Life { get => life; set => life = value; }

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
    }


    public CharacterData FindCharacterData(Player player)
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
