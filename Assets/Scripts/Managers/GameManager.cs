using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [Header("Game References")]
    [SerializeField] private GameBoardManager boardManager;
    [SerializeField] private List<BoardPlayer> playersList;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject dicePanel;
    [SerializeField] private GameObject cardPanel;


    [Header("Game Config")]
    [SerializeField] private float timeLimitPerTurn = 20f;
    [SerializeField] private float gameSpeed = 1f;


    [Header("Check Values")]
    [SerializeField] private int gameRound = 0;
    [SerializeField] private int currentPlayerTurnIndex = -1;
    [SerializeField] private List<GameMoment> momentList;
    [SerializeField] private bool isMomentRunnning = false;


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
        momentList.Add(new GameMoment(NewRound));
    }

    void Update()
    {
        if (!isMomentRunnning)
        {
            if (momentList.Count > 0)
            {
                if (momentList[0] != null)
                {
                    momentList[0].PlayMoment();
                }
                else
                {
                    momentList.RemoveAt(0);
                }
            }
            else
            {
                Debug.LogError("Ciclo terminado - GAME OVER");
                isMomentRunnning = true; //Con esta línea se detiene el ciclo completamente
            }
        }
    }

    public void MomentDestroy(GameMoment gMoment)
    {
        momentList.Remove(gMoment);
        isMomentRunnning = false;
    }

    private void NewRound()
    {
        gameRound++;
        momentList.Add(new GameMoment(NewTurn));
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
                //Añadimos un momento para 
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
        //Se realiza el check de disponibilidad del player
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

    public void BtnOpenDicePanel()
    {
        dicePanel.SetActive(true);
    }

}


