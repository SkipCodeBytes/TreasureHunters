using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private GameBoardManager _boardManager;
    [SerializeField] private BoardPlayer currentPlayerTurn;

    public static GameManager Instance { get => _instance; set => _instance = value; }
    public GameBoardManager BoardManager { get => _boardManager; set => _boardManager = value; }

    private void Awake()
    {
        if(_instance == null) _instance = this;
        else Destroy(this);

    }


    private void Start()
    {
        
    }

}
