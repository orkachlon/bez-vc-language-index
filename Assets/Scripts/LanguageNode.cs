using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class LanguageNode : MonoBehaviour, IPointerClickHandler  {
    private const float TranslationDuration = .75f;

    [SerializeField] private LanguageLayout langLayout;
    [SerializeField] [NotNull] private AncestryConnection ancestryConnectionPrefab;

    [SerializeField] [HideInInspector] private ChildNameToNodeDictionary children = new();
    [SerializeField] [HideInInspector] private LanguageData langData;
    [SerializeField] [HideInInspector] private List<AncestryConnection> ancestryChildConnections;
    [SerializeField] [HideInInspector] private Vector3 positionInGraph;
    [SerializeField] [HideInInspector] private Vector3 endPosition;
    
    private static int _clickDisablers = 0;

    public static event Action<LanguageNode> OnLangNodeClicked;

    private void Start() {
        langLayout = gameObject.GetLanguageComponent<LanguageLayout>(langLayout, "LanguageLayout");
        positionInGraph = transform.localPosition;
        var globalPos = transform.localToWorldMatrix * positionInGraph;
        endPosition = new Vector3(0, globalPos.y, -Mathf.Sqrt(globalPos.x * globalPos.x + globalPos.z * globalPos.z));
        
        GraphRotator.OnGraphFinishedRotating += OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked += ToNode;
    }

    private void OnDestroy() {
        GraphRotator.OnGraphFinishedRotating -= OnLangNodeClickedActions;
        BackClickReceiver.OnBackArrowClicked -= ToNode;
    }

    private void ToNode() {
        gameObject.SetActive(true);
        SetEndPosition();
        ToggleAncestryConnections(true);
        langLayout.ToNode();
    }

    public void ToItem() {
        gameObject.SetActive(true);
        ToggleAncestryConnections(true);
        langLayout.ToItem();
    }

    private void ToItemParent(LanguageNode child) {
        // show us and only our connection to langNode
        gameObject.SetActive(true);
        // set layout to relative
        langLayout.ToItemRelative();
        ToggleAncestryConnections(connection => connection.GetChild().GetName() == child.GetName());
        AlignParentToChildItem(child);
    }

    private void ToItemChild(LanguageNode parent) {
        // show only us without connections
        gameObject.SetActive(true);
        // Set layout to relative mode
        langLayout.ToItemRelative();
        ToggleAncestryConnections(false);
        AlignChildToParentItem(parent);
    }
 
    private void AlignChildToParentItem(LanguageNode parent) {
        var childs = parent.children.Values
            .OrderBy(child => -Vector3.SignedAngle(parent.transform.forward, child.transform.forward, Vector3.up))
            .ToList();
        var i = childs.IndexOf(this);
        var pp = parent.GetEndPosition();
        var screenWidth = 18f;
        var cellSize = screenWidth / childs.Count;
        var p = new Vector3((i * cellSize) - pp.x - screenWidth * 0.5f + cellSize * 0.5f, pp.y - 4, pp.z);
        StartCoroutine(LerpLanguageNodeTranslation(p, TranslationDuration));
    }

    private void AlignParentToChildItem(LanguageNode langChild) {
        var parents = LanguageManager.GetParents(langChild)
            .OrderBy(child => -Vector3.SignedAngle(langChild.transform.forward, child.transform.forward, Vector3.up))
            .ToList();
        var i = parents.IndexOf(this);
        var cp = langChild.GetEndPosition();
        var screenWidth = 10f;
        var cellSize = screenWidth / parents.Count;
        var p = new Vector3((i * cellSize) - cp.x - screenWidth * 0.5f + cellSize * 0.5f, cp.y + 4, cp.z);
        StartCoroutine(LerpLanguageNodeTranslation(p, TranslationDuration));
    }

    private IEnumerator LerpLanguageNodeTranslation(Vector3 nodeEndPosition, float translationDuration) {
        RegisterClickDisabler();
        var time = 0f; 
        while (time < translationDuration) {
            // lerp node translation
            var t = time / translationDuration;
            t = t * t * (3f - 2f * t); // ease animation
            transform.position = Vector3.Lerp(transform.position, nodeEndPosition, t);
            // increment
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = nodeEndPosition;
        UnregisterClickDisabler();
    }

    private void ToggleAncestryConnections(bool show) {
        ToggleAncestryConnections(_ => show);
    }

    private void ToggleAncestryConnections(Func<AncestryConnection, bool> comparison) {
        foreach (var connection in ancestryChildConnections) {
            if (comparison.Invoke(connection)) {
                if (!connection.gameObject.activeInHierarchy) {
                    connection.gameObject.SetActive(true);
                    StartCoroutine(((IFadable) connection).FadeIn(0.5f));
                }
            }
            else {
                if (connection.gameObject.activeInHierarchy) {
                    StartCoroutine(((IFadable) connection).FadeOut(0.5f, () => connection.gameObject.SetActive(false)));
                }
            }
        }
    }

    public void SetLangData(LanguageData newLangData) {
        langData = newLangData;
        name = newLangData.name;
        
        langLayout.SetName(langData.name);
        langLayout.SetPhonetic(langData.phonetic);
        langLayout.SetYears(langData.years);
        langLayout.SetMap(langData.name);
        langLayout.SetAlphabet(langData.alphabet);
        langLayout.SetDescription(langData.description);
        langLayout.ToNode();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!IsClickEnabled()) {
            return;
        }
        SetEndPosition();
        GraphRotator.SetEndRotation(this);
        
        OnLangNodeClicked?.Invoke(this);
    }

    public void SetEndPosition() {
        transform.localPosition = positionInGraph;
    }

    public Vector3 GetEndPosition() {
        return endPosition;
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

    public string GetAlphabet() {
        return langData.alphabet;
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
        if (langNode.GetName() == GetName()) { // if it's us
            ToItem();
        } else if (children.Values.Contains(langNode)) {        // if we are a parent
            ToItemParent(langNode);
        } else if (langNode.children.ContainsKey(GetName())) {  // if we are a child
            ToItemChild(langNode);
        } else {                                                // otherwise, we are unrelated
            ToggleAncestryConnections(false);
            if (gameObject.activeInHierarchy) {
                StartCoroutine(((IFadable) langLayout).FadeOut(0.5f, () => gameObject.SetActive(false)));
            }
        }
    }

    public static void RegisterClickDisabler() {
        Interlocked.Increment(ref _clickDisablers);
    }
    
    public static void UnregisterClickDisabler() {
        if (Interlocked.CompareExchange(ref _clickDisablers, 0, 0) == 0) {
            return;
        }
        Interlocked.Decrement(ref _clickDisablers);
    }

    public static bool IsClickEnabled() {
        return Interlocked.CompareExchange(ref _clickDisablers, 0, 0) == 0;
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