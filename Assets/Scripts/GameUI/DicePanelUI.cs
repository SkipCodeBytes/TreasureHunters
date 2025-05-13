using UnityEngine;

public class DicePanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform Deco_1;
    [SerializeField] private RectTransform Deco_2;
    [SerializeField] private Vector3 rotationSpeed;

    void Update()
    {
        Deco_1.rotation = Quaternion.Euler(Deco_1.rotation.eulerAngles + rotationSpeed * Time.deltaTime);
        Deco_2.rotation = Quaternion.Euler(Deco_2.rotation.eulerAngles - rotationSpeed * Time.deltaTime);
    }
}
