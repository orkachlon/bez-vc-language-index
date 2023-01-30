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

    [SerializeField] private TextContainer languageName;
    [SerializeField] private TextContainer years;
    [SerializeField] private TextContainer influences;
    [SerializeField] private Image map;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] [NotNull] private AncestryConnection ancestryConnectionPrefab;


    private static readonly string InfluencesTitle = "Influences: ";
    private static readonly string YearsTitle = "Years: ";
    [SerializeField] [HideInInspector] private ChildNameToNodeDictionary children = new();
    
    [SerializeField] [HideInInspector] private LanguageData langData;
    [SerializeField] [HideInInspector] private List<AncestryConnection> ancestryConnections;
    [SerializeField] [HideInInspector] private Camera mainCamera;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Awake() {
        mainCamera = Camera.main;
        languageName = GetLanguageComponent<TextContainer>(languageName, "LanguageName");
        years = GetLanguageComponent<TextContainer>(years, "Years");
        influences = GetLanguageComponent<TextContainer>(influences, "Influences");
        map = GetLanguageComponent<Image>(map, "Map");
        BindCameraToCanvas();

        OnLangNodeClicked += ToggleLanguageDetails;
        OnLangNodeClicked += ToggleLanguageVisibility;
        AncestryConnection.OnConnectionClicked += ToggleLanguageDetails;
        AncestryConnection.OnConnectionClicked += ToggleLanguageVisibility;
        BackArrowClickReceiver.OnBackArrowClicked += () => ToggleLanguageDetails(null);
        BackArrowClickReceiver.OnBackArrowClicked += () => ToggleLanguageVisibility(this);
    }

    private void OnDestroy() {
        // throw new NotImplementedException();
    }

    private void ToggleLanguageVisibility(LanguageNode langNode) {
        // if it's us, then show us and our connections
        if (langNode == this) {
            gameObject.SetActive(true);
            foreach (var connection in ancestryConnections) {
                connection.gameObject.SetActive(true);
            }
            return;
        }
        // if we are a child of langNode, then show only us without connections
        if (langNode.children.Values.Contains(this)) {
            gameObject.SetActive(true);
            foreach (var connection in ancestryConnections) {
                connection.gameObject.SetActive(false);
            }
            return;
        }
        // if we are its parent, then show us and only our connection to langNode
        if (children.Values.Contains(langNode)) {
            gameObject.SetActive(true);
            foreach (var connection in ancestryConnections) {
                connection.gameObject.SetActive(connection.GetChild() == langNode);
            }
            return;
        }
        gameObject.SetActive(false);
    }

    private void ToggleLanguageDetails(LanguageNode langNode) {
        if (langNode != this) {
            // hide details
            if (years) {
                years.gameObject.SetActive(false);
            }
            if (influences) {
                influences.gameObject.SetActive(false);
            }
            if (map) {
                map.gameObject.SetActive(false);
            }
            return;
        }
        // show details
        // years = GetLanguageComponent<TextContainer>(years, "Years");
        if (years) {
            years.gameObject.SetActive(true);
        }

        // influences = GetLanguageComponent<TextContainer>(influences, "Influences");
        if (influences) {
            influences.gameObject.SetActive(true);
        }

        // map = GetLanguageComponent<Image>(map, "Map");
        if (map) {
            map.gameObject.SetActive(true);
        }
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
        
        this.langData = langData;
        name = langData.name;
        languageName.SetText(this.langData.name);
        years.SetText(YearsTitle + this.langData.years); // todo remove years title
        influences.SetText(InfluencesTitle + string.Join(", ", this.langData.influences));
        map.sprite = Resources.Load<Sprite>(this.langData.pathToMap);
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