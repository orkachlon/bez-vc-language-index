using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[ExecuteAlways]
[Serializable]
public class TextContainer : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textElement;
    [SerializeField] private int maxCharacters;
    [SerializeField] private Image bgImage;

    // Update is called once per frame
    void Update() {
        if (!textElement || !bgImage) {
            return;
        }
        // resize bg to text bounding box
        bgImage.rectTransform.sizeDelta = textElement.rectTransform.sizeDelta;
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        gameObject.layer = layerID;
        textElement.gameObject.layer = layerID;
        bgImage.gameObject.layer = layerID;
    }

    public void SetText(string text) {
        textElement.text = text;
    }
}
