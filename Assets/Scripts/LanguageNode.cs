using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class LanguageNode {
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
        if (lineRenderer is null) {
            MonoBehaviour.print($"LineRenderer is null on {name}!");
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