using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int coinsQuantity;
    [SerializeField] private int safeRelicsQuantity;
    [SerializeField] private List<GemItemData> gemItems;
    [SerializeField] private List<CardItemData> cardItems;
    [SerializeField] private bool hasRelicItem;
    [SerializeField] private RelicItemData relicItemData;
    [SerializeField] private GameObject relicVisual;

    public int CoinsQuantity { get => coinsQuantity; set => coinsQuantity = value; }
    public int SafeRelicsQuantity { get => safeRelicsQuantity; set => safeRelicsQuantity = value; }
    public List<GemItemData> GemItems { get => gemItems; set => gemItems = value; }
    public List<CardItemData> CardItems { get => cardItems; set => cardItems = value; }
    public bool HasRelicItem { get => hasRelicItem; set => hasRelicItem = value; }
    public RelicItemData RelicItemData { get => relicItemData; set => relicItemData = value; }

    private GameManager _gm;
    private ItemManager _im;
    private PlayerManager _pm;

    private void Awake()
    {
        _gm = GameManager.Instance;
        _im = ItemManager.Instance;
        _pm = GetComponent<PlayerManager>();
    }



    public void AddCoins(int quantity)
    {
        coinsQuantity += quantity;
    }

    public void AddItem(int itemId)
    {
        ItemType itemType = _im.GetItemType(itemId);
        switch (itemType) {
            case ItemType.Gem:
                AddGem(itemId);
                break;

            case ItemType.Card:
                AddCard(itemId);
                break;

            case ItemType.Relic:
                AddRelic(itemId);
                break;

            default:
                Debug.LogError("ItemID: " + itemId + " no obtenible por inventario");
                break;
        }
    }

    public void AddGem(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        ItemData data = _im.GetItemData(itemId);
        GemItemData gem = data as GemItemData;

        if (gem != null) gemItems.Add(gem);
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
        /*
        if (itemId == -1) return;
        gemItems.Add((GemItemData)_im.GetItemData(itemId));*/
    }

    public void AddCard(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        ItemData data = _im.GetItemData(itemId);
        CardItemData card = data as CardItemData;

        if (card != null) cardItems.Add(card);
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
    }

    public void AddRelic(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        if (relicItemData != null) return;
        //DROP RELIC EN CASO DE QUE TENGA ALGUNO EN POSESIÓN

        ItemData data = _im.GetItemData(itemId);
        RelicItemData relic = data as RelicItemData;

        relicVisual.SetActive(true);

        if (relic != null) relicItemData = relic;
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
    }

    public void DropRelic()
    {

    }

    public void SaveRelic()
    {
        if (relicItemData != null)
        {
            safeRelicsQuantity++;
            relicItemData = null;
            relicVisual.SetActive(false);
            _pm.Graphics.ConfetiParticle.Play();
            StartCoroutine(CinematicAnimation.WaitTime(0.4f, () => _pm.Graphics.PlayCheerAnim()));
            
        }
    }


    //Drop items
}
