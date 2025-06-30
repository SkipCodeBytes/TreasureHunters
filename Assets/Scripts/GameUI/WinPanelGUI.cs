using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanelGUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<RectTransform> slotList = new List<RectTransform>();

    [SerializeField] private GameObject cameraFocus;
    [SerializeField] private GameObject winnerStamp;

    [SerializeField] private string initSceneName;

    private List<int> playerPoints;
    private List<PlayerManager> playersWinSorted = new List<PlayerManager>();
    private GameManager _gm;

    /*
     * Ganar = 2500 puntos
     * Star = 1500 puntos
     * Reliquia = 500 puntos
     * Gem = 100 puntos
     * Coin = 1 punto
     */

    public void StartWinPanel()
    {
        _gm = GameManager.Instance;
        playerPoints = new List<int>();

        //Calculamos las puntuaciones
        for (int i = 0; i < _gm.PlayersArray.Length; i++)
        {
            playerPoints.Add(0);

            if(_gm.PlayersArray[i] != null)
            {
                playerPoints[i] += _gm.PlayersArray[i].Inventory.CoinsQuantity;
                playerPoints[i] += (_gm.PlayersArray[i].Inventory.GemItems.Count * 100);
                playerPoints[i] += _gm.PlayersArray[i].Inventory.RelicItemData == null ? 0 : 500;
                playerPoints[i] += (_gm.PlayersArray[i].Rules.GameStarsQuantity * 1500);
                playerPoints[i] += _gm.PlayersArray[i].Rules.GameStarsQuantity >= _gm.PlayersArray[i].Rules.starsToWin ? 2500 : 0;
            }
        }

        //Ordenamos las puntuaciones
        List<int> sortedPoints = new List<int>();
        sortedPoints = playerPoints.OrderByDescending(x => x).ToList();

        //Ordenamos los jugadores
        for (int i = 0; i < sortedPoints.Count; i++)
        {
            int playerIndex = playerPoints.IndexOf(sortedPoints[i]);
            playerPoints[playerIndex] = -1;
            playersWinSorted.Add(_gm.PlayersArray[playerIndex]);
        }

        //Asignamos los datos a los slots
        for (int i = 0; i < slotList.Count; i++)
        {
            if(playersWinSorted[i] == null) continue;
            slotList[i].GetChild(1).GetComponent<Text>().text = playersWinSorted[i].Player.NickName;
            slotList[i].GetChild(2).GetComponent<Image>().sprite = playersWinSorted[i].SelectedCharacter.characterSprite;
            slotList[i].GetChild(3).GetComponent<Text>().text = "Puntos" + sortedPoints[i];
        }

        //Enfocamos cámara
        cameraFocus.transform.position = playersWinSorted[0].transform.position;
        cameraFocus.transform.rotation = playersWinSorted[0].transform.rotation;

        List<float> defaultXposition = new List<float>();

        for(int i = 0; i < slotList.Count; i++)
        {
            defaultXposition.Add(slotList[i].anchoredPosition.x);
            slotList[i].anchoredPosition = new Vector2(-500f, slotList[i].anchoredPosition.y);
            StartCoroutine(SlotAnimation(i, defaultXposition[i], 0.7f * i));
        }
        StartCoroutine(CinematicAnimation.WaitTime(2.5f, () => playersWinSorted[0].Graphics.PlayCheerAnim()));
        StartCoroutine(CinematicAnimation.WaitTime(4f, 
            () => StartCoroutine(
                CinematicAnimation.UiMoveTo(
                    winnerStamp.GetComponent<RectTransform>(), 
                    new Vector2(-180f, 70f), 0.3f,
                    () => SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("StrongHit")))
        )));
    }

    private IEnumerator SlotAnimation(int index, float defaultXPos, float time)
    {
        yield return new WaitForSeconds(time);
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("PlayerOk"));
        StartCoroutine(CinematicAnimation.UiMoveTo(slotList[index], new Vector2(defaultXPos, slotList[index].anchoredPosition.y), 0.7f));
    }

    public void btnEndGame()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }


    public override void OnLeftRoom()
    {
        // Cargar escena del lobby
        SceneManager.LoadScene(initSceneName);
    }
}
