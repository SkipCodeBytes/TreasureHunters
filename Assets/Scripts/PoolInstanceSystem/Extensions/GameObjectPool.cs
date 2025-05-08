using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectPool
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 0;
    [SerializeField] private bool isExpandible = false;
    private List<GameObject> _poolContent;
    private GameObject _pool;

    public GameObject Prefab { get => prefab;}

    public void FillPool()
    {
        _poolContent = new List<GameObject>();
        _pool = new GameObject(prefab.name + "Pool");
        _pool.transform.parent = InstanceManager.Instance.transform;
        for (int i = 0; i < poolSize; i++){
            GameObject obj = GameObject.Instantiate(prefab, _pool.transform);
            _poolContent.Add(obj);
            obj.SetActive(false);
        }
    }

    public GameObject GetAvailableInstance(){
        foreach (GameObject objInstance in _poolContent)
        {
            if (!objInstance.activeInHierarchy)
            {
                objInstance.SetActive(true);
                return objInstance;
            }
        }
        if (isExpandible)
        {
            GameObject obj = GameObject.Instantiate(prefab, _pool.transform);
            _poolContent.Add(obj);
            return obj;
        }
        return null;
    }
}
