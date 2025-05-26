using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private GameManager _gm;
    private Animator _animator;
    private Transform _transform;
    private int _targetPlayerIndex;

    [SerializeField] private Animator _skeletonAnimator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    private void InitAnimation()
    {
        //GameObject effectObj = InstanceManager.Instance.GetObject(smokeEffect);
        //effectObj.transform.position = transform.position;
        _animator.Play("ShopAwake");
    }
}
