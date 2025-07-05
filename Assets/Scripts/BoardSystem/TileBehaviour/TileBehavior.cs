using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class TileBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 _interactionPositionOffset = Vector3.zero;
    [SerializeField] private float _interactionViewRotation = 0f;
    [SerializeField] private List<GameObject> _hideableProps = new List<GameObject>();
    [SerializeField] private List<Vector3> _restPoints = new List<Vector3>();

    private Dictionary<Vector3, BoardPlayer> _restPointDicc = new Dictionary<Vector3, BoardPlayer>();
    protected TileBoard _tileBoard;

    public GameObject trampCardGraphic;
    public CardItemData trampCardInTile;

    [SerializeField] private List<ItemObject> rewardItems = new List<ItemObject>();
    //[SerializeField] private int relicRewardCount = 0;
    [SerializeField] private List<int> rewardItemsID = new List<int>();

    public List<GameObject> HideableProps { get => _hideableProps; set => _hideableProps = value; }
    public List<Vector3> RestPoints { get => _restPoints; set => _restPoints = value; }
    public Vector3 InteractionPositionOffset { get => _interactionPositionOffset; set => _interactionPositionOffset = value; }
    public float InteractionViewRotation { get => _interactionViewRotation; set => _interactionViewRotation = value; }

    protected virtual void Awake()
    {
        _tileBoard = transform.parent.GetComponent<TileBoard>();
        for (int i = 0; i < _restPoints.Count; i++) _restPointDicc[_restPoints[i]] = null;
        rewardItemsID.Insert(0, 0);
    }

    protected virtual void Start()
    {
        HideProps();
    }


    public virtual void HideProps()
    {
        for (int i = 0; i < _hideableProps.Count; i++)
        {
            _hideableProps[i].SetActive(false);
        }
    }

    public virtual void UnhideProps()
    {
        for (int i = 0; i < _hideableProps.Count; i++)
        {
            _hideableProps[i].SetActive(true);
        }
    }




    //Se llama cuando el jugador llega a un tile y se detiene en el Tile
    public abstract void StartTileEvent();

    //Se llama cuando el tile aplica el efecto / Ejemplo: El ChestTile responde luego de lanzar dados
    public abstract void SettingTileEvent();


    public abstract void PlayTileEvent();



    //Asigna y devuelve el index del espacio libre que ocupa
    public int TakeUpFreeSpaceIndex(BoardPlayer boardPlayer)
    {
        for(int i = 0; i < _restPoints.Count; i++)
        {
            if (_restPointDicc.ContainsKey(_restPoints[i]))
            {
                //Buscamos un espacio libre
                if (_restPointDicc[_restPoints[i]] == null) {

                    //Eliminamos si existe este jugador registrado en este tile
                    for (int j = 0; j < _restPoints.Count; j++)
                    {
                        if (!_restPointDicc.ContainsKey(_restPoints[j])) continue;
                        if (_restPointDicc[_restPoints[j]] == boardPlayer)
                        {
                            _restPointDicc[_restPoints[j]] = null;
                        }
                    }
                    _restPointDicc[_restPoints[i]] = boardPlayer;
                    return i;
                }
                //En caso de que el diccionario indique que está ocupado por un jugador, pero se encuentra en otro tile
                else if (_tileBoard != _restPointDicc[_restPoints[i]].CurrentTilePosition) {
                    _restPointDicc[_restPoints[i]] = boardPlayer;
                    return i;
                } else continue;
            }
        }
        Debug.LogWarning("No hay espacios libres");
        return -1;
    }

    //Solo asigna, se usa solo en el momento de sincronizar este dato
    public void SetSpace(BoardPlayer boardPlayer, int spaceIndex)
    {
        if (spaceIndex == -1) return;
        _restPointDicc[_restPoints[spaceIndex]] = boardPlayer;
    }

    //Deja libre el espacio de descanso
    public void LeaveFreeSpace(BoardPlayer boardPlayer)
    {
        bool hideProps = true;
        for (int i = 0; i < _restPoints.Count; i++)
        {
            if (_restPointDicc[_restPoints[i]] == boardPlayer)
            {
                _restPointDicc[_restPoints[i]] = null;
                continue;
            }
            if (_restPointDicc[_restPoints[i]] != null) hideProps = false;
        }

        if (hideProps) HideProps();
    }



    /*
    public Vector3 GetInteractionPosition()
    {
        return transform.position + _interactionPositionOffset;
    }*/

    public Vector3 GetInteractionPosition()
    {
        return transform.position + transform.rotation * _interactionPositionOffset;
    }

    /*
    public Vector3 GetIteractionViewPoint()
    {
        Vector3 nuevaPos = transform.position + _interactionPositionOffset + Quaternion.Euler(0, _interactionViewRotation, 0) * Vector3.forward;
        return nuevaPos;
    }*/
    public Vector3 GetIteractionViewPoint()
    {
        Vector3 offsetWorld = transform.rotation * _interactionPositionOffset;
        Quaternion totalRotation = transform.rotation * Quaternion.Euler(0, _interactionViewRotation, 0);
        return transform.position + offsetWorld + totalRotation * Vector3.forward;
    }


    public void AddTileRewards(int[] RewardsID, ItemObject[] rewardsObjs)
    {
        List<ItemObject> rewardsObjsList = new List<ItemObject>(rewardsObjs);
        rewardItems.AddRange(rewardsObjsList);

        rewardItemsID[0] += RewardsID[0];
        for(int i = 1; i < RewardsID.Length; i++)
        {
            rewardItemsID.Add(RewardsID[i]);
        }
    }

    public void GetTileRewards(int playerId)
    {
        if (rewardItems.Count <= 0) return;
        GameManager.Instance.GmView.RPC("PlayerGetTileReward", Photon.Pun.RpcTarget.All, playerId, _tileBoard.Order.x, _tileBoard.Order.y);
    }

    public void SyncroGetTileRewards(int playerId)
    {
        PlayerManager targetPlayer = GameManager.Instance.PlayersArray[playerId];
        targetPlayer.Inventory.AddCoins(rewardItemsID[0]);

        List<int> noTakeObjID = new List<int>();

        for (int i = 1; i < rewardItemsID.Count; i++)
        {
            ItemType itemType = ItemManager.Instance.GetItemType(rewardItemsID[i]);
            if(itemType == ItemType.Relic)
            {
                if (targetPlayer.Inventory.RelicItemData != null)
                {
                    noTakeObjID.Add(rewardItemsID[i]);
                    continue;
                }
                targetPlayer.Inventory.availableToReceiveRelic = true;
            }
            targetPlayer.Inventory.AddItem(rewardItemsID[i]);
        }
        rewardItemsID = new List<int>();
        rewardItemsID.Add(0);
        rewardItemsID.AddRange(noTakeObjID);

        List<ItemObject> noTakeObj = new List<ItemObject>();

        for (int i = 0; i < rewardItems.Count; i++)
        {
            ItemType itemType = ItemManager.Instance.GetItemType(rewardItems[i].IDReference);
            if (itemType == ItemType.Relic)
            {
                if (targetPlayer.Inventory.availableToReceiveRelic)
                {
                    targetPlayer.Inventory.availableToReceiveRelic = false;
                    rewardItems[i].TakeObjectAnimation(targetPlayer.transform, 0.8f);
                    continue;
                }
                else
                {
                    noTakeObj.Add(rewardItems[i]);
                    continue;
                }
            }
            rewardItems[i].TakeObjectAnimation(targetPlayer.transform, 0.8f);
        }

        rewardItems = noTakeObj;

        StartCoroutine(CinematicAnimation.WaitTime(0.8f, () => {
            GameManager.Instance.GuiManager.SlotInfoUIList[playerId].SetPlayerInfo();
            }));
    }

    public bool AddTrampCard(CardItemData cardItemData)
    {
        if (trampCardInTile != null) return false;

        trampCardInTile = cardItemData;
        trampCardGraphic.SetActive(true);
        return true;
    }

    public void CheckTrampCard()
    {
        if (trampCardInTile != null)
        {
            GameManager.Instance.GmView.RPC("PlayCardEffect", Photon.Pun.RpcTarget.All, GameManager.Instance.CurrentPlayerTurnIndex, ItemManager.Instance.GetItemID(trampCardInTile));
        }
        else
        {
            GameManager.Instance.MomentManager.IsWaitingForEvent = false;
        }
    }

    public void RemoveTrampCard()
    {
        if (trampCardInTile != null)
        {
            //GameManager.Instance.PlayCardEffect(GameManager.Instance.CurrentPlayerTurnIndex, trampCardInTile);
            trampCardInTile = null;
            trampCardGraphic.SetActive(false);
        }
    }


#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        /*
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + _interactionPositionOffset, 0.2f);*/
        Vector3 offsetWorld = transform.rotation * _interactionPositionOffset;
        Vector3 worldPosition = transform.position + offsetWorld;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(worldPosition, 0.2f);

        Gizmos.color = Color.green;
        for (int i = 0; i < _restPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position + _restPoints[i], 0.2f);
        }

        /*
        float radians = _interactionViewRotation * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, transform.position + _interactionPositionOffset, Quaternion.LookRotation(direction), 0.5f, EventType.Repaint);*/
        Quaternion totalRotation = transform.rotation * Quaternion.Euler(0, _interactionViewRotation, 0);
        Vector3 direction = totalRotation * Vector3.forward;

        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, worldPosition, Quaternion.LookRotation(direction), 0.5f, EventType.Repaint);
    }
#endif
}
