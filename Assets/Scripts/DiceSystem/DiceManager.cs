using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    [Header("Config Values")]
    [SerializeField] private Camera diceCamera;
    [SerializeField] private Transform dicePool;
    [SerializeField] private float separationSpace = 0.5f;
    [SerializeField] private float timeToCheck = 1.2f;
    [SerializeField] float fieldViewToResults = 30f;
    [SerializeField] float transicionTime = 0.3f;
    [SerializeField] float waitViewResultsTime = 1.5f;

    [Header("Check Values")]
    [SerializeField] private float fieldViewCamBase;
    [SerializeField] private float checkTimer = 0;
    [SerializeField] private List<DiceScript> diceList;
    [SerializeField] List<DiceScript> chosenDice;
    [SerializeField] private bool isCheckingResults = false;
    [SerializeField] private int resultValue = 0;

    [Header("Test Values")]
    [SerializeField] int diceQuantityProbe = 1;

    //ORDENAR LOS DADOS DE MANERA CIRCULAR

    public int ResultValue { get => resultValue; set => resultValue = value; }

    void Awake()
    {
        List<DiceScript> chosenDice = new List<DiceScript>();
        for (int i = 0; i < dicePool.childCount; i++)
        {
            diceList.Add(dicePool.GetChild(i).GetComponent<DiceScript>());
        }
        fieldViewCamBase = diceCamera.fieldOfView;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HideDices();
            UseDice(diceQuantityProbe);
        }*/

        if (chosenDice.Count > 0 && !isCheckingResults)
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
            StartCoroutine(CinematicAnimation.FieldViewLerp(diceCamera, fieldViewToResults, transicionTime, TransicionFinish));
        }
    }

    private void TransicionFinish()
    {
        //Debug.Log("Resultado es: " + GetDiceValues());
        resultValue = GetDiceValues();
        StartCoroutine(CinematicAnimation.WaitTime(waitViewResultsTime, HideDices));
    }


    private bool AreAllDiceStill()
    {
        bool areAllDiceStill = true;
        for (int i = 0; i < chosenDice.Count; i++)
        {
            if (!chosenDice[i].IsItStill || !chosenDice[i].HasBeenRolled)
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
        for (int i = 0; i < chosenDice.Count; i++)
        {
            resultValue += chosenDice[i].DiceValue;
        }
        return resultValue;
    }


    public void HideDices()
    {
        diceCamera.gameObject.SetActive(false);
        if (chosenDice.Count > 0)
        {
            for (int i = 0; i < chosenDice.Count; i++)
            {
                if (chosenDice[i] == null) return;
                chosenDice[i].gameObject.SetActive(false);
            }
            chosenDice.Clear();
        }
        EventManager.TriggerEvent("DiceManagerFinish", true);
    }

    public void UseDice(int quantity = 1)
    {
        diceCamera.gameObject.SetActive(true);
        diceCamera.fieldOfView = fieldViewCamBase;
        isCheckingResults = false;
        resultValue = 0;
        diceList = diceList.OrderBy(x => Random.value).ToList();
        chosenDice = diceList.Take(quantity).ToList();

        int Rows = Mathf.CeilToInt((float)chosenDice.Count / 3f);
        int currentRow = 0;

        Vector3 dicePosition = Vector3.zero;
        for (int i = 0; i < chosenDice.Count; i++)
        {
            if ((i % 3 == 0) && (i != 0)) currentRow++;
            if (chosenDice[i] == null) continue;
            chosenDice[i].gameObject.SetActive(true);
            chosenDice[i].resetDice(new Vector3(separationSpace * (i - (currentRow * 3)), 0, currentRow * separationSpace));
        }

        int Columns = quantity > 3 ? 3 : quantity;
        dicePool.localPosition = new Vector3(-(Columns - 1) * separationSpace / 2, dicePool.localPosition.y, (-(Rows - 1) * separationSpace / 2) - 0.1f);
    }

}
