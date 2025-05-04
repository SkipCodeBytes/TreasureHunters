using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiRoomOptionSelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Material selectionMaterial;
    private string _roomName;
    private Image _imageBackground;
    private Material _defaultMaterial;

    public string RoomName { get => _roomName; set => _roomName = value; }

    private void Start()
    {
        _imageBackground = GetComponent<Image>();
        _defaultMaterial = _imageBackground.material;
    }

    public void SetDefaultMaterial()
    {
        _imageBackground.material = _defaultMaterial;
    }

    //private void room;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (NetworkSearchRoom.CursorSelection != null)
            NetworkSearchRoom.CursorSelection.GetComponent<UiRoomOptionSelector>().SetDefaultMaterial();
        NetworkSearchRoom.CursorSelection = this.GetComponent<RectTransform>();
        NetworkSearchRoom.CursorSelection.GetComponent<Image>().material = selectionMaterial;
    }
}
