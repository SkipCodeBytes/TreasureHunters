
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GUIManager guiManager;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private SoundLibrary soundLibrary;

    [SerializeField] private BoardCameraController cameraController;
    [SerializeField] private GlobalLightScript globalLight;



    [Header("Game Config")]
    [SerializeField] private Vector3 playerSpawnPoint;


    [Header("Player Values - ReadOnly")]
    [SerializeField] private bool isHostPlayer = false;
    [SerializeField] private int playerIndex = 0;
    [SerializeField] private PlayerManager playerReference;

    [Header("Game Values - ReadOnly")]
    [SerializeField] private Player hostPlayer;
    [SerializeField] private PlayerManager[] playersArray = new PlayerManager[4];
    [SerializeField] private List<TileBoard> homeTileList = new List<TileBoard>();
    [SerializeField] private List<int> playersPhotonId = new List<int>();
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int currentDiceOwnerIndex;
    [SerializeField] private int lastDiceResult = 0;
    [SerializeField] private int[] lastRewards;
    [SerializeField] private int currentPlayerTurnIndex = -1;
    [SerializeField] private int secondaryPlayerTurn = -1;
    [SerializeField] private PlayerDiceAction diceAction;

    [Header("Battle System - ReadOnly")]
    [SerializeField] private bool reverseBattle = false;
    [SerializeField] private bool isEvadeAction = false;
    [SerializeField] private int ofensivePlayerValue = 0;
    [SerializeField] private int defensivePlayerValue = 0;

    //[SerializeField] public Dictionary<Vector2, List<ItemObject>> tileReward;


    //Componentes de GameManager
    private GameRPC _gameRPC;
    private PhotonView _gmView;
    private HostManager _hostManager;
    private GuestManager _guestManager;
    private MomentManager _momentManager;
    private GameMoments _gameMoments;
    private GameRules _gameRules;

    public static GameManager Instance { get => _instance; }

    public GameBoardManager BoardManager { get => boardManager; }
    public DiceManager DiceManager { get => diceManager; set => diceManager = value; }
    public GameRPC GameRPC { get => _gameRPC; set => _gameRPC = value; }
    public BoardCameraController CameraController { get => cameraController; set => cameraController = value; }
    public GameMoments GameMoments { get => _gameMoments; set => _gameMoments = value; }



    public PhotonView GmView { get => _gmView; set => _gmView = value; }
    public MomentManager MomentManager { get => _momentManager; set => _momentManager = value; }
    public HostManager HostManager { get => _hostManager; set => _hostManager = value; }
    public GuestManager GuestManager { get => _guestManager; set => _guestManager = value; }
    public GUIManager GuiManager { get => guiManager; set => guiManager = value; }
    public GameRules GameRules { get => _gameRules; set => _gameRules = value; }


    public bool IsHostPlayer { get => isHostPlayer; set => isHostPlayer = value; }
    public PlayerManager PlayerReference { get => playerReference; set => playerReference = value; }
    public int PlayerIndex { get => playerIndex; set => playerIndex = value; }


    public int GameRound { get => gameRound; set => gameRound = value; }
    public Player HostPlayer { get => hostPlayer; set => hostPlayer = value; }
    public List<TileBoard> HomeTileList { get => homeTileList; set => homeTileList = value; }
    public int CurrentPlayerTurnIndex { get => currentPlayerTurnIndex; set => currentPlayerTurnIndex = value; }
    public PlayerManager[] PlayersArray { get => playersArray; set => playersArray = value; }
    public PlayerDiceAction DiceAction { get => diceAction; set => diceAction = value; }
    public int LastDiceResult { get => lastDiceResult; set => lastDiceResult = value; }
    public List<int> PlayersPhotonId { get => playersPhotonId; set => playersPhotonId = value; }
    public int[] LastRewards { get => lastRewards; set => lastRewards = value; }
    public GlobalLightScript GlobalLight { get => globalLight; set => globalLight = value; }
    public int SecondaryPlayerTurn { get => secondaryPlayerTurn; set => secondaryPlayerTurn = value; }
    public bool ReverseBattle { get => reverseBattle; set => reverseBattle = value; }
    public int OfensivePlayerValue { get => ofensivePlayerValue; set => ofensivePlayerValue = value; }
    public int DefensivePlayerValue { get => defensivePlayerValue; set => defensivePlayerValue = value; }
    public bool IsEvadeAction { get => isEvadeAction; set => isEvadeAction = value; }
    public int CurrentDiceOwnerIndex { get => currentDiceOwnerIndex; set => currentDiceOwnerIndex = value; }
    public MusicManager MusicManager { get => musicManager; set => musicManager = value; }
    public SoundLibrary SoundLibrary { get => soundLibrary; set => soundLibrary = value; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);

        _gameRPC = GetComponent<GameRPC>();
        _gmView = GetComponent<PhotonView>();
        _hostManager = GetComponent<HostManager>();
        _guestManager = GetComponent<GuestManager>();
        _momentManager = GetComponent<MomentManager>();
        _gameMoments = GetComponent<GameMoments>();
        _gameRules = GetComponent<GameRules>();

        isHostPlayer = NetworkRoomsManager.IsMasterPlayer;
        hostPlayer = NetworkRoomsManager.HostPlayer;
    }


    void Start()
    {
        playerReference = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint, Quaternion.identity).GetComponent<PlayerManager>();
        cameraController.FocusTarget(playerReference.gameObject);

        if (isHostPlayer) { 
            _guestManager.enabled = false;
            _hostManager.Init();
        }
        else{ 
            _hostManager.enabled = false;
            _guestManager.Init();
        }
    }



    public void GeneratePlayerIndex()
    {
        for (int i = 0; i < playersArray.Length; i++)
        {
            if (playersArray[i] == playerReference)
            {
                playerIndex = i;
            }
        }
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