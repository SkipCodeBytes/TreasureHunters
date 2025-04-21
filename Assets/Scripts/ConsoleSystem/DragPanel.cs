using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 offset;
    private RectTransform panelRectTransform;

    void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = (Vector2)panelRectTransform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        panelRectTransform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData) { }
}
