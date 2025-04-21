using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapExplorer : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previusButton;
    [SerializeField] private ScrollRect scrollRect;
    private List<TapAccess> _tapsAccess = new List<TapAccess>();
    private GameObject _currentTap;

    public GameObject CurrentTap { get => _currentTap; set => _currentTap = value; }

    private void Start()
    {
        foreach (Transform button in transform)
        {
            if (!button.gameObject.activeInHierarchy) continue;
            TapAccess tapAccess = button.GetComponent<TapAccess>();
            _tapsAccess.Add(tapAccess);
            if (tapAccess.TapScrollView.activeInHierarchy) _currentTap = tapAccess.TapScrollView;
        }
        scrollRect.content = _currentTap.GetComponent<RectTransform>();
    }


    public int getTapOrder(GameObject tapView)
    {
        int tapOrder = -1;
        for (int i = 0; i < _tapsAccess.Count; i++)
        {
            if (_tapsAccess[i].TapScrollView == tapView)
            {
                tapOrder = i;
                break;
            }
        }
        return tapOrder;
    }

    public void nextTap()
    {
        int tapIndex = getTapOrder(_currentTap);
        if (_tapsAccess.Count - 1 > tapIndex)
        {
            tapIndex++;
            _tapsAccess[tapIndex].OpenTap();
        }
        else
        {
            nextButton.interactable = false;
        }
    }
    public void previusTap()
    {
        int tapIndex = getTapOrder(_currentTap);
        if (0 < tapIndex)
        {
            tapIndex--;
            _tapsAccess[tapIndex].OpenTap();
        }
    }

    public void tapExplorerCheck(int index)
    {
        if (0 < index) previusButton.interactable = true;
        else previusButton.interactable = false;
        if (index < _tapsAccess.Count - 1) nextButton.interactable = true;
        else nextButton.interactable = false;

        scrollRect.content = _currentTap.GetComponent<RectTransform>();
    }
}

