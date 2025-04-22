using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableBar : MonoBehaviour
{
    private List<string> optionStringsList = new List<string>();
    [SerializeField] private Button btnNextOption;
    [SerializeField] private Button btnPrevOption;
    [SerializeField] private Button btnApply;
    private Text txtView;
    private Action prevBtnAction;
    private Action nextBtnAction;
    private Action applyBtnAction;
    private int explorerIndex = 0;

    public int ExplorerIndex
    {
        get => explorerIndex;
        set
        {
            explorerIndex = value;
            navigationCheck();
        }
    }


    private void Awake()
    {
        txtView = btnApply.transform.GetChild(0).GetComponent<Text>();
    }

    //CONFIGURAR la lista
    public void setOptionList(List<string> stringList, int defaultIndex = 0)
    {
        optionStringsList = stringList;
        explorerIndex = defaultIndex;
        navigationCheck();
    }

    //CONFIGURAR las acciones tras usar los botones
    public void setActionEvents(Action applyBtn = null, Action prevBtn = null, Action nextBtn = null)
    {
        prevBtnAction = prevBtn;
        nextBtnAction = nextBtn;
        applyBtnAction = applyBtn;
    }

    //EVENTO DE BOTÓN SIGUIENTE
    public void nextOption()
    {
        if (explorerIndex < optionStringsList.Count - 1)
        {
            explorerIndex++;
            navigationCheck();
            if(nextBtnAction != null) nextBtnAction?.Invoke();
        }
    }

    //EVENTO DE BOTÓN ANTERIOR
    public void prevOption()
    {
        if (explorerIndex > 0)
        {
            explorerIndex--;
            navigationCheck();
            if(prevBtnAction != null) prevBtnAction?.Invoke();
        }
    }

    private void navigationCheck()
    {
        if (explorerIndex == 0) { btnPrevOption.interactable = false; }
        else { btnPrevOption.interactable = true; }
        if (explorerIndex + 1 < optionStringsList.Count) { btnNextOption.interactable = true; }
        else { btnNextOption.interactable = false; }
        if(optionStringsList.Count > 0) txtView.text = optionStringsList[explorerIndex];
    }

    //EVENTO DE BOTÓN APLICAR
    public void applySelection() { 
        if(applyBtnAction != null) applyBtnAction?.Invoke();
        Debug.Log("Apply Selection");
    }
}
