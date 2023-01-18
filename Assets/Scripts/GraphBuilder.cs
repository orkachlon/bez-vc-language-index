using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[ExecuteAlways]
public class GraphBuilder : MonoBehaviour {
    private const string PathToGraphJson = "./Assets/Graphs/LanguageGraph.json";
    [SerializeField]
    private float heightFactor = 1f;
    [SerializeField]
    private float radiusFactor = 1f;

    private GameObject _nodesContainer;
    private List<Dictionary<string, LanguageNode>> _languagesByLevels;

    [Serializable]
    private class LanguageNode {
        private const float LineRendererWidthMultiplier = 0.1f;
        public string name;
        public string years;
        public Dictionary<string, EChildType> Children;
        public List<string> influences;
        public string pathToMap;
        public string pathToAlphabet;
        public GameObject gameObject;

        public void ConnectToParent(LanguageNode parent, EChildType childType) {
            var lineRendererGO = new GameObject($"{parent.name}->{name}");
            lineRendererGO.transform.parent = gameObject.transform;
            var lineRenderer = lineRendererGO.gameObject.AddComponent<LineRenderer>();
            if (lineRenderer == null) {
                print($"LineRenderer is null on {name}!");
                return;
            }
            lineRenderer.widthMultiplier = LineRendererWidthMultiplier;
            lineRenderer.material = childType.GetMaterial();
            if (EChildType.Revive.Equals(childType)) {
                lineRenderer.sharedMaterial.mainTextureScale = new Vector2(1f / lineRenderer.widthMultiplier, 1f);
            }
            else {
                lineRenderer.sharedMaterial.mainTextureScale = new Vector2(1f, 1f);
            }
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.SetPosition(0, parent.gameObject.transform.position);
            lineRenderer.SetPosition(1, gameObject.transform.position);
        }

        public override string ToString() {
            return $"\tName: {name},\n" +
                   $"\tYears: {years},\n" +
                   $"\tChildren: [{string.Join(", ", Children)}],\n" +
                   $"\tInfluences: [{string.Join(", ", influences)}]\n";
        }
    }


    private void Update() {
        if (!Application.isPlaying) {
            return;
        }
        if (_languagesByLevels == null) {
            return;
        }
        foreach (var languagesInLevel in _languagesByLevels) {
            foreach (var (_, langNode) in languagesInLevel) {
                foreach (Transform childLangNode in langNode.gameObject.transform) {
                    var lineRenderer = childLangNode.GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, langNode.gameObject.transform.position);
                    lineRenderer.SetPosition(1, childLangNode.position);
                }
            }
        }
    }

    public void BuildGraph() {
        if (_nodesContainer != null) {
            DestroyImmediate(_nodesContainer);
        }
        if (_nodesContainer == null) {
            _nodesContainer = new GameObject("NodeContainer") {
                transform = { position = Vector3.zero }
            };
        }
        
        var jsonText = ReadJsonText();
        if (jsonText == null) {
            return;
        }

        // create objects from json
        _languagesByLevels = JsonConvert.DeserializeObject<List<Dictionary<string, LanguageNode>>>(jsonText);
        if (_languagesByLevels == null) {
            return;
        }
        PrintLanguageDict();
        
        // instantiate and place nodes in space
        PlaceNodes();
        ConnectEdges();

        // make the object rotatable
        _nodesContainer.AddComponent<ObjectRotator>();
    }

    private void ConnectEdges() {
        for (var i = 0; i < _languagesByLevels.Count; i++) {
            foreach (var (_, langNode) in _languagesByLevels[i]) {
                ConnectLanguageToChildren(langNode, i + 1);
            }
        }
    }
    
    private void ConnectLanguageToChildren(LanguageNode parentLanguage, int childMinLevel) {
        foreach (var (childName, childType) in parentLanguage.Children) {
            FindLanguage(childName)
                ?.ConnectToParent(parentLanguage, childType);
        }
    }

    private LanguageNode FindLanguage(string langName, int fromLevel = 0) {
        if (fromLevel > _languagesByLevels.Count) {
            print($"{fromLevel} is higher than tree height ({_languagesByLevels.Count})!");
            return null;
        }
        for (var treeLevel = fromLevel; treeLevel < _languagesByLevels.Count; treeLevel++) {
            if (_languagesByLevels[treeLevel].ContainsKey(langName)) {
                return _languagesByLevels[treeLevel][langName];
            }
        }
        print($"language {langName} was not found in tree from level {fromLevel}");
        return null;
    }
    
    private void PlaceNodes() {
        if (_languagesByLevels == null) {
            return;
        }
        for (var i = 0; i < _languagesByLevels.Count; i++) {
            var level = _languagesByLevels.Count - i + 1;
            var levelContainer = new GameObject((i + 1).ToString()) {
                transform = { position = Vector3.zero }
            };
            var siblingIndex = 0;
            foreach (var (langName, langData) in _languagesByLevels[i]) {
                var langSphere = PlaceNode(level, _languagesByLevels[i].Count, siblingIndex++, langData);
                if (langSphere == null) {
                    continue;
                }
                langSphere.transform.parent = levelContainer.transform;
            }
            levelContainer.transform.parent = _nodesContainer.transform;
        }
    }

    private void PrintLanguageDict() {
        if (_languagesByLevels == null) {
            return;
        }

        var description = "";
        for (var i = 0; i < _languagesByLevels.Count; i++) {
            description += $"{i + 1}\n{string.Join("\n", _languagesByLevels[i].Values)}\n\n";
        }
        print(description);
    }

    private static string ReadJsonText() {
        // validate json path
        if (!File.Exists(PathToGraphJson)) {
            return null;
        }

        // read json and validate
        var jsonText = File.ReadAllText(PathToGraphJson);
        return jsonText is {Length: 0} ? null : jsonText;
    }

    private GameObject PlaceNode(int level, int numOfSiblings, int siblingIndex, LanguageNode langNode) {
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 1 || langNode == null) {
            return null;
        }
        
        var radius = RadiusScalingFunction(numOfSiblings - 1) * radiusFactor;
        var angle = Mathf.PI * 2 * siblingIndex / numOfSiblings;
        var x = radius * Mathf.Cos(angle);
        var y = (level - 1) * heightFactor;
        var z = radius * Mathf.Sin(angle);
        var nodeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nodeSphere.name = langNode.name;
        nodeSphere.transform.position = new Vector3(x, y, z);
        langNode.gameObject = nodeSphere;
        return nodeSphere;
    }

    private float RadiusScalingFunction(float value) {
        return 1 - 1 / (1 + value);
    }
}
