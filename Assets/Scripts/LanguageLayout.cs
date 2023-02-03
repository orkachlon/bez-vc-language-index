﻿using System;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class LanguageLayout : MonoBehaviour {

    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private YearsContainer years;
    [SerializeField] private MapContainer map;
    [SerializeField] [CanBeNull] private TextContainer alphabet;

    [SerializeField] private float yearsSpacing;
    [SerializeField] private float mapSpacing;


    private void Start() {
        uiCanvas = GetComponent<Canvas>();
        languageName = gameObject.GetLanguageComponent<LanguageNameContainer>(languageName, "LanguageName");
        years = gameObject.GetLanguageComponent<YearsContainer>(years, "Years");
        map = gameObject.GetLanguageComponent<MapContainer>(map, "Map");
    }

    private void Update() {
        AlignYears();
        AlignMap();
    }

    public void ToNode() {
        // hide all fields but name
        years.ToNode();
        map.ToNode();
        // not all languages have an alphabet
        if (alphabet) {
            alphabet.ToNode();
        }

        // erase phonetic from name
        languageName.ToNode();
        
        // render by depth
        uiCanvas.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void ToItem() {
        // show all fields
        years.ToItem();
        map.ToItem();
        // not all languages have an alphabet
        if (alphabet) {
            alphabet.ToItem();
        }
        // add phonetic to name
        languageName.ToItem();
        
        // scale boxes
        // name and years
        if (years.GetTextBoxSize().x > languageName.GetTextBoxSize().x) {
            languageName.SetBGWidth(years.GetBGSize().x);
        }
        else {
            years.SetBGWidth(languageName.GetBGSize().x);
        }
        
        // align boxes
        // years
        AlignYears();
        // map
        AlignMap();

        // move item to UI layer (render in front of lines)
        uiCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
        languageName.MoveToLayer("UI");
        years.MoveToLayer("UI");
        map.MoveToLayer("UI");
    }

    private void AlignYears() {
        var bottomBound = languageName.GetBottomLeft().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(currPosition.x, bottomBound - yearsSpacing, currPosition.z);
        years.SetPosition(newPosition);
    }

    private void AlignMap() {
        float bottomBound;
        Vector3 currPosition;
        Vector3 newPosition;
        var leftBound = languageName.GetBottomLeft().x;
        bottomBound = years.GetBottomLeft().y;
        currPosition = transform.position;
        newPosition = new Vector3(leftBound - mapSpacing, bottomBound + map.GetSize().y / 2f, currPosition.z);
        map.SetPosition(newPosition);
    }

    public void ToItemRelative() {
        languageName.ToItemRelative();
        years.ToItemRelative();
        map.ToItemRelative();
        if (alphabet) {
            alphabet.ToItemRelative();
        }
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
    
    public void SetMap(string pathToMap) {
        map.LoadMap(pathToMap);
    }

}