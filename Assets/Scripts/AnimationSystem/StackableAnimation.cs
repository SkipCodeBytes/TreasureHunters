using System;
using UnityEngine;

[System.Serializable]
public class StackableAnimation
{
    
    [SerializeField] private AnimationType _animationType;
    [SerializeField] private Transform _affectedTransform;
    [SerializeField] private Vector3 _target;
    [SerializeField] private float _speed;
    [SerializeField] private Action _initCallback;
    [SerializeField] private Action _endCallback;

    private MonoBehaviour _runnerScript;
    private Coroutine _coroutineReference = null;
    private bool _isUsed = false;
    private bool _isInProgress = false;

    public AnimationType AnimationType { get => _animationType; set => _animationType = value; }
    public Transform AffectedTransform { get => _affectedTransform; set => _affectedTransform = value; }
    public Vector3 Target { get => _target; set => _target = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public bool IsInProgress { get => _isInProgress; }
    public bool IsUsed { get => _isUsed; set => _isUsed = value; }
    public Coroutine CoroutineReference { get => _coroutineReference; set => _coroutineReference = value; }
    public Action InitCallback { get => _initCallback; set => _initCallback = value; }
    public Action EndCallback { get => _endCallback; set => _endCallback = value; }

    public StackableAnimation(MonoBehaviour runnerScript, AnimationType type, Transform affected, Vector3 target, float speed, Action initCallback = null, Action endCallback = null)
    {
        _runnerScript = runnerScript;
        _animationType = type;
        _affectedTransform = affected;
        _target = target;
        _speed = speed;
        _initCallback = initCallback;
        _endCallback = endCallback;
    }

    public void LaunchAnimation()
    {
        _isInProgress = true;


        switch (_animationType)
        {
            case AnimationType.RotateTo:

                //Comprobamos si hay rotación mínima, de lo contrario, se omite el _initCallback
                Vector3 direction = (_target - _affectedTransform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    float angleDifference = Quaternion.Angle(_affectedTransform.rotation, targetRotation);
                    if (angleDifference > 0.1f) { 
                        _initCallback?.Invoke();
                        _coroutineReference = _runnerScript.StartCoroutine(CinematicAnimation.RotateToWorldPoint(_affectedTransform, _target, _speed, FinishAnimation));
                    } else
                    {
                        FinishAnimation();
                    }
                } else
                {
                    FinishAnimation();
                }
                break;


            case AnimationType.MoveTo:

                //Comprobamos si hay distancia mínima, de lo contrario, se omite el _initCallback
                float distance = Vector3.Distance(_affectedTransform.position, _target);
                if(distance > 0.1f) { 
                    _initCallback?.Invoke();
                    _coroutineReference = _runnerScript.StartCoroutine(CinematicAnimation.MoveTowardTheTargetAt(_affectedTransform, _target, _speed, FinishAnimation));
                } else
                {
                    FinishAnimation();
                }
                break;


            case AnimationType.ParabolicMotion:
                _initCallback?.Invoke();
                _coroutineReference = _runnerScript.StartCoroutine(CinematicAnimation.ParabolicMotion(_affectedTransform, _target, _speed, FinishAnimation));
                break;


            default:
                break;
        }
    }


    private void FinishAnimation()
    {
        _isUsed = true;
        _isInProgress = false;
        if (_endCallback != null) { _endCallback?.Invoke(); }
    }
}

public enum AnimationType
{
    RotateTo,
    MoveTo,
    ParabolicMotion
}
