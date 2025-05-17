using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

public class DiceManager : MonoBehaviour
{
    [Header("Config Values")]
    [SerializeField] private Camera diceCamera;
    [SerializeField] private UiDicePanel diceCanvas;
    [SerializeField] private Transform dicePool;
    [SerializeField] private float separationRadiusBase = 0.5f;
    [SerializeField] private float timeToCheck = 1.2f;
    [SerializeField] float fieldViewToResults = 30f;
    [SerializeField] float transicionTime = 0.3f;
    [SerializeField] float waitViewResultsTime = 1.5f;

    [Header("Check Values")]
    [SerializeField] private float fieldViewCamBase;
    [SerializeField] private float checkTimer = 0;
    [SerializeField] private List<DiceScript> diceList;
    [SerializeField] List<DiceScript> chosenDiceList;
    [SerializeField] private bool isCheckingResults = false;
    [SerializeField] private int resultValue = 0;
    [SerializeField] private int ownerPlayerIndex = 0;

    private GameManager _gm;
    public UiDicePanel DiceCanvas { get => diceCanvas; set => diceCanvas = value; }

    void Awake()
    {
        _gm = GameManager.Instance;
        List<DiceScript> chosenDice = new List<DiceScript>();
        for (int i = 0; i < dicePool.childCount; i++)
        {
            diceList.Add(dicePool.GetChild(i).GetComponent<DiceScript>());
            dicePool.GetChild(i).gameObject.SetActive(false);
        }
        fieldViewCamBase = diceCamera.fieldOfView;
    }

    private void Update()
    {
        if (ownerPlayerIndex != _gm.PlayerIndex) return;
        if (chosenDiceList.Count > 0 && !isCheckingResults)
        {
            CheckDiceStatus();
        }
    }

    private void CheckDiceStatus()
    {
        if (AreAllDiceStill()) checkTimer += Time.deltaTime;
        else checkTimer = 0;

        if (checkTimer >= timeToCheck)
        {
            isCheckingResults = true;
            resultValue = GetDiceValues();
            _gm.GmView.RPC("SentDiceResults", RpcTarget.All, resultValue);
        }
    }

    public void EndAnimationFocusDices()
    {
        StartCoroutine(CinematicAnimation.FieldViewLerp(diceCamera, fieldViewToResults, transicionTime, TransicionFinish));
    }

    private void TransicionFinish()
    {
        StartCoroutine(CinematicAnimation.WaitTime(waitViewResultsTime, HideDices));
    }


    private bool AreAllDiceStill()
    {
        bool areAllDiceStill = true;
        for (int i = 0; i < chosenDiceList.Count; i++)
        {
            if (!chosenDiceList[i].IsItStill || !chosenDiceList[i].HasBeenRolled)
            {
                areAllDiceStill = false;
                break;
            }
        }
        return areAllDiceStill;
    }

    private int GetDiceValues()
    {
        int resultValue = 0;
        for (int i = 0; i < chosenDiceList.Count; i++)
        {
            resultValue += chosenDiceList[i].DiceValue;
        }
        return resultValue;
    }


    public void HideDices()
    {
        diceCamera.gameObject.SetActive(false);
        if (chosenDiceList.Count > 0)
        {
            for (int i = 0; i < chosenDiceList.Count; i++)
            {
                if (chosenDiceList[i] == null) return;
                chosenDiceList[i].gameObject.SetActive(false);
            }
            chosenDiceList.Clear();
        }
        EventManager.TriggerEvent("EndEvent");
    }

    public void UseDice(int playerTargetIndex, int quantity = 1)
    {
        ownerPlayerIndex = playerTargetIndex;
        if (quantity < 1) quantity = 1;
        diceCamera.gameObject.SetActive(true);
        diceCamera.fieldOfView = fieldViewCamBase;
        isCheckingResults = false;
        resultValue = 0;

        chosenDiceList = diceList.Take(quantity).ToList();

        float angleSeparation = 360.0f / quantity;
        float radius = separationRadiusBase + 0.02f * quantity;
        for (int i = 0; i < chosenDiceList.Count; i++)
        {
            if (chosenDiceList[i] == null) continue;

            float angleDeg = angleSeparation * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 position = new Vector3(Mathf.Cos(angleRad) * radius, 0f, Mathf.Sin(angleRad) * radius);
            chosenDiceList[i].gameObject.SetActive(true);
            chosenDiceList[i].ResetDice(position);
            chosenDiceList[i].ChangeOwner(playerTargetIndex);
        }
    }

}
