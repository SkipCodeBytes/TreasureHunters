using UnityEngine;
using UnityEngine.EventSystems;

public class ClicToClose : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
