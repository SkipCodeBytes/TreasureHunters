using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Gem Item")]
public class GemItemData : ItemData
{
    [SerializeField] private Material gemMaterial;
    [SerializeField] private Sprite gemItemImage;

    public Material GemMaterial { get => gemMaterial; set => gemMaterial = value; }
    public Sprite GemItemImage { get => gemItemImage; set => gemItemImage = value; }
}
