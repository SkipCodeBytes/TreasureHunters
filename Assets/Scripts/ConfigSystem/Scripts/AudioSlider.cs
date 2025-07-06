using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider dialogVolumeSlider;

    private void Awake()
    {
        generalVolumeSlider.onValueChanged.AddListener(updateGeneralVolume);
        musicVolumeSlider.onValueChanged.AddListener(updateMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(updateSfxVolume);
        dialogVolumeSlider.onValueChanged.AddListener(updateDialogVolume);

    }

    private void OnEnable()
    {
        GetCurrentVolume();
    }

    void Start()
    {
        EventManager.StartListening("SliderEndVolume", SetCurrentVolume);
        EventManager.StartListening("SliderClicVolume", SetCurrentVolume);
    }


    private void updateGeneralVolume(float value)
    {
        audioMixer.SetFloat("GeneralVolume", Mathf.Log10(value) * 20);
    }

    private void updateMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
    private void updateSfxVolume(float value)
    {
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(value) * 20);
    }

    private void updateDialogVolume(float value)
    {
        audioMixer.SetFloat("DialogVolume", Mathf.Log10(value) * 20);
    }

    private void SetCurrentVolume()
    {
        Debug.Log("Valores de audio guardados");
        ConfigManager.Instance.MyGameConfig.GeneralVolume = generalVolumeSlider.value;
        ConfigManager.Instance.MyGameConfig.MusicVolume = musicVolumeSlider.value;
        ConfigManager.Instance.MyGameConfig.SfxVolume = sfxVolumeSlider.value;
        ConfigManager.Instance.MyGameConfig.DialogVolume = dialogVolumeSlider.value;
        ConfigManager.Instance.SaveData();
    }


    public void GetCurrentVolume()
    {
        generalVolumeSlider.value = ConfigManager.Instance.MyGameConfig.GeneralVolume;
        musicVolumeSlider.value = ConfigManager.Instance.MyGameConfig.MusicVolume;
        sfxVolumeSlider.value = ConfigManager.Instance.MyGameConfig.SfxVolume;
        dialogVolumeSlider.value = ConfigManager.Instance.MyGameConfig.DialogVolume;

        updateGeneralVolume(generalVolumeSlider.value);
        updateMusicVolume(musicVolumeSlider.value);
        updateSfxVolume(sfxVolumeSlider.value);
        updateDialogVolume(dialogVolumeSlider.value);
    }
}
