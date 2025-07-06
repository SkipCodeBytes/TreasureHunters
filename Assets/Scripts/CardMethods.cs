using System.Linq;
using UnityEngine;

public static class CardMethods
{
    public static int CasterPlayerIndex = -1;

    [Invocable]
    public static void SinEfecto()
    {
        Debug.LogWarning("Esta Carta no tiene efectos");
        GameManager _gm = GameManager.Instance;
        _gm.StartCoroutine(CinematicAnimation.WaitTime(0.20f, () => _gm.GuiManager.CardPanelUI.closeCallback?.Invoke()));
    }

    [Invocable]
    public static void CurarUnPunto()
    {
        Debug.Log("EFECTO CURAR");
        GameManager _gm = GameManager.Instance;
        _gm.PlayersArray[CasterPlayerIndex].Rules.AddLife(1);
        _gm.PlayersArray[CasterPlayerIndex].Graphics.HealingParticle.Play();
        _gm.GuiManager.SlotInfoUIList[CasterPlayerIndex].SetPlayerInfo();
        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            _gm.StartCoroutine(CinematicAnimation.WaitTime(1f, () => _gm.GuiManager.CardPanelUI.closeCallback?.Invoke()));
        }
    }

    [Invocable]
    public static void TeletransporteCasillaAleatoria()
    {
        Debug.Log("EFECTO TELETRANSPORTE");
        GameManager _gm = GameManager.Instance;
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Shoot"));
        _gm.PlayersArray[CasterPlayerIndex].Graphics.ContinousSmoke.Play();
        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            Vector2Int randomOrder = _gm.BoardManager.TileDicc.Keys.ElementAt(Random.Range(0, _gm.BoardManager.TileDicc.Count));
            _gm.PlayersArray[CasterPlayerIndex].View.RPC("SetPlayerTilePosition", Photon.Pun.RpcTarget.All, randomOrder.x, randomOrder.y);
            _gm.StartCoroutine(CinematicAnimation.WaitTime(1f, () => _gm.GuiManager.CardPanelUI.closeCallback?.Invoke()));
        }
    }

    [Invocable]
    public static void SaltarTurno()
    {
        Debug.Log("EFECTO SALTAR TURNO");
        GameManager _gm = GameManager.Instance;
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("UseTramp"));
        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            _gm.GmView.RPC("AddPlayerEffect", Photon.Pun.RpcTarget.All, CasterPlayerIndex, EffectManager.Instance.GetEffectId("SkipTurn"));
            _gm.StartCoroutine(CinematicAnimation.WaitTime(0.5f, () => EventManager.TriggerEvent("EndEvent")));
        }
    }

    [Invocable]
    public static void SaltarEventoDeCasilla()
    {
        Debug.Log("EFECTO SALTAR EVENTO CASILLA");
        GameManager _gm = GameManager.Instance;
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("UseTramp"));

        _gm.PlayersArray[CasterPlayerIndex].BoardPlayer.CurrentTilePosition.TileBehavior.HideProps();

        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            _gm.MomentManager.MomentList.Clear();
            _gm.StartCoroutine(CinematicAnimation.WaitTime(0.12f, () => _gm.MomentManager.IsWaitingForEvent = false));
        }
    }


    [Invocable]
    public static void SiguienteCasilla()
    {
        Debug.Log("EFECTO SIGUIENTE CASILLA");
        GameManager _gm = GameManager.Instance;
        _gm.PlayersArray[CasterPlayerIndex].BoardPlayer.CurrentTilePosition.TileBehavior.HideProps();
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("UseTramp"));
        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            _gm.MomentManager.MomentList.Clear();
            _gm.MomentManager.MomentList.Add(new Moment(_gm.GameMoments.MovePlayerLastTile));
            _gm.MomentManager.MomentList.Add(new Moment(_gm.GameMoments.CheckTrampCards));
            _gm.MomentManager.MomentList.Add(new Moment(_gm.GameMoments.OpenTileEvent));

            _gm.StartCoroutine(CinematicAnimation.WaitTime(0.12f, () => _gm.MomentManager.IsWaitingForEvent = false));
        }
    }

    [Invocable]
    public static void AtaqueMejorado() 
    {
        Debug.Log("ATAQUE MEJORADO");
        GameManager _gm = GameManager.Instance;
        _gm.PlayersArray[CasterPlayerIndex].Rules.AttackStatMod = 2;
        _gm.GuiManager.BattlePanelGui.ResetInfoValues();

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("ActiveCard"));
    }

    [Invocable]
    public static void DefensaMejorada()
    {
        Debug.Log("DEFENSA MEJORADO");
        GameManager _gm = GameManager.Instance;
        _gm.PlayersArray[CasterPlayerIndex].Rules.DefenseStatMod = 2;
        _gm.GuiManager.BattlePanelGui.ResetInfoValues();

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("ActiveCard"));
    }

    [Invocable]
    public static void EvasiónMejorada()
    {
        Debug.Log("EVASION MEJORADO");
        GameManager _gm = GameManager.Instance;
        _gm.PlayersArray[CasterPlayerIndex].Rules.EvasionStatMod = 2;
        _gm.GuiManager.BattlePanelGui.ResetInfoValues();

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("ActiveCard"));
    }

    [Invocable]
    public static void DadoMovimientoAumentado()
    {
        Debug.Log("MOVIMIENTO MEJORADO");
        GameManager _gm = GameManager.Instance;
        if (_gm.IsHostPlayer) _gm.GameRules.AdicionalDice = 1;

        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("ActiveCard"));

        if (CasterPlayerIndex == _gm.PlayerIndex)
        {
            _gm.StartCoroutine(CinematicAnimation.WaitTime(1f, () => _gm.GuiManager.CardPanelUI.closeCallback?.Invoke()));
        }
    }
}
