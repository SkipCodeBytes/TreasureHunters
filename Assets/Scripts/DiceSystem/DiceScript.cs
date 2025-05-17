
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class DiceScript : MonoBehaviourPunCallbacks
{
    [Header("Config Values")]
    [SerializeField] private LayerMask sideLayer;
    [SerializeField] private Vector3 checkOriginPoint = new Vector3(0,0.2f,0);
    [SerializeField] private float CheckSize = 0.24f;
    [SerializeField] private float force = 1f;
    [SerializeField] private float torque = 0.02f;

    [Header("Check Values")]
    [SerializeField] private int diceValue = 0;
    [SerializeField] private bool hasBeenRolled = false;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool isItStill = false;
    [SerializeField] private List<BoxCollider> sideColliders;

    private PhotonView _diceView;
    private Vector2 LaunchDirection;
    private Vector3 staticPosition;

    private Rigidbody _rb;

    public int DiceValue { get => diceValue; set => diceValue = value; }
    public bool HasBeenRolled { get => hasBeenRolled; set => hasBeenRolled = value; }
    public bool IsItStill { get => isItStill; set => isItStill = value; }
    public PhotonView DiceView { get => _diceView; set => _diceView = value; }

    void Awake()
    {
        _diceView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        sideColliders = new List<BoxCollider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            sideColliders.Add(transform.GetChild(i).GetComponent<BoxCollider>());
        }
        ResetDice(Vector3.zero);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (hasBeenRolled)
        {
            isItStill = _rb.IsSleeping();
            if (diceValue == 0 && isItStill)
            {
                checkResult();
            }
        }
        else
        {
            diceValue = 0;
        }

        if (Input.GetMouseButtonUp(0) && hasBeenRolled == false && isSelected)
        {
            hasBeenRolled = true;
            _rb.useGravity = true;
            _rb.AddForce(new Vector3(-LaunchDirection.x, 0.15f, -LaunchDirection.y) * force, ForceMode.Impulse);
        }

        if(!hasBeenRolled) transform.localPosition = staticPosition;
    }

    private void checkResult()
    {
        Ray ray = new Ray(transform.position + checkOriginPoint, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, CheckSize, sideLayer))
        {
            Debug.Log("Resultado del dado es: " + hit.collider.name);
            diceValue = int.Parse(hit.collider.name);
        }
    }

    public void ResetDice(Vector3 initPosition) 
    {
        staticPosition = initPosition;
        transform.localPosition = staticPosition;
        hasBeenRolled = false;
        isSelected = false;
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.AddTorque(Random.onUnitSphere * torque, ForceMode.Impulse);
        diceValue = 0;
    }

    public void ChangeOwner(int playerTargetIndex)
    {
        Debug.Log("Nuevo Propietario de dados: " + playerTargetIndex);
        photonView.RPC("TransferOwner", photonView.Owner, playerTargetIndex);
    }

    [PunRPC]
    private void TransferOwner(int newOwnerIndex)
    {
        Debug.Log("Nuevo Propietario de dados: " + GameManager.Instance.PlayersArray[newOwnerIndex].Player.NickName);
        photonView.TransferOwnership(GameManager.Instance.PlayersArray[newOwnerIndex].Player);
    }

    private void OnMouseDown()
    {
        if (!photonView.IsMine) return;
        isSelected = true;
    }

    private void OnMouseDrag()
    {
        if (!photonView.IsMine) return;
        Vector2 cursorPosition = GameController.Instance.GameControls.Player.CursorPoint.ReadValue<Vector2>();
        Vector2 centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        if (cursorPosition != centerScreen)
        {
            LaunchDirection = (cursorPosition - centerScreen).normalized;
            //Debug.DrawLine(transform.position, new Vector3(transform.position.x + LaunchDirection.x, transform.position.y, transform.position.z + LaunchDirection.y));
        }
    }

}
