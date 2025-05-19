using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityGameBoard.Tiles;

public class HostManager : MonoBehaviour
{
    private GameManager _gm;

    [Header("Game Config")]
    [SerializeField] private float waitToInitGame = 2f;

    [Header("Check Values - ReadOnly")]
    [SerializeField] private bool isCicleEnabled = false;
    [SerializeField] private bool isGamePlaying = false;
    [SerializeField] private bool isPreparingScene = false;
    [SerializeField] private bool[] syncronisedPlayers = new bool[4];

    private MomentManager _momentManager;

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

        if (!isGamePlaying)
        {
            PrepareScene();
        }
        else
        {
            PlayGameCycle();
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
            if (_gm.PlayersArray[i] == null) continue;

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





    //********************************************************************************************************************//
    //*******************************************  ACCIONES INICIO DE CICLO  *********************************************//
    //********************************************************************************************************************//

    //Espera la conexión de todos los jugadores y establece aleatoriamente el orden de los turnos
    private void PrepareScene()
    {
        if (!isPreparingScene)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) if (_gm.PlayersArray[i] == null) { return; }

            Debug.Log("All players are Ready");
            isPreparingScene = true;

            _gm.PlayersArray.Shuffle();

            //Establecemos el hogar de cada jugador
            for (int i = 0; i < _gm.PlayersArray.Length; i++)
            {
                if (_gm.PlayersArray[i] == null) { continue; }
                _gm.PlayersArray[i].View.RPC("SetPlayerHomeTile", RpcTarget.All, _gm.HomeTileList[i].Order.x, _gm.HomeTileList[i].Order.y);
            }

            for (int i = 0; i < _gm.PlayersArray.Length; i++)
            {
                if (_gm.PlayersArray[i] != null) _gm.PlayersPhotonId.Add(_gm.PlayersArray[i].Player.ActorNumber);
                else _gm.PlayersPhotonId.Add(-1);
            }

            //Sincronizamos los turnos elegidos
            _gm.GmView.RPC("FirstSyncGameData", RpcTarget.Others, _gm.PlayersPhotonId[0], _gm.PlayersPhotonId[1], _gm.PlayersPhotonId[2], _gm.PlayersPhotonId[3]);
            _gm.GeneratePlayerIndex();

            StartCoroutine(StartingGame(waitToInitGame));
        }
    }

    //Damos unos segundos de tiempo antes de iniciar
    private IEnumerator StartingGame(float waitTime)
    {
        _momentManager.MomentList.Add(new Moment(NewGame));
        yield return new WaitForSeconds(waitTime);
        isGamePlaying = true;
    }



    //Muestra a todos los jugadores el orden de juego
    private void NewGame()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("PlayPresentationPanel", RpcTarget.All);
        _momentManager.MomentList.Add(new Moment(ShowPlayerInfoUI));
        _momentManager.MomentList.Add(new Moment(NewRound));
    }

    //Solo activa el panel de información de los jugadores
    private void ShowPlayerInfoUI()
    {
        WaitForEvent();
        WaitForSyncro();
        //También se acomoda la cámara, se espera que termine ese evento.
        _gm.GmView.RPC("ShowPlayerInfoUI", RpcTarget.All);
    }
    




    //Llamada automática después de evento tras finalizar algún momento
    private void GenericMomentEnd() => _momentManager.IsMomentRunnning = false;
    private void GenericEndEvent() => _momentManager.IsWaitingForEvent = false;














    //********************************************************************************************************************//
    //*******************************************  ACCIONES FUERA DE CICLO  **********************************************//
    //********************************************************************************************************************//

    //Es necesario configurar la acción que llevará acabo
    //Ejemplo: _gm.DiceAction = (PlayerDiceAction)actionDice;
    //Esta función se llama a través de public void GameRPC.btnOpenDice(int actionDice)
    public void StartDicePanel()
    {
        _momentManager.MomentList.Add(new Moment(OpenDicePanel));
        GenericEndEvent();
    }

    private void OpenDicePanel()
    {
        WaitForSyncro();
        WaitForEvent();
        _momentManager.MomentList.Add(new Moment(CloseDicePanel));
        _gm.GmView.RPC("OpenDicePanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex, _gm.GameRules.GetDiceQuantityUse((int)_gm.DiceAction));
    }

    private void CloseDicePanel()
    {
        WaitForEvent();
        WaitForSyncro();
        _gm.GmView.RPC("CloseDicePanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
    }











    //********************************************************************************************************************//
    //**********************************************  ACCIONES DE CICLO  *************************************************//
    //********************************************************************************************************************//


    private void NewRound()
    {
        WaitForEvent();
        WaitForSyncro();

        //Comprobamos si hay jugadores suficientes en juego
        int activePlayersCount = 0;
        for (int i = 0; i < _gm.PlayersArray.Length; i++)
        {
            if (_gm.PlayersArray[i] != null) activePlayersCount++;
        }

        //AJUSTARLO LUEGO PARA GANAR LA PARTIDA EN CASO QUEDE UNO SOLO
        if (activePlayersCount > 0)
        {
            _gm.GmView.RPC("NewRound", RpcTarget.All);
            _momentManager.MomentList.Add(new Moment(NewTurn));

            //_gm.RoundInfoPanel.SetActive(true);
            //_gm.RoundInfoPanel.Star
        }
        else Debug.LogError("No hay jugadores disponibles");
    }


    public void NewTurn()
    {
        WaitForEvent();
        WaitForSyncro();
        while (true)
        {
            _gm.CurrentPlayerTurnIndex++;
            if (_gm.CurrentPlayerTurnIndex >= _gm.PlayersArray.Length) 
            { 
                _gm.CurrentPlayerTurnIndex = -1;
                _momentManager.MomentList.Add(new Moment(NewRound));
                GenericEndEvent();
                return;
            }
            if (_gm.PlayersArray[_gm.CurrentPlayerTurnIndex] != null) break;
        }
        _gm.GmView.RPC("NewTurn", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
        _momentManager.MomentList.Add(new Moment(CheckTurnStatus));
    }


    private void CheckTurnStatus()
    {
        //SE REVISA LA DISPONIBILIDAD DEL PLAYER (Desmayado, retenido, bloqueado, etc)
        _momentManager.MomentList.Add(new Moment(OpenPlayerActionPanel));
        /*
        if (_gm.CurrentPlayerTurnIndex == -1) { _momentManager.MomentList.Add(new Moment(NewRound)); }
        else { _momentManager.MomentList.Add(new Moment(OpenPlayerActionPanel)); }*/
    }

    private void OpenPlayerActionPanel()
    {
        //HABILITAR SOLO LAS ACCIONES QUE TIENE DISPONIBLES REALIZAR
        WaitForEvent();
        _gm.GmView.RPC("OpenPlayerActionPanel", RpcTarget.All, _gm.CurrentPlayerTurnIndex);
    }










    //---------------- CICLO COMÚN ----------------

    //PREPARANDO INICIO DEL JUEGO

    //INICIO DEL CICLO

    /*


    public void Ply_UseCardAction()
    {

    }


    private void MovePlayer()
    {
        WaitForEvent();
        WaitForSyncro();
        //_gm.InitMoventPlayer();
        //Aquí el player realizará el movimiento
        //Debug.Log("El jugador" + _gm.BoardPlayers[_gm.CurrentPlayerTurnIndex].Player.NickName + ", se moverá " + _gm.DiceResult + " casillas");
    }*/
}
