using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotInfoUi : MonoBehaviour
{
    [SerializeField] private int playerIndexReference = 0;
    private BoardPlayer _playerReference;
    private PlayerRules _playerRules;
    private PlayerInventory _playerInventory;

    [SerializeField] private Image iconCharacterReference;

    [SerializeField] private Text HpInfoText;
    [SerializeField] private Text CoinInfoText;
    [SerializeField] private Text GemInfoText;
    [SerializeField] private GameObject ItemContent;
    [SerializeField] private int itemSlotPoolSize = 5;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private List<GameObject> slotPoolObj;
    [SerializeField] private List<Image> slotPoolImg;

    public void StartChargingPlayerInfo()
    {
        _playerReference = GameManager.Instance.BoardPlayers[playerIndexReference];
        if (_playerReference == null) return;
        iconCharacterReference.sprite = _playerReference.SelectedCharacter.characterSprite;

        _playerRules = _playerReference.GetComponent<PlayerRules>();
        _playerInventory = _playerReference.GetComponent<PlayerInventory>();
        SetPlayerInfo();

        for (int i = 0; i < itemSlotPoolSize; i++) CreateSlot();
    }

    private void CreateSlot()
    {
        GameObject Slot = Instantiate(itemSlotPrefab, ItemContent.transform);
        slotPoolObj.Add(Slot);
        Slot.SetActive(false);
        slotPoolImg.Add(Slot.transform.GetChild(0).GetComponent<Image>());
    }

    public void SetPlayerInfo()
    {
        HpInfoText.text = _playerRules.Life + "/" + _playerReference.SelectedCharacter.lifeStat;
        CoinInfoText.text = "$" + _playerInventory.CoinsQuantity;
        GemInfoText.text = "" + _playerInventory.GemItems.Count;

        int slotsCount = _playerInventory.CardItems.Count + 1;

        for (int i = 0; i < slotsCount; i++)
        {
            if (i >= slotPoolObj.Count) { 
                CreateSlot();
                continue;
            }
            slotPoolObj[i].SetActive(false);
        }

        for(int i = 0; i < _playerInventory.CardItems.Count; i++)
        {
            slotPoolImg[i].sprite = _playerInventory.CardItems[i].Icon;
            slotPoolObj[i].SetActive(true);
        }

        if (_playerInventory.RelicItem != null)
        {
            slotPoolImg[_playerInventory.CardItems.Count - 1].sprite = _playerInventory.RelicItem.Icon;
            slotPoolObj[_playerInventory.CardItems.Count - 1].SetActive(true);
        }

    }

}
