using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField, TextArea(3, 5)] private string itemDescription;
    [SerializeField] private ItemObject itemObjectPrefab;
    [SerializeField] private Sprite icon;
    [SerializeField] private int price;

    public string ItemName { get => itemName; set => itemName = value; }
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }
    public ItemObject ItemObjectPrefab { get => itemObjectPrefab; set => itemObjectPrefab = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public int Price { get => price; set => price = value; }
}