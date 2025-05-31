using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    private Animator _animator;
    private int _targetPlayerIndex;

    [SerializeField] private Vector3 itemSpawnPos;

    [SerializeField] private float itemTimeDrop;
    [SerializeField] private float itemDropMaxRadio;
    [SerializeField] private float itemDropHeight;

    [SerializeField] private float itemTimeStand;

    private GameManager _gm;
    private int[] _rewardArray;

    private ItemObject _rewardObj;

    [SerializeField] private ParticleSystem construcParticle;

    [SerializeField] private Animator _skeletonAnimator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.Play("ShopAwake");
        construcParticle.Play();
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        construcParticle.Stop();
        construcParticle.Clear();
    }


    public void PresentAnimation(int playerIndex, int[] rewards)
    {
        _rewardArray = rewards;
        _targetPlayerIndex = playerIndex;
        _animator.SetTrigger("Drop");
    }


    //Llamado desde la animación
    public void DropObjAnimation()
    {

        for (int i = 0; i < _rewardArray.Length; i++)
        {
            if (i != 0) //_rewardArray[i]
            {
                _rewardObj = ItemManager.Instance.GenerateItemInScene(_rewardArray[i]);
                ItemType itemType = ItemManager.Instance.GetItemType(_rewardArray[i]);

                switch (itemType)
                {
                    case ItemType.Card:
                        if (_rewardObj is CardItemObject cardItem) cardItem.SetInPickableMode();
                        break;

                    case ItemType.Gem:
                        break;

                    case ItemType.Relic:
                        break;

                    default:
                        break;
                }
                break;
            }
        }

        _rewardObj.DropAnimation(transform.position + itemSpawnPos, transform.position, itemDropHeight, itemDropMaxRadio, itemTimeDrop);
        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop + itemTimeStand, EndAnimationShop));
    }

    //Referencia desde animación
    public void EndAnimationShop()
    {
        _animator.SetTrigger("Destroy");
        _rewardObj.TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform.position, itemTimeDrop);
    }

    public void CloseAnimation()
    {
        _gm.GuiManager.SlotInfoUIList[_targetPlayerIndex].SetPlayerInfo();
        EventManager.TriggerEvent("EndEvent", true);
        gameObject.SetActive(false);
    }
}
