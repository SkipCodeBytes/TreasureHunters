using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    // Event name
    // Event action
    // Suscriptores

    private EventManager _eventManager;
    private List<string> _events = new List<string>();


    private void OnEnable()
    {
        _eventManager = (EventManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("EVENT LIST", EditorStyles.boldLabel);
            refreshKeysDicctionary();
            for (int i = 0; i < _events.Count; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(_events[i], EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                if (_eventManager.EventDictionary.TryGetValue(_events[i], out SortedDictionary<int, List<Action>> actionsSortedList))
                {
                    for (int j = 0; j < actionsSortedList.Count; j++)
                    {
                        foreach (int priorityKey in actionsSortedList.Keys)
                        {
                            if (actionsSortedList.TryGetValue(priorityKey, out List<Action> actionList))
                            {
                                for (int k = 0; k < actionList.Count; k++)
                                {
                                    EditorGUILayout.LabelField(priorityKey + " _    SCRIPT: " + actionList[k].Method.DeclaringType.ToString() + "     METHOD: " + actionList[k].Method.Name + "()");
                                }
                            }
                        }
                    }

                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }
    }

    private void refreshKeysDicctionary()
    {
        if (_eventManager.EventDictionary != null)
        {
            _events = new List<string>();
            foreach (string key in _eventManager.EventDictionary.Keys)
            {
                _events.Add(key);
            }
        }
    }
}
