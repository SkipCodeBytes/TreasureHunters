using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private ItemObject itemObjectPrefab;
    [SerializeField] private Sprite icon;
    [SerializeField] private int price;

    public ItemObject ItemObjectPrefab { get => itemObjectPrefab; set => itemObjectPrefab = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public int Price { get => price; set => price = value; }
}