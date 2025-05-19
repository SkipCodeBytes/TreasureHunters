using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    [SerializeField] private Vector3 itemSpawnPos;
    [SerializeField] private float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;

    private GameManager _gm;
    private Animator _animator;
    private Transform _transform;
    private PlayerManager _targetPlayer;
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
        _targetPlayer = _gm.PlayersArray[playerIndex];
        _animator.SetTrigger("OpenChest");
        _rewardObjs = new List<ItemObject>();

        for (int i = 0; i < rewards.Length; i++) {
            if (i == 0)
            {
                for (int j = 0; j < rewards[0]; j++)
                {
                    _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(0));
                }
            } else
            {
                _rewardObjs.Add(ItemManager.Instance.GenerateItemInScene(rewards[i]));
            }
        }

        for (int i = 0; i < _rewardObjs.Count; i++) {
            _rewardObjs[i].DropAnimation(_transform.position + itemSpawnPos, _transform.position, itemDropMaxRadio, itemTimeDrop);
        }
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop * 2, CloseChestAnimation));

    }

    public void CloseChestAnimation()
    {
        _animator.SetTrigger("CloseChest");
        for (int i = 0; i < _rewardObjs.Count; i++)
        {
            _rewardObjs[i].TakeObjectAnimation(_targetPlayer.transform.position, itemTimeDrop);
        }
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop * 2, () => EventManager.TriggerEvent("EndChestEvent")));
    }

    public void HideChest()
    {
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
