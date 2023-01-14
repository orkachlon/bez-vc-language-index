using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[ExecuteInEditMode]
public class GraphPlacer : MonoBehaviour {
    private const string PathToGraphJson = "./Assets/Graphs/TestGraph.json";
    [SerializeField]
    private float heightFactor = 1f;
    [SerializeField]
    private float radiusFactor = 1f;
    
    [Serializable]
    private class LanguageData {
        public string name;
        public string years;
        public string ancestor;
        public List<string> parents;
        public List<string> extinctParents;
        public string pathToMap;
        public string pathToAlphabet;
        public List<string> influences;

        public override string ToString() {
            return $"Name: {name},\n" +
                   $"Years: {years},\n" +
                   $"Ancestor: {ancestor},\n" +
                   $"Parents: [{string.Join(", ", parents)}],\n" +
                   $"Extinct Parents: [{string.Join(", ", extinctParents)}],\n" +
                   $"Influences: [{string.Join(", ", influences)}]";
        }
    }
    
    // Start is called before the first frame update
    void Start() {
        // validate json path
        if (!File.Exists(PathToGraphJson)) {
            return;
        }

        // read json and validate
        var jsonText = File.ReadAllText(PathToGraphJson);
        if (jsonText is { Length: 0 }) { // this checks for null
            return;
        }

        // create objects from json
        var langNodeDict = JsonConvert.DeserializeObject<Dictionary<string, List<LanguageData>>>(jsonText);
        if (langNodeDict == null) {
            return;
        }
        foreach (var (treeLevel, langList) in langNodeDict) {
            print(treeLevel);
            print(string.Join("\n", langList));
        }
        
        // instantiate and place nodes in space
        foreach (var (treeLevel, langList) in langNodeDict) {
            var level = int.Parse(treeLevel);
            var levelContainer = new GameObject(treeLevel) {
                transform = {
                    position = Vector3.zero
                }
            };
            for (var i = 0; i < langList.Count; i++) {
                var langSphere = PlaceNode(level, langList.Count, i, langList[i]);
                if (langSphere == null) {
                    continue;
                }
                langSphere.transform.parent = levelContainer.transform;
            }
        }
    }

    private GameObject PlaceNode(int level, int numOfSiblings, int siblingIndex, LanguageData langData) {
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 1 || langData == null) {
            return null;
        }
        
        var radius = (numOfSiblings - 1) * radiusFactor;
        var angle = Mathf.PI * 2 * siblingIndex / numOfSiblings;
        var x = radius * Mathf.Cos(angle);
        var y = level * heightFactor;
        var z = radius * Mathf.Sin(angle);
        var nodeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nodeSphere.name = langData.name;
        nodeSphere.transform.position = new Vector3(x, y, z);
        return nodeSphere;
    }
}
