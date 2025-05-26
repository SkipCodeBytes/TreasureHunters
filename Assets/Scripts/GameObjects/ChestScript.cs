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

    private GameManager _gm;
    private Animator _animator;
    private Transform _transform;
    private int _targetPlayerIndex;

    private int[] _rewardArray;
    private List<ItemObject> _rewardObjs;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }



    private void OnEnable()
    {
        _animator.Play("ChestAwake");
    }

    public void OpenChestAnimation(int playerIndex, int[] rewards)
    {
        _rewardArray = rewards;
        _targetPlayerIndex = playerIndex;
        _animator.SetTrigger("Open");
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
        _animator.SetTrigger("Close");
        for (int i = 0; i < _rewardObjs.Count; i++)
        {
            _rewardObjs[i].TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform.position, itemTimeDrop);
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
