using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiItem : MonoBehaviour
{
    [SerializeField] private Image borderImg;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text title;
    [SerializeField] private Text description;

    [SerializeField] private Sprite battleBorder;
    [SerializeField] private Sprite trampBorder;
    [SerializeField] private Sprite misteryBorder;
    [SerializeField] private Sprite eventBorder;
    [SerializeField] private Sprite specialBorder;
    [SerializeField] private Sprite defaultBorder;

    private ItemData item;

    public ItemData Item { get => item; set => item = value; }

    public void SetItemInfo(ItemData itemData)
    {
        item = itemData;
        ItemType itemType = ItemManager.Instance.GetItemType(ItemManager.Instance.GetItemID(itemData));

        switch (itemType) {
            case ItemType.Card:
                SetInfoCard(itemData as CardItemData);
                break;

            case ItemType.Gem:
                SetInfoGem(itemData as GemItemData);
                break;

            case ItemType.Relic:
                SetInfoRelic(itemData as RelicItemData);
                break;

            default:
                break;
        }
    }

    private void SetInfoCard(CardItemData cardItemData)
    {
        switch (cardItemData.CardType)
        {
            case CardType.Battle:
                borderImg.sprite = battleBorder;
                break;

            case CardType.Tramp:
                borderImg.sprite = trampBorder;
                break;

            case CardType.Mistery:
                borderImg.sprite = misteryBorder;
                break;

            case CardType.Event:
                borderImg.sprite = eventBorder;
                break;

            case CardType.Special:
                borderImg.sprite = specialBorder;
                break;

            default:
                borderImg.sprite = defaultBorder;
                break;
        }
        imgIcon.sprite = cardItemData.ImageCard;
        title.text = cardItemData.ItemName;
        description.text = cardItemData.ItemDescription;
    }

    private void SetInfoGem(GemItemData gemItemData)
    {
        borderImg.sprite = defaultBorder;
        imgIcon.sprite = gemItemData.Icon;
        title.text = gemItemData.ItemName;
        description.text = gemItemData.ItemDescription;
    }

    private void SetInfoRelic(RelicItemData relicItemData)
    {
        borderImg.sprite = defaultBorder;
        imgIcon.sprite = relicItemData.Icon;
        title.text = relicItemData.ItemName;
        description.text = relicItemData.ItemDescription;
    }
}
