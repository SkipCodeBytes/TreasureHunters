using UnityEngine;

public class PedestalScript : MonoBehaviour
{
    [SerializeField] private Light strongLight;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem featherParticles;
    [SerializeField] private ParticleSystem continousSmokeParticle;

    [SerializeField] private Vector3 itemSpawnPos;

    [SerializeField] private float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;
    [SerializeField] private float itemDropHeight;

    [SerializeField] private float itemTimeStand;

    private GameManager _gm;

    private float _defaultLightIntensity;
    private Animator _animator;
    private int _targetPlayerIndex;
    private int[] _rewardArray;

    private ItemObject cardObj;

    void Awake()
    {
        _animator = GetComponent<Animator>();


        _defaultLightIntensity = strongLight.intensity;
        strongLight.intensity = 0;
    }




    private void OnEnable()
    {
        _animator.Play("Spawn");
    }
    private void Start()
    {
        _gm = GameManager.Instance;
        _animator.Play("IdlePedestal");
        smokeParticles.Stop();
        featherParticles.Stop();
        continousSmokeParticle.Stop();
        smokeParticles.Clear();
        featherParticles.Clear();
        continousSmokeParticle.Clear();
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
        cardObj = ItemManager.Instance.GenerateItemInScene(_rewardArray[2]);
        if (cardObj is CardItemObject card) card.SetInPickableMode();
        cardObj.DropAnimation(transform.position + itemSpawnPos, transform.position, itemDropHeight, itemDropMaxRadio, itemTimeDrop);
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop + itemTimeStand, EndAnimationPedestal));
    }


    public void PlayParticles()
    {
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Pedestal"));
        smokeParticles.Play();
        featherParticles.Play();
        StartCoroutine(LerpUtils.LerpFloat(value => strongLight.intensity = value, 0f, _defaultLightIntensity, 0.8f));
    }


    public void StopParticles() {

        strongLight.intensity = 0;
        featherParticles.Stop();
    }

    public void ContinousSmokeParticles()
    {
        continousSmokeParticle.Play();
    }

    //Referencia desde animación
    public void EndAnimationPedestal()
    {
        _animator.SetTrigger("Destroy");
        cardObj.TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform.position, itemTimeDrop);
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
