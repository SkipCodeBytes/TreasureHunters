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

    void Start()
    {
        scanScreenResolution();
        resolutionBar.setActionEvents(applyResolutionSize);
        refreshRateBar.setActionEvents(applyRefreshRate);
    }

    private void scanScreenResolution()
    {
        availableResolutions = getAvailableResolutions(out currentIndexResolution);
        toggleFullScreen.isOn = Screen.fullScreen;
    }


    private List<List<Resolution>> getAvailableResolutions(out Vector2Int currentIndex)
    {
        List<List<Resolution>> resolutionsList = new List<List<Resolution>>();
        List<string> resolutionsString = new List<string>();
        List<string> refreshRatesStringList = new List<string>();

        Resolution currentResolution = Screen.currentResolution;
        Resolution[] resolutions = Screen.resolutions;

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
                    resolutionsString.Add(resolutions[i].height + "x" + resolutions[i].width);
                }
            }
            else
            {
                resolutionsList.Add(new List<Resolution>());
                resolutionsList[0].Add(resolutions[0]);
                resolutionsString.Add(resolutions[0].height + "x" + resolutions[0].width);
            }



            if (resolutions[i].height == currentResolution.height &&
                resolutions[i].width == currentResolution.width &&
                resolutions[i].refreshRateRatio.Equals(currentResolution.refreshRateRatio))
            {
                index = new Vector2Int(resolutionsList.Count - 1, resolutionsList[resolutionsList.Count - 1].Count - 1);
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
        currentIndexResolution[0] = resolutionBar.ExplorerIndex;
        currentIndexResolution[1] = availableResolutions[resolutionBar.ExplorerIndex].Count - 1;
        applyScrenMode();

        List<string> refreshRatesStringList = new List<string>();

        for (int i = 0; i < availableResolutions[currentIndexResolution[0]].Count; i++)
        {
            refreshRatesStringList.Add((float)Math.Round(availableResolutions[currentIndexResolution[0]][i].refreshRateRatio.value, 2) + "Hz");
        }
        refreshRateBar.setOptionList(refreshRatesStringList, currentIndexResolution[1]);
    }


    public void applyRefreshRate()
    {
        resolutionBar.ExplorerIndex = currentIndexResolution[0];
        currentIndexResolution[1] = refreshRateBar.ExplorerIndex;
        applyScrenMode();
    }

    public void applyScrenMode()
    {
        FullScreenMode screenMode;
        if (toggleFullScreen.isOn) screenMode = FullScreenMode.FullScreenWindow; else screenMode = FullScreenMode.Windowed;
        Screen.SetResolution(
            availableResolutions[currentIndexResolution.x][currentIndexResolution.y].width,
            availableResolutions[currentIndexResolution.x][currentIndexResolution.y].height,
            screenMode,
            availableResolutions[currentIndexResolution.x][currentIndexResolution.y].refreshRateRatio);

        ConsoleScript.writeConsoleMessage(
            $"Resolucion: {Screen.currentResolution.width}x{Screen.currentResolution.height} {Screen.currentResolution.refreshRateRatio}Hz, Modo: {screenMode}");
    }

}
