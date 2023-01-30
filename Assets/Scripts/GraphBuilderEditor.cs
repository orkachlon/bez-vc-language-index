using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphBuilder))]
[CanEditMultipleObjects]
public class GraphBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        var graphBuilder = target as GraphBuilder;
        if (!graphBuilder) {
            return;
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Build Graph")) {
            graphBuilder.BuildGraph();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (GUILayout.Button("Delete Graph")) {
            graphBuilder.DeleteGraph();
            serializedObject.ApplyModifiedProperties();
            return;
        }
    }
}
