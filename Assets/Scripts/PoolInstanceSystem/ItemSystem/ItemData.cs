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


[CreateAssetMenu(menuName = "Game/Items/Coin Item")]
public class CoinItemData : ItemData
{
}

[CreateAssetMenu(menuName = "Game/Items/Relic Item")]
public class RelicItemData : ItemData
{
}

[CreateAssetMenu(menuName = "Game/Items/Card Item")]
public class CardItemData : ItemData
{
    [SerializeField] private string cardName;
    [SerializeField] private CardType cardType;
    [SerializeField, TextArea(3, 5)] private string cardDescription;
    [SerializeField] private Sprite imageCard;
    [SerializeField] private int manaCost;

    public string CardName { get => cardName; }
    public CardType CardType { get => cardType; }
    public string CardDescription { get => cardDescription; }
    public Sprite ImageCard { get => imageCard; }
    public int ManaCost { get => manaCost; set => manaCost = value; }
}

[CreateAssetMenu(menuName = "Game/Items/Gem Item")]
public class GemItemData : ItemData
{
    [SerializeField] private Material gemMaterial;
    [SerializeField] private float glowIntensity;

    public Material GemMaterial { get => gemMaterial; set => gemMaterial = value; }
    public float GlowIntensity { get => glowIntensity; set => glowIntensity = value; }
}