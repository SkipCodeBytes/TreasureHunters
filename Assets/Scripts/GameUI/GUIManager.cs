using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TurnOrderUi turnOrderUi;
    [SerializeField] private ActionPanelUI playerActionPanel;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private RoundInfoUI roundInfoPanel;
    [SerializeField] private DicePanelUI dicePanelUI;
    [SerializeField] private List<PlayerSlotInfoUi> slotInfoUIList = new List<PlayerSlotInfoUi>();


    public GameObject PlayerInfoPanel { get => playerInfoPanel; set => playerInfoPanel = value; }
    public RoundInfoUI RoundInfoPanel { get => roundInfoPanel; set => roundInfoPanel = value; }
    public ActionPanelUI PlayerActionPanel { get => playerActionPanel; set => playerActionPanel = value; }
    public DicePanelUI DicePanelUI { get => dicePanelUI; set => dicePanelUI = value; }
    public List<PlayerSlotInfoUi> SlotInfoUIList { get => slotInfoUIList; set => slotInfoUIList = value; }
    public TurnOrderUi TurnOrderUi { get => turnOrderUi; set => turnOrderUi = value; }

    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    public void btnMovePlayer() => _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, 0);
    public void btnCardPlayer() => _gm.GmView.RPC("OpenCardPanel", _gm.HostPlayer);

}
