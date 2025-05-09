using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    [SerializeField] private List<ItemData> registeredItems = new List<ItemData>();

    public static ItemManager Instance { get => _instance; }

    private void Awake()
    {
        if(_instance != null) _instance = this;
        else Destroy(gameObject);
    }

    //Función para consultar ID de item y retornar
    public ItemData GetItemData(int itemID)
    {
        if(itemID < registeredItems.Count) {
            if (registeredItems[itemID] == null)
            {
                Debug.LogError("Objeto Null, ID:" + itemID);
                return null;
            }
            return registeredItems[itemID]; 
        }
        Debug.LogError("Objeto no registrado, ID:" + itemID);
        return null;
    }

    //Función para consultar itemDeOrigen y retornarID
    public int GetItemID(ItemData itemData) {
        if (registeredItems.Contains(itemData))
        {
            return registeredItems.IndexOf(itemData);
        }
        Debug.LogError("Objeto no registrado:" + itemData);
        return -1;
    }

    //Función para generar item en escena del juego
    public ItemObject GenerateItemInScene(int itemID)
    {
        ItemData selectedItem = GetItemData(itemID);
        ItemObject itemObject = null;

        if (selectedItem != null) {
            itemObject = InstanceManager.Instance.GetObject(selectedItem.ItemObjectPrefab.gameObject).GetComponent<ItemObject>();
            itemObject.SetItemObjectValues(selectedItem);
        }

        return itemObject;
    }
}
