using System.Collections.Generic;
using UnityEngine;

public class MomentManager : MonoBehaviour
{
    [Header("Moment Systems")]
    [SerializeField] private Moment currentMoment;
    [SerializeField] private List<Moment> momentList;

    [SerializeField] private bool isMomentRunnning = false;
    [SerializeField] private bool isWaitingForEvent = false;
    [SerializeField] private bool isWaitingForSyncro = false;

    [SerializeField] private bool isMomentManagerFree = false;

    [Header("Debug and Testing options")]
    [SerializeField] private bool stepMomentMode = false;

    public Moment CurrentMoment { get => currentMoment; set => currentMoment = value; }
    public List<Moment> MomentList { get => momentList; set => momentList = value; }
    public bool IsMomentRunnning { get => isMomentRunnning; set => isMomentRunnning = value; }
    public bool IsWaitingForEvent { get => isWaitingForEvent; set => isWaitingForEvent = value; }
    public bool IsWaitingForSyncro { get => isWaitingForSyncro; set => isWaitingForSyncro = value; }
    public bool IsMomentManagerFree { get => isMomentManagerFree; set => isMomentManagerFree = value; }

    public void MomentUpdate()
    {
        if (isWaitingForEvent) return;
        if (isMomentRunnning) return;
        if (isWaitingForSyncro) return;

        if (momentList.Count > 0)
        {
            isMomentManagerFree = false;
            if (stepMomentMode) { if (Input.GetKeyDown(KeyCode.Q)) ReadNextMoment(); }
            else ReadNextMoment();
        }
        else
        {
            isMomentManagerFree = true;
        }
    }

    //Brinda oportunidad de leer el siguiente momento
    public void ReadNextMoment()
    {
        if (momentList[0] != null)
        {
            currentMoment = momentList[0];
            momentList.RemoveAt(0);
            isMomentRunnning = true;
            currentMoment.PlayMoment();
        }
        else momentList.RemoveAt(0);
    }

    //Cancela un momento para ir por otro
    public void ReplaceCurrentMoment(Moment gameMoment)
    {
        isMomentRunnning = false;
        momentList.Insert(0, gameMoment);
        currentMoment.CancelMoment();
    }

    //Pospone el momento actual para ir por otro
    public void InterveneCurrentMoment(Moment gameMoment)
    {
        isMomentRunnning = false;
        momentList.Insert(0, currentMoment);
        momentList.Insert(0, gameMoment);
        currentMoment.CancelMoment();
    }
}
