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

    [Header("In use Mode")]
    [SerializeField] private Vector3 inUseScale = Vector3.one;
    [SerializeField] private Vector3 inUseRotationSpeed = Vector3.left;
    [SerializeField] private float inUseLevitationHeight = 0;

    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }


    private void OnEnable()
    {
        SetInPickableMode();
        StartCoroutine(CinematicAnimation.WaitTime(Random.Range(0.08f, 0.2f), () =>
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Card"))));
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

}
