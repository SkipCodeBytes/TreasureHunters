using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private GameManager _gm;
    private PlayerManager _pm;

    public List<string> EffectList = new List<string>();

    private void Start()
    {
        _gm = GameManager.Instance;
        _pm = GetComponent<PlayerManager>();

    }

    public void SetEffect(GameEffect gameEffect)
    {
        EffectList.Add(gameEffect.EffectName);
    }

    public void DropEffect(string effectName)
    {
        EffectList.Remove(effectName);
    }

}
