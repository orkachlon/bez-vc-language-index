using System;
using JetBrains.Annotations;
using UnityEngine;

public class LanguageLayout : MonoBehaviour {

    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private YearsContainer years;
    [SerializeField] private MapContainer map;
    [SerializeField] [CanBeNull] private TextContainer alphabet;

    [SerializeField] private float yearsSpacing;
    [SerializeField] private float mapSpacing;

    private void Start() {
        languageName = gameObject.GetLanguageComponent<LanguageNameContainer>(languageName, "LanguageName");
        years = gameObject.GetLanguageComponent<YearsContainer>(years, "Years");
        map = gameObject.GetLanguageComponent<MapContainer>(map, "Map");
    }

    public void ToNode() {
        // hide all fields but name
        years.Hide();
        map.Hide();
        // not all languages have an alphabet
        if (alphabet) {
            alphabet.Hide();
        }

        // erase phonetic from name
        languageName.ToNode();
    }

    public void ToItem() {
        // show all fields
        years.Show();
        map.Show();
        // not all languages have an alphabet
        if (alphabet) {
            alphabet.Show();
        }
        // add phonetic to name
        languageName.ToItem();
        
        // scale boxes
        // name and years
        var newWidth = 0f;
        newWidth = years.GetTextBoxSize().x > languageName.GetTextBoxSize().x ? 
            years.GetBGSize().x :
            languageName.GetBGSize().x;
        years.SetBGWidth(newWidth);
        languageName.SetBGWidth(newWidth);
        
        // align boxes
        // years
        var bottomBound = languageName.GetBottomLeft().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(currPosition.x, bottomBound - yearsSpacing, currPosition.z);
        years.transform.position = newPosition;
        // map
        var leftBound = languageName.GetBottomLeft().x;
        bottomBound = years.GetBottomLeft().y;
        currPosition = transform.position;
        newPosition = new Vector3(leftBound - mapSpacing, bottomBound + map.GetSize().y / 2f, currPosition.z);
        map.transform.position = newPosition;
    }

    public void ToItemRelative() {
        languageName.ToItemRelative();
    }
    
    public void SetName(string newName) {
        languageName.SetName(newName);
        languageName.SetText(newName);
    }
    
    public void SetPhonetic(string newPhonetic) {
        languageName.SetPhonetic(newPhonetic);
    }
    
    public void SetYears(string newYears) {
        years.SetText(newYears);
    }
    
    public void SetMap(string pathToMap) {
        map.LoadMap(pathToMap);
    }

}