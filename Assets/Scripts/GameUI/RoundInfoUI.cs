using UnityEngine;
using UnityEngine.UI;

public class RoundInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject panelUI;
    [SerializeField] private float transicionTime;
    [SerializeField] private Text roundTittle;
    [SerializeField] private float timeBetweenWrite;
    [SerializeField] private float animationEndTime;
    [SerializeField] private float exitDisplaceTime;

    private Image viewPanelImage;
    private Color _currentColorCanvas;
    string defaultTxt;


    public void StartPresentation()
    {
        viewPanelImage = transform.GetChild(0).GetComponent<Image>();
        _currentColorCanvas = viewPanelImage.color;
        viewPanelImage.color = new Color(_currentColorCanvas.r, _currentColorCanvas.g, _currentColorCanvas.b, 0f);
        StartCoroutine(CinematicAnimation.ImageAlphaLerp(viewPanelImage, _currentColorCanvas.a, transicionTime, showTittle));

        defaultTxt = "RONDA: " + GameManager.Instance.GameRound;
        roundTittle.text = "";
    }

    private void showTittle()
    {
        StartCoroutine(CinematicAnimation.TextTypewriter(roundTittle, defaultTxt, timeBetweenWrite));
        StartCoroutine(CinematicAnimation.WaitTime(animationEndTime, exitAnimation));
    }

    private void exitAnimation()
    {
        Vector2 displacePoint = new Vector2(Screen.width * 2, 0f);
        StartCoroutine(CinematicAnimation.MoveTo(panelUI, displacePoint, exitDisplaceTime, closeCanvas));
    }

    private void closeCanvas()
    {
        EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
    }
}
