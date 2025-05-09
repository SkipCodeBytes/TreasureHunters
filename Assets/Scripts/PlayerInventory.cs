using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int coinsQuantity;
    [SerializeField] private int safeRelicsQuantity;
    [SerializeField] private List<GemItemData> gemItems;
    [SerializeField] private List<CardItemData> cardItems;
    [SerializeField] private RelicItemData relicItem;

    public int CoinsQuantity { get => coinsQuantity; }
    public int SafeRelicsQuantity { get => safeRelicsQuantity; }
    public List<GemItemData> GemItems { get => gemItems; }
    public List<CardItemData> CardItems { get => cardItems; }
    public RelicItemData RelicItem { get => relicItem; }

    //Drop items
}
