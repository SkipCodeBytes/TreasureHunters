using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class DiceScript : MonoBehaviour
{
    [Header("Config Values")]
    [SerializeField] private LayerMask sideLayer;
    [SerializeField] private Vector3 checkOriginPoint;
    [SerializeField] private float CheckSize;

    [Header("Check Values")]
    [SerializeField] private int diceValue = 0;
    [SerializeField] private bool _hasBeenRolled = false;
    [SerializeField] private List<BoxCollider> sideColliders;
    private Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        sideColliders = new List<BoxCollider>();
        for (int i = 0; i < transform.childCount; i++)
        {
            sideColliders.Add(transform.GetChild(i).GetComponent<BoxCollider>());
        }
        resetDice();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.IsSleeping())
        {
            if (diceValue == 0 && _hasBeenRolled)
            {
                checkResult();
            }
        }
        else
        {
            diceValue = 0;
        }

        Debug.DrawRay(transform.position + checkOriginPoint, Vector3.down * CheckSize, Color.red, 1f);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            resetDice();
        }
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

    private void resetDice()
    {
        transform.localPosition = Vector3.zero;
        _hasBeenRolled = false;
        _rb.useGravity = false;
       // _rb.linearVelocity = Vector3.zero;
        //_rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;
        diceValue = 0;
    }

    void OnMouseDown()
    {
        _hasBeenRolled = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        //_rb.AddForce(Random.onUnitSphere * fuerza, ForceMode.Impulse);
        //_rb.AddTorque(Random.onUnitSphere * torque, ForceMode.Impulse); 
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position + test1, transform.position + test2);
        //Gizmos.DrawWireCube(transform.position + test1, test2);
    }
}
