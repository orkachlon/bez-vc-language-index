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
    [SerializeField] [HideInInspector] private List<AncestryConnection> ancestryConnections;
    [SerializeField] [HideInInspector] private Camera mainCamera;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Awake() {
        mainCamera = Camera.main;
        langLayout = gameObject.GetLanguageComponent<LanguageLayout>(langLayout, "LanguageLayout");
        BindCameraToCanvas();

        OnLangNodeClicked += OnLangNodeClickedActions;
        AncestryConnection.OnConnectionClicked += OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked += ToNode;
    }

    private void OnDestroy() {
        OnLangNodeClicked -= OnLangNodeClickedActions;
        AncestryConnection.OnConnectionClicked -= OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked -= ToNode;
    }

    private void ToNode() {
        gameObject.SetActive(true);
        ToggleAncestryConnections(true);
        langLayout.ToNode();
    }

    private void ToItem() {
        gameObject.SetActive(true);
        ToggleAncestryConnections(true);
        langLayout.ToItem();
    }

    private void ToItemRelative(LanguageNode focusedRelative) {
        // if we are a child of langNode, then show only us without connections
        if (focusedRelative.children.Values.Contains(this)) {
            gameObject.SetActive(true);
            // Set layout to relative mode
            langLayout.ToItemRelative();
            ToggleAncestryConnections(false);
            return;
        }
        // if we are its parent, then show us and only our connection to langNode
        if (!children.Values.Contains(focusedRelative))
            return;
        gameObject.SetActive(true);
        // set layout to relative
        langLayout.ToItemRelative();
        ToggleAncestryConnections(connection => connection.GetChild() == focusedRelative);
    }

    private void ToggleLanguageVisibility(LanguageNode langNode) {
        // if it's us, then show us and our connections
        if (langNode == this) {
            gameObject.SetActive(true);
            ToggleAncestryConnections(true);
            langLayout.ToItem();
            return;
        }
        // if we are a child of langNode, then show only us without connections
        if (langNode.children.Values.Contains(this) || children.Values.Contains(langNode)) {
            ToItemRelative(langNode);
        }
        // otherwise, we are unrelated
        gameObject.SetActive(false);
    }

    private void ToggleAncestryConnections(bool show) {
        ToggleAncestryConnections(_ => show);
    }

    private void ToggleAncestryConnections(Func<AncestryConnection, bool> comparison) {
        foreach (var connection in ancestryConnections) {
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

    private void Update() {
        if (!mainCamera) {
            return;
        }
        uiCanvas.transform.forward = mainCamera.transform.forward;
    }

    private void BindCameraToCanvas() {
        if (!uiCanvas) {
            uiCanvas = GetComponentInChildren<Canvas>();
        }
        if (!uiCanvas) {
            throw new MissingComponentException("LanguageNode is missing Canvas component!");
        }
        uiCanvas.worldCamera = Camera.main;
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

    public IEnumerable<KeyValuePair<string, LanguageNode>> GetChildren() {
        return children.AsReadOnlyCollection();
    }

    public void AddChild([NotNull] LanguageNode childLangNode) {
        children[childLangNode.GetName()] = childLangNode;
    }

    public void ConnectEdgesRecursively(float parentConnectionOffset, float childConnectionOffset) {
        foreach (var (childLangName, childLangNode) in children) {
            if (ConnectionExists(childLangNode)) {
                continue;
            }

            var connection = Instantiate(ancestryConnectionPrefab);
            connection.Connect(
                this,
                childLangNode,
                GetChildType(childLangName),
                parentConnectionOffset,
                childConnectionOffset
                );
            ancestryConnections.Add(connection);
            childLangNode.ConnectEdgesRecursively(parentConnectionOffset, childConnectionOffset);
        }
    }

    private bool ConnectionExists(LanguageNode childLangNode) {
        foreach (var connection in ancestryConnections) {
            if (connection.GetChild() == childLangNode) {
                return true;
            }
        }
        return false;
    }


    private void OnLangNodeClickedActions(LanguageNode langNode) {
        // if it's us, then show us and our connections
        if (langNode == this) {
            ToItem();
            return;
        }
        // if we are a relative
        if (langNode.children.Values.Contains(this) || children.Values.Contains(langNode)) {
            ToItemRelative(langNode);
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