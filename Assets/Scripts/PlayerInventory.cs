using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{

    [SerializeField] private Vector3 itemSpawnPos;

    public float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;
    [SerializeField] private float itemDropHeight;

    [SerializeField] private int coinsQuantity;
    [SerializeField] private List<GemItemData> gemItems;
    [SerializeField] private List<CardItemData> cardItems;
    [SerializeField] private RelicItemData relicItemData;
    [SerializeField] private GameObject relicVisual;

    public bool availableToReceiveRelic = false;


    public int CoinsQuantity { get => coinsQuantity; set => coinsQuantity = value; }
    public List<GemItemData> GemItems { get => gemItems; set => gemItems = value; }
    public List<CardItemData> CardItems { get => cardItems; set => cardItems = value; }
    public RelicItemData RelicItemData { get => relicItemData; set => relicItemData = value; }

    private GameManager _gm;
    private ItemManager _im;
    private PlayerManager _pm;

    private void Awake()
    {
        _gm = GameManager.Instance;
        _im = ItemManager.Instance;
        _pm = GetComponent<PlayerManager>();
    }



    public void AddCoins(int quantity)
    {
        coinsQuantity += quantity;
    }

    public void AddItem(int itemId)
    {
        ItemType itemType = _im.GetItemType(itemId);
        switch (itemType) {
            case ItemType.Gem:
                AddGem(itemId);
                break;

            case ItemType.Card:
                AddCard(itemId);
                break;

            case ItemType.Relic:
                AddRelic(itemId);
                break;

            default:
                Debug.LogError("ItemID: " + itemId + " no obtenible por inventario");
                break;
        }
    }

    public void AddGem(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        ItemData data = _im.GetItemData(itemId);
        GemItemData gem = data as GemItemData;

        if (gem != null) gemItems.Add(gem);
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
        /*
        if (itemId == -1) return;
        gemItems.Add((GemItemData)_im.GetItemData(itemId));*/
    }

    public void AddCard(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        ItemData data = _im.GetItemData(itemId);
        CardItemData card = data as CardItemData;

        if (card != null) cardItems.Add(card);
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
    }

    public void AddRelic(int itemId)
    {
        if (itemId == -1 || itemId == 0) return;
        if (relicItemData != null) return; 

        ItemData data = _im.GetItemData(itemId);
        RelicItemData relic = data as RelicItemData;


        if (relic != null) { 
            relicItemData = relic;
            StartCoroutine(CinematicAnimation.WaitTime(1.8f, () => relicVisual.SetActive(true)));
        }
        else Debug.LogError($"[AddGem] El item con ID {itemId} no es de tipo GemItemData. Tipo real: {data?.GetType().Name}");
    }


    public int[] CalculateRandomDrop()
    {
        List<int> dropItemsID = new List<int>();

        dropItemsID.Add(Mathf.CeilToInt(coinsQuantity / 3));

        if (relicItemData != null) dropItemsID.Add(ItemManager.Instance.GetItemID(relicItemData));

        int dropGemsQuantity = Mathf.CeilToInt(gemItems.Count / 3);
        List<GemItemData> aleatorios = gemItems.GetRandomElements(dropGemsQuantity);

        for(int i = 0; i < aleatorios.Count; i++)
        {
            dropItemsID.Add(ItemManager.Instance.GetItemID(aleatorios[i]));
        }

        return dropItemsID.ToArray();
    }


    public ItemObject[] DropObjects(int[] dropObjects)
    {
        List<ItemObject> _rewardObjs = new List<ItemObject>();

        for (int i = 0; i < dropObjects.Length; i++)
        {
            if (i == 0)
            {
                coinsQuantity -= dropObjects[0];
                for (int j = 0; j < dropObjects[0]; j++)
                {
                    _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(0));
                }
            }
            else
            {
                if (dropObjects[i] == 0) continue;

                _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(dropObjects[i]));
                ItemType itemType = ItemManager.Instance.GetItemType(dropObjects[i]);

                switch (itemType)
                {
                    case ItemType.Relic:
                        relicItemData = null;
                        relicVisual.SetActive(false);
                        break;

                    case ItemType.Gem:
                        gemItems.Remove(ItemManager.Instance.GetItemData(dropObjects[i]) as GemItemData);
                        break;

                    default:
                        Debug.LogError("Item no dropeable: ID" + dropObjects[i]);
                        break;
                }
            }
        }

        //Animación
        for (int i = 0; i < _rewardObjs.Count; i++)
        {
            _rewardObjs[i].DropAnimation(_pm.Transform.position + itemSpawnPos, _pm.Transform.position, itemDropHeight, itemDropMaxRadio, itemTimeDrop);
        }

        //_gm.GuiManager.SlotInfoUIList[playerIndex].SetPlayerInfo();

        return _rewardObjs.ToArray();
    }


    public void SaveRelic()
    {
        if (relicItemData != null)
        {
            relicItemData = null;
            relicVisual.SetActive(false);

            _pm.Rules.AddGameStar();
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + itemSpawnPos, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDropMaxRadio);
    }
}
