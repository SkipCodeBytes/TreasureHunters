
using UnityEngine;

public class ActionPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject panelInfo;

    [SerializeField] private GameObject useCardButton;

    public void OpenActionPanel()
    {
        actionPanel.SetActive(true);
        panelInfo.SetActive(false);
        useCardButton.SetActive(true);
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

    public void btnMovePlayer()
    {
        GameManager.Instance.DiceAction = PlayerDiceAction.Move;
        GameManager.Instance.GmView.RPC("OpenDiceForAction", GameManager.Instance.HostPlayer, GameManager.Instance.CurrentPlayerTurnIndex, (int)GameManager.Instance.DiceAction);
    }

    public void btnCardPlayer()
    {
        GameManager.Instance.GuiManager.CardPanelUI.gameObject.SetActive(true);
        GameManager.Instance.GuiManager.CardPanelUI.InitCardPanel(btnMovePlayer);
        useCardButton.SetActive(false);
        gameObject.SetActive(false);
    }

}
