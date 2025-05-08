using UnityEngine;

public class GemScript : MonoBehaviour, IItemObject
{
    [SerializeField] private GemForm gemForm;
    [SerializeField] private GemColor gemColor;

    public void setItemObject(GameItem gameItemData)
    {
        gemForm = gameItemData.GemForm;
        gemColor = gameItemData.GemColor;
    }
}
