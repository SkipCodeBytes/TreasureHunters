using UnityEngine;
using UnityEngine.UI;

public class RevivePanelUI : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private Text reviveInfo;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject infoPanel;

    public void StartPanel()
    {
        _gm = GameManager.Instance;
        reviveInfo.text = "Para revivir,\n obtén " + _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Rules.ReviveValue +" o  más";
    }

    public void btnDiceToRevive()
    {
        _gm.DiceAction = PlayerDiceAction.Revive;
        _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.CurrentPlayerTurnIndex, (int)_gm.DiceAction);
        gameObject.SetActive(false);
    }

    public void OpenActionPanel()
    {
        actionPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    public void OpenInfoPanel()
    {
        actionPanel.SetActive(false);
        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);    
    }
}
