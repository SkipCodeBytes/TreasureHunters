using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMapSelector : MonoBehaviour
{
    [SerializeField] private Text txtMapName;
    [SerializeField] private Image mapImage;

    [SerializeField] private int selectedMapIndex = 0;
    [SerializeField] private List<PlayableMapArea> playableMapList;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private PhotonView view;

    public int SelectedMapIndex { get => selectedMapIndex; set => selectedMapIndex = value; }
    public List<PlayableMapArea> PlayableMapList { get => playableMapList; set => playableMapList = value; }

    void Start()
    {
        RefreshMapInfo();
    }

    public void DisableButtons()
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
    }

    public void EnableButtons()
    {
        nextButton.interactable = true;
        prevButton.interactable = true;
    }

    public void nextMapSelection()
    {
        selectedMapIndex++;
        if(selectedMapIndex >= playableMapList.Count)
        {
            selectedMapIndex = 0;
        }
        RefreshMapInfo();
        view.RPC("SyncroSelectedMap", RpcTarget.OthersBuffered, selectedMapIndex);
    }

    public void prevMapSelection()
    {
        selectedMapIndex--;
        if (selectedMapIndex < 0)
        {
            selectedMapIndex = playableMapList.Count - 1;
        }
        RefreshMapInfo();
        view.RPC("SyncroSelectedMap", RpcTarget.OthersBuffered, selectedMapIndex);
    }

    public void RefreshMapInfo()
    {
        txtMapName.text = playableMapList[selectedMapIndex].MapName;
        mapImage.sprite = playableMapList[selectedMapIndex].MapImage;
    }

}
