using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerGraphics : MonoBehaviourPunCallbacks
{
    private GameManager _gm;
    private BoardPlayer _boardPlayer;
    private Animator _animator;

    [SerializeField] private float walkSpeed = 0.5f;
    [SerializeField] private float runSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;

    //[SerializeField] private 

    private int animStatus = 0;
    private int previusAnimation = 0;

    [SerializeField] private bool isResting = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isRotating = false;

    [SerializeField] private List<StackableAnimation> stackableAnimations = new List<StackableAnimation>();

    //private Coroutine _currentAnimationCourutine;
    private PhotonView _view;
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
        _boardPlayer = GetComponent<BoardPlayer>();
        _view = GetComponent<PhotonView>();
        _gm = GameManager.Instance;
    }

    private void Update()
    {
        if (_view.IsMine)
        {
            //Stackeable Animation Cycle

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


            if (_boardPlayer.IsPlayerTurn)
            {
                isResting = false;
            }
            else //if (stackableAnimations.Count == 0)
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

        animatePlayer();
    }



    public GameObject GeneratePlayerModel()
    {
        GameObject character = Instantiate(_boardPlayer.SelectedCharacter.characterPrefab, transform.position, transform.rotation, transform);
        _animator = character.GetComponent<Animator>();
        return character;
    }




    private void GoToRestInCurrentTile()
    {
        if (_boardPlayer.CurrentTilePosition == null) return;
        Vector3 restPosition = _boardPlayer.CurrentTilePosition.BehaviorScript.TakeUpFreeSpace(_boardPlayer) + _boardPlayer.CurrentTilePosition.transform.position;
        MovePlayerAtPoint(restPosition, false);
        RotatePlayerAtPoint(_boardPlayer.CurrentTilePosition.transform.position, setRestingAnimation);
    }






    private void animatePlayer()
    {
        if (_animator == null) return;
        if (_view.IsMine)
        {
            animStatus = 0;
            if (isWalking) animStatus = 1;
            if (isRunning) animStatus = 2;
            if (isResting) animStatus = 3;
            if (isRotating) animStatus = 8;

            if(previusAnimation != animStatus)
            {
                previusAnimation = animStatus;
                _view.RPC("SyncAnimation", RpcTarget.Others, animStatus);
            }
        }

        _animator.SetInteger("State", animStatus);
    }









    public void ClearAnimationStatus()
    {
        isWalking = false;
        isResting = false;
        isRunning = false;
        isRotating = false;
        //if (_currentAnimationCourutine != null) StopCoroutine(_currentAnimationCourutine);
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




    //GENERAL TRANSFORM

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



    [PunRPC]
    private void SyncAnimation(int newAnimStatus) => animStatus = newAnimStatus;
}
