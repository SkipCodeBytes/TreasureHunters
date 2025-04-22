using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private Canvas _canvas;
    private Vector2 _offset;
    private RectTransform _panelRectTransform;

    void Awake()
    {
        _panelRectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Convertimos la posición del mouse a posición local dentro del panel
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _panelRectTransform,
            eventData.position,
            eventData.pressEventCamera, // clave en Screen Space - Camera
            out Vector2 localMousePosition
        );

        _offset = _panelRectTransform.anchoredPosition - localMousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convertimos la posición del mouse a local dentro del padre del panel
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _panelRectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        _panelRectTransform.anchoredPosition = localPoint + _offset;
    }
}
