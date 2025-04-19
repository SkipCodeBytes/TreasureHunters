
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private List<BoardPlayer> playersList;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Game Config")]
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 1f;


    [Header("Check Values")]
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int currentPlayerTurnIndex = -1;
    [SerializeField] private int diceResult = 0;
    [SerializeField] private List<GameMoment> momentList;
    [SerializeField] private GameMoment currentMoment;
    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;

    public static GameManager Instance { get => _instance; }
    public GameBoardManager BoardManager { get => boardManager; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }


    void Start()
    {
        //Esperamos unos segundos
        //Sorteamos el orden de los jugadores
        //Comienza el primer turno
        momentList.Insert(0, new GameMoment(NewRound));
    }

    void Update()
    {
        if (!isMomentRunnning && !isWaitingForEvent)
        {
            if (momentList.Count > 0)
            {
                ReadNextMoment();
            }
            else
            {
                Debug.LogError("Ciclo terminado - GAME OVER");
                isMomentRunnning = true; //Con esta línea se detiene el ciclo completamente
            }
        }
    }

    private void ReadNextMoment()
    {
        if (momentList[0] != null)
        {
            currentMoment = momentList[0];
            momentList.RemoveAt(0);
            isMomentRunnning = true;
            EventManager.StartListening("E_" + currentMoment.MomentName, MomentEnd);
            currentMoment.PlayMoment();
        }
        else
        {
            momentList.RemoveAt(0);
        }
    }

    public void MomentEnd()
    {
        EventManager.StopListening("E_" + currentMoment.MomentName, MomentEnd);
        isMomentRunnning = false;
    }

    //Cancela un momento para ir por otro
    public void CancelMoment(GameMoment gameMoment)
    {
        EventManager.StopListening("E_" + currentMoment.MomentName, MomentEnd);
        isMomentRunnning = false;
        momentList.Insert(0, gameMoment);
    }

    //Pospone el momento actual para ir por otro
    public void InterveneMoment(GameMoment gameMoment)
    {
        EventManager.StopListening("E_" + currentMoment.MomentName, MomentEnd);
        isMomentRunnning = false;
        momentList.Insert(0, currentMoment);
        momentList.Insert(0, gameMoment);
    }






    //MOMENTO DE USO DE DADOS

    public void OpenDicePanel()
    {
        diceResult = 0;
        diceManager.UseDice(2);
        EventManager.StartListening("DiceManagerFinish", setDiceResults);
        isWaitingForEvent = true;
    }

    public void setDiceResults()
    {
        diceResult = diceManager.ResultValue;
        isWaitingForEvent = false;
    }




    private void NewRound()
    {
        gameRound++;
        //Comprobamos si hay jugadores suficientes en juego
        int activePlayersCount = 0;
        for (int i = 0; i < playersList.Count; i++) {
            if (playersList[i] != null) activePlayersCount++;
        }
        //AJUSTARLO LUEGO PARA GANAR LA PARTIDA EN CASO QUEDE UNO SOLO
        if (activePlayersCount > 0) momentList.Add(new GameMoment(NewTurn));
        else Debug.LogError("No hay jugadores disponibles");
    }

    private void NewTurn()
    {
        currentPlayerTurnIndex++;
        if (currentPlayerTurnIndex < playersList.Count)
        {
            //En caso el jugador haya salido del juego, saltamos el turno
            if (playersList[currentPlayerTurnIndex] != null)
            {
                //Enfocamos la cámara del juego
                //cinemachineCamera.Follow = playersList[currentPlayerTurnIndex].transform;
                //cinemachineCamera.LookAt = playersList[currentPlayerTurnIndex].transform;

                momentList.Add(new GameMoment(PlayerCheckStatus));
            }
            else
            {
                momentList.Add(new GameMoment(NewTurn));
            }
        }
        else
        {
            currentPlayerTurnIndex = -1;
            momentList.Add(new GameMoment(NewRound));
        }
    }

    private void PlayerCheckStatus()
    {
        //Se realiza el check de disponibilidad del player (Desmayado, retenido, bloqueado, etc)
        momentList.Add(new GameMoment(OpenPlayerPanel));
    }

    private void OpenPlayerPanel()
    {
        playerPanel.SetActive(true);
    }


    public void BtnOpenCardPanel()
    {
        cardPanel.SetActive(true);
    }

    public void BtnMovePlayer()
    {
        OpenDicePanel();
        momentList.Add(new GameMoment(MovePlayer));
    }

    public void MovePlayer()
    {
        playersList[currentPlayerTurnIndex].MovePlayer(diceResult);
        diceResult = 0;
    }
    

}


