using System;
using UnityEngine;

public class CardViewUI : MonoBehaviour
{
    [SerializeField] private GUICard cardPrefab;

    [SerializeField] private RectTransform centerView;
    [SerializeField] private RectTransform rightView;
    [SerializeField] private RectTransform leftView;

    [SerializeField] private float transicionInitDuration = 0.8f;
    [SerializeField] private float waitDuration = 0.5f;
    [SerializeField] private float transicionEndDuration = 0.8f;
    [SerializeField] private Vector3 scaleZoomAnim = new Vector3(1.5f, 1.5f, 1f);

    [SerializeField] private Vector3 centerCardView;
    [SerializeField] private Vector3 rightCardView;
    [SerializeField] private Vector3 leftCardView;


    private Action callback;

    public void StartCardView(Action callbackAfterClose = null)
    {
        callback = callbackAfterClose;

        foreach (Transform child in centerView)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in rightView)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in leftView)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetCardView(CardItemData cardData, int position = 0)
    {
        GUICard cardGUI = Instantiate(cardPrefab.gameObject, centerView).GetComponent<GUICard>();

        switch (position)
        {
            case 1:
                cardGUI.GetComponent<RectTransform>().SetParent(rightView);
                break;

            case 2:
                cardGUI.GetComponent<RectTransform>().SetParent(leftView);
                break;

            default:
                cardGUI.GetComponent<RectTransform>().SetParent(centerView);
                break;

        }
        cardGUI.IsInteractable = false;
        cardGUI.SetData(cardData);
    }

    public void PlayAnimation()
    {
        SoundController.Instance.PlaySound(GameManager.Instance.SoundLibrary.GetClip("ShowCards"));

        centerView.anchoredPosition = new Vector3(0f, -800f);
        StartCoroutine(CinematicAnimation.UiMoveTo(centerView, centerCardView, transicionInitDuration));
        StartCoroutine(LerpUtils.LerpVector3(x => centerView.localScale = x, new Vector3(0.75f, 0.75f, 0.75f), scaleZoomAnim, transicionInitDuration));


        leftView.anchoredPosition = new Vector3(-1200f, -180f);
        StartCoroutine(CinematicAnimation.UiMoveTo(leftView, leftCardView, transicionInitDuration));
        StartCoroutine(LerpUtils.LerpVector3(x => leftView.localScale = x, new Vector3(0.75f, 0.75f, 0.75f), scaleZoomAnim, transicionInitDuration));


        rightView.anchoredPosition = new Vector3(1200f, -180f);
        StartCoroutine(CinematicAnimation.UiMoveTo(rightView, rightCardView, transicionInitDuration));
        StartCoroutine(LerpUtils.LerpVector3(x => rightView.localScale = x, new Vector3(0.75f, 0.75f, 0.75f), scaleZoomAnim, transicionInitDuration));

        StartCoroutine(CinematicAnimation.WaitTime(transicionInitDuration + waitDuration, EndAnimation));
    }

    private void EndAnimation()
    {
        SoundController.Instance.PlaySound(GameManager.Instance.SoundLibrary.GetClip("Owl"));

        StartCoroutine(CinematicAnimation.UiMoveTo(centerView, new Vector3(0f, 800f), transicionEndDuration));
        StartCoroutine(CinematicAnimation.UiMoveTo(leftView, new Vector3(leftCardView.x, 800f), transicionEndDuration));
        StartCoroutine(CinematicAnimation.UiMoveTo(rightView, new Vector3(rightCardView.x, 800f), transicionEndDuration));

        StartCoroutine(LerpUtils.LerpVector3(x => centerView.localScale = x, scaleZoomAnim, new Vector3(0.25f, 0.25f, 0.25f), transicionInitDuration));
        StartCoroutine(LerpUtils.LerpVector3(x => leftView.localScale = x, scaleZoomAnim, new Vector3(0.25f, 0.25f, 0.25f), transicionInitDuration));
        StartCoroutine(LerpUtils.LerpVector3(x => rightView.localScale = x, scaleZoomAnim, new Vector3(0.25f, 0.25f, 0.25f), transicionInitDuration));

        StartCoroutine(CinematicAnimation.WaitTime(transicionEndDuration, CloseCardView));
    }

    private void CloseCardView()
    {
        callback?.Invoke();
        gameObject.SetActive(false);
    }
}
