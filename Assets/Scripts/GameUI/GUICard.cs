using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUICard : MonoBehaviour, IPointerClickHandler
{
    public CardItemData cardItemData;
    public Image BorderImg;
    public Image ImageCard;
    public Text Tittle;
    public Text Description;

    public bool IsInteractable = true;

    public void SetData(CardItemData cardData)
    {
        GameManager _gm = GameManager.Instance;
        cardItemData = cardData;
        ImageCard.sprite = cardItemData.ImageCard;
        Tittle.text = cardItemData.ItemName;
        Description.text = cardItemData.ItemDescription;

        Tittle.color = Color.white;
        Description.color = Color.white;

        switch (cardItemData.CardType)
        {
            case CardType.Battle:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.battleBorder;
                break;

            case CardType.Tramp:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.trampBorder;
                break;

            case CardType.Mistery:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.misteryBorder;
                break;

            case CardType.Event:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.eventBorder;
                break;

            case CardType.Special:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.specialBorder;
                break;

            default:
                BorderImg.sprite = _gm.GuiManager.CardPanelUI.defaultBorder;
                break;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        GameManager.Instance.GuiManager.CardPanelUI.UseCard(cardItemData);
    }
}
