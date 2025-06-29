using UnityEngine;

public class GemItemObject : ItemObject
{
    [SerializeField] private GemItemData gemItemData;
    [SerializeField] private MeshRenderer m_Renderer;

    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    protected override void Awake()
    {
        base.Awake();
        m_Renderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(CinematicAnimation.WaitTime(Random.Range(0.08f, 0.2f), () =>
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("GemObj"))));
    }

    override public void SetItemObjectValues(int ID, ItemData data)
    {
        base.SetItemObjectValues(ID, data);
        gemItemData = data as GemItemData;
        m_Renderer.material = gemItemData.GemMaterial;
    }
}
