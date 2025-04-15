using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    /*
     * Se implementa este código en un objeto en el juego
     * 
     * Se pueden utilizar 3 acciones para implementar este código
     * 
     * - Suscribirse al evento          StartListening(string eventName, Action listener, int priority = 0)
     * - Desuscribirse al evento        StopListening(string eventName, Action listener)
     * - Lanzar evento                  TriggerEvent(string eventName)
     */

    private Dictionary<string, SortedDictionary<int, List<Action>>> _eventDictionary;

    private static EventManager _eventManager;

    public static EventManager instance
    {
        get
        {
            if (!_eventManager)
            {
                _eventManager = FindFirstObjectByType(typeof(EventManager)) as EventManager;

                if (!_eventManager)
                {
                    Debug.LogError("Necesitas un EventManager en la escena.");
                }
                else
                {
                    _eventManager.Init();
                }
            }

            return _eventManager;
        }
    }

    public Dictionary<string, SortedDictionary<int, List<Action>>> EventDictionary { get => _eventDictionary; set => _eventDictionary = value; }

    void Init()
    {
        if (_eventDictionary == null)
        {
            _eventDictionary = new Dictionary<string, SortedDictionary<int, List<Action>>>();
        }
    }

    public static void StartListening(string eventName, Action listener, int priority = 0)
    {
        if (!instance._eventDictionary.TryGetValue(eventName, out SortedDictionary<int, List<Action>> priorityDict))
        {
            //En caso de que no logre obtener "priorityDict", crea uno nuevo
            priorityDict = new SortedDictionary<int, List<Action>>();
            instance._eventDictionary.Add(eventName, priorityDict);
        }

        if (!priorityDict.TryGetValue(priority, out List<Action> actionList))
        {
            //En caso de que no logre obtener "actionList", crea uno nuevo
            actionList = new List<Action>();
            priorityDict.Add(priority, actionList);
        }

        //A�ade el "listener a la lista de acciones"
        actionList.Add(listener);
    }


    public static void StopListening(string eventName, Action listener)
    {
        if (_eventManager == null) return;

        if (instance._eventDictionary.TryGetValue(eventName, out SortedDictionary<int, List<Action>> priorityDict))
        {
            foreach (var actionList in priorityDict.Values)
            {
                if (actionList.Contains(listener))
                {
                    actionList.Remove(listener);
                    break;
                }
            }
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if (instance._eventDictionary.TryGetValue(eventName, out SortedDictionary<int, List<Action>> priorityDict))
        {
            foreach (var actionList in priorityDict.Values)
            {
                foreach (Action action in actionList)
                {
                    action?.Invoke();
                }
            }
        }
    }

}
