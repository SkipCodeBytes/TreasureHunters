using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    [SerializeField] private List<GameItem> registeredItems = new List<GameItem>();

    public static ItemManager Instance { get => _instance; }

    private void Awake()
    {
        if(_instance != null) _instance = this;
        else Destroy(gameObject);
    }



    public GameItem GetItem(int itemID)
    {
        if(itemID < registeredItems.Count)
        {
            return registeredItems[itemID];
        }
        Debug.Log("Objeto no registrado, ID:" + itemID);
        return null;
    }
}
