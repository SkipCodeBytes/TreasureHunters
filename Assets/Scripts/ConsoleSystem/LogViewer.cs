using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogViewer : MonoBehaviour
{
    //public Text consolaTexto; // Asigna un Text UI en el Canvas

    void OnEnable()
    {
        Application.logMessageReceived += CapturarErrores;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= CapturarErrores;
    }

    void CapturarErrores(string log, string stackTrace, LogType type)
    {
        ConsoleScript.writeConsoleMessage( $"\n[{type}] {log}", Color.red);
    }
}
