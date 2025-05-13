using System.Collections;
using UnityEngine;

public class CameramanScript : MonoBehaviour
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

    public void FocusPanoramicView(bool isInmediate = false)
    {
        _followTarget = false;
        _currentTarget = null;
        if (_moveCoorutine != null)
        {
            StopCoroutine(_moveCoorutine);
            _moveCoorutine = null;
        }
        if (isInmediate) gameObject.transform.position = panoramicViewPosition;
        else _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTo(gameObject, panoramicViewPosition, timeToFocus, focusComplete));
    }

    public void FocusTarget(GameObject target)
    {
        _followTarget = false;
        _currentTarget = target;
        if (_moveCoorutine != null)
        {
            StopCoroutine(_moveCoorutine);
            _moveCoorutine = null;
        }
        _moveCoorutine = StartCoroutine(CinematicAnimation.MoveTo(gameObject, target.transform.position, timeToFocus, focusComplete));
    }

    private void focusComplete()
    {
        _followTarget = true;
        EventManager.TriggerEvent("EndEvent", true);
    }
}
