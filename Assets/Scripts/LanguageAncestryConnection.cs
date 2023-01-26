using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LanguageAncestryConnection : MonoBehaviour {
    private const float LineRendererWidthMultiplier = 0.1f;


    [SerializeField] private LanguageNode parent;
    [SerializeField] private LanguageNode child;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private BoxCollider boxCollider;
    
    private float _parentOffset;
    private float _childOffset;

    private void Start() {
        GetLineRenderer();
        GetCollider();
    }

    private void Update() {
        UpdateLinePositions();
        SetCollider();
    }

    private void OnMouseDown() {
        print($"clicked {name}");
    }

    private void GetLineRenderer() {
        if (lineRenderer)
            return;
        print("LineRenderer not found from link, trying to fetch...");
        lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer) {
            throw new MissingComponentException("LanguageAncestryConnection must have a LineRenderer component!");
        }
    }

    private void GetCollider() {
        if (boxCollider)
            return;
        print("EdgeCollider2D not found from link, trying to fetch...");
        boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) {
            throw new MissingComponentException("LanguageAncestryConnection must have a BoxCollider component!");
        }
    }

    private void UpdateLinePositions() {
        if (!parent || !child) {
            return;
        }
        var (parentLinePos, childLinePos) = GetOffsetPositions();

        lineRenderer.SetPosition(0, parentLinePos);
        lineRenderer.SetPosition(1, childLinePos);
    }

    private (Vector3 parentLinePos, Vector3 childLinePos) GetOffsetPositions() {
        var parentPos = parent.transform.position;
        var childPos = child.transform.position;
        var parentLinePos = parentPos + (childPos - parentPos).normalized * _parentOffset;
        var childLinePos = childPos + (parentPos - childPos).normalized * _childOffset;
        return (parentLinePos, childLinePos);
    }

    public void Connect(LanguageNode parentConnect, LanguageNode childConnect, EChildType childType, float parentOffset, float childOffset) {
        // update fields
        name = $"{parentConnect.GetName()} -> {childConnect.GetName()}";
        transform.parent = parentConnect.transform;
        parent = parentConnect;
        child = childConnect;
        _parentOffset = parentOffset;
        _childOffset = childOffset;
        
        // draw line
        if (!lineRenderer) {
            print($"LineRenderer is null on {name}!");
            return;
        }
        SetLineRenderer(childType);
        
        // create collider
        SetCollider();
    }

    private void SetLineRenderer(EChildType childType) {
        // looks
        lineRenderer.widthMultiplier = LineRendererWidthMultiplier;
        lineRenderer.material = childType.GetMaterial();
        lineRenderer.sharedMaterial.mainTextureScale = EChildType.Revive.Equals(childType)
            ? new Vector2(1f / lineRenderer.widthMultiplier, 1f)
            : new Vector2(1f, 1f);
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.sortingLayerName = "Lines";

        // line position
        var (parentLinePos, childLinePos) = GetOffsetPositions();
        lineRenderer.SetPosition(0, parentLinePos);
        lineRenderer.SetPosition(1, childLinePos);
        
        // object position
        var lineDirection = child.transform.position - parent.transform.position;
        transform.position = parent.transform.position + lineDirection / 2f;
        var tmp = Vector3.Cross(lineDirection, Vector3.forward);
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(tmp, lineDirection), -lineDirection);
    }

    private void SetCollider() {
        var (parentLinePos, childLinePos) = GetOffsetPositions();
        var lineDirection = childLinePos - parentLinePos;
        var width = lineRenderer.widthMultiplier;
        var height = lineDirection.magnitude;
        var colliderTransform = boxCollider.transform;
        
        boxCollider.size = new Vector3(width, height, .1f);
        boxCollider.center = Vector3.zero;
        colliderTransform.position = transform.position;
        colliderTransform.rotation = transform.rotation;
    }
}
