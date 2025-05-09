
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerActionPanel;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private TurnOrderUi turnOrderUi;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private CameramanScript cameraman;

    [Header("Game Config")]
    [SerializeField] private Vector3 playerSpawnPoint;
    [SerializeField] private float waitToInitGame = 2f;
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 2f;


    [Header("Check Values")]
    [SerializeField] private bool isHostPlayer = false;
    [SerializeField] private bool isGamePlaying = false;
    [SerializeField] private bool isPreparingScene = false;
    [SerializeField] private BoardPlayer playerReference;
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int diceResult = 0;

    [SerializeField] private BoardPlayer[] boardPlayers = new BoardPlayer[4];
    [SerializeField] private List<TileBoard> homeTileList;
    [SerializeField] private List<PlayerSlotInfoUi> slotInfoUIList;
    [SerializeField] private int currentPlayerTurnIndex = -1;

    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;
    [SerializeField] private GameMoment currentMoment;
    [SerializeField] private List<GameMoment> momentList;

    [Header("Debug and Test options")]
    [SerializeField] private bool stepMomentMode = false;


    public static GameManager Instance { get => _instance; }
    public GameBoardManager BoardManager { get => boardManager; }
    public BoardPlayer[] BoardPlayers { get => boardPlayers; set => boardPlayers = value; }
    public int CurrentPlayerTurnIndex { get => currentPlayerTurnIndex; set => currentPlayerTurnIndex = value; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }


    void Start()
    {
        isHostPlayer = NetworkRoomsManager.IsMasterPlayer;
        playerReference = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint, Quaternion.identity).GetComponent<BoardPlayer>();

        cameraman.FocusPanoramicView(true);
        cameraman.FocusTarget(playerReference.gameObject);

        if (isHostPlayer)
        {
            EventManager.StartListening("EndMoment", GenericMomentEnd);
            EventManager.StartListening("EndEvent", GenericEndEvent);
            EventManager.StartListening("DiceManagerFinish", SetDiceResults);
            EventManager.StartListening("CameraFocusComplete", EndCameraFocus);
            EventManager.StartListening("EndPlayerMovent", EndMovePlayer);

            homeTileList = boardManager.GetAllTileOfType(TileType.Home);

            if (homeTileList.Count < 4)
            {
                Debug.LogError("No hay suficientes casas");
                return;
            }

            ListExtensions.Shuffle(homeTileList);
        }
        
    }


    void Update()
    {
        if (isHostPlayer)
        {
            if (isGamePlaying)
            {
                PlayGameCycle();
            }
            else
            {
                PrepareScene();
            }
        }
    }

    private void PrepareScene()
    {
        if (!isPreparingScene)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (boardPlayers[i] == null) { return; }
            }

            Debug.Log("All players are Ready");
            isPreparingScene = true;

            boardPlayers.Shuffle();

            for (int i = 0; i < boardPlayers.Length; i++)
            {
                if (boardPlayers[i] == null) continue;
                boardPlayers[i].View.RPC("SetPlayerInfo", boardPlayers[i].Player, homeTileList[i].Order.x, homeTileList[i].Order.y);
                boardPlayers[i].CurrentTilePosition = homeTileList[i];
                boardPlayers[i].HomeTile = homeTileList[i];

                //boardPlayers[i].PlayerRules.Life = 0;
            }

            List<int> plyrID = new List<int>();

            for (int i = 0; i < boardPlayers.Length; i++) {
                if (boardPlayers[i] != null) plyrID.Add(boardPlayers[i].Player.ActorNumber);
                else plyrID.Add(-1);
            }

            photonView.RPC("FirstSyncGameData", RpcTarget.Others, plyrID[0], plyrID[1], plyrID[2], plyrID[3]);
            StartCoroutine(StartGame(waitToInitGame));
        }
    }

    private void PlayGameCycle()
    {
        if (!isMomentRunnning && !isWaitingForEvent)
        {
            if (momentList.Count > 0)
            {
                if (stepMomentMode)
                {
                    if (Input.GetKeyDown(KeyCode.Q)) ReadNextMoment();
                }
                else
                {
                    ReadNextMoment();
                }

            }
            else
            {
                //momentList.Insert(0, new GameMoment(EndTurn));
            }
        }
    }


    private IEnumerator StartGame(float waitTime)
    {
        momentList.Insert(0, new GameMoment(NewGame));
        yield return new WaitForSeconds(waitTime);
        isGamePlaying = true;
    }




    //---------------- OPERACIONES CON MOMENTOS ----------------

    //Brinda oportunidad de leer el siguiente momento
    private void ReadNextMoment()
    {
        if (momentList[0] != null)
        {
            currentMoment = momentList[0];
            momentList.RemoveAt(0);
            isMomentRunnning = true;
            currentMoment.PlayMoment();
        }
        else
        {
            momentList.RemoveAt(0);
        }
    }

    //Cancela un momento para ir por otro
    private void CancelMoment(GameMoment gameMoment)
    {
        isMomentRunnning = false;
        momentList.Insert(0, gameMoment);
        currentMoment.CancelMoment();
    }

    //Pospone el momento actual para ir por otro
    private void InterveneMoment(GameMoment gameMoment)
    {
        isMomentRunnning = false;
        momentList.Insert(0, currentMoment);
        momentList.Insert(0, gameMoment);
        currentMoment.CancelMoment();
    }

    //Llamada automática después de evento tras finalizar algún momento
    private void GenericMomentEnd() => isMomentRunnning = false; 
    


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

    private void GenericEndEvent() => isWaitingForEvent = false;

    //---------------- CICLO COMÚN ----------------

    private void NewGame()
    {
        isWaitingForEvent = true;
        turnOrderUi.StartPresentation();
        for (int i = 0; i < boardPlayers.Length; i++)
        {
            if (boardPlayers[i] == null) continue;
            boardPlayers[i].PlayerRules.SetDefaultValues();
        }
        momentList.Add(new GameMoment(ShowPlayerInfoUI));
        //momentList.Add(new GameMoment(NewRound));
    }

    private void ShowPlayerInfoUI()
    {
        slotInfoUIList = new List<PlayerSlotInfoUi>();
        for (int i = 0; i < playerInfoPanel.transform.childCount; i++) {
            PlayerSlotInfoUi plySlotInfo = playerInfoPanel.transform.GetChild(i).GetComponent<PlayerSlotInfoUi>();
            if (plySlotInfo == null) continue;
            plySlotInfo.StartChargingPlayerInfo();
            slotInfoUIList.Add(plySlotInfo);
        }
        playerInfoPanel.SetActive(true);
    }

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