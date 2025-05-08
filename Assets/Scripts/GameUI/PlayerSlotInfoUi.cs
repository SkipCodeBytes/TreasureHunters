using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotInfoUi : MonoBehaviour
{
    [SerializeField] private int playerIndexReference = 0;
    private BoardPlayer _playerReference;
    private PlayerRules _playerRules;

    [SerializeField] private Image iconCharacterReference;

    [SerializeField] private Text HpInfoText;
    [SerializeField] private Text CoinInfoText;
    [SerializeField] private Text GemInfoText;
    [SerializeField] private Text ItemInfoText;

    [SerializeField] private GameObject cardSpriteUiPrefab;
    [SerializeField] private GameObject relicSpriteUiPrefab;



    void StartChargingPlayerInfo()
    {
        _playerReference = GameManager.Instance.BoardPlayers[playerIndexReference];
        if (_playerReference == null) return;
        iconCharacterReference.sprite = _playerReference.SelectedCharacter.characterSprite;
    }

}
