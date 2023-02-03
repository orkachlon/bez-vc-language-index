using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapContainer : MonoBehaviour, IItemContainer {

    [SerializeField] private Image map;
    [SerializeField] private float spacing;


    protected void Start() {
        if (map == null) {
            throw new Exception("Map container doesn't have a map!");
        }
    }

    public Vector2 GetSize() {
        return map.rectTransform.sizeDelta;
    }

    public void LoadMap(string pathToMap) {
        
    }

    public void SetPosition(Vector3 newPos) {
        transform.position = newPos;
    }
    
    public void ToItem() {
        gameObject.SetActive(true);
    }

    public void ToItemRelative() {
        gameObject.SetActive(false);
    }

    public void ToNode() {
        gameObject.SetActive(false);
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        map.gameObject.layer = layerID;
    }
}