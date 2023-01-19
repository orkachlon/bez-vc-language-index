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
    [SerializeField]
    private bool isGraphBuilt = false;
    
    [SerializeField]
    private GameObject nodesContainer;
    private List<Dictionary<string, LanguageNode>> _languagesByLevels;

    private void Start() {
        BuildGraph();
    }

    private void Update() {
        UpdateLinePositions();
    }

    private void UpdateLinePositions() {
        if (!Application.isPlaying) {
            return;
        }

        if (!isGraphBuilt) {
            return;
        }

        for (var level = 0; level < _languagesByLevels.Count; level++) {
            foreach (var (_, langNode) in _languagesByLevels[level]) {
                foreach (var (childName, _) in langNode.Children) {
                    var child = FindLanguage(childName, level + 1);
                    foreach (Transform connection in child.gameObject.transform) {
                        var lineRenderer = connection.GetComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, langNode.gameObject.transform.position);
                        lineRenderer.SetPosition(1, child.gameObject.transform.position);
                    }
                }
            }
        }
    }

    public void BuildGraph() {
        InitNodeContainer();
        CreateDictFromJson();
        // for debugging json
        PrintLanguageDict();
        // create physical graph
        PlaceNodes();
        ConnectEdges();
        // make the object rotatable
        nodesContainer.AddComponent<ObjectRotator>();
        isGraphBuilt = true;
    }

    private void CreateDictFromJson() {
        var jsonText = ReadJsonText();
        if (jsonText == null) {
            throw new JsonException("Couldn't read text from json file!");
        }

        // create objects from json
        _languagesByLevels = JsonConvert.DeserializeObject<List<Dictionary<string, LanguageNode>>>(jsonText);
        if (_languagesByLevels == null) {
            throw new JsonSerializationException("Couldn't create dictionary from json text!");
        }
    }

    private void InitNodeContainer() {
        var wasDestroyed = DestroyGameObject(nodesContainer);
        if (!wasDestroyed) {
            var container = GameObject.FindWithTag("NodeContainer");
            wasDestroyed = DestroyGameObject(container);
        }

        if (wasDestroyed || nodesContainer is null) {
            isGraphBuilt = false;
            nodesContainer = new GameObject("NodeContainer") {
                transform = {position = Vector3.zero},
                tag = "NodeContainer"
            };
        }
    }

    private void ConnectEdges() {
        for (var level = 0; level < _languagesByLevels.Count; level++) {
            foreach (var (_, langNode) in _languagesByLevels[level]) {
                ConnectLanguageToChildren(langNode, level + 1);
            }
        }
    }

    private void ConnectLanguageToChildren(LanguageNode parentLanguage, int childMinLevel) {
        foreach (var (childName, childType) in parentLanguage.Children) {
            FindLanguage(childName, childMinLevel)
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
                if (langSphere is null) {
                    continue;
                }
                langSphere.transform.parent = levelContainer.transform;
            }
            levelContainer.transform.parent = nodesContainer.transform;
        }
    }

    private void PrintLanguageDict() {
        if (_languagesByLevels is null) {
            return;
        }

        var description = "";
        for (var i = 0; i < _languagesByLevels.Count; i++) {
            description += $"{i + 1}\n{string.Join("\n", _languagesByLevels[i].Values)}\n\n";
        }
        print(description);
    }

    private GameObject PlaceNode(int level, int numOfSiblings, int siblingIndex, LanguageNode langNode) {
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 1 || langNode is null) {
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

    private static bool DestroyGameObject(GameObject container) {
        if (container is null) return false;
        
        if (Application.isPlaying) {
            Destroy(container);
        }
        else {
            DestroyImmediate(container);
        }
        return true;
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
}
