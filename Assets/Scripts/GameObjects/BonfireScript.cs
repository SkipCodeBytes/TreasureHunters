
using UnityEngine;

public class BonfireScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private ParticleSystem smokeParticle;
    [SerializeField] private Light fireLight;

    private Animator _animator;
    private float _defaultLightIntensity;
    private GameManager _gm;

    public Animator Animator { get => _animator; set => _animator = value; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _defaultLightIntensity = fireLight.intensity; 
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        fireLight.intensity = _defaultLightIntensity;
        _animator.Play("BonfireIdle");
    }

    private void OnEnable()
    {
        fireLight.intensity = 0;
        _animator.Play("BonfireAwake");
    }


    public void PlayFireParticles()
    {
        fireParticle.Clear();
        fireParticle.Play();

        StartCoroutine(LerpUtils.LerpFloat(value => fireLight.intensity = value, 0f, _defaultLightIntensity, 0.8f));
    }

    public void PlaySmokeParticles()
    {
        smokeParticle.Clear();
        smokeParticle.Play();
    }

    public void StopFireParticles()
    {
        StartCoroutine(LerpUtils.LerpFloat(value => fireLight.intensity = value, _defaultLightIntensity, 0f, 0.8f));
        fireParticle.Stop();
    }
    public void StopSmokeParticles()
    {
        smokeParticle.Stop();
    }

    public void HideElements()
    {
        gameObject.SetActive(false);
    }

    public void PlayFallHitSound()
    {
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("FallHit"));
    }
}
