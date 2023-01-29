using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageNode : MonoBehaviour, IPointerClickHandler  {

    [SerializeField] private TextContainer languageName;
    [SerializeField] private TextContainer years;
    [SerializeField] private TextContainer influences;
    [SerializeField] private Image map;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] [NotNull] private AncestryConnection ancestryConnectionPrefab;


    private static readonly string InfluencesTitle = "Influences: ";
    private static readonly string YearsTitle = "Years: ";
    private readonly Dictionary<string, LanguageNode> _children = new();
    
    private LanguageData _langData;
    private Camera _camera;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Start() {
        _camera = Camera.main;
        languageName = GetLanguageComponent<TextContainer>(languageName, "LanguageName");
        years = GetLanguageComponent<TextContainer>(years, "Years");
        influences = GetLanguageComponent<TextContainer>(influences, "Influences");
        map = GetLanguageComponent<Image>(map, "Map");
        BindCameraToCanvas();

        OnLangNodeClicked += ShowLanguageDetails;
        BackArrowClickReceiver.OnBackArrowClicked += HideLanguageDetails;
    }

    private void ShowLanguageDetails(LanguageNode langNode) {
        years.enabled = true;
        influences.enabled = true;
        map.enabled = true;
    }

    private void HideLanguageDetails() {
        years.enabled = false;
        influences.enabled = false;
        map.enabled = false;
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

    private T GetLanguageComponent<T>(T component, string requestedTag) where T: Component {
        if (!component) {
            component = gameObject.FindComponentInChildWithTag<T>(requestedTag);
        } else {
            return component;
        }
        if (!component) {
            throw new MissingComponentException($"LanguageNode is missing {requestedTag} {typeof(T)} component!");
        }

        return component;
    }

    public void SetLangData(LanguageData langData) {
        languageName = GetLanguageComponent<TextContainer>(languageName, "LanguageName");
        years = GetLanguageComponent<TextContainer>(years, "Years");
        influences = GetLanguageComponent<TextContainer>(influences, "Influences");
        map = GetLanguageComponent<Image>(map, "Map");
        
        _langData = langData;
        name = langData.name;
        languageName.SetText(_langData.name);
        years.SetText(YearsTitle + _langData.years); // todo remove years title
        influences.SetText(InfluencesTitle + string.Join(", ", _langData.influences));
        map.sprite = Resources.Load<Sprite>(_langData.pathToMap);
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
    
    public bool Equals(LanguageNode other) {
        return base.Equals(other) && other is not null && 
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
        return me is not null && 
               other is not null && 
               me.GetName().Equals(other.GetName());
    }

    public static bool operator !=(LanguageNode me, LanguageNode other) {
        return !(me == other);
    }
}