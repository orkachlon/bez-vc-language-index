using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteAlways]
public class GraphBuilder : MonoBehaviour {
    [SerializeField] private TextAsset graphJsonFile;
    
    [SerializeField] private float heightFactor = 1f;
    [SerializeField] private float radiusFactor = 1f;
    [SerializeField] [NotNull] private LanguageNode languageNodePrefab;
    
    [SerializeField] [HideInInspector] private GameObject nodesContainer;
    [SerializeField] [HideInInspector] private List<LanguageDataInLevel> langDataByLevels;
    [SerializeField] [HideInInspector] private List<LanguageNodesInLevel> langNodesByLevels;

    private void Awake() {
        BuildGraph();
        LanguageManager.PopulateNodes(langNodesByLevels);
    }

    public void BuildGraph() {
        InitNodeContainers();
        CreateDictFromJson();
        // for debugging json
        // PrintLanguageDict();
        // create physical graph
        PlaceNodes();
        ConnectLanguageTree();
        ConnectEdges();
    }

    public void DeleteGraph() {
        if (nodesContainer != null) {
            DestroyGameObject(nodesContainer);
        }
    }

    private void CreateDictFromJson() {
        if (langDataByLevels.Count > 0) {
            return;
        }

        var jsonText = graphJsonFile.text;
        if (jsonText == null) {
            throw new JsonException("Couldn't read text from json file!");
        }

        // create objects from json
        langDataByLevels = JsonConvert.DeserializeObject<List<LanguageDataInLevel>>(jsonText);
        if (langDataByLevels == null) {
            throw new JsonSerializationException("Couldn't create dictionary from json text!");
        }
    }

    private void InitNodeContainers() {
        if (langNodesByLevels == null) {
            langNodesByLevels = new List<LanguageNodesInLevel>();
        }
        if (langDataByLevels == null) {
            langDataByLevels = new List<LanguageDataInLevel>();
        }
        if (nodesContainer == null) {
            nodesContainer = new GameObject("NodeContainer") {
                transform = {position = Vector3.zero},
                tag = "NodeContainer"
            };
            nodesContainer.AddComponent<GraphRotator>();
            langDataByLevels.Clear();
            langNodesByLevels.Clear();
        }
    }

    private void ConnectEdges() {
        langNodesByLevels[0]["Proto-Semitic"].ConnectEdgesRecursively();
    }

    private LanguageNode FindLanguage(string langName, int fromLevel = 0) {
        if (fromLevel > langNodesByLevels.Count) {
            print($"{fromLevel} is higher than tree height ({langDataByLevels.Count})!");
            return null;
        }
        for (var level = fromLevel; level < langNodesByLevels.Count; level++) {
            if (langNodesByLevels[level].ContainsKey(langName)) {
                return langNodesByLevels[level][langName];
            }
        }
        print($"language {langName} was not found in tree from level {fromLevel}");
        return null;
    }

    private void PrintLanguageDict() {
        if (langDataByLevels == null) {
            return;
        }

        var description = "";
        for (var i = 0; i < langDataByLevels.Count; i++) {
            description += $"{i + 1}\n{string.Join("\n", langDataByLevels[i].Values)}\n\n";
        }
        print(description);
    }

    private void PlaceNodes() {
        if (langDataByLevels == null) {
            throw new ArgumentException("_langDataByLevels is null! Make sure json was read correctly.");
        }
        if (langNodesByLevels.Count > 0) {
            // assume graph was built in editor
            return;
        }
        for (var level = 0; level < langDataByLevels.Count; level++) {
            // create new level dictionary
            langNodesByLevels.Add(new LanguageNodesInLevel());
            var levelContainer = new GameObject((level + 1).ToString()) {
                transform = { position = Vector3.zero }
            };
            var siblingIndex = 0;
            foreach (var (langName, langData) in langDataByLevels[level]) {
                var langNode = PlaceNode(level, siblingIndex++, langData);
                if (langNode == null) {
                    continue;
                }
                // organize hierarchy in scene
                langNode.transform.SetParent(levelContainer.transform, false);
                // add langNode to level dictionary
                langNodesByLevels[level][langName] = langNode;
            }
            // organize hierarchy in scene
            levelContainer.transform.parent = nodesContainer.transform;
        }
    }

    [CanBeNull]
    private LanguageNode PlaceNode(int level, int siblingIndex, LanguageData langData) {
        
        var numOfSiblings = langDataByLevels[level].Count;
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 0 || langData == null) {
            return null;
        }
        var radius = RadiusScalingFunction(numOfSiblings - 1) * radiusFactor;
        var angle = Mathf.PI * 2 * siblingIndex / numOfSiblings;
        var x = radius * Mathf.Cos(angle);
        var y = (langDataByLevels.Count - level) * heightFactor;
        var z = radius * Mathf.Sin(angle);
        var position = new Vector3(x, y, z);
        var lookDirection = new Vector3(x, 0, z).normalized;
        lookDirection = lookDirection == Vector3.zero ? -Vector3.forward : lookDirection;
        var langNode = Instantiate(languageNodePrefab, position, Quaternion.LookRotation(lookDirection, Vector3.up));
        if (langNode == null) {
            return null;
        }
        langNode.SetLangData(langData);
        return langNode;
    }

    private void ConnectLanguageTree() {
        if (langNodesByLevels.Count == 0) {
            throw new ArgumentException("Language Nodes Dict must already be populated on ConnectLanguageTree!");
        }
        for (var level = 0; level < langNodesByLevels.Count; level++) {
            foreach (var (parentLangName, parentLangNode) in langNodesByLevels[level]) {
                var langData = langDataByLevels[level][parentLangName];
                foreach (var (childLangName, childLangType) in langData.childToType) {
                    parentLangNode.AddChild(FindLanguage(childLangName, level + 1));
                }
            }
        }
    }

    private float RadiusScalingFunction(float value) {
        // return 1 - 1 / (1 + value);
        return Mathf.Log(1 + value);
    }

    private static bool DestroyGameObject(GameObject destroyObject) {
        if (destroyObject == null) return false;

        foreach (Transform t in destroyObject.transform) {
            DestroyGameObject(t.gameObject);
        }
        if (Application.isPlaying) {
            Destroy(destroyObject);
        }
        else {
            DestroyImmediate(destroyObject);
        }
        return true;
    }
}

[Serializable]
public class LanguageDataInLevel : SerializableDictionary<string, LanguageData> {
    
}

[Serializable]
public class LanguageNodesInLevel : SerializableDictionary<string, LanguageNode> {
    
}
