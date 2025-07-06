using System;
using UnityEngine;

public class CardPanelUI : MonoBehaviour
{
    public Sprite battleBorder;
    public Sprite trampBorder;
    public Sprite misteryBorder;
    public Sprite eventBorder;
    public Sprite specialBorder;
    public Sprite defaultBorder;

    [SerializeField] private RectTransform cardContent;
    [SerializeField] private GameObject guiCardPrefab;

    [SerializeField] private GameObject infoTextEmpty;

    public Action closeCallback;

    public void InitCardPanel(Action afterCloseCallback = null)
    {
        GameManager _gm = GameManager.Instance;
        closeCallback = afterCloseCallback;

        foreach (RectTransform child in cardContent)
        {
            Destroy(child.gameObject);
        }

        if(_gm.PlayersArray[_gm.PlayerIndex].Inventory.CardItems.Count == 0)
        {
            infoTextEmpty.SetActive(true);
        }
        else
        {

            infoTextEmpty.SetActive(false);

            for (int i = 0; i < _gm.PlayersArray[_gm.PlayerIndex].Inventory.CardItems.Count; i++)
            {
                GameObject card = Instantiate(guiCardPrefab, cardContent);
                card.GetComponent<GUICard>().SetData(_gm.PlayersArray[_gm.PlayerIndex].Inventory.CardItems[i]);
            }
        }
    }


    public void UseCard(CardItemData cardData)
    {
        //Filtramos si es la situación adecuada para usar la carta o no
        bool isInBattle = GameManager.Instance.GuiManager.BattlePanelGui.gameObject.activeInHierarchy;

        switch (cardData.CardType)
        {
            case CardType.Battle:
                if (!isInBattle)
                {
                    NotificationUI.Instance.SetMessage("No se puede usar en esta situación", Color.red);
                    return;
                }
                break;

            default:
                if (isInBattle)
                {
                    NotificationUI.Instance.SetMessage("No se puede usar en esta situación", Color.red);
                    return;
                }
                break;
        }

        int cardIndex = ItemManager.Instance.GetItemID(cardData);
        GameManager.Instance.GmView.RPC("UseCard", Photon.Pun.RpcTarget.All, GameManager.Instance.PlayerIndex, cardIndex);
        gameObject.SetActive(false);
    }

    public void btnCancel()
    {
        ClosePanel();
    }

    public void ClosePanel()
    {
        closeCallback?.Invoke();
        gameObject.SetActive(false);
        closeCallback = null;
    }
}
