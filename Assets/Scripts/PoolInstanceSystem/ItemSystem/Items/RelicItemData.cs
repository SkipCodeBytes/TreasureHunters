using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Relic Item")]
public class RelicItemData : ItemData
{
    [SerializeField] private Sprite relicItemImage;

    public Sprite RelicItemImage { get => relicItemImage; set => relicItemImage = value; }
}

