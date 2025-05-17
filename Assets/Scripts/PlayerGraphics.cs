using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviourPunCallbacks
{
    private GameManager _gm;
    private PlayerManager _pm;
    private Animator _animator;

    [Header("Player Config")]

    [SerializeField] private float walkSpeed = 0.5f;
    [SerializeField] private float runSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Player Values - ReadOnly")]

    [SerializeField] private int animStatus = 0;
    [SerializeField] private int previusAnimation = 0;

    [SerializeField] private bool isResting = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isRotating = false;

    [SerializeField] private List<StackableAnimation> stackableAnimations = new List<StackableAnimation>();

    public int AnimStatus { get => animStatus; set => animStatus = value; }

    /*
     * 0 - Idle
     * 1 - Walk
     * 2 - Run
     * 3 - Sit
     * 4 - Fall
     * 5 - Block
     * 6 - Death
     * 7 - Lie
     * 8 - Rotating
     */

    private void Awake()
    {
        _pm = GetComponent<PlayerManager>();
        _gm = GameManager.Instance;
    }

    private void Update()
    {
        if (_pm.View.IsMine)
        {
            StackeableAnimationCycle();
            AnimationLogic();
            GenerateAnimStatus();
        }

        if (_animator != null) _animator.SetInteger("State", animStatus);
    }



    public GameObject GeneratePlayerModel()
    {
        GameObject character = Instantiate(_pm.SelectedCharacter.characterPrefab, transform.position, transform.rotation, transform);
        _animator = character.GetComponent<Animator>();
        return character;
    }


    private void StackeableAnimationCycle()
    {
        if (stackableAnimations.Count > 0)
        {
            if (!stackableAnimations[0].IsInProgress)
            {
                if (stackableAnimations[0].IsUsed)
                {
                    stackableAnimations.RemoveAt(0);
                    return;
                }
                else
                {
                    stackableAnimations[0].LaunchAnimation();
                }
            }
        }
    }

    private void AnimationLogic()
    {

        if (_pm.IsPlayerTurn)
        {
            isResting = false;
        }
        else 
        {
            if (!isWalking)
            {
                if (!isRotating)
                {
                    if (!isResting)
                    {
                        GoToRestInCurrentTile();
                    }
                }
            }
        }
    }

    private void GenerateAnimStatus()
    {
        if (_animator == null) return;
        if (_pm.View.IsMine)
        {
            animStatus = 0;
            if (isWalking) animStatus = 1;
            if (isRunning) animStatus = 2;
            if (isResting) animStatus = 3;
            if (isRotating) animStatus = 8;

            if (previusAnimation != animStatus)
            {
                previusAnimation = animStatus;
                _pm.View.RPC("SyncroAnimStatus", RpcTarget.Others, animStatus);
            }
        }
    }




    //Comportamiento automático:

    private void GoToRestInCurrentTile()
    {
        if (_pm.BoardPlayer.CurrentTilePosition == null) return;
        int restPosIndex = _pm.BoardPlayer.CurrentTilePosition.TileBehavior.TakeUpFreeSpaceIndex(_pm.BoardPlayer);
        Debug.Log("Lugar " + restPosIndex);
        Vector3 restPosition = _pm.BoardPlayer.CurrentTilePosition.TileBehavior.RestPoints[restPosIndex] + _pm.BoardPlayer.CurrentTilePosition.transform.position;
        _pm.View.RPC("SyncroEnterRestSpace", RpcTarget.Others, _gm.PlayerIndex, restPosIndex);
        MovePlayerAtPoint(restPosition, false);
        RotatePlayerAtPoint(_pm.BoardPlayer.CurrentTilePosition.transform.position, setRestingAnimation);
    }
    


    //Asignar estados

    public void ClearAnimationStatus()
    {
        isWalking = false;
        isResting = false;
        isRunning = false;
        isRotating = false;
    }

    private void setWalkingAnimation()
    {
        ClearAnimationStatus();
        isWalking = true;
    }

    private void setRunningAnimation()
    {
        ClearAnimationStatus();
        isRunning = true;
    }

    private void setRotatingAnimation()
    {
        ClearAnimationStatus();
        isRotating = true;
    }

    private void setRestingAnimation()
    {
        ClearAnimationStatus();
        isResting = true;
    }




    //GENERAL ORDERS - Se pueden dar varias órdenes, se apilarán y ejecutarán en orden

    public void MovePlayerAtPoint(Vector3 Point, bool running = true, Action callback = null)
    {
        Action init = null;
        float moventSpeed = 0;
        if (running) { 
            init = setRunningAnimation;
            moventSpeed = runSpeed;
        }
        else { 
            init = setWalkingAnimation;
            moventSpeed = walkSpeed;
        }
        RotatePlayerAtPoint(Point);
        stackableAnimations.Add(new StackableAnimation(this, AnimationType.MoveTo, gameObject.transform, Point, moventSpeed, init, callback));
    }

    public void RotatePlayerAtPoint(Vector3 Point, Action callback = null)
    {
        Vector3 target = new Vector3(Point.x, transform.position.y, Point.z);
        stackableAnimations.Add(new StackableAnimation(this, AnimationType.RotateTo, gameObject.transform, Point, rotationSpeed, setRotatingAnimation, callback));
    }



}
