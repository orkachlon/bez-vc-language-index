using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapContainer : MonoBehaviour, IItemContainer {

    [SerializeField] private Image map;


    protected void Start() {
        if (map == null) {
            throw new Exception("Map container doesn't have a map!");
        }
    }

    public Vector2 GetSize() {
        return map.rectTransform.sizeDelta;
    }

    public void LoadMap(string languageName) {
        if (!File.Exists(Directory.GetCurrentDirectory() + $"/Assets/Resources/Maps/{languageName}.png")) {
            return;
        }
        map.sprite = Resources.Load<Sprite>($"Maps/{languageName}");
    }

    public void SetPosition(Vector3 newPos) {
        transform.position = newPos;
    }
    
    public void ToItem() {
        gameObject.SetActive(true);
        MoveToLayer("UI");
    }

    public void ToItemRelative() {
        gameObject.SetActive(false);
        MoveToLayer("Default");
    }

    public void ToNode() {
        gameObject.SetActive(false);
        MoveToLayer("Default");
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        map.gameObject.layer = layerID;
    }
}