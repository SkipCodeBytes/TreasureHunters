
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomCharacterSelector : MonoBehaviour
{
    [SerializeField] private float camSpeed = 10f;
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
    private List<Animator> charactersAnimator;

    private Coroutine _camLerp;

    public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }
    public List<PlayableCharacter> PlayableCharacters { get => playableCharacters; set => playableCharacters = value; }

    void Start()
    {
        playableCharacters = new List<PlayableCharacter>();
        charactersAnimator = new List<Animator>();
        for (int i = 0; i < charactersContent.childCount; i++)
        {
            PlayableCharacter playableCharacter = charactersContent.GetChild(i).GetComponent<PlayableCharacter>();
            Animator anim = charactersContent.GetChild(i).GetComponent<Animator>();
            if (!playableCharacter.gameObject.activeInHierarchy) continue;
            if(playableCharacter != null) { 
                playableCharacters.Add(playableCharacter);
                charactersAnimator.Add(anim);
                anim.SetInteger("State", 3);
            }
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
        charactersAnimator[selectedIndex].SetInteger("State", 3);
        selectedIndex++;
        if(selectedIndex > playableCharacters.Count - 1)
        {
            selectedIndex = 0;
        }
        setSelection();
    }

    public void previusCharacter()
    {
        charactersAnimator[selectedIndex].SetInteger("State", 3);
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = playableCharacters.Count - 1;
        }
        setSelection();
    }

    private void setSelection()
    {
        charactersAnimator[selectedIndex].SetInteger("State", 0);
        if (_camLerp != null) StopCoroutine(_camLerp);
        _camLerp = StartCoroutine(CinematicAnimation.MoveTowardTheTargetAt(targetCamPreview, playableCharacters[selectedIndex].transform.position, camSpeed));
        //targetCamPreview.position = playableCharacters[selectedIndex].transform.position;

        SoundController.Instance.PlaySound(playableCharacters[selectedIndex].CharacterData.turnAudio);

        txtCharacterName.text = playableCharacters[selectedIndex].CharacterData.characterName;
        txtCharacterAtk.text = playableCharacters[selectedIndex].CharacterData.attackStat + "";
        txtCharacterDef.text = playableCharacters[selectedIndex].CharacterData.defenseStat + "";
        txtCharacterEva.text = playableCharacters[selectedIndex].CharacterData.evadeStat + "";
        txtCharacterHp.text = playableCharacters[selectedIndex].CharacterData.lifeStat + "";
        txtCharacterRec.text = playableCharacters[selectedIndex].CharacterData.reviveStat + "";
    }
}
