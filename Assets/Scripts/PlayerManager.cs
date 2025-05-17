using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Values - ReadOnly")]

    [SerializeField] private bool isPlayerTurn = false;
    [SerializeField] private CharacterData selectedCharacter;

    public bool IsPlayerTurn { get => isPlayerTurn; set => isPlayerTurn = value; }
    public CharacterData SelectedCharacter { get => selectedCharacter; set => selectedCharacter = value; }


    //Datos de jugador
    private Player _player;

    public Player Player { get => _player; set => _player = value; }

    //Componentes del Player
    private PhotonView _view;
    private PlayerRPC _RPCScript;
    private PlayerRules _rules;
    private PlayerGraphics _graphics;
    private PlayerInventory _inventory;
    private BoardPlayer _boardPlayer;

    public PhotonView View { get => _view; set => _view = value; }
    public PlayerRPC RPCScript { get => _RPCScript; set => _RPCScript = value; }
    public PlayerRules Rules { get => _rules; set => _rules = value; }
    public PlayerGraphics Graphics { get => _graphics; set => _graphics = value; }
    public PlayerInventory Inventory { get => _inventory; set => _inventory = value; }
    public BoardPlayer BoardPlayer { get => _boardPlayer; set => _boardPlayer = value; }


    //Referencias externas
    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;

        _view = GetComponent<PhotonView>();
        _RPCScript = GetComponent<PlayerRPC>();
        _rules = GetComponent<PlayerRules>();
        _graphics = GetComponent<PlayerGraphics>();
        _inventory = GetComponent<PlayerInventory>();
        _boardPlayer = GetComponent<BoardPlayer>();
    }


    private void Start()
    {
        if (_view.IsMine)
        {
            _view.RPC("SharePlayerReady", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
    }
}
