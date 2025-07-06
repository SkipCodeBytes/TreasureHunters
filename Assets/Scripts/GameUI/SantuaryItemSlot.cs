using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SantuaryItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Image gemImage;
    private Color _defaultColor;
    private Image _background;

    public GemItemData gemData;
    public bool IsSelected = false;

    private RuinPanelUI _ruinUI;

    public void SetElement(RuinPanelUI ui, GemItemData data)
    {
        _ruinUI = ui;
        _background = GetComponent<Image>();
        _defaultColor = _background.color;
        gemData = data;
        gemImage.sprite = data.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelected) 
        {
            _ruinUI.RemoveSelectElement(this);
        }
        else
        {
            _ruinUI.AddToSelectElement(this);
        }
    }

    public void SetToSelectedElement()
    {
        _background.color = selectedColor;
        IsSelected = true;
    }

    public void SetToUnselectedElement()
    {
        _background.color = _defaultColor;
        IsSelected = false;
    }

}