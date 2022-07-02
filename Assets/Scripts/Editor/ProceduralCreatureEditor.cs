using UnityEngine;
using UnityEditor;
using Assets.Scripts.Creatures;

[CustomEditor(typeof(ProceduralCreature))]
public class ProceduralCreatureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ProceduralCreature script = (ProceduralCreature) target;
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
    }
}