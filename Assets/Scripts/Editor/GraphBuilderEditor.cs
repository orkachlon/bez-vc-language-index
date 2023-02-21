using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(GraphBuilder))]
    [CanEditMultipleObjects]
    public class GraphBuilderEditor : UnityEditor.Editor {
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
        
            if (GUILayout.Button("Rebuild Graph")) {
                graphBuilder.DeleteGraph();
                graphBuilder.BuildGraph();
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
