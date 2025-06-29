using System.Collections.Generic;
using UnityEngine;

public class RuinPedestal : MonoBehaviour
{
    [SerializeField] private List<Transform> pedestalList = new List<Transform>();
    [SerializeField] private Vector3 gemVisualOffset;

    private Animator animator;
    private GameManager _gm;

    private void Awake()
    {
        animator = GetComponent<Animator> ();
    }


    private int targetPlayerId;
    private ItemObject[] gemsObjs;
    private Coroutine itemAnim;

    private void OnEnable()
    {
        animator.Play("Awake");
    }

    public void PlayRuinEvent(int playerId, ItemObject[] gemsUsed)
    {
        _gm = GameManager.Instance;
        targetPlayerId = playerId;
        gemsObjs = gemsUsed;
        for (int i = 0; i < gemsUsed.Length; i++)
        {
            StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gemsUsed[i].transform, pedestalList[i].position + gemVisualOffset, 0.8f));
        }
        StartCoroutine(CinematicAnimation.WaitTime(1.2f, PlayCollectGems));
    }

    private void PlayCollectGems()
    {
        for (int i = 0; i < gemsObjs.Length; i++)
        {
            itemAnim = StartCoroutine(CinematicAnimation.MoveTowardTheTargetFor(gemsObjs[i].transform, pedestalList[i].position + new Vector3(0,-0.5f,0), 0.6f));
        }
        _gm.PlayersArray[targetPlayerId].Rules.AddGameStar();

        animator.Play("Interactue");
        if (_gm.IsHostPlayer)
        {
            RuinsTile.GenerateGemsNeeded();
        }
    }


    public void EndAnimation()
    {
        StopCoroutine(itemAnim);
        _gm.GuiManager.SlotInfoUIList[targetPlayerId].SetPlayerInfo();
        for (int i = 0; i < gemsObjs.Length; i++) gemsObjs[i].gameObject.SetActive(false);
        if (_gm.CurrentPlayerTurnIndex == _gm.PlayerIndex) EventManager.TriggerEvent("EndEvent");
        gameObject.SetActive(false);
    }


    protected void OnDrawGizmosSelected()
    {
        for (int i = 0; i < pedestalList.Count; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pedestalList[i].position + gemVisualOffset, 0.1f);
        }
    }
}
