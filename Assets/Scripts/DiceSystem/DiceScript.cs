
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class DiceScript : MonoBehaviour
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
    private Vector2 LaunchDirection;
    private Vector3 staticPosition;

    private Rigidbody _rb;

    public int DiceValue { get => diceValue; set => diceValue = value; }
    public bool HasBeenRolled { get => hasBeenRolled; set => hasBeenRolled = value; }
    public bool IsItStill { get => isItStill; set => isItStill = value; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        sideColliders = new List<BoxCollider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            sideColliders.Add(transform.GetChild(i).GetComponent<BoxCollider>());
        }
        resetDice(Vector3.zero);
    }

    void Update()
    {
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
            _rb.AddForce(new Vector3(-LaunchDirection.x, 0.5f, -LaunchDirection.y) * force, ForceMode.Impulse);
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

    public void resetDice(Vector3 initPosition)
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

    private void OnMouseDown()
    {
        isSelected = true;
    }

    private void OnMouseDrag()
    {
        Vector2 cursorPosition = GameController.Instance.GameControls.Player.CursorPoint.ReadValue<Vector2>();
        Vector2 centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        if (cursorPosition != centerScreen)
        {
            LaunchDirection = (cursorPosition - centerScreen).normalized;
            //Debug.DrawLine(transform.position, new Vector3(transform.position.x + LaunchDirection.x, transform.position.y, transform.position.z + LaunchDirection.y));
        }
    }

}
