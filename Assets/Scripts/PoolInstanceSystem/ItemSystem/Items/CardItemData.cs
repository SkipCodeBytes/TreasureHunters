using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Card Item")]
public class CardItemData : ItemData
{
    [SerializeField] private CardType cardType;
    [SerializeField] private Sprite imageCard;
    [SerializeField] private int cardLevel;
    [SerializeField] private string methodName;

    public CardType CardType { get => cardType; }
    public Sprite ImageCard { get => imageCard; }
    public int CardLevel { get => cardLevel; set => cardLevel = value; }
    public string MethodName { get => methodName; }
}

