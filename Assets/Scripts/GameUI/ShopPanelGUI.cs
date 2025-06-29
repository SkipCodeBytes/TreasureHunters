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
        ItemType itemType = ItemManager.Instance.GetItemType(itemId);

        //if (ItemType.Relic == itemType && _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.HasRelicItem) { return; }
        if (itemObtained.Price <= _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.CoinsQuantity) {
            _gm.GmView.RPC("SyncroAddShopReward", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex, itemId, itemObtained.Price);
            gameObject.SetActive(false);
        }
    }


    public void SkipPanel()
    {
        Vector2Int tileOrder = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition.Order;
        _gm.GmView.RPC("SyncroSkipShop", Photon.Pun.RpcTarget.All, tileOrder.x, tileOrder.y);
        gameObject.SetActive(false);
        EventManager.TriggerEvent("EndEvent");
    }
}
