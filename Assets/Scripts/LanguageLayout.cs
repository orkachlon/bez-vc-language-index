using UnityEngine;

[RequireComponent(typeof(Canvas))]
[ExecuteAlways]
public class LanguageLayout : MonoBehaviour {

    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private YearsContainer years;
    [SerializeField] private MapContainer map;
    [SerializeField] private AlphabetContainer alphabet;

    [SerializeField] private float yearsSpacing;
    [SerializeField] private float mapSpacing;
    
    [SerializeField] [HideInInspector] private Camera mainCamera;


    private void Start() {
        mainCamera = Camera.main;
        uiCanvas = GetComponent<Canvas>();
        BindCameraToCanvas();
        RotateTowardsCamera();
        
        // Get all container components
        languageName = gameObject.GetLanguageComponent<LanguageNameContainer>(languageName, "LanguageName");
        years = gameObject.GetLanguageComponent<YearsContainer>(years, "Years");
        alphabet = gameObject.GetLanguageComponent<AlphabetContainer>(alphabet, "Alphabet");
        map = gameObject.GetLanguageComponent<MapContainer>(map, "Map");
    }

    private void Update() {
        RotateTowardsCamera();
        AlignYears();
        AlignMap();
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
    }

    public void ToItem() {
        // bring layout in front of lines
        gameObject.layer = LayerMask.NameToLayer("UI");
        languageName.ToItem();
        // years
        years.ToItem();
        AlignYears();
        if (years.GetTextBoxSize().x > languageName.GetTextBoxSize().x) {
            languageName.SetBGWidth(years.GetBGSize().x);
        }
        else {
            years.SetBGWidth(languageName.GetBGSize().x);
        }
        
        map.ToItem();
        
        // not all languages have an alphabet
        if (!alphabet.IsEmpty()) {
            alphabet.ToItem();
        }

        // align boxes
        // map
        AlignMap();
        // alphabet
        AlignAlphabet();
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
    }

    private void AlignYears() {
        var bottomBound = languageName.GetBottomLeft().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(currPosition.x, bottomBound - yearsSpacing, currPosition.z);
        years.SetPosition(newPosition);
    }

    private void AlignMap() {
        var leftBound = languageName.GetBottomLeft().x;
        var bottomBound = years.GetBottomLeft().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(leftBound - mapSpacing, bottomBound + map.GetSize().y / 2f, currPosition.z);
        map.SetPosition(newPosition);
    }

    private void AlignAlphabet() {
        if (!alphabet.IsEmpty()) {
            return;
        }
        // todo align alphabet
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
        map.LoadMap(langName);
    }
    
    public void SetAlphabet(string newAlphabet) {
        alphabet.SetText(newAlphabet);
    }
}