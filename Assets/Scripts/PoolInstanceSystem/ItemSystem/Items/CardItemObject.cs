using UnityEngine;

public class CardItemObject : ItemObject
{

    [Header("Board Tramp Mode")]
    [SerializeField] private Vector3 inBoardScale = Vector3.one;
    [SerializeField] private Vector3 inBoardRotationSpeed = Vector3.left;
    [SerializeField] private float inBoardLevitationHeight = 0;

    [Header("Pickable Object Mode")]
    [SerializeField] private Vector3 inPickableScale = Vector3.one;
    [SerializeField] private Vector3 inPickableRotationSpeed = Vector3.left;
    [SerializeField] private float inPickableLevitationHeight = 0;
    /*
    [SerializeField] private float dropAndSpawnParabolicTime = 1f;
    [SerializeField] private float collectTime = 1f;*/

    [Header("In use Mode")]
    [SerializeField] private Vector3 inUseScale = Vector3.one;
    [SerializeField] private Vector3 inUseRotationSpeed = Vector3.left;
    [SerializeField] private float inUseLevitationHeight = 0;
    /*
    [SerializeField] private float inUseAnimationTargetYOffset = 1f;
    [SerializeField] private float inUseAnimationTime = 1f;*/


    private void OnEnable()
    {
        SetInPickableMode();
    }

    public void SetInBoardMode()
    {
        transform.localScale = inBoardScale;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        rotationSpeed = inBoardRotationSpeed;
        levitationHeight = inBoardLevitationHeight;
    }

    public void SetInPickableMode()
    {
        transform.localScale = inPickableScale;
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        rotationSpeed = inPickableRotationSpeed;
        levitationHeight = inPickableLevitationHeight;
    }

    public void SetInUseMode()
    {
        transform.localScale = inUseScale;
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        rotationSpeed = inUseRotationSpeed;
        levitationHeight = inUseLevitationHeight;
    }
    /*
    public void PlayBoardAnimation()
    {
        EndAnimation();
    }

    public void PlayCollectableSpawnAnimation(Vector3 target)
    {
        StartCoroutine(CinematicAnimation.ParabolicMotion(transform, target, dropAndSpawnParabolicTime, EndAnimation));
    }
    public void PlayCollectAnimation(Vector3 target)
    {
        StartCoroutine(CinematicAnimation.MoveTowardTheTargetAt(transform, target, collectTime, EndAnimation));
    }

    public void PlayUseAnimation(Vector3 target)
    {
        StartCoroutine(CinematicAnimation.MoveTowardTheTargetAt(transform, transform.position + (Vector3.up * inUseAnimationTargetYOffset), inUseAnimationTime, EndAnimation));
    }

    private void EndAnimation()
    {
        gameObject.SetActive(false);
    }*/



    override public void SetItemObjectValues(ItemData gameItemData)
    {

    }
}
