using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    private BoardPlayer _boardPlayer;
    private Animator _animator;

    private void Awake()
    {
        _boardPlayer = GetComponent<BoardPlayer>();
    }

    public GameObject GeneratePlayerModel()
    {
        GameObject character = Instantiate(_boardPlayer.SelectedCharacter.characterPrefab, transform.position, transform.rotation, transform);
        _animator = character.GetComponent<Animator>();
        return character;
    }
}
