
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class GameConfig
{
    //[SerializeField] private Resolution _screenResolution;
    [SerializeField] private int _screenHeight;
    [SerializeField] private int _screenWidth;
    [SerializeField] private double _refreshRate;
    [SerializeField] private FullScreenMode _fullscreenMode;

    [SerializeField] private float _generalVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;
    [SerializeField] private float _dialogVolume;

    //public Resolution ScreenResolution { get => _screenResolution; set => _screenResolution = value; }
    public int ScreenHeight { get => _screenHeight; set => _screenHeight = value; }
    public int ScreenWidth { get => _screenWidth; set => _screenWidth = value; }
    public double RefreshRate { get => _refreshRate; set => _refreshRate = value; }
    public FullScreenMode FullscreenMode { get => _fullscreenMode; set => _fullscreenMode = value; }
    public float GeneralVolume { get => _generalVolume; set => _generalVolume = value; }
    public float MusicVolume { get => _musicVolume; set => _musicVolume = value; }
    public float SfxVolume { get => _sfxVolume; set => _sfxVolume = value; }
    public float DialogVolume { get => _dialogVolume; set => _dialogVolume = value; }

    public void SetDefaultValues()
    {
        _screenHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
        _screenWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
        _refreshRate = Screen.resolutions[Screen.resolutions.Length - 1].refreshRateRatio.value;
        _fullscreenMode = FullScreenMode.FullScreenWindow;
        _generalVolume = 1f;
        _musicVolume = 1f;
        _sfxVolume = 1f;
        _dialogVolume = 1f;

        Debug.Log("Configuración con valores por defecto");
    }
    
    public void ApplyConfig()
    {
        int index = -1;
        Resolution[] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].height == _screenHeight &&
                resolutions[i].width == _screenWidth &&
                resolutions[i].refreshRateRatio.value == _refreshRate)
            {
                Debug.Log("Se ha encontrado la resolución");
                index = i;
                break;
            }
        }

        if (index == -1) index = resolutions.Length - 1;

        _screenHeight = resolutions[index].height;
        _screenWidth = resolutions[index].width;
        _refreshRate = resolutions[index].refreshRateRatio.value;

        Screen.SetResolution(_screenWidth, _screenHeight, _fullscreenMode, resolutions[index].refreshRateRatio);
        Debug.Log("Se ha aplicado configuración");
    }

    public void DebugValues()
    {
        Debug.Log($"{_screenWidth}x{_screenHeight} {RefreshRate} {FullscreenMode}" +
            $"\n G:{GeneralVolume} M:{_musicVolume} S:{_sfxVolume} D:{_dialogVolume}");
    }
}
