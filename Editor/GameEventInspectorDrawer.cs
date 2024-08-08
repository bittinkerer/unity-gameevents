using Packages.Estenis.GameEvent_;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEventObject))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (GameEventObject)target;

        if (GUILayout.Button("Trigger", GUILayout.Height(40)))
        {
            
        }

    }
}