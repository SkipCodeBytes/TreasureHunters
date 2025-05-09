using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameRPC : MonoBehaviourPunCallbacks
{
    
    [PunRPC]
    public void FirstSyncGameData(int playerId1, int playerId2, int playerId3, int playerId4)
    {
        List<Player> playerOrder = new List<Player>();
        BoardPlayer[] array = new BoardPlayer[4];

        if (playerId1 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId1)); else playerOrder.Add(null);
        if (playerId2 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId2)); else playerOrder.Add(null);
        if (playerId3 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId3)); else playerOrder.Add(null);
        if (playerId4 != -1) playerOrder.Add(PhotonNetwork.CurrentRoom.GetPlayer(playerId4)); else playerOrder.Add(null);

        for (int i = 0; i < playerOrder.Count; i++) {
            for(int j = 0; j < array.Length; j++)
            {
                if (playerOrder[i] == null) continue;
                if (playerOrder[i] == GameManager.Instance.BoardPlayers[j].Player)
                {
                    array[i] = GameManager.Instance.BoardPlayers[j];
                    break;
                }
            }
        }

        GameManager.Instance.BoardPlayers = array;
    }


    [PunRPC]
    public void SyncGameData(int playerIndex, int life, int coins, int gems, int cards, int safeRelics, bool relic)
    {
        //Num de cartas a disposición
        //Num de gemas a disposición
        //Salud de los jugadores
        //Monedas

        /*
         HOST > PLAYER - CANVAS 1 Y 2

                Host Game Manager
                Guest Game Manager
         */
    }
}
