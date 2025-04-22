using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableScreen : MonoBehaviour
{
    [SerializeField] private SelectableBar resolutionBar;
    [SerializeField] private SelectableBar refreshRateBar;
    [SerializeField] private Toggle toggleFullScreen;
    
    private List<List<Resolution>> availableResolutions = new List<List<Resolution>>();
    private Vector2Int currentIndexResolution = Vector2Int.zero;

    void OnEnable()
    {
        scanScreenResolution();
        resolutionBar.setActionEvents(applyResolutionSize);
        refreshRateBar.setActionEvents(applyRefreshRate);
        Debug.Log("Open Selectable Screen " + currentIndexResolution);
    }

    private void scanScreenResolution()
    {
        availableResolutions = getAvailableResolutions(out currentIndexResolution);
        toggleFullScreen.SetIsOnWithoutNotify(Screen.fullScreen);
    }


    private List<List<Resolution>> getAvailableResolutions(out Vector2Int currentIndex)
    {
        List<List<Resolution>> resolutionsList = new List<List<Resolution>>();
        List<string> resolutionsString = new List<string>();
        List<string> refreshRatesStringList = new List<string>();

        //Resolution currentResolution = Screen.currentResolution;
        Resolution[] resolutions = Screen.resolutions;

        //Debug.Log(currentResolution.width + "x" + currentResolution.height);
        Debug.Log("Counts: " + resolutions.Length);

        Vector2Int index = new Vector2Int(0, 0);
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutionsList.Count > 0)
            {
                if (resolutions[i].height == resolutions[i - 1].height && resolutions[i].width == resolutions[i - 1].width)
                {
                    resolutionsList[resolutionsList.Count - 1].Add(resolutions[i]);
                }
                else
                {
                    resolutionsList.Add(new List<Resolution>());
                    resolutionsList[resolutionsList.Count - 1].Add(resolutions[i]);
                    resolutionsString.Add(resolutions[i].width + "x" + resolutions[i].height);
                }
            }
            else
            {
                resolutionsList.Add(new List<Resolution>());
                resolutionsList[0].Add(resolutions[0]);
                resolutionsString.Add(resolutions[0].width + "x" + resolutions[0].height);
            }



            if (resolutions[i].height == Screen.height &&
                resolutions[i].width == Screen.width &&
                resolutions[i].refreshRateRatio.value == ConfigManager.Instance.MyGameConfig.RefreshRate)
            {
                index = new Vector2Int(resolutionsList.Count - 1, resolutionsList[resolutionsList.Count - 1].Count - 1);
                Debug.Log("Resolution Found: " + index  + " Value " + i);
            }
        }
        
        resolutionBar.setOptionList(resolutionsString, index.x);
        
        for (int i = 0; i < resolutionsList[index.x].Count; i++)
        {
            refreshRatesStringList.Add((float)Math.Round(resolutionsList[index.x][i].refreshRateRatio.value, 2) + "Hz");
        }
        refreshRateBar.setOptionList(refreshRatesStringList, index.y);
        
        currentIndex = index;
        return resolutionsList;
    }


    public void applyResolutionSize()
    {
        Debug.Log("Apply rs");
        currentIndexResolution[0] = resolutionBar.ExplorerIndex;
        currentIndexResolution[1] = availableResolutions[resolutionBar.ExplorerIndex].Count - 1;
        applyScreenMode();

        List<string> refreshRatesStringList = new List<string>();

        for (int i = 0; i < availableResolutions[currentIndexResolution[0]].Count; i++)
        {
            refreshRatesStringList.Add((float)Math.Round(availableResolutions[currentIndexResolution[0]][i].refreshRateRatio.value, 2) + "Hz");
        }
        refreshRateBar.setOptionList(refreshRatesStringList, currentIndexResolution[1]);
    }


    public void applyRefreshRate()
    {
        Debug.Log("Apply rr");
        resolutionBar.ExplorerIndex = currentIndexResolution[0];
        currentIndexResolution[1] = refreshRateBar.ExplorerIndex;
        applyScreenMode();
    }

    public void applyScreenMode()
    {
        Debug.Log("Apply Screen");
        FullScreenMode screenMode;
        if (toggleFullScreen.isOn) screenMode = FullScreenMode.FullScreenWindow; else screenMode = FullScreenMode.Windowed;

        Resolution newResolution = new Resolution();
        newResolution.width = availableResolutions[currentIndexResolution.x][currentIndexResolution.y].width;
        newResolution.height = availableResolutions[currentIndexResolution.x][currentIndexResolution.y].height;
        newResolution.refreshRateRatio = availableResolutions[currentIndexResolution.x][currentIndexResolution.y].refreshRateRatio;

        Screen.SetResolution(newResolution.width, newResolution.height, screenMode, newResolution.refreshRateRatio);

        ConfigManager.Instance.MyGameConfig.ScreenWidth = newResolution.width;
        ConfigManager.Instance.MyGameConfig.ScreenHeight = newResolution.height;
        ConfigManager.Instance.MyGameConfig.RefreshRate = newResolution.refreshRateRatio.value;
        ConfigManager.Instance.MyGameConfig.FullscreenMode = screenMode;
        ConfigManager.Instance.SaveData();
        /*
        ConsoleScript.writeConsoleMessage(
            $"Resolucion: {Screen.currentResolution.width}x{Screen.currentResolution.height} {Screen.currentResolution.refreshRateRatio}Hz, Modo: {screenMode}");*/
        ConsoleScript.writeConsoleMessage(
            $"Resolucion: {newResolution.width}x{newResolution.height} {newResolution.refreshRateRatio}Hz, Modo: {screenMode}");
    }

}
