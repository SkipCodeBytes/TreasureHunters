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

    private MomentManager _momentManager;

    //Tendrá lógica del ciclo del juego  

    private void Awake()
    {
        _gm = GameManager.Instance;
        _momentManager = _gm.MomentManager;
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
        if (_momentManager.IsWaitingForSyncro)
        {
            CheckSyncro();
            return;
        }

        _momentManager.MomentUpdate();
    }



    //Sincronización de tareas
    private bool CheckSyncro()
    {
        if (!_momentManager.IsWaitingForSyncro) return true;
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
        _momentManager.IsWaitingForSyncro = false;
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
                _gm.BoardPlayers[i].View.RPC("SetPlayerInfo", RpcTarget.All, _gm.HomeTileList[i].Order.x, _gm.HomeTileList[i].Order.y);
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
        _momentManager.MomentList.Add(new GameMoment(NewGame));
        yield return new WaitForSeconds(waitTime);
        isGamePlaying = true;
    }

    //Muestra a todos los jugadores el orden de juego
    private void NewGame()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("PlayPresentationPanel", RpcTarget.All);
        _momentManager.MomentList.Add(new GameMoment(ShowPlayerInfoUI));
        _momentManager.MomentList.Add(new GameMoment(NewRound));
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
        _momentManager.IsWaitingForSyncro = true;
    }

    private void WaitForEvent()
    {
        _momentManager.IsWaitingForEvent = true;
    }




    //Llamada automática después de evento tras finalizar algún momento
    private void GenericMomentEnd() => _momentManager.IsMomentRunnning = false;
    private void GenericEndEvent() => _momentManager.IsWaitingForEvent = false;



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
            _momentManager.MomentList.Add(new GameMoment(NewTurn));
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
        _momentManager.MomentList.Add(new GameMoment(CheckTurnStatus));
    }


    private void CheckTurnStatus()
    {
        //SE REVISA LA DISPONIBILIDAD DEL PLAYER (Desmayado, retenido, bloqueado, etc)

        if (_gm.CurrentPlayerTurnIndex == -1)
        {
            _momentManager.MomentList.Add(new GameMoment(NewRound));
        } else
        {
            _momentManager.MomentList.Add(new GameMoment(OpenPlayerActionPanel));
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
        _momentManager.MomentList.Add(new GameMoment(OpenDicePanel));
        _momentManager.IsWaitingForEvent = false;
    }

    private void OpenDicePanel()
    {
        WaitForSyncro();
        WaitForEvent();
        _momentManager.MomentList.Add(new GameMoment(CloseDicePanel));
        _gm.GmView.RPC("OpenDicePanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex, _gm.DiceManager.DicesQuantityForAction[(int)_gm.DiceAction]);
    }

    private void CloseDicePanel()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("CloseDicePanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        /*
        switch (_gm.DiceAction)
        {
            case PlayerDiceAction.Move:
                //_momentManager.MomentList.Add(new GameMoment(MovePlayer));
                break;
        }*/

    }


    private void MovePlayer()
    {
        WaitForEvent();
        WaitForSyncro();
        //_gm.InitMoventPlayer();
        //Aquí el player realizará el movimiento
        //Debug.Log("El jugador" + _gm.BoardPlayers[_gm.CurrentPlayerTurnIndex].Player.NickName + ", se moverá " + _gm.DiceResult + " casillas");
    }
}
