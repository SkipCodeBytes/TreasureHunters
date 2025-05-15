
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private CameramanScript cameraman;

    [Header("UI References")]
    [SerializeField] private TurnOrderUi turnOrderUi;
    [SerializeField] private ActionPanelUI playerActionPanel;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private RoundInfoUI roundInfoPanel;
    [SerializeField] private DicePanelUI dicePanelUI;


    [Header("Game Config")]
    [SerializeField] private Vector3 playerSpawnPoint;
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 2f;



    [Header("Check Values")]

    [SerializeField] private int playerIndex = 0;
    [SerializeField] private PlayerDiceAction diceAction;
    [SerializeField] private bool isHostPlayer = false;
    [SerializeField] private BoardPlayer playerReference;
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int diceResult = 0;

    [SerializeField] private BoardPlayer[] boardPlayers = new BoardPlayer[4];
    [SerializeField] private List<TileBoard> homeTileList = new List<TileBoard>();
    [SerializeField] private List<PlayerSlotInfoUi> slotInfoUIList = new List<PlayerSlotInfoUi>();
    [SerializeField] private int currentPlayerTurnIndex = -1;


    [Header("Moment Systems")]
    [SerializeField] private GameMoment localCurrentMoment;
    [SerializeField] private List<GameMoment> localMomentList;


    private GameRPC _gameRPC;
    private PhotonView _gmView;
    private HostManager _hostManager;
    private GuestManager _guestManager;
    private MomentManager _momentManager;

    private Player _hostPlayer;

    public static GameManager Instance { get => _instance; }
    public GameBoardManager BoardManager { get => boardManager; }
    public BoardPlayer[] BoardPlayers { get => boardPlayers; set => boardPlayers = value; }
    public int CurrentPlayerTurnIndex { get => currentPlayerTurnIndex; set => currentPlayerTurnIndex = value; }


    public PhotonView GmView { get => _gmView; set => _gmView = value; }
    public List<TileBoard> HomeTileList { get => homeTileList; set => homeTileList = value; }
    public GameObject PlayerInfoPanel { get => playerInfoPanel; set => playerInfoPanel = value; }
    public List<PlayerSlotInfoUi> SlotInfoUIList { get => slotInfoUIList; set => slotInfoUIList = value; }
    public TurnOrderUi TurnOrderUi { get => turnOrderUi; set => turnOrderUi = value; }
    public GameRPC GameRPC { get => _gameRPC; set => _gameRPC = value; }
    public int PlayerIndex { get => playerIndex; set => playerIndex = value; }
    public HostManager HostManager { get => _hostManager; set => _hostManager = value; }
    public GuestManager GuestManager { get => _guestManager; set => _guestManager = value; }
    public BoardPlayer PlayerReference { get => playerReference; set => playerReference = value; }
    public CameramanScript Cameraman { get => cameraman; set => cameraman = value; }
    public int GameRound { get => gameRound; set => gameRound = value; }



    public RoundInfoUI RoundInfoPanel { get => roundInfoPanel; set => roundInfoPanel = value; }
    public ActionPanelUI PlayerActionPanel { get => playerActionPanel; set => playerActionPanel = value; }
    public DiceManager DiceManager { get => diceManager; set => diceManager = value; }
    public DicePanelUI DicePanelUI { get => dicePanelUI; set => dicePanelUI = value; }
    public int DiceResult { get => diceResult; set => diceResult = value; }
    public Player HostPlayer { get => _hostPlayer; set => _hostPlayer = value; }
    public PlayerDiceAction DiceAction { get => diceAction; set => diceAction = value; }
    public MomentManager MomentManager { get => _momentManager; set => _momentManager = value; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);

        _gameRPC = GetComponent<GameRPC>();
        _gmView = GetComponent<PhotonView>();
        _hostManager = GetComponent<HostManager>();
        _guestManager = GetComponent<GuestManager>();
        _momentManager = GetComponent<MomentManager>();

        _hostPlayer = NetworkRoomsManager.HostPlayer;
    }


    void Start()
    {
        isHostPlayer = NetworkRoomsManager.IsMasterPlayer;
        playerReference = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint, Quaternion.identity).GetComponent<BoardPlayer>();

        cameraman.FocusPanoramicView(true);
        cameraman.FocusTarget(playerReference.gameObject);

        if (isHostPlayer) { 
            _guestManager.enabled = false;
            _hostManager.Init();
        }
        else{ 
            _hostManager.enabled = false;
            _guestManager.Init();
        }

        SetListener();
    }



    public void GeneratePlayerIndex()
    {
        for (int i = 0; i < boardPlayers.Length; i++)
        {
            if (boardPlayers[i] == playerReference)
            {
                playerIndex = i;
            }
        }
    }

    public void SetListener()
    {
        EventManager.StartListening("EndPlayerMovent", EndMovePlayer);
    }








    public void btnMovePlayer()
    {
        GmView.RPC("btnOpenDice", HostPlayer, 0);
    }
    public void btnCardPlayer()
    {
        GmView.RPC("btnOpenCard", HostPlayer);
    }


    public void SendDiceResults(int result)
    {
        GmView.RPC("SentDiceResults", RpcTarget.All, result);
        //diceResult = diceManager.ResultValue;
    }




    //---------------- MOMENTOS GENERALES ----------------

    //MOMENTO DE MOVIMIENTO DE JUGADOR

    public void InitMoventPlayer()
    {
        boardPlayers[currentPlayerTurnIndex].View.RPC("SyncroLeaveRestSpace", RpcTarget.All, currentPlayerTurnIndex);
        for (int i = 0; i < diceResult; i++)
        {
            if(i == diceResult - 1)
            {
                _momentManager.MomentList.Add(new GameMoment(MovePlayerLastTile));
            } else
            {
                _momentManager.MomentList.Add(new GameMoment(MovePlayerNextTile));
            }
        }
    }

    private void MovePlayerNextTile()
    {
        _momentManager.IsWaitingForEvent = true;
        boardPlayers[currentPlayerTurnIndex].MoveNextTile();
    }

    private void MovePlayerLastTile()
    {
        _momentManager.IsWaitingForEvent = true;
        boardPlayers[currentPlayerTurnIndex].MoveLastTile();
    }

    private void EndMovePlayer()
    {
        _momentManager.IsWaitingForEvent = false;
    }









    //MOMENTO DE MOVIMIENTO DE JUGADOR


























    //---------------- CICLO COMÚN ----------------

    //Preparando el juego

    //INICIO DEL CICLO



    private void PlayerCheckStatus()
    {
        //momentList.Insert(0, new GameMoment(OpenPlayerActionPanel));
    }

    private void OpenPlayerActionPanel()
    {
        //HABILITAR SOLO LAS ACCIONES QUE TIENE DISPONIBLES REALIZAR
        //playerActionPanel.SetActive(true);
        //isWaitingForEvent = true;
    }

    private void EndTurn()
    {
        //momentList.Clear();
        //momentList.Insert(0, new GameMoment(NewTurn));
    }
}




/*
 Para intervenciones
EventManager.StartListening("InitMoment", InterventionTest);


    private void InterventionTest()
    {
        if(currentMoment.MomentName == "PlayerCheckStatus")
        {
            Debug.Log("SE HA INTERVENIDO");
            InterveneMoment(new GameMoment(MovePlayer));
        }
    }


 */