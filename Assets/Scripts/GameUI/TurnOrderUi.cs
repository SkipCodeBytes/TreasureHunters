using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUi : MonoBehaviour
{
    [SerializeField] private GameObject viewPanel;
    [SerializeField] private float transicionTime;

    [SerializeField] private RectTransform desplazablePanel;
    [SerializeField] private float displaceTime;

    [SerializeField] private GameObject tittlePanel;
    [SerializeField] private Text tittle;
    [SerializeField] private float timeBetweenWrite;

    [SerializeField] private List<GameObject> playerSlots = new List<GameObject>();
    [SerializeField] private float playerSlotsTransicionTime;
    [SerializeField] private Vector3 eulerRotationAnim;

    [SerializeField] private float timeToClosePanel;

    private List<BoardPlayer> playerOrder = new List<BoardPlayer>();

    private Image viewPanelImage;
    private Color _currentColorCanvas;
    private int _indexView = 0;

    private void Awake()
    {
        viewPanelImage = viewPanel.GetComponent<Image>();
        tittlePanel.SetActive(false);
        for (int i = 0; i < playerSlots.Count; i++) { playerSlots[i].SetActive(false); }
    }

    public void StartPresentation()
    {
        _currentColorCanvas = viewPanelImage.color;
        viewPanelImage.color = new Color(_currentColorCanvas.r, _currentColorCanvas.g, _currentColorCanvas.b, 0f);
        viewPanel.SetActive(true);
        StartCoroutine(CinematicAnimation.ImageAlphaLerp(viewPanelImage, _currentColorCanvas.a, transicionTime, showTittle));
        setSlotValues();
    }

    private void setSlotValues()
    {
        for(int i = 0; i < GameManager.Instance.BoardPlayers.Length; i++)
        {
            if (GameManager.Instance.BoardPlayers[i] != null)
            {
                playerOrder.Add(GameManager.Instance.BoardPlayers[i]);
            }
        }
        Debug.Log("Players: " + GameManager.Instance.BoardPlayers.Length);
        Debug.Log("Players: " + playerOrder.Count);
    }

    private void showTittle()
    {
        string defaultTxt = tittle.text;
        tittle.text = "";
        tittlePanel.SetActive(true);
        StartCoroutine(CinematicAnimation.TextTypewriter(tittle, defaultTxt, timeBetweenWrite, showPlayerSlot));
    }

    private void showPlayerSlot()
    {
        if (playerSlots.Count < _indexView + 1) {
            StartCoroutine(CinematicAnimation.WaitTime(timeToClosePanel, displacePanel));
            return;
        }
        playerSlots[_indexView].SetActive(true);

        StartCoroutine(CinematicAnimation.WaitTime(playerSlotsTransicionTime / 2f, setTextPlayerOrder));
        StartCoroutine(CinematicAnimation.EulerRotation(playerSlots[_indexView].transform, eulerRotationAnim, playerSlotsTransicionTime, showPlayerSlot));
        _indexView++;
    }

    private void setTextPlayerOrder()
    {
        if (playerOrder.Count < _indexView) return;
        if (playerOrder[_indexView-1] == null) return;
        Text playerTxt = playerSlots[_indexView-1].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        playerTxt.text = playerOrder[_indexView-1].Player.NickName;
    }

    private void displacePanel()
    {
        Vector2 displacePoint = new Vector2(Screen.width, 0f);
        StartCoroutine(CinematicAnimation.MoveTo(desplazablePanel, displacePoint, displaceTime, hidePanel));
    }

    private void hidePanel()
    {
        StartCoroutine(CinematicAnimation.ImageAlphaLerp(viewPanelImage, 0f, transicionTime, closeCanvas));
    }

    private void closeCanvas()
    {
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
    }
}
