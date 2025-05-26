using System.Collections.Generic;
using UnityEngine;

public class InstanceManager : MonoBehaviour
{
    public static InstanceManager Instance;
    [SerializeField] private List<GameObjectPool> _gamePool = new List<GameObjectPool>();

    void Awake(){
        Instance = this;
        foreach(GameObjectPool pool in _gamePool) pool.FillPool();
    }

    public GameObject GetObject(GameObject prefab){
        foreach (GameObjectPool pool in _gamePool){
            if(pool.Prefab == prefab){
                return pool.GetAvailableInstance();
            }
        }
        throw new System.Exception("Object not found in instance manager");
    }
}


