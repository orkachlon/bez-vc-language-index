using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
[Serializable]
[ExecuteAlways]
public class AncestryConnection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IFadable {
    
    public static event Action<LanguageNode> OnConnectionClicked; 
    
    [Header("Nodes")]
    [SerializeField] private LanguageNode parent;
    [SerializeField] private LanguageNode child;
    
    [Header("Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private BoxCollider boxCollider;
    
    [Header("Parameters")]
    [SerializeField] [Range(0, 1)] private float parentOffset;
    [SerializeField] [Range(0, 1)] private float childOffset;
    [SerializeField] [Range(0, 1)] private float lineWide = 0.2f;
    [SerializeField] [Range(0, 1)] private float lineNarrow = 0.1f;

    [SerializeField] [HideInInspector] private EChildType childType;
    [SerializeField] [HideInInspector] private bool itemMode;
    
    private void Awake() {
        GetLineRenderer();
        GetCollider();

        GraphRotator.OnGraphFinishedRotating += ToItem;
        BackClickReceiver.OnBackArrowClicked += ToNode;
    }

    private void OnDestroy() {
        GraphRotator.OnGraphFinishedRotating -= ToItem;
        BackClickReceiver.OnBackArrowClicked -= ToNode;
    }

    private void Update() {
        if (!parent || !child) {
            return;
        }
        UpdateLinePositions();
        SetLineWidth();
        SetLineTransform();
        SetCollider();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!LanguageNode.IsClickEnabled()) {
            return;
        }
        LanguageNameTooltip.HideTooltipStatic();
        var languageToFocus = child == SelectionManager.GetSelectedLanguage() ? parent : child;
        languageToFocus.SetEndPosition();
        GraphRotator.SetEndRotation(languageToFocus);
        
        OnConnectionClicked?.Invoke(languageToFocus);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!SelectionManager.GetSelectedLanguage()) {
            return;
        }

        var tooltipString = GetTooltipString();
        LanguageNameTooltip.ShowTooltipStatic(tooltipString);
    }

    public void OnPointerExit(PointerEventData eventData) {
        LanguageNameTooltip.HideTooltipStatic();
    }

    private string GetTooltipString() {

        var otherNode = SelectionManager.GetSelectedLanguage() == child ? parent : child;
        var tooltipString = $"<align=\"left\"><font=NotoSerif-Italic SDF>{otherNode.GetYears()}</font>";
        
        if (otherNode.GetAlphabet().Length > 0) {
            tooltipString = $"<align=\"right\">{otherNode.GetAlphabet()}\n" + tooltipString;
        }
        return tooltipString;
    }

    private void ToItem(LanguageNode langNode) {
        itemMode = true;
    }

    private void ToNode() {
        itemMode = false;
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
        if (itemMode) {
            return (parent.transform.position, child.transform.position);
        }
        var parentPos = parent.transform.position;
        var childPos = child.transform.position;
        var parentLinePos = parentPos + (childPos - parentPos).normalized * parentOffset;
        var childLinePos = childPos + (parentPos - childPos).normalized * childOffset;
        return (parentLinePos, childLinePos);
    }

    public void Connect(LanguageNode parentConnect, LanguageNode childConnect, EChildType childTypeConnect) {
        // update fields
        name = $"{parentConnect.GetName()} -> {childConnect.GetName()}";
        transform.parent = parentConnect.transform;
        parent = parentConnect;
        child = childConnect;
        childType = childTypeConnect;

        // draw line
        if (!lineRenderer) {
            print($"LineRenderer is null on {name}!");
            return;
        }
        SetLineRenderer();
        SetLineTransform();
        SetCollider();
    }

    public LanguageNode GetChild() {
        return child;
    }

    public LanguageNode GetParent() {
        return parent;
    }

    private void SetLineRenderer() {
        // looks
        SetLineType();

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
        var width = lineWide;
        var height = lineDirection.magnitude;
        var colliderTransform = boxCollider.transform;
        
        boxCollider.size = new Vector3(width, height, .1f);
        boxCollider.center = Vector3.zero;
        colliderTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }
    
    public void SetLineType() {
        // lineRenderer.widthMultiplier = LineWide;
        lineRenderer.material = childType.GetMaterial();
        // was used to scale dotted/ dashed line properly
        // lineRenderer.sharedMaterial.mainTextureScale = EChildType.Revive.Equals(childType)
        //     ? new Vector2(1f / lineRenderer.widthMultiplier, 1f)
        //     : new Vector2(1f, 1f);
        // lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.sortingLayerName = "Lines";
        SetLineWidth();
    }

    private void SetLineWidth() {
        switch (childType) {
            case EChildType.Replace:
                lineRenderer.endWidth = lineNarrow;
                lineRenderer.startWidth = lineWide;
                break;
            case EChildType.Add:
                lineRenderer.widthMultiplier = lineWide;
                break;
            case EChildType.Revive:
                lineRenderer.startWidth = lineNarrow;
                lineRenderer.endWidth = lineWide;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(childType), childType, null);
        }
    }

    public float GetOpacity() {
        return lineRenderer.material.color.a;
    }
    
    public void SetOpacity(float percent) {
        var lineMaterial = lineRenderer.material;
        var currColor = lineMaterial.color;
        var newColor = new Color(currColor.r, currColor.g, currColor.b, percent);
        lineMaterial.color = newColor;
    }
}
