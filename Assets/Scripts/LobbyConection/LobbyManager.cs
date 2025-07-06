using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private AudioSlider audioSlider;
    void Start()
    {
        audioSlider.GetCurrentVolume();
    }
}
