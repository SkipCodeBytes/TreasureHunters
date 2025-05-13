using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityGameBoard.Tiles;

public class HostManager : MonoBehaviourPunCallbacks
{
    private GameManager _gm;

    [Header("Config Values")]
    [SerializeField] private float waitToInitGame = 2f;

    [Header("Check Values")]
    [SerializeField] private bool isCicleEnabled = true;
    [SerializeField] private bool isGamePlaying = false;
    [SerializeField] private bool isPreparingScene = false;

    [Header("Task Syncro")]
    [SerializeField] private bool[] syncronisedPlayers = new bool[4];

    [Header("Moment Systems")]
    [SerializeField] private GameMoment currentMoment;
    [SerializeField] private List<GameMoment> momentList;

    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;
    [SerializeField] private bool isWaitingForSyncro = false;

    [Header("Debug and Testing options")]
    [SerializeField] private bool stepMomentMode = false;


    //Tendrá lógica del ciclo del juego  

    private void Awake()
    {
        _gm = GameManager.Instance;
    }

    public void Init()
    {
        ConfigEventListeners();

        _gm.HomeTileList = _gm.BoardManager.GetAllTileOfType(TileType.Home);

        if (_gm.HomeTileList.Count < 4)
        {
            Debug.LogError("No hay suficientes casas");
            return;
        }

        ListExtensions.Shuffle(_gm.HomeTileList);
        isCicleEnabled = true;
    }

    private void Update()
    {
        if (!isCicleEnabled) return;

        if (isGamePlaying)
        {
            PlayGameCycle();
        }
        else
        {
            PrepareScene();
        }
    }

    private void PlayGameCycle()
    {
        if (isWaitingForEvent) return;
        if (isMomentRunnning) return;
        if (isWaitingForSyncro)
        {
            CheckSyncro();
            return;
        }

        if (momentList.Count > 0)
        {
            if (stepMomentMode) { if (Input.GetKeyDown(KeyCode.Q)) ReadNextMoment(); }
            else ReadNextMoment();
        }
        else
        {
            //momentList.Insert(0, new GameMoment(EndTurn));
        }
    }



    //Sincronización de tareas
    private bool CheckSyncro()
    {
        if (!isWaitingForSyncro) return true;
        for (int i = 0; i < syncronisedPlayers.Length; i++)
        {
            if (i == _gm.PlayerIndex)
            {
                if (!syncronisedPlayers[i]) syncronisedPlayers[i] = true;
                continue;
            }
            if (_gm.BoardPlayers[i] == null) continue;

            if (!syncronisedPlayers[i]) return false;
        }
        isWaitingForSyncro = false;
        return true;
    }

    public void SetSyncroPlayer(int playerIndex)
    {
        Debug.Log("Syncro Player" + playerIndex);
        syncronisedPlayers[playerIndex] = true;
    }




    //Espera la conexión de todos los jugadores y establece aleatoriamente el orden de los turnos
    private void PrepareScene()
    {
        if (!isPreparingScene)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) if (_gm.BoardPlayers[i] == null) { return; }
            Debug.Log("All players are Ready");
            isPreparingScene = true;

            _gm.BoardPlayers.Shuffle();

            //Establecemos el hogar de cada jugador
            for (int i = 0; i < _gm.BoardPlayers.Length; i++)
            {
                if (_gm.BoardPlayers[i] == null) continue;
                _gm.BoardPlayers[i].View.RPC("SetPlayerInfo", _gm.BoardPlayers[i].Player, _gm.HomeTileList[i].Order.x, _gm.HomeTileList[i].Order.y);
                _gm.BoardPlayers[i].CurrentTilePosition = _gm.HomeTileList[i];
                _gm.BoardPlayers[i].HomeTile = _gm.HomeTileList[i];
            }

            List<int> plyrID = new List<int>();

            for (int i = 0; i < _gm.BoardPlayers.Length; i++)
            {
                if (_gm.BoardPlayers[i] != null) plyrID.Add(_gm.BoardPlayers[i].Player.ActorNumber);
                else plyrID.Add(-1);
            }

            //Sincronizamos los turnos elegidos
            _gm.GmView.RPC("FirstSyncGameData", RpcTarget.Others, plyrID[0], plyrID[1], plyrID[2], plyrID[3]);

            _gm.GeneratePlayerIndex();

            StartCoroutine(StartingGame(waitToInitGame));
        }
    }

    //Damos unos segundos de tiempo antes de iniciar
    private IEnumerator StartingGame(float waitTime)
    {
        momentList.Insert(0, new GameMoment(NewGame));
        yield return new WaitForSeconds(waitTime);
        isGamePlaying = true;
    }

    //Muestra a todos los jugadores el orden de juego
    private void NewGame()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("PlayPresentationPanel", RpcTarget.All);
        momentList.Add(new GameMoment(ShowPlayerInfoUI));
        momentList.Add(new GameMoment(NewRound));
    }

    //Solo activa el panel de información de los jugadores
    private void ShowPlayerInfoUI()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("ShowPlayerInfoUI", RpcTarget.All);
        //También se acomoda la cámara, se espera que termine ese evento.
    }





    //************************** OPERACIONES DE CICLO *************************************

    private void ConfigEventListeners()
    {
        EventManager.StartListening("EndMoment", GenericMomentEnd);
        EventManager.StartListening("EndEvent", GenericEndEvent);
    }

    private void WaitForSyncro()
    {
        for (int i = 0; i < syncronisedPlayers.Length; i++)
        {
            syncronisedPlayers[i] = false;
        }
        isWaitingForSyncro = true;
    }

    private void WaitForEvent()
    {
        isWaitingForEvent = true;
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
        else momentList.RemoveAt(0);
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
    private void GenericEndEvent() => isWaitingForEvent = false;



    //---------------- CICLO COMÚN ----------------

    //PREPARANDO INICIO DEL JUEGO

    //INICIO DEL CICLO
    private void NewRound()
    {
        WaitForEvent();
        WaitForSyncro();

        //Comprobamos si hay jugadores suficientes en juego
        int activePlayersCount = 0;
        for (int i = 0; i < _gm.BoardPlayers.Length; i++)
        {
            if (_gm.BoardPlayers[i] != null) activePlayersCount++;
        }

        //AJUSTARLO LUEGO PARA GANAR LA PARTIDA EN CASO QUEDE UNO SOLO
        if (activePlayersCount > 0) 
        {
            _gm.GmView.RPC("NewRound", RpcTarget.All);
            momentList.Add(new GameMoment(NewTurn));
            /*
            _gm.RoundInfoPanel.SetActive(true);
            _gm.RoundInfoPanel.Star*/
        }
        else Debug.LogError("No hay jugadores disponibles");
    }


    private void NewTurn()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("NewTurn", RpcTarget.All);
        momentList.Add(new GameMoment(CheckTurnStatus));
    }


    private void CheckTurnStatus()
    {
        //SE REVISA LA DISPONIBILIDAD DEL PLAYER (Desmayado, retenido, bloqueado, etc)

        if (_gm.CurrentPlayerTurnIndex == -1)
        {
            momentList.Add(new GameMoment(NewRound));
        } else
        {
            momentList.Add(new GameMoment(OpenPlayerActionPanel));
        }

    }

    private void OpenPlayerActionPanel()
    {
        WaitForEvent();
        //WaitForSyncro();
        _gm.GmView.RPC("OpenPlayerActionPanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        //momentList.Add(new GameMoment(CheckTurnStatus));

        //HABILITAR SOLO LAS ACCIONES QUE TIENE DISPONIBLES REALIZAR
        //playerActionPanel.SetActive(true);
        //isWaitingForEvent = true;
    }

    public void Ply_UseCardAction()
    {

    }

    public void Ply_ThrowDicesAction()
    {
        momentList.Add(new GameMoment(OpenDicePanel));
        isWaitingForEvent = false;
    }

    private void OpenDicePanel()
    {
        WaitForSyncro();
        WaitForEvent();
        switch (_gm.DiceAction)
        {
            case PlayerDiceAction.Move:
                momentList.Add(new GameMoment(MovePlayer));
                break;
        }
        
        _gm.GmView.RPC("OpenDicePanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex, _gm.DiceManager.DicesQuantityForAction[(int)_gm.DiceAction]);
    }

    private void MovePlayer()
    {
        Debug.Log("El jugador" + _gm.BoardPlayers[_gm.CurrentPlayerTurnIndex].Player.NickName + ", se moverá " + _gm.DiceResult + " casillas");
    }
}
