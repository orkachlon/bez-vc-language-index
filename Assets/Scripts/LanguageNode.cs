using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageNode : MonoBehaviour, IPointerClickHandler  {
    private const float LineRendererWidthMultiplier = 0.1f;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] [NotNull] private LanguageAncestryConnection ancestryConnectionPrefab;


    private readonly Dictionary<string, LanguageNode> _children = new();
    
    private LanguageData _langData;
    private Camera _camera;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Start() {
        _camera = Camera.main;
        GetTMProComponent();
        BindCameraToCanvas();
    }

    private void Update() {
        if (!_camera) {
            return;
        }
        uiCanvas.transform.forward = _camera.transform.forward;
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

    private void GetTMProComponent() {
        if (!title) {
            title = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (!title) {
            throw new MissingComponentException("LanguageNode is missing TextMeshPro component!");
        }
    }

    public void SetLangData(LanguageData langData) {
        _langData = langData;
        name = langData.name;
        if (!title) {
            GetTMProComponent();
        }
        title.text = _langData.name;
    }

    // not being used
    private void OnMouseDown() {
        OnLangNodeClicked?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnLangNodeClicked?.Invoke(this);
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

    public IEnumerable<KeyValuePair<string, LanguageNode>> GetChildren() {
        return _children.AsReadOnlyCollection();
    }

    public void AddChild([NotNull] LanguageNode childLangNode) {
        _children[childLangNode.GetName()] = childLangNode;
    }

    public void ConnectEdgesRecursively(float parentConnectionOffset, float childConnectionOffset) {
        foreach (var (childLangName, childLangNode) in _children) {
            Instantiate(ancestryConnectionPrefab).Connect(
                this,
                childLangNode,
                GetChildType(childLangName),
                parentConnectionOffset,
                childConnectionOffset
                );
            childLangNode.ConnectEdgesRecursively(parentConnectionOffset, childConnectionOffset);
        }
    }

    public override string ToString() {
        return _langData.ToString();
    }
}