using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialPanel : MonoBehaviour
{
    public Text tituloText;
    public Text descripcionText;
    public Image imagenUI;

    public int tutorialIndex = 0;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private List<TutorialPanelData> data = new List<TutorialPanelData>();

    [Header("UI de progreso")]
    [SerializeField] private Text progresoText;

    private void Start()
    {
        if (data.Count > 0)
        {
            Mostrar(data[tutorialIndex]);
            ActualizarProgreso();
        }

        prevButton.interactable = false;
    }

    private void OnEnable()
    {
        tutorialIndex = 0;
        if (data.Count > 0)
        {
            Mostrar(data[tutorialIndex]);
            ActualizarProgreso();
        }

        ActualizarBotones();
    }

    public void Mostrar(TutorialPanelData panelData)
    {
        if (panelData == null) return;

        tituloText.text = panelData.titulo;
        descripcionText.text = panelData.descripcion;
        imagenUI.sprite = panelData.imagen;

        ActualizarProgreso();
    }

    public void btnPrev()
    {
        if (tutorialIndex <= 0) return;

        tutorialIndex--;
        Mostrar(data[tutorialIndex]);
        ActualizarBotones();
    }

    public void btnNext()
    {
        if (tutorialIndex >= data.Count - 1) return;

        tutorialIndex++;
        Mostrar(data[tutorialIndex]);
        ActualizarBotones();
    }

    private void ActualizarBotones()
    {
        prevButton.interactable = tutorialIndex > 0;
        nextButton.interactable = tutorialIndex < data.Count - 1;
    }

    private void ActualizarProgreso()
    {
        if (progresoText != null)
        {
            progresoText.text = $"{tutorialIndex + 1} / {data.Count}";
        }
    }
}
