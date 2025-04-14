using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameBoardMap
{
    private static GameBoardManager _managerInstance;

    static GameBoardMap() { EditorApplication.update += RunOnceOnStartup; }

    private static void RunOnceOnStartup()
    {
        EditorApplication.update -= RunOnceOnStartup;

        _managerInstance = Object.FindFirstObjectByType<GameBoardManager>();

        if (_managerInstance != null)
        {
            _managerInstance.recoverGameBoard();
        }
    }
}
