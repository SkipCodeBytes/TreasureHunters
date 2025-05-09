using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Card Item")]
public class CardItemData : ItemData
{
    [SerializeField] private string cardName;
    [SerializeField] private CardType cardType;
    [SerializeField, TextArea(3, 5)] private string cardDescription;
    [SerializeField] private Sprite imageCard;
    [SerializeField] private int cardLevel;

    public string CardName { get => cardName; }
    public CardType CardType { get => cardType; }
    public string CardDescription { get => cardDescription; }
    public Sprite ImageCard { get => imageCard; }
    public int CardLevel { get => cardLevel; set => cardLevel = value; }
}

