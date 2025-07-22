using UnityEngine;

[CreateAssetMenu(fileName = "TutorialPanel", menuName = "Tutorial/Tutorial Panel", order = 0)]
public class TutorialPanelData : ScriptableObject
{
    [Header("Contenido del Tutorial")]
    public string titulo;
    [TextArea(3, 10)]
    public string descripcion;
    public Sprite imagen;

}
