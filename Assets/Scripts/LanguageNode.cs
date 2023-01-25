using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public class LanguageNode : MonoBehaviour {
    private const float LineRendererWidthMultiplier = 0.1f;

    private LanguageData _langData;
    
    public static event Action<LanguageNode> OnLangNodeClicked;
    
    public void SetLangData(LanguageData langData) {
        _langData = langData;
        name = langData.name;
    }

    private void OnMouseDown() {
        // rotate graph so that node is in front of camera
        OnLangNodeClicked?.Invoke(this);
        // zoom in to node
    }

    public void ConnectToParent(LanguageNode parent, EChildType childType) {
        var lineRendererGO = new GameObject($"{parent.GetName()} -> {GetName()}");
        lineRendererGO.transform.parent = transform;
        var lineRenderer = lineRendererGO.gameObject.AddComponent<LineRenderer>();
        if (lineRenderer is null) {
            print($"LineRenderer is null on {GetName()}!");
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
        lineRenderer.SetPosition(0, parent.transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    public EChildType GetChildType(string childName) {
        if (!_langData.Children.ContainsKey(childName)) {
            throw new ArgumentException($"{childName} wasn't found in {_langData.name}'s children!");
        }

        return _langData.Children[childName];
    }

    public string GetName() {
        return _langData.name;
    }

    public IEnumerable<KeyValuePair<string, EChildType>> GetChildren() {
        return _langData.Children.AsReadOnlyCollection();
    }

    public override string ToString() {
        return _langData.ToString();
    }
}