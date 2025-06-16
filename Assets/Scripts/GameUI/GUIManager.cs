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
    [SerializeField] private ShopPanelGUI shopPanelGUI;
    [SerializeField] private BattlePanelGui battlePanelGui;
    [SerializeField] private RevivePanelUI revivePanelUI;
    [SerializeField] private RuinPanelUI ruinPanelUI;

    public GameObject PlayerInfoPanel { get => playerInfoPanel; set => playerInfoPanel = value; }
    public RoundInfoUI RoundInfoPanel { get => roundInfoPanel; set => roundInfoPanel = value; }
    public ActionPanelUI PlayerActionPanel { get => playerActionPanel; set => playerActionPanel = value; }
    public DicePanelUI DicePanelUI { get => dicePanelUI; set => dicePanelUI = value; }
    public List<PlayerSlotInfoUi> SlotInfoUIList { get => slotInfoUIList; set => slotInfoUIList = value; }
    public TurnOrderUi TurnOrderUi { get => turnOrderUi; set => turnOrderUi = value; }
    public ShopPanelGUI ShopPanelGUI { get => shopPanelGUI; set => shopPanelGUI = value; }
    public BattlePanelGui BattlePanelGui { get => battlePanelGui; set => battlePanelGui = value; }
    public RevivePanelUI RevivePanelUI { get => revivePanelUI; set => revivePanelUI = value; }
    public RuinPanelUI RuinPanelUI { get => ruinPanelUI; set => ruinPanelUI = value; }

    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    public void btnMovePlayer()
    {
        _gm.DiceAction = PlayerDiceAction.Move;
        _gm.GmView.RPC("OpenDiceForAction", _gm.HostPlayer, _gm.CurrentPlayerTurnIndex, (int)_gm.DiceAction);
    }
    public void btnCardPlayer() => _gm.GmView.RPC("OpenCardPanel", _gm.HostPlayer);

}
