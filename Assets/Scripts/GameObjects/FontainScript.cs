using UnityEngine;

public class FontainScript : MonoBehaviour
{
    private GameManager _gm;

    private Animator _animator;
    private int _targetPlayerIndex;
    private int[] _rewardArray;

    [SerializeField] private Vector3 itemSpawnPos;

    [SerializeField] private float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;
    [SerializeField] private float itemDropHeight;

    [SerializeField] private float itemTimeStand;

    private ItemObject gemObj;

    [SerializeField] private ParticleSystem GlowParticles;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        GlowParticles.Stop();
        GlowParticles.Clear();
    }

    private void OnEnable()
    {
        _animator.Play("Awake");
    }


    void Start()
    {
        _gm = GameManager.Instance;
    }

    public void PresentAnimation(int playerIndex, int[] rewards)
    {
        _rewardArray = rewards;
        _targetPlayerIndex = playerIndex;
        _animator.SetTrigger("Drop");
    }


    //Llamado desde la animación
    public void DropCardAnimation()
    {
        gemObj = ItemManager.Instance.GenerateItemInScene(_rewardArray[1]);
        if (gemObj is GemItemObject gem) gem.SetItemObjectValues(_rewardArray[1], ItemManager.Instance.GetItemData(_rewardArray[1]));
        gemObj.DropAnimation(transform.position + itemSpawnPos, transform.position, itemDropHeight, itemDropMaxRadio, itemTimeDrop);
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop + itemTimeStand, EndAnimation));
        GlowParticles.Play();
    }

    public void EndAnimation()
    {
        _animator.SetTrigger("Destroy");
        gemObj.TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform, itemTimeDrop);
    }

    public void CloseAnimation()
    {
        _gm.GuiManager.SlotInfoUIList[_targetPlayerIndex].SetPlayerInfo();
        EventManager.TriggerEvent("EndEvent", true);
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + itemSpawnPos, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDropMaxRadio);
    }
#endif


}
