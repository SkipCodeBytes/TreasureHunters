using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    private GameManager _gm;

    private void Awake()
    {
        Instance = this;
    }

    public List<GameEffect> effects = new List<GameEffect>();

    private void Start()
    {
        _gm = GameManager.Instance;
        EventManager.StartListening("InitMoment", SkipTurn);
    }

    public void SkipTurn()
    {
        if (_gm.GameRound <= 1) return;
        if (_gm.MomentManager.CurrentMoment.MomentName != "CheckTurnStatus") return;
        if (!_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].PlayerEffects.EffectList.Contains("SkipTurn")) return;

        if (_gm.IsHostPlayer)
        {
            _gm.MomentManager.InterveneCurrentMoment(new Moment(_gm.GameMoments.SkipTurnEffect));
        }
        Debug.LogWarning("SkipTurnEffect");
    }



    public void InvokeMethodByName(string methodName)
    {
        MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method != null)
        {
            method.Invoke(this, null); // null si el método no tiene parámetros
        }
        else
        {
            Debug.LogWarning($"Método '{methodName}' no encontrado.");
        }
    }

    //() => InvokeMethodByName(effects[i].EffectName)


    /// <summary>
    /// Obtiene un efecto por nombre. Lanza excepción si no se encuentra.
    /// </summary>
    public GameEffect GetGameEffect(string effectName)
    {
        var effect = effects.FirstOrDefault(e => e != null && e.EffectName == effectName);
        if (effect == null)
            throw new ArgumentException($"No se encontró ningún GameEffect con el nombre '{effectName}'.");

        return effect;
    }

    /// <summary>
    /// Obtiene un efecto por índice. Lanza excepción si el índice es inválido.
    /// </summary>
    public GameEffect GetGameEffect(int effectId)
    {
        if (effectId < 0 || effectId >= effects.Count)
            throw new IndexOutOfRangeException($"El Effect ID '{effectId}' está fuera de rango.");

        return effects[effectId];
    }

    /// <summary>
    /// Obtiene el índice de un GameEffect. Lanza excepción si no está en la lista.
    /// </summary>
    public int GetEffectId(GameEffect effect)
    {
        int index = effects.IndexOf(effect);
        if (index == -1)
            throw new ArgumentException("El GameEffect proporcionado no está en la lista.");

        return index;
    }

    /// <summary>
    /// Obtiene el índice de un efecto por su nombre. Lanza excepción si no se encuentra.
    /// </summary>
    public int GetEffectId(string effectName)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i] != null && effects[i].EffectName == effectName)
                return i;
        }

        throw new ArgumentException($"No se encontró ningún GameEffect con el nombre '{effectName}'.");
    }
}
