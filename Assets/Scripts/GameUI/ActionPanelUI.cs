using UnityEngine;

public class ActionPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject panelInfo;

    public void OpenActionPanel()
    {
        actionPanel.SetActive(true);
        panelInfo.SetActive(false);

    }

    public void OpenInfoPanel() 
    {
        actionPanel.SetActive(false);
        panelInfo.SetActive(true);
    }

    public void CloseAll()
    {
        actionPanel.SetActive(false);
        panelInfo.SetActive(false);
    }
}
