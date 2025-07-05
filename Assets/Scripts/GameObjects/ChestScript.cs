using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private Vector3 itemSpawnPos;

    [SerializeField] private float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;
    [SerializeField] private float itemDropHeight;

    [SerializeField] private float itemTimeStand;
    [SerializeField] private ParticleSystem glowParticle;

    private GameManager _gm;
    private Animator _animator;
    private Transform _transform;
    private int _targetPlayerIndex;

    private int[] _rewardArray;
    private List<ItemObject> _rewardObjs;
    private TileBehavior _tileBehavior;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        _tileBehavior = GetComponentInParent<TileBehavior>();
        glowParticle.Stop();
        glowParticle.Clear();
    }



    private void OnEnable()
    {
        _animator.Play("ChestAwake");

    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    public void OpenChestAnimation(int playerIndex, int[] rewards)
    {
        _rewardArray = rewards;
        _targetPlayerIndex = playerIndex;
        _animator.SetTrigger("Open");
        glowParticle.Play();
    }

    //Referencia desde animación
    public void SpawnObjectsAnimation()
    {
        _rewardObjs = new List<ItemObject>();
        for (int i = 0; i < _rewardArray.Length; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < _rewardArray[0]; j++)
                {
                    _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(0));
                }
            }
            else
            {
                if (_rewardArray[i] == 0) continue;
                _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(_rewardArray[i]));
            }
        }

        for (int i = 0; i < _rewardObjs.Count; i++)
        {
            _rewardObjs[i].DropAnimation(_transform.position + itemSpawnPos, _transform.position, itemDropHeight, itemDropMaxRadio, itemTimeDrop);
        }
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop + itemTimeStand, CloseChestAnimation));
    }

    //All
    public void CloseChestAnimation()
    {
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.PlayCheerAnim();
        _animator.SetTrigger("Close");
        for (int i = 0; i < _rewardObjs.Count; i++)
        {
            int itemID = _rewardObjs[i].IDReference;
            ItemType itemType = ItemManager.Instance.GetItemType(itemID);
            if(itemType == ItemType.Relic)
            {
                if (!_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.availableToReceiveRelic)
                {
                    _tileBehavior.AddTileRewards(new int[] {0, itemID }, new ItemObject[] {_rewardObjs[i]});
                    continue;
                } else
                {
                    _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.availableToReceiveRelic = false;
                }
            }
            _rewardObjs[i].TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform, itemTimeDrop);
        }
        
    }

    //Referencia desde animación
    public void HideChest()
    {
        EventManager.TriggerEvent("EndEvent", true);
        _gm.GuiManager.SlotInfoUIList[_targetPlayerIndex].SetPlayerInfo();
        gameObject.SetActive(false);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + itemSpawnPos, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDropMaxRadio);
    }
#endif
}
