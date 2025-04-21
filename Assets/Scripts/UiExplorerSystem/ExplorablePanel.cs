using UnityEngine;

public class ExplorablePanel : MonoBehaviour
{
    [SerializeField] private ExplorablePanel previusPanel;
    [SerializeField] private ExplorablePanel nextPanel;

    public void CloseAndReturn()
    {
        previusPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void NextPanel()
    {
        nextPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OpenPanel(ExplorablePanel panel)
    {
        panel.gameObject.SetActive(true);
    }

    public void ClosePanel(ExplorablePanel panel)
    {
        panel.gameObject.SetActive(false);
    }

    public void CloseCurrentPanel()
    {
        gameObject.SetActive(false);
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // Cierra el ejecutable
#endif
    }
}
