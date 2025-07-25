using System.Collections;
using UnityEngine;

public class BoardCameraController : MonoBehaviour
{
    [SerializeField] private float timeToFocus = 0.8f;
    [SerializeField] private Vector3 panoramicViewPosition = Vector3.zero;
    private GameObject _currentTarget;
    private Coroutine _moveCoorutine;
    private bool _followTarget = false;

    private void Update()
    {
        if (_followTarget)
        {
            if(_currentTarget != null) transform.position = _currentTarget.transform.position;
            else transform.position = panoramicViewPosition;
        }
    }

    public void FocusPanoramicView(bool isInmediate = false, bool endFocusEvent = true)
    {
        _followTarget = false;
        _currentTarget = null;
        if (_moveCoorutine != null)
        {
            StopCoroutine(_moveCoorutine);
            _moveCoorutine = null;
        }
        if (isInmediate) gameObject.transform.position = panoramicViewPosition;
        else if(endFocusEvent) _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gameObject, panoramicViewPosition, timeToFocus, focusComplete));
        else _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gameObject, panoramicViewPosition, timeToFocus));
    }

    public void FocusTarget(GameObject target, bool endFocusEvent = true)
    {
        _followTarget = false;
        _currentTarget = target;
        if (_moveCoorutine != null)
        {
            StopCoroutine(_moveCoorutine);
            _moveCoorutine = null;
        }

        if (endFocusEvent) _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gameObject, target.transform.position, timeToFocus, focusComplete));
        else _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gameObject, target.transform.position, timeToFocus));
    }

    private void focusComplete()
    {
        _followTarget = true;
        EventManager.TriggerEvent("EndEvent", true);
    }
}
