using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapContainer : MonoBehaviour {

    [SerializeField] private Image map;
    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private YearsContainer years;
    [SerializeField] private float spacing;


    protected void Start() {
        if (map == null) {
            throw new Exception("Map container doesn't have a map!");
        }
        if (languageName == null) {
            throw new Exception("Map container isn't linked to a language name!");
        }
        if (years == null) {
            throw new Exception("Map container isn't linked to a years container!");
        }
    }

    private void Update() {
        SetPosition();
    }

    public Vector2 GetSize() {
        return map.rectTransform.sizeDelta;
    }

    public void LoadMap(string pathToMap) {
        
    }

    public void Show() {
        gameObject.SetActive(true);
        // // position
        // SetPosition();
    }
    
    private void SetPosition() {
        var leftBound = languageName.GetBottomLeft().x;
        var bottomBound = years.GetBottomLeft().y;
        var currPosition = transform.position;
        var newPosition = new Vector3(leftBound - spacing, bottomBound + map.rectTransform.sizeDelta.y / 2f, currPosition.z);
        transform.position = newPosition;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}