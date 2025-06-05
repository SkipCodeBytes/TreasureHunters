using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private RectTransform panelUI;
    [SerializeField] private float transicionTime;
    [SerializeField] private Text roundTittle;
    [SerializeField] private float timeBetweenWrite;
    [SerializeField] private float animationEndTime;
    [SerializeField] private float exitDisplaceTime;

    private Vector3 _initPosition;
    private Image viewPanelImage;
    private Color _currentColorCanvas;
    string defaultTxt;

    private void Awake()
    {
        _initPosition = panelUI.anchoredPosition;
        viewPanelImage = transform.GetChild(0).GetComponent<Image>();
        _currentColorCanvas = viewPanelImage.color;
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    public void StartPresentation()
    {
        Debug.Log("Start Presentation");
        panelUI.anchoredPosition = _initPosition;
        viewPanelImage.color = new Color(_currentColorCanvas.r, _currentColorCanvas.g, _currentColorCanvas.b, 0f);
        defaultTxt = "RONDA: " + GameManager.Instance.GameRound;
        roundTittle.text = "";
        StartCoroutine(CinematicAnimation.UiImageAlphaLerp(viewPanelImage, _currentColorCanvas.a, transicionTime, showTittle));
    }

    private void showTittle()
    {
        Debug.Log("Show Tittle");
        SoundController.Instance.PlaySound(_gm.SoundLibrary.DiceResult);
        StartCoroutine(CinematicAnimation.UiTextTypewriter(roundTittle, defaultTxt, timeBetweenWrite));
        StartCoroutine(CinematicAnimation.WaitTime(animationEndTime, exitAnimation));
    }

    private void exitAnimation()
    {
        Debug.Log("ExitAnimation");
        Vector2 displacePoint = new Vector2(Screen.width * 2, 0f);
        StartCoroutine(CinematicAnimation.UiMoveTo(panelUI, displacePoint, exitDisplaceTime, closeCanvas));
    }

    private void closeCanvas()
    {
        Debug.Log("CloseCanvas");
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
    }
}
