using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileBehaviorScript : MonoBehaviour
{
    [SerializeField] private Vector3 _interactionPositionOffset = Vector3.zero;
    [SerializeField] private float _interactionViewRotation = 0f;
    [SerializeField] private List<GameObject> _hideableProps = new List<GameObject>();
    [SerializeField] private List<Vector3> _restPoints = new List<Vector3>();

    private Dictionary<Vector3, BoardPlayer> _restPointDicc = new Dictionary<Vector3, BoardPlayer>();
    private TileBoard _tileBoard;

    public List<GameObject> HideableProps { get => _hideableProps; set => _hideableProps = value; }
    public List<Vector3> RestPoints { get => _restPoints; set => _restPoints = value; }

    protected virtual void Awake()
    {
        _tileBoard = transform.parent.GetComponent<TileBoard>();
    }

    protected virtual void Start()
    {
        HideProps();
    }

    public virtual void ApplyTileEffect()
    {
        UnhideProps();
        Debug.Log("Aplicando efecto");
    }

    public void HideProps()
    {
        for(int i = 0; i < _hideableProps.Count; i++)
        {
            _hideableProps[i].SetActive(false);
        }
    }

    public void UnhideProps()
    {
        for (int i = 0; i < _hideableProps.Count; i++)
        {
            _hideableProps[i].SetActive(true);
        }
    }

    public Vector3 GetInteractionPosition()
    {
        return transform.position + _interactionPositionOffset;
    }

    public Vector3 TakeUpFreeSpace(BoardPlayer boardPlayer)
    {
        for(int i = 0; i < _restPoints.Count; i++)
        {
            if (_restPointDicc.ContainsKey(_restPoints[i]))
            {
                //Buscamos un espcio libre
                if (_restPointDicc[_restPoints[i]] == null) {

                    //Eliminamos si existe este jugador registrado en este tile
                    for (int j = 0; j < _restPoints.Count; j++)
                    {
                        if (!_restPointDicc.ContainsKey(_restPoints[j])) continue;
                        if (_restPointDicc[_restPoints[j]] == boardPlayer)
                        {
                            _restPointDicc[_restPoints[j]] = null;
                        }
                    }
                    _restPointDicc[_restPoints[i]] = boardPlayer;
                    return _restPoints[i];
                }
                //En caso de que el diccionario indique que está ocupado por un jugador, pero se encuentra en otro tile
                else if (_tileBoard != _restPointDicc[_restPoints[i]].CurrentTilePosition) {
                    _restPointDicc[_restPoints[i]] = boardPlayer;
                    return _restPoints[i];
                } else continue;
            } else
            {
                //En caso de que no exista este key, crea un nuevo (Armando el diccionario)
                _restPointDicc[_restPoints[i]] = null;

                //Eliminamos si existe este jugador registrado en este tile
                for (int j = 0; j < _restPoints.Count; j++)
                {
                    if (!_restPointDicc.ContainsKey(_restPoints[j])) continue;
                    if (_restPointDicc[_restPoints[j]] == boardPlayer)
                    {
                        _restPointDicc[_restPoints[j]] = null;
                    }
                }

                return _restPoints[i];
            }
        }
        Debug.LogWarning("No hay espacios libres");
        return Vector3.zero;
    }

    public void LeaveFreeSpace(BoardPlayer boardPlayer)
    {
        for (int i = 0; i < _restPoints.Count; i++)
        {
            if (_restPointDicc[_restPoints[i]] == boardPlayer)
            {
                _restPointDicc[_restPoints[i]] = null;
                return;
            }
        }

        Debug.Log("No existe este personaje en descanso");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + _interactionPositionOffset, 0.2f);

        Gizmos.color = Color.green;
        for (int i = 0; i < _restPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(transform.position + _restPoints[i], 0.2f);
        }

        float radians = _interactionViewRotation * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, transform.position + _interactionPositionOffset, Quaternion.LookRotation(direction), 0.5f, EventType.Repaint);
    }
#endif
}
