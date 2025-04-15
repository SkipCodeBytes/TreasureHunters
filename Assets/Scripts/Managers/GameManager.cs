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


    [Header("Game Config")]
    [SerializeField] private float timeLimitPerTurn = 20f;


    [Header("Check Values")]
    [SerializeField] private int gameRound = 0;
    [SerializeField] private BoardPlayer currentPlayerTurn;
    [SerializeField] private List<GameMoment> momentList;
    


    public static GameManager Instance { get => _instance; }
    public GameBoardManager BoardManager { get => boardManager; }

    private void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(this);
    }


    private void Start()
    {
        //Esperamos unos segundos
        //Sorteamos el orden de los jugadores
        //Comienza el primer turno
    }


    private void IniciarRonda() {
        //Lanzar Evento
    }

    private void NuevoTurno() { }

    private void SaltarTurno() { }



}


