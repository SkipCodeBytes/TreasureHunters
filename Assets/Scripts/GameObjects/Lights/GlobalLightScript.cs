using UnityEngine;

public class GlobalLightScript : MonoBehaviour
{
    [SerializeField] private float intensityTransicionDuration = 0.5f;
    private Light _light;
    private float _defaultIntensity;

    private Coroutine _changeIntensityCoroutine;

    void Start()
    {
        _light = GetComponent<Light>();
        _defaultIntensity = _light.intensity;
    }


    public void TurnOffLight()
    {
        if (_changeIntensityCoroutine != null) StopCoroutine(_changeIntensityCoroutine);

        _changeIntensityCoroutine = StartCoroutine(LerpUtils.LerpFloat(value => _light.intensity = value, _defaultIntensity, 0f, intensityTransicionDuration));
    }

    public void TurnOnLight()
    {
        if (_changeIntensityCoroutine != null) StopCoroutine(_changeIntensityCoroutine);

        _changeIntensityCoroutine = StartCoroutine(LerpUtils.LerpFloat(value => _light.intensity = value, 0f, _defaultIntensity, intensityTransicionDuration));
    }

}
