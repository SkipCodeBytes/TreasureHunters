using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private Camera diceCamera;
    [SerializeField] private Transform dicePool;
    [SerializeField] private float separationSpace = 0.5f;

    [SerializeField] private List<DiceScript> diceList;
    [SerializeField] List<DiceScript> chosenDice;

    [SerializeField] int diceQuantityProbe = 1;


    void Start()
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
        Debug.Log(Rows);
        Debug.Log(Columns);
        dicePool.localPosition = new Vector3(-(Columns -1) * separationSpace/2, dicePool.localPosition.y, (-(Rows - 1) * separationSpace/2) - 0.1f) ;
    }

}
