using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExplorablePanel))]
public class InteractueToNext : MonoBehaviour
{
    ExplorablePanel _explorablePanel;
    void Start() => _explorablePanel = GetComponent<ExplorablePanel>();
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) _explorablePanel.NextPanel();
    }
}
