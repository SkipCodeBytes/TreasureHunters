using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemTypeToScriptMap
{
    private static readonly Dictionary<ItemType, Type> _map = new Dictionary<ItemType, Type>
    {
        { ItemType.Coin, typeof(CoinItemData) },
        { ItemType.Relic, typeof(RelicItemData) },
        { ItemType.Gem, typeof(GemItemData) },
        { ItemType.Card, typeof(CardItemData) }
    };


    //USO:  Type tipo = ItemTypeToScriptMap.GetItemDataType(ItemType.Gem);
    public static Type GetItemDataType(ItemType itemType)
    {
        if (_map.TryGetValue(itemType, out var type))
            return type;

        Debug.LogWarning($"No se encontró un tipo asociado para {itemType}");
        return null;
    }

    public static ItemType? GetItemTypeFromData(ItemData itemData)
    {
        foreach (var pair in _map)
        {
            if (pair.Value == itemData.GetType())
                return pair.Key;
        }

        Debug.LogWarning("Tipo de clase no registrado en el diccionario.");
        return null;
    }
}
