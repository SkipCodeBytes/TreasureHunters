using System;
using UnityEngine;

public class ItemObject: MonoBehaviour
{
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

    virtual public void SetItemObjectValues(ItemData gameItemData) { }

    public void DropAnimation(Vector3 initPosition, Vector3 targetPosition, float height, float maxRadius, float animTime)
    {
        _transform.position = initPosition;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * maxRadius;
        Vector3 punto = targetPosition + new Vector3(offset.x, levitationHeight, offset.y);
        StartCoroutine(CinematicAnimation.ParabolicMotion(_transform, punto, animTime, height));
    }

    public void TakeObjectAnimation(Vector3 target, float animTime)
    {
        StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(_transform, target + new Vector3(0, 0.25f, 0), animTime, HideObject));
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }
}
