using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Realtime;
using UnityEngine;

public class PlayerRPC : MonoBehaviourPunCallbacks
{
    private GameManager _gm;
    private PlayerManager _pm;

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
        _gm = GameManager.Instance;
    }

    //------------- RPC DE INICIO, NO VOLVER A LLAMAR ---------------

    //PlayerManager.Start() / RpcTarget.All
    [PunRPC]
    public void SharePlayerReady(Player player)
    {
        for (int i = 0; i < _gm.PlayersArray.Length; i++)
        {
            if (_gm.PlayersArray[i] == null)
            {
                _gm.PlayersArray[i] = _pm;
                break;
            }
        }
        _pm.Player = player;
        _pm.SelectedCharacter = _pm.Rules.FindCharacterData(player);

        if (_pm.SelectedCharacter == null)
        {
            Debug.LogError("Personaje no registrado");
            return;
        }
        else
        {
            _pm.Graphics.GeneratePlayerModel();
            _pm.Rules.Life = _pm.SelectedCharacter.lifeStat;
        }
        Debug.Log("Jugador " + player.NickName + " listo");
    }



    //HostManager.PrepareScene() / RpcTarget.All
    [PunRPC]
    public void SetPlayerHomeTile(int tileOrderX, int tileOrderY)
    {
        TileBoard tile = _gm.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];
        SetPlayerTilePosition(tileOrderX, tileOrderY);
        _pm.BoardPlayer.HomeTile = tile;
    }

    //CardMethod.EfectoNinja() / All
    [PunRPC]
    public void SetPlayerTilePosition(int tileOrderX, int tileOrderY)
    {
        TileBoard tile = _gm.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];
        _pm.BoardPlayer.SetTilePosition(tile);
    }


    //------------- RPC DE SINCRONIZACIÓN, LLAMAR SOLO CUANDO HAY CAMBIOS ---------------

    //PlayerGrapics.GenerateAnimStatus() / Others
    [PunRPC] 
    public void SyncroAnimStatus(int newAnimStatus) => _pm.Graphics.AnimStatus = newAnimStatus;


    //BoardPlayer.MoveLastTile() / All
    [PunRPC]
    public void SyncroEnterInTile(int tileOrderX, int tileOrderY)
    {
        TileBoard tile = GameManager.Instance.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];
        _pm.BoardPlayer.PreviusTilePosition = _pm.BoardPlayer.CurrentTilePosition;
        _pm.BoardPlayer.CurrentTilePosition = tile;
        tile.TileBehavior.UnhideProps();
    }

    //GameMoments.InitMoventPlayer() / All
    [PunRPC]
    public void SyncroLeaveRestSpace(int playerIndex)
    {
        _pm.BoardPlayer.CurrentTilePosition.TileBehavior.LeaveFreeSpace(_gm.PlayersArray[playerIndex].BoardPlayer);
    }

    //PlayerGraphics.GoToRestInCurrentTile() / Others
    [PunRPC]
    public void SyncroEnterRestSpace(int playerIndex, int restPosIndex)
    {
        Debug.Log("Player sincro enter space " + playerIndex);
        //_boardPlayer.SetPlayerInfo(playerIndex, restPosIndex);
        if (_gm.PlayersArray[playerIndex] == null) return;
        _pm.BoardPlayer.CurrentTilePosition.TileBehavior.SetSpace(_gm.PlayersArray[playerIndex].BoardPlayer, restPosIndex);
    }






    //[PunRPC] private void SyncroCurrentTilePos(int tileOrderX, int tileOrderY) => _pm.BoardPlayer.CurrentTilePosition = _gm.BoardManager.TileDicc[new Vector2Int(tileOrderX, tileOrderY)];




    /* NO ELIMINAR------------------------------
     * 
     * 
     * 



    /*
    //Esto lo llama el GameManager
    [PunRPC]
    public void OpenCurrentTileEvent()
    {
        //_boardPlayer.CurrentTilePosition.BehaviorScript.UnhideProps();
        _boardPlayer.CurrentTilePosition.BehaviorScript.StartTileEvent();
    }*/

}
