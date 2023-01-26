using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteAlways]
public class GraphBuilder : MonoBehaviour {
    private const string PathToGraphJson = "./Assets/Graphs/LanguageGraph.json";
    
    [SerializeField] private float heightFactor = 1f;
    [SerializeField] private float radiusFactor = 1f;
    [SerializeField] private float parentConnectionOffset = 2f;
    [SerializeField] private float childConnectionOffset = 2f;
    [SerializeField] [NotNull] private LanguageNode languageNodePrefab;
    
    private GameObject _nodesContainer;
    private List<Dictionary<string, LanguageData>> _langDataByLevels;
    private List<Dictionary<string, LanguageNode>> _langNodesByLevels;

    private void Start() {
        // BuildGraph();
    }

    private void Awake() {
        BuildGraph();
    }

    public void BuildGraph() {
        InitNodeContainers();
        CreateDictFromJson();
        // for debugging json
        PrintLanguageDict();
        // create physical graph
        PlaceNodes();
        ConnectLanguageTree();
        ConnectEdges();
        // make the object rotatable
        _nodesContainer.AddComponent<GraphRotator>();
    }

    private void CreateDictFromJson() {
        var jsonText = ReadJsonText();
        if (jsonText == null) {
            throw new JsonException("Couldn't read text from json file!");
        }

        // create objects from json
        _langDataByLevels = JsonConvert.DeserializeObject<List<Dictionary<string, LanguageData>>>(jsonText);
        if (_langDataByLevels == null) {
            throw new JsonSerializationException("Couldn't create dictionary from json text!");
        }
    }

    private void InitNodeContainers() {
        var wasDestroyed = DestroyGameObject(_nodesContainer);
        if (!wasDestroyed) {
            var container = GameObject.FindWithTag("NodeContainer");
            wasDestroyed = DestroyGameObject(container);
        }

        if (wasDestroyed || _nodesContainer is null) {
            _nodesContainer = new GameObject("NodeContainer") {
                transform = {position = Vector3.zero},
                tag = "NodeContainer"
            };
        }

        _langNodesByLevels ??= new List<Dictionary<string, LanguageNode>>();

        if (_langNodesByLevels.Count != 0) {
            _langNodesByLevels.Clear();
        }
    }

    private void ConnectEdges() {
        _langNodesByLevels[0]["Semitic"].ConnectEdgesRecursively(parentConnectionOffset, childConnectionOffset);
    }

    private LanguageNode FindLanguage(string langName, int fromLevel = 0) {
        if (fromLevel > _langNodesByLevels.Count) {
            print($"{fromLevel} is higher than tree height ({_langDataByLevels.Count})!");
            return null;
        }
        for (var level = fromLevel; level < _langNodesByLevels.Count; level++) {
            if (_langNodesByLevels[level].ContainsKey(langName)) {
                return _langNodesByLevels[level][langName];
            }
        }
        print($"language {langName} was not found in tree from level {fromLevel}");
        return null;
    }

    private void PlaceNodes() {
        if (_langDataByLevels == null) {
            throw new ArgumentException("_langDataByLevels is null! Make sure json was read correctly.");
        }
        if (_langNodesByLevels.Count > 0) {
            throw new ArgumentException("_langNodeByLevels is supposed to be empty before placing nodes!");
        }
        for (var level = 0; level < _langDataByLevels.Count; level++) {
            // create new level dictionary
            _langNodesByLevels.Add(new Dictionary<string, LanguageNode>());
            var levelContainer = new GameObject((level + 1).ToString()) {
                transform = { position = Vector3.zero }
            };
            var siblingIndex = 0;
            foreach (var (langName, langData) in _langDataByLevels[level]) {
                var langNode = PlaceNode(level, siblingIndex++, langData);
                if (langNode is null) {
                    continue;
                }
                // organize hierarchy in scene
                langNode.transform.SetParent(levelContainer.transform, false);
                // add langNode to level dictionary
                _langNodesByLevels[level][langName] = langNode;
            }
            // organize hierarchy in scene
            levelContainer.transform.parent = _nodesContainer.transform;
        }
    }

    private void PrintLanguageDict() {
        if (_langDataByLevels is null) {
            return;
        }

        var description = "";
        for (var i = 0; i < _langDataByLevels.Count; i++) {
            description += $"{i + 1}\n{string.Join("\n", _langDataByLevels[i].Values)}\n\n";
        }
        print(description);
    }

    private LanguageNode PlaceNode(int level, int siblingIndex, LanguageData langData) {
        
        var numOfSiblings = _langDataByLevels[level].Count;
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 0 || langData is null) {
            return null;
        }
        var radius = RadiusScalingFunction(numOfSiblings - 1) * radiusFactor;
        var angle = Mathf.PI * 2 * siblingIndex / numOfSiblings;
        var x = radius * Mathf.Cos(angle);
        var y = (_langDataByLevels.Count - level) * heightFactor;
        var z = radius * Mathf.Sin(angle);
        var langNode = Instantiate(languageNodePrefab, new Vector3(x, y, z), Quaternion.identity);
        if (langNode is null) {
            return null;
        }
        langNode.SetLangData(langData);
        return langNode;
    }

    private void ConnectLanguageTree() {
        for (var level = 0; level < _langNodesByLevels.Count; level++) {
            foreach (var (parentLangName, parentLangNode) in _langNodesByLevels[level]) {
                var langData = _langDataByLevels[level][parentLangName];
                foreach (var (childLangName, childLangType) in langData.Children) {
                    parentLangNode.AddChild(FindLanguage(childLangName, level + 1));
                }
            }
        }
    }

    private float RadiusScalingFunction(float value) {
        // return 1 - 1 / (1 + value);
        return Mathf.Log(1 + value);
    }

    private static bool DestroyGameObject(Object destroyObject) {
        if (destroyObject is null) return false;
        
        if (Application.isPlaying) {
            Destroy(destroyObject);
        }
        else {
            DestroyImmediate(destroyObject);
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
