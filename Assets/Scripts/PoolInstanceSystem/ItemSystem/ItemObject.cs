using System;
using UnityEngine;

public class ItemObject: MonoBehaviour
{
    private Transform _transform;

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    virtual public void SetItemObjectValues(ItemData gameItemData) { }

    public void DropAnimation(Vector3 initPosition, Vector3 targetPosition, float height, float maxRadius, float animTime)
    {
        _transform.position = initPosition;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * maxRadius;
        Vector3 punto = targetPosition + new Vector3(offset.x, 0f, offset.y);
        StartCoroutine(CinematicAnimation.ParabolicMotion(_transform, punto, animTime, height));
    }

    public void TakeObjectAnimation(Vector3 target, float animTime)
    {
        StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(_transform, target, animTime, HideObject));
    }

    private void HideObject()
    {
        gameObject.SetActive(false);
    }
}
