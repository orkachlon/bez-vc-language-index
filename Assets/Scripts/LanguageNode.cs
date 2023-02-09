using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class LanguageNode : MonoBehaviour, IPointerClickHandler  {

    [SerializeField] private LanguageLayout langLayout;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] [NotNull] private AncestryConnection ancestryConnectionPrefab;

    [SerializeField] [HideInInspector] private ChildNameToNodeDictionary children = new();
    [SerializeField] [HideInInspector] private LanguageData langData;
    [SerializeField] [HideInInspector] private List<AncestryConnection> ancestryChildConnections;
    [SerializeField] [HideInInspector] private Vector3 positionInGraph;
    [SerializeField] [HideInInspector] private CameraController mainCam;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Start() {
        langLayout = gameObject.GetLanguageComponent<LanguageLayout>(langLayout, "LanguageLayout");
        positionInGraph = transform.localPosition;
        mainCam = Camera.main.GetComponent<CameraController>();

        OnLangNodeClicked += OnLangNodeClickedActions;
        AncestryConnection.OnConnectionClicked += OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked += ToNode;
        GraphRotator.OnGraphFinishedRotating += AlignChildToParentItem;
        GraphRotator.OnGraphFinishedRotating += AlignParentToChildItem;
    }

    private void OnDestroy() {
        OnLangNodeClicked -= OnLangNodeClickedActions;
        AncestryConnection.OnConnectionClicked -= OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked -= ToNode;
        GraphRotator.OnGraphFinishedRotating -= AlignChildToParentItem;
    }

    private void ToNode() {
        gameObject.SetActive(true);
        transform.localPosition = positionInGraph;
        ToggleAncestryConnections(true);
        langLayout.ToNode();
    }

    private void ToItem() {
        gameObject.SetActive(true);
        transform.localPosition = positionInGraph;
        ToggleAncestryConnections(true);
        langLayout.ToItem();
    }

    private void ToItemParent(LanguageNode child) {
        //  show us and only our connection to langNode
        gameObject.SetActive(true);
        // set layout to relative
        langLayout.ToItemRelative();
        ToggleAncestryConnections(connection => connection.GetChild().GetName() == child.GetName());
    }

    private void ToItemChild(LanguageNode parent) {
        // show only us without connections
        gameObject.SetActive(true);
        // Set layout to relative mode
        langLayout.ToItemRelative();
        ToggleAncestryConnections(false);
    }

    private void AlignChildToParentItem(LanguageNode parent) {
        if (!parent.children.ContainsKey(GetName())) {
            return;
        }
        var childs = parent.children.Values.ToList();
        childs = childs.OrderBy(child => -Vector3.SignedAngle(parent.transform.forward, child.transform.forward, Vector3.up))
            .ToList();
        var i = childs.IndexOf(this);
        var pp = parent.transform.position;
        var screenWidth = 18f;
        var cellSize = screenWidth / childs.Count;
        var p = new Vector3((i * cellSize) - pp.x - screenWidth / 2f + cellSize / 2f, pp.y - 4, pp.z);
        transform.position = p;
    }
    
    private void AlignParentToChildItem(LanguageNode langChild) {
        if (!children.ContainsKey(langChild.GetName())) {
            return;
        }
        var parents = LanguageManager.GetParents(langChild);
        parents = parents.OrderBy(child => -Vector3.SignedAngle(langChild.transform.forward, child.transform.forward, Vector3.up))
            .ToList();
        var i = parents.IndexOf(this);
        var pp = langChild.transform.position;
        var screenWidth = 18f;
        var cellSize = screenWidth / parents.Count;
        var p = new Vector3((i * cellSize) - pp.x - screenWidth / 2f + cellSize / 2f, pp.y + 4, pp.z);
        transform.position = p;
    }

    private void ToggleLanguageVisibility(LanguageNode langNode) {
        // if it's us, then show us and our connections
        if (langNode.GetName() == GetName()) {
            gameObject.SetActive(true);
            ToggleAncestryConnections(true);
            langLayout.ToItem();
            return;
        }
        // if we are a parent of langNode
        if (children.ContainsKey(langNode.GetName())) {
            ToItemParent(langNode);
            return;
        }
        // if we are a child of langNode
        if (langNode.children.ContainsKey(GetName())) {
            ToItemChild(langNode);
            return;
        }
        // otherwise, we are unrelated
        gameObject.SetActive(false);
    }

    private void ToggleAncestryConnections(bool show) {
        ToggleAncestryConnections(_ => show);
    }

    private void ToggleAncestryConnections(Func<AncestryConnection, bool> comparison) {
        foreach (var connection in ancestryChildConnections) {
            connection.gameObject.SetActive(comparison.Invoke(connection));
        }
    }

    private void ToggleLanguageDetails(LanguageNode langNode) {
        if (langNode != this) {
            langLayout.ToNode();
            return;
        }
        // bring canvas to render in front of lines
        uiCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
        langLayout.ToItem();
    }


    public void SetLangData(LanguageData newLangData) {
        langData = newLangData;
        name = newLangData.name;
        
        langLayout.SetName(langData.name);
        langLayout.SetPhonetic(langData.phonetic);
        langLayout.SetYears(langData.years);
        langLayout.SetMap(langData.name);
        langLayout.SetAlphabet(langData.alphabet);
        langLayout.ToNode();
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnLangNodeClicked?.Invoke(this);
    }

    public EChildType GetChildType(string childName) {
        if (!langData.childToType.ContainsKey(childName)) {
            throw new ArgumentException($"{childName} wasn't found in {langData.name}'s children!");
        }

        return langData.childToType[childName];
    }

    public string GetName() {
        return langData.name;
    }

    public ChildNameToNodeDictionary GetChildren() {
        return children;
    }

    public void AddChild([NotNull] LanguageNode childLangNode) {
        children[childLangNode.GetName()] = childLangNode;
    }

    public void ConnectEdgesRecursively() {
        foreach (var (childLangName, childLangNode) in children) {
            if (ConnectionExists(childLangNode)) {
                continue;
            }

            var connection = Instantiate(ancestryConnectionPrefab);
            connection.Connect(
                this,
                childLangNode,
                GetChildType(childLangName)
                );
            ancestryChildConnections.Add(connection);
            childLangNode.ConnectEdgesRecursively();
        }
    }

    private bool ConnectionExists(LanguageNode childLangNode) {
        return ancestryChildConnections.Any(connection => connection.GetChild() == childLangNode);
    }
    
    

    private void OnLangNodeClickedActions(LanguageNode langNode) {
        // if it's us, then show us and our connections
        if (langNode == this) {
            ToItem();
            return;
        }
        // if we are a parent
        if (children.Values.Contains(langNode)) {
            ToItemParent(langNode);
            return;
        }
        if (langNode.children.ContainsKey(GetName())) {
            ToItemChild(langNode);
            return;
        }
        // otherwise, we are unrelated
        gameObject.SetActive(false);
    }

    private void OnAncestryConnectionClicked(LanguageNode langNode) {
        ToggleLanguageDetails(langNode);
        ToggleLanguageVisibility(langNode);
    }
    
    public override string ToString() {
        return langData.ToString();
    }
    
    public bool Equals(LanguageNode other) {
        return base.Equals(other) && other != null && 
               GetName().Equals(other.GetName());
    }
    
    public override bool Equals(object obj) {
        return !ReferenceEquals(null, obj) &&
               (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((LanguageNode) obj)));
    }

    public override int GetHashCode() {
        return HashCode.Combine(base.GetHashCode(), GetName());
    }

    public static bool operator ==(LanguageNode me, LanguageNode other) {
        return me is null && other is null || 
               me is not null && 
               other is not null && 
               me.GetName().Equals(other.GetName());
    }

    public static bool operator !=(LanguageNode me, LanguageNode other) {
        return !(me == other);
    }
}

[Serializable]
public class ChildNameToNodeDictionary : SerializableDictionary<string, LanguageNode> {
        
}