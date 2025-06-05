using System.Collections.Generic;
using UnityEngine;
using UnityGameBoard.Tiles;

public class ShorcutRoadTile : TileBehavior
{
    [SerializeField] private PortalScript portalScript;

    private GameManager _gm;

    protected override void Start()
    {
        base.Start();
        _gm = GameManager.Instance;
    }
    public override void StartTileEvent()
    {
        StartCoroutine(CinematicAnimation.WaitTime(1f, () => EventManager.TriggerEvent("EndEvent")));
        //SettingTileEvent();
    }
    public override void SettingTileEvent()
    {
        List<TileBoard> shorcutTiles = _gm.BoardManager.GetAllTileOfType(TileType.ShortcutRoad);
        shorcutTiles.Remove(_tileBoard);
        TileBoard selectedTile = shorcutTiles[Random.Range(0, shorcutTiles.Count)];
    }

    public override void PlayTileEvent()
    {
        //throw new System.NotImplementedException();
    }
}
