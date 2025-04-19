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

    [Header("Check Values")]
    [SerializeField] private float checkTimer = 0;
    [SerializeField] private List<DiceScript> diceList;
    [SerializeField] List<DiceScript> chosenDice;

    [Header("Test Values")]
    [SerializeField] int diceQuantityProbe = 1;


    void Awake()
    {
        List<DiceScript> chosenDice = new List<DiceScript>();
        for (int i = 0; i < dicePool.childCount; i++)
        {
            diceList.Add(dicePool.GetChild(i).GetComponent<DiceScript>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HideDices();
            PrepareDice(diceQuantityProbe);
        }

        if (chosenDice.Count > 0)
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
            Debug.Log("Resultado es: " + GetDiceValues());
            HideDices();
        }
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
        if (chosenDice.Count > 0)
        {
            for (int i = 0; i < chosenDice.Count; i++)
            {
                if (chosenDice[i] == null) return;
                chosenDice[i].gameObject.SetActive(false);
            }
            chosenDice.Clear();
        }
    }

    public void PrepareDice(int quantity = 1)
    {
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
