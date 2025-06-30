using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRules : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]
    [SerializeField] private int life = 0;
    [SerializeField] private int reviveValue = 0;
    [SerializeField] private int gameStarsQuantity = 0;
    public int starsToWin = 4;

    [Header("Config Values")]
    [SerializeField] private List<CharacterData> availableCharacters;

    //Lista de efectos

    private PlayerManager _pm;
    private GameManager _gm;

    public int Life { get => life; set => life = value; }
    public int ReviveValue { get => reviveValue; set => reviveValue = value; }
    public int GameStarsQuantity { get => gameStarsQuantity; set => gameStarsQuantity = value; }

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
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("AddLife"));
        _pm.Graphics.HealingParticle.Play();
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

    public void AddGameStar()
    {
        gameStarsQuantity++;
        _pm.Graphics.ConfetiParticle.Play();
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Relic"));
        StartCoroutine(CinematicAnimation.WaitTime(0.4f, () => _pm.Graphics.PlayCheerAnim()));

        if (gameStarsQuantity >= starsToWin)
        {
            EventManager.TriggerEvent("WinGame", true);
            Debug.Log("Has Ganado");
        }
    }
}
