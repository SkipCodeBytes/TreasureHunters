using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapExplorer : MonoBehaviour
{
    [SerializeField] private Color selectedTapColor = Color.gray;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previusButton;
    [SerializeField] private ScrollRect scrollRect;
    private List<TapAccess> _tapsAccessList = new List<TapAccess>();
    private GameObject _currentTapContent;

    public GameObject CurrentTapContent { get => _currentTapContent; set => _currentTapContent = value; }
    public Color SelectedTapColor { get => selectedTapColor; set => selectedTapColor = value; }
    public List<TapAccess> TapsAccessList { get => _tapsAccessList; set => _tapsAccessList = value; }

    private void Start()
    {
        foreach (Transform button in transform)
        {
            if (!button.gameObject.activeInHierarchy) continue;
            TapAccess tapAccess = button.GetComponent<TapAccess>();
            _tapsAccessList.Add(tapAccess);
            if (tapAccess.TapScrollView.activeInHierarchy)
            { 
                _currentTapContent = tapAccess.TapScrollView;
                //tapAccess.GetComponent<Image>().color = selectedTapColor;
            }
        }
        scrollRect.content = _currentTapContent.GetComponent<RectTransform>();
    }


    public int getTapOrder(GameObject tapView)
    {
        int tapOrder = -1;
        for (int i = 0; i < _tapsAccessList.Count; i++)
        {
            if (_tapsAccessList[i].TapScrollView == tapView)
            {
                tapOrder = i;
                break;
            }
        }
        return tapOrder;
    }

    public void nextTap()
    {
        int tapIndex = getTapOrder(_currentTapContent);
        if (_tapsAccessList.Count - 1 > tapIndex)
        {
            tapIndex++;
            _tapsAccessList[tapIndex].OpenTap();
        }
        else
        {
            nextButton.interactable = false;
        }
    }
    public void previusTap()
    {
        int tapIndex = getTapOrder(_currentTapContent);
        if (0 < tapIndex)
        {
            tapIndex--;
            _tapsAccessList[tapIndex].OpenTap();
        }
    }

    public void tapExplorerCheck(int index)
    {
        if (0 < index) previusButton.interactable = true;
        else previusButton.interactable = false;
        if (index < _tapsAccessList.Count - 1) nextButton.interactable = true;
        else nextButton.interactable = false;

        scrollRect.content = _currentTapContent.GetComponent<RectTransform>();
    }
}

