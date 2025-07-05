using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    public Animator Animator;
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
    public ParticleSystem continousParticle;

    [SerializeField] private Animator _skeletonAnimator;
    private TileBehavior _tileBehavior;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _tileBehavior = GetComponentInParent<TileBehavior>();
    }

    private void OnEnable()
    {
        Animator.Play("ShopAwake");
        construcParticle.Play();
        if(_gm != null) SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Shop"));
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        construcParticle.Stop();
        construcParticle.Clear();
        continousParticle.Stop();
        continousParticle.Clear();
    }


    public void PresentAnimation(int playerIndex, int[] rewards)
    {
        _rewardArray = rewards;
        _targetPlayerIndex = playerIndex;
        Animator.SetTrigger("Drop");
    }


    //Llamado desde la animación
    public void DropObjAnimation()
    {
        _skeletonAnimator.SetTrigger("Cheer");
        Debug.Log(_rewardArray);
        for (int i = 0; i < _rewardArray.Length; i++)
        {
            if (i != 0 && _rewardArray[i] != 0) //_rewardArray[i]
            {
                _rewardObj = ItemManager.Instance.GenerateItemInScene(_rewardArray[i]);
                ItemType itemType = ItemManager.Instance.GetItemType(_rewardArray[i]);
                Debug.Log(_rewardObj);
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
        _gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Graphics.PlayCheerAnim();

        StartCoroutine(CinematicAnimation.WaitTime(itemTimeDrop + itemTimeStand, EndAnimationShop));
    }

    //Referencia desde animación
    public void EndAnimationShop()
    {
        Animator.SetTrigger("Destroy");
        if(_rewardObj != null)
        {
            ItemType itemType = ItemManager.Instance.GetItemType(_rewardObj.IDReference);
            if (itemType == ItemType.Relic)
            {
                if (!_gm.PlayersArray[_gm.CurrentPlayerTurnIndex].Inventory.availableToReceiveRelic)
                {
                    _tileBehavior.AddTileRewards( new int[] {0, _rewardObj.IDReference }, new ItemObject[] { _rewardObj });
                    _rewardObj = null;
                    continousParticle.Play();
                    return;
                }
            }
            _rewardObj.TakeObjectAnimation(_gm.PlayersArray[_targetPlayerIndex].transform, itemTimeDrop);
            _rewardObj = null;
        }
        continousParticle.Play();
    }

    public void CloseAnimation()
    {
        _gm.GuiManager.SlotInfoUIList[_targetPlayerIndex].SetPlayerInfo();
        EventManager.TriggerEvent("EndEvent", true);
        gameObject.SetActive(false);
    }
}
