using System;
using UnityEngine;

public class ItemObject: MonoBehaviour
{
    public int IDReference = 0;
    private Transform _transform;
    [Header("Pickable object")]
    [SerializeField] protected float levitationHeight = 0.1f;
    [SerializeField] protected Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    protected virtual void Update()
    {
        _transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    virtual public void SetItemObjectValues(int ID, ItemData gameItemData) 
    {
        IDReference = ID;
    }

    public void DropAnimation(Vector3 initPosition, Vector3 targetPosition, float height, float maxRadius, float animTime)
    {
        _transform.position = initPosition;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * maxRadius;
        Vector3 punto = targetPosition + new Vector3(offset.x, levitationHeight, offset.y);
        StartCoroutine(CinematicAnimation.ParabolicMotion(_transform, punto, animTime, height));
    }

    public void TakeObjectAnimation(Transform target, float animTime)
    {
        //StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(_transform, target + new Vector3(0, 0.25f, 0), animTime, HideObject));
        StartCoroutine(CinematicAnimation.MoveTowardDinamicTargetFor(_transform, target, new Vector3(0, 0.25f, 0), animTime, HideObject));
    }

    private void HideObject()
    {
        SoundController.Instance.PlaySound(GameManager.Instance.SoundLibrary.GetClip("TakeItem"));
        gameObject.SetActive(false);
    }
}
