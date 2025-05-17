using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    [SerializeField, HideInInspector] List<int> dicesQuantityForAction = new List<int>();
    public List<int> DicesQuantityForAction { get => dicesQuantityForAction; set => dicesQuantityForAction = value; }

    void Update()
    {
        
    }
}
