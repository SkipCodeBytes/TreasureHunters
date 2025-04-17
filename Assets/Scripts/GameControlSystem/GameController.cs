using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private InputSystem_Actions _gameControls;

    public InputSystem_Actions GameControls { get => _gameControls; }


    private void Awake() {
        if(Instance == null)
        {
            Instance = this;
            _gameControls = new InputSystem_Actions();
        } else
        {
            Destroy(Instance);
        }
    }

    private void OnEnable() { _gameControls.Enable(); }
    private void OnDisable() { _gameControls.Disable(); }

}
