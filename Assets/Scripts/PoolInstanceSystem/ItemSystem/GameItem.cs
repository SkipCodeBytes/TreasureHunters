using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameItem
{
    [SerializeField] private ItemType itemType;

    //[DynamicComponentRequirement("itemType")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Sprite icon;

    //COIN
    //RELIC


    //CARD
    //[SerializeField] private Color cardColorBase; //Config General
    [SerializeField] private CardData cardData;

    //GEM
    [SerializeField] private GemForm gemForm;
    [SerializeField] private GemColor gemColor;

    public ItemType ItemType { get => itemType; set => itemType = value; }
    public GameObject ItemPrefab { get => itemPrefab; set => itemPrefab = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public CardData CardData { get => cardData; set => cardData = value; }
    public GemForm GemForm { get => gemForm; set => gemForm = value; }
    public GemColor GemColor { get => gemColor; set => gemColor = value; }
}
