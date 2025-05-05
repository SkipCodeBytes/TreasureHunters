
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomCharacterSelector : MonoBehaviour
{
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private Transform charactersContent;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private Text txtCharacterName;
    [SerializeField] private Text txtCharacterAtk;
    [SerializeField] private Text txtCharacterDef;
    [SerializeField] private Text txtCharacterEva;
    [SerializeField] private Text txtCharacterHp;
    [SerializeField] private Text txtCharacterRec;

    [SerializeField] private Transform targetCamPreview;
    private List<PlayableCharacter> playableCharacters;

    public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }
    public List<PlayableCharacter> PlayableCharacters { get => playableCharacters; set => playableCharacters = value; }

    void Start()
    {
        playableCharacters = new List<PlayableCharacter>();
        for(int i = 0; i < charactersContent.childCount; i++)
        {
            PlayableCharacter playableCharacter = charactersContent.GetChild(i).GetComponent<PlayableCharacter>();
            if (!playableCharacter.gameObject.activeInHierarchy) continue;
            if(playableCharacter != null) playableCharacters.Add(playableCharacter);
        }
        setSelection();
    }

    public void DisableButtons()
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
    }

    public void EnableButtons()
    {
        nextButton.interactable = true;
        prevButton.interactable = true;
    }

    public void nextCharacter()
    {
        selectedIndex++;
        if(selectedIndex > playableCharacters.Count - 1)
        {
            selectedIndex = 0;
        }
        setSelection();
    }

    public void previusCharacter()
    {
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = playableCharacters.Count - 1;
        }
        setSelection();
    }

    private void setSelection()
    {
        targetCamPreview.position = playableCharacters[selectedIndex].transform.position;
        txtCharacterName.text = playableCharacters[selectedIndex].CharacterData.characterName;
        txtCharacterAtk.text = playableCharacters[selectedIndex].CharacterData.attackStat + "";
        txtCharacterDef.text = playableCharacters[selectedIndex].CharacterData.defenseStat + "";
        txtCharacterEva.text = playableCharacters[selectedIndex].CharacterData.evadeStat + "";
        txtCharacterHp.text = playableCharacters[selectedIndex].CharacterData.lifeStat + "";
        txtCharacterRec.text = playableCharacters[selectedIndex].CharacterData.reviveStat + "";
    }
}
