using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
[Serializable]
[ExecuteAlways]
public class AncestryConnection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public static event Action<LanguageNode> OnConnectionClicked; 
    
    [SerializeField] private LanguageNode parent;
    [SerializeField] private LanguageNode child;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private BoxCollider boxCollider;
    
    [SerializeField] [Range(0, 1)] private float parentOffset;
    [SerializeField] [Range(0, 1)] private float childOffset;

    private void Start() {
        GetLineRenderer();
        GetCollider();
    }

    private void Update() {
        if (!parent || !child) {
            return;
        }
        UpdateLinePositions();
        SetLineTransform();
        SetCollider();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!LanguageNode.IsClickEnabled()) {
            return;
        }
        var languageToFocus = child == SelectionManager.GetSelectedLanguage() ? parent : child;
        languageToFocus.SetEndPosition();
        GraphRotator.SetEndRotation(languageToFocus);
        
        OnConnectionClicked?.Invoke(languageToFocus);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (SelectionManager.GetSelectedLanguage() == null) {
            return;
        }
        LanguageNameTooltip.ShowTooltipStatic(SelectionManager.GetSelectedLanguage() == child
            ? parent.GetName()
            : child.GetName());
    }

    public void OnPointerExit(PointerEventData eventData) {
        LanguageNameTooltip.HideTooltipStatic();
    }

    private void GetLineRenderer() {
        if (lineRenderer)
            return;
        print("LineRenderer not found from link, trying to fetch...");
        lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer) {
            throw new MissingComponentException("AncestryConnection must have a LineRenderer component!");
        }
    }

    private void GetCollider() {
        if (boxCollider)
            return;
        print("EdgeCollider2D not found from link, trying to fetch...");
        boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) {
            throw new MissingComponentException("AncestryConnection must have a BoxCollider component!");
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
        var parentLinePos = parentPos + (childPos - parentPos).normalized * parentOffset;
        var childLinePos = childPos + (parentPos - childPos).normalized * childOffset;
        return (parentLinePos, childLinePos);
    }

    public void Connect(LanguageNode parentConnect, LanguageNode childConnect, EChildType childType) {
        // update fields
        name = $"{parentConnect.GetName()} -> {childConnect.GetName()}";
        transform.parent = parentConnect.transform;
        parent = parentConnect;
        child = childConnect;

        // draw line
        if (!lineRenderer) {
            print($"LineRenderer is null on {name}!");
            return;
        }
        SetLineRenderer(childType);
        SetLineTransform();
        SetCollider();
    }

    public LanguageNode GetChild() {
        return child;
    }

    public LanguageNode GetParent() {
        return parent;
    }

    private void SetLineRenderer(EChildType childType) {
        // looks
        childType.SetLineType(lineRenderer);

        // line position
        var (parentLinePos, childLinePos) = GetOffsetPositions();
        lineRenderer.SetPosition(0, parentLinePos);
        lineRenderer.SetPosition(1, childLinePos);
    }

    private void SetLineTransform() {
        var lineDirection = child.transform.position - parent.transform.position;
        var tmp = Vector3.Cross(lineDirection, Vector3.forward);
        transform.SetPositionAndRotation(
            parent.transform.position + lineDirection * 0.5f,
            Quaternion.LookRotation(Vector3.Cross(tmp, lineDirection), -lineDirection));
    }

    private void SetCollider() {
        var (parentLinePos, childLinePos) = GetOffsetPositions();
        var lineDirection = childLinePos - parentLinePos;
        var width = ChildTypeExtensions.LineWide;
        var height = lineDirection.magnitude;
        var colliderTransform = boxCollider.transform;
        
        boxCollider.size = new Vector3(width, height, .1f);
        boxCollider.center = Vector3.zero;
        colliderTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
