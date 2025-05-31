using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    [SerializeField] private List<ItemData> registeredItems = new List<ItemData>();

    //Esta lista separa los diferentes tipos de items
    //Dentro de esta lista están una lista de los IDs de la lista principal

    private Dictionary<Type, List<int>> _itemTypeIdMap = new Dictionary<Type, List<int>>();

    public static ItemManager Instance { get => _instance; }
    public List<ItemData> RegisteredItems { get => registeredItems; set => registeredItems = value; }

    private void Awake()
    {
        if(_instance == null) _instance = this;
        else { 
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _itemTypeIdMap = new Dictionary<Type, List<int>>();

        for (int i = 0; i < registeredItems.Count; i++)
        {
            var item = registeredItems[i];
            if (item == null) continue;

            Type itemType = item.GetType();
            if (!_itemTypeIdMap.ContainsKey(itemType))
            {
                _itemTypeIdMap[itemType] = new List<int>();
            }

            _itemTypeIdMap[itemType].Add(i);
        }
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

    public int GetRandomItemIndexOfType<T>() where T : ItemData
    {
        Type type = typeof(T);
        if (!_itemTypeIdMap.TryGetValue(type, out var list) || list.Count == 0)
        {
            Debug.LogWarning($"No hay items registrados del tipo {type.Name}");
            return -1;
        }

        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public ItemType GetItemType(int itemId)
    {
        ItemData data = GetItemData(itemId);
        if (data == null)
        {
            Debug.LogWarning($"Item no encontrado con ID {itemId}");
            return default;
        }

        ItemType? resolvedType = ItemTypeToScriptMap.GetItemTypeFromData(data);

        if (resolvedType.HasValue)
            return resolvedType.Value;

        Debug.LogWarning($"No se pudo determinar el tipo del item con ID {itemId}");
        return default;
    }
}
