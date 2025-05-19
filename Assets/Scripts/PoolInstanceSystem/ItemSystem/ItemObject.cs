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

    public void DropAnimation(Vector3 initPosition, Vector3 targetPosition, float maxRadius, float animTime)
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * maxRadius;
        Vector3 punto = _transform.position + new Vector3(offset.x, 0f, offset.y);
        StartCoroutine(CinematicAnimation.ParabolicMotion(_transform, punto, animTime));
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
