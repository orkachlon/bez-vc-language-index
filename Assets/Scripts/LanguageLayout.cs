using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[ExecuteAlways]
public class LanguageLayout : MonoBehaviour {

    [SerializeField] private Canvas uiCanvas;
    [Header("Containers")]
    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private YearsContainer years;
    [SerializeField] private MapContainer map;
    [SerializeField] private AlphabetContainer alphabet;
    [SerializeField] private DescriptionContainer description;
    [SerializeField] private ImageContainer picture;

    [Header("Spacing")]
    [SerializeField] [Range(0, 1)] private float yearsSpacing;
    [SerializeField] [Range(0, 1)] private float alphabetSpacing;
    [SerializeField] [Range(0, 1)] private float mapSpacing;
    [SerializeField] [Range(0, 1)] private float descriptionSpacing;
    
    [SerializeField] [HideInInspector] private Camera mainCamera;


    private void Awake() {
        mainCamera = Camera.main;
        uiCanvas = GetComponent<Canvas>();
        BindCameraToCanvas();
        RotateTowardsCamera();
        
        // Get all container components
        languageName = gameObject.GetLanguageComponent<LanguageNameContainer>(languageName, "LanguageName");
        years = gameObject.GetLanguageComponent<YearsContainer>(years, "Years");
        alphabet = gameObject.GetLanguageComponent<AlphabetContainer>(alphabet, "Alphabet");
        description = gameObject.GetLanguageComponent<DescriptionContainer>(description, "Description");
        map = gameObject.GetLanguageComponent<MapContainer>(map, "Map");
        picture = gameObject.GetLanguageComponent<ImageContainer>(picture, "Picture");
    }

    private void Update() {
        RotateTowardsCamera();
        AlignYears();
        AlignMap();
        AlignAlphabet();
        AlignDescription();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(languageName.transform.position, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(years.transform.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(map.transform.position, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(alphabet.transform.position, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(description.transform.position, 0.1f);
    }

    public void ToNode() {
        // move layout to same layer as lines
        gameObject.layer = LayerMask.NameToLayer("Default");
        // hide all fields but name
        years.ToNode();
        map.ToNode();
        // not all languages have an alphabet
        if (!alphabet.IsEmpty()) {
            alphabet.ToNode();
        }

        // erase phonetic from name
        languageName.ToNode();
        description.ToNode();
        picture.ToNode();
    }

    public void ToItem() {
        // bring layout in front of lines
        gameObject.layer = LayerMask.NameToLayer("UI");
        languageName.ToItem();
        years.ToItem();
        map.ToItem();
        alphabet.ToItem();
        description.ToItem();
        picture.ToItem();
        
        // set BG sizes
        var midWidth = Mathf.Max(
            years.GetSize().x, 
            languageName.GetSize().x
            );
        
        years.SetWidth(midWidth);
        languageName.SetWidth(midWidth);
        if (!alphabet.IsEmpty()) {
            // alphabet.SetWidth(midWidth);
        }

        AlignYears();
        AlignAlphabet();
        AlignMap();
        AlignDescription();
    }

    public void ToItemRelative() {
        // bring layout in front of lines
        gameObject.layer = LayerMask.NameToLayer("UI");
        
        languageName.ToItemRelative();
        years.ToItemRelative();
        map.ToItemRelative();
        if (!alphabet.IsEmpty()) {
            alphabet.ToItemRelative();
        }
        description.ToItemRelative();
        picture.ToItemRelative();
    }

    private void AlignYears() {
        var topBound = languageName.GetTopRight().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(currPosition.x, topBound + yearsSpacing, currPosition.z);
        years.SetPosition(newPosition);
    }

    private void AlignMap() {
        var nameBotLeft = languageName.GetBotLeft();
        var newPosition = new Vector3(nameBotLeft.x - mapSpacing, nameBotLeft.y, transform.position.z);
        map.SetPosition(newPosition);
    }

    private void AlignAlphabet() {
        if (alphabet.IsEmpty()) {
            return;
        }
        var nameTopRight = languageName.GetTopRight();
        var newPosition = new Vector3(nameTopRight.x + alphabetSpacing, nameTopRight.y, transform.position.z);
        alphabet.SetPosition(newPosition);
    }

    private void AlignDescription() {
        var mapBotLeft = map.GetBotLeft();
        var newPosition = new Vector3(mapBotLeft.x, mapBotLeft.y - descriptionSpacing, transform.position.z);
        description.SetPosition(newPosition);
        var descWidth = Mathf.Abs(languageName.GetTopRight().x - map.GetBotLeft().x);
        description.SetWidth(descWidth);
    }

    private void RotateTowardsCamera() {
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
        uiCanvas.worldCamera = mainCamera == null ? Camera.main : mainCamera;
    }

    public void SetName(string newName) {
        languageName.SetName(newName);
    }
    
    public void SetPhonetic(string newPhonetic) {
        languageName.SetPhonetic(newPhonetic);
    }
    
    public void SetYears(string newYears) {
        years.SetText(newYears);
    }

    public void SetMap(string langName) {
        map.LoadImage(langName);
    }

    public void SetAlphabet(string newAlphabet) {
        alphabet.SetText(newAlphabet);
    }

    public void SetDescription(string newDescription) {
        description.SetText(newDescription);
    }
}