using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameRPC : MonoBehaviourPunCallbacks
{/*
    [PunRPC]
    public void FirstSyncGameData(int playerId1, int playerId2, int playerId3, int playerId4)
    {
        BoardPlayer[] array = new BoardPlayer[4];
        for (int i = 0; i < 4; i++)
        {
            if (GameManager.Instance.BoardPlayers[i].Player == PhotonNetwork.CurrentRoom.GetPlayer(playerId1))
            {
                array[1] = GameManager.Instance.BoardPlayers[i];
                continue;
            }
            if (GameManager.Instance.BoardPlayers[i].Player == PhotonNetwork.CurrentRoom.GetPlayer(playerId2))
            {
                array[2] = GameManager.Instance.BoardPlayers[i];
                continue;
            }
            if (GameManager.Instance.BoardPlayers[i].Player == PhotonNetwork.CurrentRoom.GetPlayer(playerId3))
            {
                array[3] = GameManager.Instance.BoardPlayers[i];
                continue;
            }
            if (GameManager.Instance.BoardPlayers[i].Player == PhotonNetwork.CurrentRoom.GetPlayer(playerId4))
            {
                array[4] = GameManager.Instance.BoardPlayers[i];
                continue;
            }
        }
        
        //Orden de los jugadores
        //Num de cartas a disposición
        //Num de gemas a disposición
        //Salud de los jugadores
        //Monedas
    }*/


    [PunRPC]
    public void SyncGameData()
    {
        //Num de cartas a disposición
        //Num de gemas a disposición
        //Salud de los jugadores
        //Monedas
    }
}
