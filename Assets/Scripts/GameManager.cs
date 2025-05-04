
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private BoardPlayer[] playersArray = new BoardPlayer[4];
    [SerializeField] private GameObject playerActionPanel;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private CameramanScript cameraman;

    [Header("Game Config")]
    [SerializeField] private float waitToInitGame = 2f;
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 2f;


    [Header("Check Values")]
    [SerializeField] private bool isHostPlayer = false;
    [SerializeField] private BoardPlayer playerReference;
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int currentPlayerTurnIndex = -1;
    [SerializeField] private int diceResult = 0;
    [SerializeField] private List<TileBoard> homeTileList;
    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;
    [SerializeField] private GameMoment currentMoment;
    [SerializeField] private List<GameMoment> momentList;

    [Header("Debug and Test options")]
    [SerializeField] private bool stepMomentMode = false;

    public static GameManager Instance { get => _instance; }
    public GameBoardManager BoardManager { get => boardManager; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }


    void Start()
    {
        isHostPlayer = NetworkRoomsManager.IsMasterPlayer;
        playerReference = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity).GetComponent<BoardPlayer>();

        if (isHostPlayer)
        {
            EventManager.StartListening("EndMoment", MomentEnd);
            EventManager.StartListening("DiceManagerFinish", SetDiceResults);
            EventManager.StartListening("CameraFocusComplete", EndCameraFocus);
            EventManager.StartListening("EndPlayerMovent", EndMovePlayer);

            homeTileList = new List<TileBoard>();

            foreach (Vector2Int key in boardManager.TileDicc.Keys)
            {
                if (boardManager.TileDicc[key].Type == UnityGameBoard.Tiles.TileType.Home)
                {
                    homeTileList.Add(boardManager.TileDicc[key]);
                }
            }
            if (homeTileList.Count < 4)
            {
                Debug.LogError("No hay suficientes casas");
                return;
            }


            //Sorteamos el orden de los jugadores
            StartCoroutine(PrepareScene(waitToInitGame/2));
        }


    }
    private IEnumerator PrepareScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        /*
        Dictionary<int, Player> luckyDiccionary = new Dictionary<int, Player>();

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            int LuckyValue = (int)player.CustomProperties["luckyNumber"];
            while (true)
            {
                if (luckyDiccionary.ContainsKey(LuckyValue))
                {
                    LuckyValue = Random.Range(1, 100);
                }
                else
                {
                    luckyDiccionary[LuckyValue] = player;
                    break;
                }
            }
        }

        var order = luckyDiccionary.OrderBy(kv => kv.Key);
        int i = 0;
        foreach (var kv in order)
        {
            //playersArray[i]
            Debug.Log($"{kv.Key}: {kv.Value}");
        }
        */
        StartCoroutine(StartGame(waitToInitGame/2));
    }


    private IEnumerator StartGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Comienza el primer turno
        momentList.Insert(0, new GameMoment(NewRound));
    }




    void Update()
    {
        if (!isMomentRunnning && !isWaitingForEvent)
        {
            if (momentList.Count > 0)
            {
                if (stepMomentMode)
                {
                    if (Input.GetKeyDown(KeyCode.Q)) ReadNextMoment();
                } else
                {
                    ReadNextMoment();
                }
                
            }
            else
            {
                //Se detiene el ciclo completamente con un momento vacío
                momentList.Insert(0, new GameMoment(EndTurn));
                //Debug.LogWarning("Ciclo terminado - GAME OVER");
            }
        }
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
    private void MomentEnd() => isMomentRunnning = false;
    


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
        cameraman.FocusTarget(playersArray[currentPlayerTurnIndex].gameObject);
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
        playersArray[currentPlayerTurnIndex].MoveNextTile();
    }

    private void EndMovePlayer()
    {
        isWaitingForEvent = false;
    }



    //---------------- CICLO COMÚN ----------------

    //INICIO DEL CICLO

    private void NewRound()
    {
        gameRound++;
        //Comprobamos si hay jugadores suficientes en juego
        int activePlayersCount = 0;
        for (int i = 0; i < playersArray.Length; i++) {
            if (playersArray[i] != null) activePlayersCount++;
        }
        //AJUSTARLO LUEGO PARA GANAR LA PARTIDA EN CASO QUEDE UNO SOLO
        if (activePlayersCount > 0) momentList.Insert(0, new GameMoment(NewTurn));
        else Debug.LogError("No hay jugadores disponibles");
    }


    private void NewTurn()
    {
        currentPlayerTurnIndex++;
        if (currentPlayerTurnIndex < playersArray.Length)
        {
            //En caso el jugador haya salido del juego, saltamos el turno
            if (playersArray[currentPlayerTurnIndex] != null)
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