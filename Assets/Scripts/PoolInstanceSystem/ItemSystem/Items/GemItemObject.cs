using UnityEngine;

public class GemItemObject : ItemObject
{
    [SerializeField] private GemItemData gemItemData;
    [SerializeField] private MeshRenderer m_Renderer;

    protected override void Awake()
    {
        base.Awake();
        m_Renderer = GetComponent<MeshRenderer>();
    }

    override public void SetItemObjectValues(ItemData data)
    {
        gemItemData = data as GemItemData;
        m_Renderer.material = gemItemData.GemMaterial;
    }
}
