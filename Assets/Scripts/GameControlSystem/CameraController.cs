using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float autoRotateSpeed = 5f;
    [SerializeField] private float inactiveTime = 0.2f;

    private CinemachineOrbitalFollow _orbitalFollow;
    private CinemachineInputAxisController _inputAxisController;

    private bool isControlDrag = false;
    private bool isInactiveMode = false;

    private float _inactiveTimer = 0;

    void Awake()
    {
        _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        _inputAxisController = GetComponent<CinemachineInputAxisController>();

        _inputAxisController.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isControlDrag)
            {
                isControlDrag = true;
                isInactiveMode = false;
                _inputAxisController.enabled = true;
                _inactiveTimer = 0;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isControlDrag)
            {
                _inputAxisController.enabled = false;
                isControlDrag = false;
            }
        }

        if (!isControlDrag && !isInactiveMode)
        {
            _inactiveTimer += Time.deltaTime;
            if (_inactiveTimer > inactiveTime)
            {
                isInactiveMode = true;
            }
        }

        if (isInactiveMode)
        {
            _orbitalFollow.HorizontalAxis.Value += autoRotateSpeed * Time.deltaTime;
            if(_orbitalFollow.HorizontalAxis.Value > 180)
            {
                _orbitalFollow.HorizontalAxis.Value -= 360;
            }
        }
    }
}
