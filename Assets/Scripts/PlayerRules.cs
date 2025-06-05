using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRules : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]
    [SerializeField] private int life = 0;
    [SerializeField] private int reviveValue = 0;

    [Header("Config Values")]
    [SerializeField] private List<CharacterData> availableCharacters;

    //Lista de efectos

    private PlayerManager _pm;
    private GameManager _gm;

    public int Life { get => life; set => life = value; }
    public int ReviveValue { get => reviveValue; set => reviveValue = value; }

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
        _gm = GameManager.Instance;
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

    public void AddLife(int value, bool forceAdd = false)
    {
        SoundController.Instance.PlaySound(_gm.SoundLibrary.AddLife);
        if (forceAdd)
        {
            life += value;
        }
        else
        {
            if (life >= _pm.SelectedCharacter.lifeStat) return;
            if(life < _pm.SelectedCharacter.lifeStat)
            {
                life += value;
                if (life > _pm.SelectedCharacter.lifeStat) life = _pm.SelectedCharacter.lifeStat;
            }
        }
    }

    public void GetDamage(int value)
    {
        life -= value;
        if(life <= 0) { 
            life = 0;
            reviveValue = _pm.SelectedCharacter.reviveStat;
        }
    }
}
