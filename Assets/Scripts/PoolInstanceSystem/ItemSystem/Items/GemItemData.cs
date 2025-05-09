using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Gem Item")]
public class GemItemData : ItemData
{
    [SerializeField] private Material gemMaterial;
    [SerializeField] private float glowIntensity;

    public Material GemMaterial { get => gemMaterial; set => gemMaterial = value; }
    public float GlowIntensity { get => glowIntensity; set => glowIntensity = value; }
}
