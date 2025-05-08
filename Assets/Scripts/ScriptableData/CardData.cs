using UnityEngine;


//ELIMINAR SCRIPT, EN CAMBIO, CREAR SCRIPTTABLEOBJECTS PARA CARTAS

[CreateAssetMenu(fileName = "New Card", menuName = "Game/Card")]
public class CardData : ScriptableObject
{
    [SerializeField] private string cardName;
    [SerializeField] private CardType cardType;
    [SerializeField, TextArea(3,5)] private string cardDescription;
    [SerializeField] private Sprite imageCard;

    //public MonoBehaviour effect;

    public string CardName { get => cardName; }
    public CardType CardType { get => cardType; }
    public string CardDescription { get => cardDescription; }
    public Sprite ImageCard { get => imageCard; }
}


