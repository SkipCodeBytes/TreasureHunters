using UnityEngine;

public class PlayableCharacter : MonoBehaviour
{
    [SerializeField] private UnitData characterData;

    public UnitData CharacterData { get => characterData;}

}
