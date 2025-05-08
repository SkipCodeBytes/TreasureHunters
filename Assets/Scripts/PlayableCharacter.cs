using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;

    public CharacterData CharacterData { get => characterData;}

}
