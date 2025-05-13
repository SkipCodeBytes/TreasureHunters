using System.Collections.Generic;
using UnityEngine;

public class UiDicePanel : MonoBehaviour
{
    [SerializeField] private GameObject TurnPanel;
    [SerializeField] private GameObject NoTurnPanel;

    [SerializeField] private List<RectTransform> leftDirectionAnimation = new List<RectTransform>();
    [SerializeField] private List<RectTransform> rightDirectionAnimation = new List<RectTransform>();

    [SerializeField] private float speedAnimation;
    [SerializeField] private float maxDistance;

    public void OpenTurnPanel()
    {
        TurnPanel.SetActive(true);
        NoTurnPanel.SetActive(false);
    }
    public void OpenNoTurnPanel()
    {
        TurnPanel.SetActive(false);
        NoTurnPanel.SetActive(true);
    }


    private void Update()
    {
        if (!TurnPanel.activeInHierarchy) return;
        for (int i = 0; i < leftDirectionAnimation.Count; i++) 
        {
            leftDirectionAnimation[i].localPosition += Vector3.left * speedAnimation * Time.deltaTime;
            if (leftDirectionAnimation[i].localPosition.x < -maxDistance) leftDirectionAnimation[i].localPosition = new Vector3(maxDistance * 3, 0);
        }

        for (int i = 0; i < rightDirectionAnimation.Count; i++)
        {
            rightDirectionAnimation[i].localPosition += Vector3.right * speedAnimation * Time.deltaTime;
            if (rightDirectionAnimation[i].localPosition.x > maxDistance) rightDirectionAnimation[i].localPosition = new Vector3(-maxDistance * 3, 0);
        }

    }
}
