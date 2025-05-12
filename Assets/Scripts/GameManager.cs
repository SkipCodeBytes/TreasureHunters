
using Photon.Pun;
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
    [SerializeField] private GameObject playerActionPanel;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private RoundInfoUI roundInfoPanel;

    [Header("Game Config")]
    [SerializeField] private Vector3 playerSpawnPoint;
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 2f;


    [Header("Check Values")]

    [SerializeField] private int playerIndex = 0;
    [SerializeField] private bool isHostPlayer = false;
    [SerializeField] private BoardPlayer playerReference;
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int diceResult = 0;

    [SerializeField] private BoardPlayer[] boardPlayers = new BoardPlayer[4];
    [SerializeField] private List<TileBoard> homeTileList = new List<TileBoard>();
    [SerializeField] private List<PlayerSlotInfoUi> slotInfoUIList = new List<PlayerSlotInfoUi>();
    [SerializeField] private int currentPlayerTurnIndex = -1;

    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;
    [SerializeField] private GameMoment currentMoment;
    [SerializeField] private List<GameMoment> momentList;



    private GameRPC _gameRPC;
    private PhotonView _gmView;
    private HostManager _hostManager;
    private GuestManager _guestManager;


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

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);

        _gameRPC = GetComponent<GameRPC>();
        _gmView = GetComponent<PhotonView>();
        _hostManager = GetComponent<HostManager>();
        _guestManager = GetComponent<GuestManager>();
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

        EventManager.StartListening("DiceManagerFinish", SetDiceResults);
        EventManager.StartListening("CameraFocusComplete", EndCameraFocus);
        EventManager.StartListening("EndPlayerMovent", EndMovePlayer);

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

































    /*
    public void playerSyncData(int playerIndex)
    {
        _gmView.RPC("SyncPlayerData", RpcTarget.Others, i,
                BoardPlayers[playerIndex].PlayerRules.Life,
                BoardPlayers[playerIndex].PlayerInventory.CoinsQuantity,
                BoardPlayers[playerIndex].PlayerInventory.GemItems.Count,
                BoardPlayers[playerIndex].PlayerInventory.CardItems.Count,
                BoardPlayers[playerIndex].PlayerInventory.SafeRelicsQuantity,
                BoardPlayers[playerIndex].PlayerInventory.HasRelicItem);
    }*/



    //---------------- MOMENTOS GENERALES ----------------

    //MOMENTO DE USO DE DADOS
    public void OpenDicePanel()
    {
        diceResult = 0;
        diceManager.UseDice(2);
        isWaitingForEvent = true;
    }

    public void SetDiceResults()
    {
        diceResult = diceManager.ResultValue;
        isWaitingForEvent = false;
    }


    //MOMENTO DE ENFOQUE DE CÁMARA
    private void CameraFocusPlayer()
    {
        cameraman.FocusTarget(boardPlayers[currentPlayerTurnIndex].gameObject);
        isWaitingForEvent = true;
    }

    private void EndCameraFocus()
    {
        isWaitingForEvent = false;
    }


    //MOMENTO DE MOVIMIENTO DE JUGADOR
    public void SetPlayerNumberMoves()
    {
        for(int i = 0; i < diceResult; i++)
        {
            momentList.Insert(0, new GameMoment(MovePlayerNextTile));
        }
        diceResult = 0;
    }

    private void MovePlayerNextTile()
    {
        isWaitingForEvent = true;
        boardPlayers[currentPlayerTurnIndex].MoveNextTile();
    }

    private void EndMovePlayer()
    {
        isWaitingForEvent = false;
    }


    //---------------- CICLO COMÚN ----------------

    //Preparando el juego

    //INICIO DEL CICLO
    private void NewRound()
    {
        gameRound++;
        //Comprobamos si hay jugadores suficientes en juego
        int activePlayersCount = 0;
        for (int i = 0; i < boardPlayers.Length; i++) {
            if (boardPlayers[i] != null) activePlayersCount++;
        }
        //AJUSTARLO LUEGO PARA GANAR LA PARTIDA EN CASO QUEDE UNO SOLO
        if (activePlayersCount > 0) momentList.Insert(0, new GameMoment(NewTurn));
        else Debug.LogError("No hay jugadores disponibles");
    }


    private void NewTurn()
    {
        currentPlayerTurnIndex++;
        if (currentPlayerTurnIndex < boardPlayers.Length)
        {
            //En caso el jugador haya salido del juego, saltamos el turno
            if (boardPlayers[currentPlayerTurnIndex] != null)
            {
                momentList.Insert(0, new GameMoment(PlayerCheckStatus));
                momentList.Insert(0, new GameMoment(CameraFocusPlayer));
            }
            else
            {
                momentList.Insert(0, new GameMoment(NewTurn));
            }
        }
        else
        {
            currentPlayerTurnIndex = -1;
            momentList.Insert(0, new GameMoment(NewRound));
        }
    }






    private void PlayerCheckStatus()
    {
        //SE REVISA LA DISPONIBILIDAD DEL PLAYER (Desmayado, retenido, bloqueado, etc)
        momentList.Insert(0, new GameMoment(OpenPlayerActionPanel));
    }

    private void OpenPlayerActionPanel()
    {
        //HABILITAR SOLO LAS ACCIONES QUE TIENE DISPONIBLES REALIZAR
        playerActionPanel.SetActive(true);
        isWaitingForEvent = true;
    }

    public void BtnOpenCardPanel()
    {
        playerActionPanel.SetActive(false);
        cardPanel.SetActive(true);
    }

    public void BtnMovePlayer()
    {
        OpenDicePanel();
        playerActionPanel.SetActive(false);
        EventManager.StartListening("DiceManagerFinish", SetPlayerNumberMoves, 1);
    }

    private void EndTurn()
    {
        momentList.Clear();
        momentList.Insert(0, new GameMoment(NewTurn));
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