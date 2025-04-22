using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderAdvancedEvents : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerUpHandler
{
    private Slider _slider;
    [SerializeField] private string beginDragEvent = string.Empty;
    [SerializeField] private string clicEvent = string.Empty;
    [SerializeField] private string endDragEvent = string.Empty;
    [SerializeField] private float btnChangeValueRate = 0.05f;

    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (beginDragEvent != string.Empty) EventManager.TriggerEvent("SliderBegin" + beginDragEvent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (clicEvent != string.Empty) EventManager.TriggerEvent("SliderClic" + clicEvent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (endDragEvent != string.Empty) EventManager.TriggerEvent("SliderEnd" + endDragEvent);
    }

    public void btnDecreaseValue()
    {
        if(_slider.value - btnChangeValueRate > _slider.minValue)  _slider.value -= btnChangeValueRate;
        else _slider.value = _slider.minValue;
        EventManager.TriggerEvent("SliderEnd" + endDragEvent);
    }
    public void btnIncreaseValue()
    {
        if (_slider.value + btnChangeValueRate < _slider.maxValue) _slider.value += btnChangeValueRate;
        else _slider.value = _slider.maxValue;
        EventManager.TriggerEvent("SliderEnd" + endDragEvent);
    }
}