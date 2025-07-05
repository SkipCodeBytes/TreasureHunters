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
    [SerializeField] private GameObject waitIfoUI;
    [SerializeField] private WinPanelGUI winPanelGUI;
    [SerializeField] private CardPanelUI cardPanelUI;
    [SerializeField] private CardViewUI cardViewUI;

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
    public GameObject WaitIfoUI { get => waitIfoUI; set => waitIfoUI = value; }
    public WinPanelGUI WinPanelGUI { get => winPanelGUI; set => winPanelGUI = value; }
    public CardPanelUI CardPanelUI { get => cardPanelUI; set => cardPanelUI = value; }
    public CardViewUI CardViewUI { get => cardViewUI; set => cardViewUI = value; }
}
