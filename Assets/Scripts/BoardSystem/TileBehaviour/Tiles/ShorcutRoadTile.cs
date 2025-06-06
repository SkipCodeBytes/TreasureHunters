using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;

public class ShorcutRoadTile : TileBehavior
{
    [SerializeField] private PortalScript portalScript;

    public static Vector2Int currentTileOrder;
    public static Vector2Int nextTileOrder;

    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }
    public override void StartTileEvent()
    {
        SettingTileEvent();
    }
    public override void SettingTileEvent()
    {
        currentTileOrder = _tileBoard.Order;
        List<TileBoard> shorcutTiles = _gm.BoardManager.GetAllTileOfType(TileType.ShortcutRoad);
        shorcutTiles.Remove(_tileBoard);
        TileBoard selectedTile = shorcutTiles[Random.Range(0, shorcutTiles.Count)];
        nextTileOrder = selectedTile.Order;

        _gm.GmView.RPC("SyncroPortalEffect", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex, selectedTile.Order.x, selectedTile.Order.y);
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.MovePlayerAtPoint(transform.position, false, Teleport);

        /*
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.MovePlayerAtPoint(transform.position, false,
            () => {
                transform.position = selectedTile.transform.position;
                StartCoroutine(CinematicAnimation.WaitTime(0.5f, () => SoundController.Instance.PlaySound(_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].SelectedCharacter.surprisedAudio)));
                StartCoroutine(CinematicAnimation.WaitTime(1.5f, () => EventManager.TriggerEvent("EndEvent")));
                }
        );*/
    }

    
    public override void PlayTileEvent()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].BoardPlayer.CurrentTilePosition = _gm.BoardManager.TileDicc[nextTileOrder];
        _gm.BoardManager.TileDicc[nextTileOrder].TileBehavior.UnhideProps();
    }


    private void Teleport()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Transform.position = _gm.BoardManager.TileDicc[nextTileOrder].transform.position;
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.MovePlayerAtPoint(
            _gm.BoardManager.TileDicc[nextTileOrder].TileBehavior.GetInteractionPosition(), false, EndTurn);
    }

    private void EndTurn()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.ClearAnimationStatus();
        EventManager.TriggerEvent("EndEvent");
        _gm.GmView.RPC("SyncroEndPortal", Photon.Pun.RpcTarget.All, _gm.CurrentPlayerTurnIndex);
    }

    public override void HideProps()
    {
        //base.HideProps();
        portalScript.DestroyPortal();
    }

}
