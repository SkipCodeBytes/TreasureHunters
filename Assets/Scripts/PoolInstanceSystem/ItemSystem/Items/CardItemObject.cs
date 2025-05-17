using UnityEngine;

public class CardItemObject : ItemObject
{

    [Header("Board Tramp Mode")]
    [SerializeField] private Vector3 inBoardScale = Vector3.one;
    [SerializeField] private Vector3 inBoardDirection = Vector3.left;
    [SerializeField] private float inBoardSpeedRotation = 0.1f;

    [Header("Collectable Object Mode")]
    [SerializeField] private Vector3 inCollectableScale = Vector3.one;
    [SerializeField] private Vector3 inCollectableDirection = Vector3.left;
    [SerializeField] private float inCollectableSpeedRotation = 0.1f;
    [SerializeField] private float dropAndSpawnParabolicTime = 1f;
    [SerializeField] private float collectTime = 1f;

    [Header("In use Mode")]
    [SerializeField] private Vector3 inUseScale = Vector3.one;
    [SerializeField] private Vector3 inUseDirection = Vector3.left;
    [SerializeField] private float inUseSpeedRotation = 0.1f;
    [SerializeField] private float inUseAnimationTargetYOffset = 1f;
    [SerializeField] private float inUseAnimationTime = 1f;

    //En tablero aparece grande y girando
    //Como objeto solo aparece y se persigue al jugador
    //Como carta en uso, gira sobre la 

    //private int _cardMode = 0;

    private Vector3 _currentDirection = Vector3.left;
    private float _currentSpeed = 0.1f;

    private void Update()
    {
        transform.Rotate(_currentDirection * _currentSpeed * Time.deltaTime);
    }

    public void SetInBoardMode()
    {
        transform.localScale = inBoardScale;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _currentDirection = inBoardDirection;
        _currentSpeed = inBoardSpeedRotation;
    }

    public void SetInCollectableMode()
    {
        transform.localScale = inCollectableScale;
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        _currentDirection = inCollectableDirection;
        _currentSpeed = inCollectableSpeedRotation;
    }

    public void SetInUseMode()
    {
        transform.localScale = inUseScale;
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        _currentDirection = inUseDirection;
        _currentSpeed = inUseSpeedRotation;
    }

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
    }



    override public void SetItemObjectValues(ItemData gameItemData)
    {

    }
}
