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

    public int CoinsQuantity { get => coinsQuantity; set => coinsQuantity = value; }
    public int SafeRelicsQuantity { get => safeRelicsQuantity; set => safeRelicsQuantity = value; }
    public List<GemItemData> GemItems { get => gemItems; set => gemItems = value; }
    public List<CardItemData> CardItems { get => cardItems; set => cardItems = value; }
    public bool HasRelicItem { get => hasRelicItem; set => hasRelicItem = value; }
    public RelicItemData RelicItemData { get => relicItemData; set => relicItemData = value; }


    //Drop items
}
