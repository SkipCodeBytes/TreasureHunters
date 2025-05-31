using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelGUI : MonoBehaviour
{
    [SerializeField] private List<GuiItem> itemsGUI;
    [SerializeField] private List<Text> itemPricesTxt;
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    public void StartShop()
    {
        for (int i = 0; i < itemsGUI.Count; i++) {
            ItemData item = ItemManager.Instance.GetItemData(Random.Range(1, ItemManager.Instance.RegisteredItems.Count));
            itemsGUI[i].SetItemInfo(item);
            itemPricesTxt[i].text = "$" + item.Price;
            Debug.Log("ShopItemName: " + item.name);
        }
    }

    public void btnBuyItem(GuiItem itemGui)
    {
        ItemData itemObtained = itemGui.Item;
        int itemId = ItemManager.Instance.GetItemID(itemObtained);
        if (itemObtained.Price <= _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.CoinsQuantity) {
            _gm.GmView.RPC("SyncroAddShopReward", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex, itemId, itemObtained.Price);
            gameObject.SetActive(false);
        }
    }


    public void SkipPanel()
    {
        gameObject.SetActive(false);
        EventManager.TriggerEvent("EndEvent");
    }
}
