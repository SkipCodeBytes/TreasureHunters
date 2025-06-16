using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuinPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefab;

    [SerializeField] private Transform itemNeededContent;
    [SerializeField] private List<Image> itemNeededImage;

    private GameManager _gm;
    private Dictionary<int, int> playerGems = new Dictionary<int, int>();

    public void StartRuinPanel()
    {
        _gm = GameManager.Instance;

        for (int i = 0; i < itemNeededImage.Count; i++)
        {
            itemNeededImage[i].sprite = RuinsTile.GemsNeeded[i].Icon;
        }

        PlayerInventory player = _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory;

        int childCount = itemNeededContent.childCount;
        for (int i = 0; i < childCount; i++) 
        {
            Destroy(itemNeededContent.GetChild(0));
        }

        playerGems.Clear();

        for (int i = 0; i < player.GemItems.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemNeededContent);
            RectTransform rectSlot = slot.GetComponent<RectTransform>();
            rectSlot.anchoredPosition = new Vector2(0f, -15f - (i * 85));

            rectSlot.GetChild(0).GetComponent<Image>().sprite = player.GemItems[i].Icon;
            rectSlot.GetChild(1).GetComponent<Text>().text = player.GemItems[i].ItemName;

            int itemId = ItemManager.Instance.GetItemID(player.GemItems[i]);

            if (playerGems.ContainsKey(itemId)) playerGems[itemId] = playerGems[itemId]++;
            else playerGems[itemId] = 1;
        }
    }

    public void btnSkip()
    {
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
    }

    public void btnSubmit()
    {
        //RPC para perder objetos del inventario y brindar una estrella
    }
}
