using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[ExecuteInEditMode]
public class GraphBuilder : MonoBehaviour {
    private const string PathToGraphJson = "./Assets/Graphs/LanguageGraph.json";
    [SerializeField]
    private float heightFactor = 1f;
    [SerializeField]
    private float radiusFactor = 1f;

    private GameObject _nodesContainer;

    [Serializable]
    private class LanguageData {
        public string name;
        public string years;
        public Dictionary<string, DescendantType> Descendants;
        public List<string> influences;
        public string pathToMap;
        public string pathToAlphabet;

        public override string ToString() {
            return $"Name: {name},\n" +
                   $"Years: {years},\n" +
                   $"Descendants: [{string.Join(", ", Descendants)}],\n" +
                   $"Influences: [{string.Join(", ", influences)}]";
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
        var langNodeDict = JsonConvert.DeserializeObject<Dictionary<string, List<LanguageData>>>(jsonText);
        if (langNodeDict == null) {
            return;
        }
        PrintLanguageDict(langNodeDict);

        // instantiate and place nodes in space
        PlaceNodes(langNodeDict);
        ConnectEdges(langNodeDict);
    }

    private void ConnectEdges(Dictionary<string, List<LanguageData>> langNodeDict) {
        foreach (var (treeLevel, langList) in langNodeDict) {
            ConnectLanguagesInLevel(treeLevel, langList);
        }
    }

    private void ConnectLanguagesInLevel(string treeLevel, List<LanguageData> langList) {
        var levelContainer = _nodesContainer.transform.Find(treeLevel);
        var nextLevelContainer = _nodesContainer.transform.Find((int.Parse(treeLevel) + 1).ToString());
        if (nextLevelContainer == null) {
            return;
        }
        foreach (var parentLanguage in langList) {
            ConnectParentToChildren(levelContainer, parentLanguage, nextLevelContainer);
        }
    }

    private static void ConnectParentToChildren(Transform levelContainer, LanguageData parentLanguage, Transform nextLevelContainer) {
        var parentLangTransform = levelContainer.Find(parentLanguage.name);
        foreach (var (childName, childType) in parentLanguage.Descendants) {
            ConnectParentAndChild(parentLangTransform, nextLevelContainer, childName, childType);
        }
    }

    private static void ConnectParentAndChild(Transform parentLangTransform, Transform nextLevelContainer, string childName, DescendantType childType) {
        var childLangTransform = nextLevelContainer.Find(childName);
        if (childLangTransform == null) {
            print($"couldn't find {childName} under {nextLevelContainer.name}");
            return;
        }

        var lineRendererGO = new GameObject($"{parentLangTransform.name}->{childName}");
        lineRendererGO.transform.parent = childLangTransform;
        var lineRenderer = lineRendererGO.gameObject.AddComponent<LineRenderer>();
        if (lineRenderer == null) {
            print($"LineRenderer is null on {childLangTransform.name}!");
            return;
        }
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = childType.GetMaterial();
        lineRenderer.SetPosition(0, parentLangTransform.position);
        lineRenderer.SetPosition(1, childLangTransform.position);
    }

    private void PlaceNodes(Dictionary<string, List<LanguageData>> langNodeDict) {
        if (langNodeDict == null) {
            return;
        }
        foreach (var (treeLevel, langList) in langNodeDict) {
            var level = int.Parse(treeLevel);
            var levelContainer = new GameObject(treeLevel) {
                transform = { position = Vector3.zero }
            };
            for (var i = 0; i < langList.Count; i++) {
                var langSphere = PlaceNode(level, langList.Count, i, langList[i]);
                if (langSphere == null) {
                    continue;
                }
                langSphere.transform.parent = levelContainer.transform;
            }
            levelContainer.transform.parent = _nodesContainer.transform;
        }
    }

    private static void PrintLanguageDict(Dictionary<string, List<LanguageData>> langNodeDict) {
        if (langNodeDict == null) {
            return;
        }

        var description = "";
        foreach (var (treeLevel, langList) in langNodeDict) {
            description += $"{treeLevel}\n{string.Join("\n", langList)}\n\n";
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

    private GameObject PlaceNode(int level, int numOfSiblings, int siblingIndex, LanguageData langData) {
        if (numOfSiblings == 0 || siblingIndex < 0 || siblingIndex >= numOfSiblings || level < 1 || langData == null) {
            return null;
        }
        
        var radius = (numOfSiblings - 1) * radiusFactor;
        var angle = Mathf.PI * 2 * siblingIndex / numOfSiblings;
        var x = radius * Mathf.Cos(angle);
        var y = (level - 1) * heightFactor;
        var z = radius * Mathf.Sin(angle);
        var nodeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nodeSphere.name = langData.name;
        nodeSphere.transform.position = new Vector3(x, y, z);
        return nodeSphere;
    }
}

internal enum DescendantType {
    Replace,
    Add,
    Revive
}

internal static class DescendantTypeExtensions {

    private static readonly Material ReplaceMaterial = Resources.Load<Material>("ReplaceChildTypeMaterial");
    public static Material GetMaterial(this DescendantType type) {
        return type switch {
            DescendantType.Replace => ReplaceMaterial,
            DescendantType.Add => ReplaceMaterial,
            DescendantType.Revive => ReplaceMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
