using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;
using static System.TimeZoneInfo;

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


    public void StartPresentation()
    {
        _currentColorCanvas = viewPanelImage.color;
        StartCoroutine(CinematicAnimation.ImageAlphaLerp(viewPanelImage, _currentColorCanvas.a, transicionTime, showTittle));
    }

    private void showTittle()
    {
        string defaultTxt = roundTittle.text;
        roundTittle.text = "";
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
