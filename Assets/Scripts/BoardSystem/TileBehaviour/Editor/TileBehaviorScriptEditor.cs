using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(BasicTileScript))]
public class TileBehaviorScriptEditor : Editor
{
    private TileBehavior _basicTileScript;
    public override void OnInspectorGUI()
    {
        _basicTileScript = (TileBehavior)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate default lists"))
        {
            _basicTileScript.RestPoints.Clear();
            _basicTileScript.RestPoints.Add(new Vector3(0.75f, 0, 0.75f));
            _basicTileScript.RestPoints.Add(new Vector3(0.75f, 0, -0.75f));
            _basicTileScript.RestPoints.Add(new Vector3(-0.75f, 0, 0.75f));
            _basicTileScript.RestPoints.Add(new Vector3(-0.75f, 0, -0.75f));

            _basicTileScript.HideableProps.Clear();

            for (int i = 0; i < _basicTileScript.transform.GetChild(0).childCount; i++) 
            {
                _basicTileScript.HideableProps.Add(_basicTileScript.transform.GetChild(0).GetChild(i).gameObject);
            }
            EditorUtility.SetDirty(_basicTileScript);
        }
    }

    
}
