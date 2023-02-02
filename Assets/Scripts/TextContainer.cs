using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[ExecuteAlways]
[Serializable]
public class TextContainer : MonoBehaviour {

    [SerializeField] protected TextMeshProUGUI textElement;
    [SerializeField] protected Image bgImage;
    [SerializeField] private float textPadding;

    // not actually in use
    public static readonly Vector3 StartPos = new(0, 0, 21.5f);

    [SerializeField] [HideInInspector] protected float fontStartSize;
    [SerializeField] [HideInInspector] protected Vector2 textBoxStartSize;


    protected virtual void Start() {
        fontStartSize = textElement.fontSize;
        
        AdjustTextBoxSize();

        // adjust background size to fit text
        AdjustBGSize();
        textBoxStartSize = textElement.rectTransform.sizeDelta;
    }

    private void Update() {
        if (!textElement || !bgImage) {
            return;
        }
        // resize bg to text bounding box
        AdjustBGSize();
    }

    public void SetMaterial(Vector3 newPos) {
        if (!gameObject.activeInHierarchy) {
            return;
        }
    }

    public void ResetMaterial() {
        textElement.fontSharedMaterial.SetVector("FrontOfGraph", StartPos);
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        textElement.gameObject.layer = layerID;
        bgImage.gameObject.layer = layerID;
    }

    public void SetText(string text) {
        textElement.text = text;
    }

    public void SetFontSize(float newSize) {
        textElement.fontSize = newSize;
        AdjustTextBoxSize();
    }

    public void ResetFontSize() {
        textElement.rectTransform.sizeDelta = textBoxStartSize;
        textElement.fontSize = fontStartSize;
    }
    
    protected void AdjustTextBoxSize() {
        textElement.ForceMeshUpdate(true);
        var textRect = textElement.GetRenderedValues(false);
        textElement.rectTransform.sizeDelta = textRect + Vector2.one * textPadding * 2f;
    }

    protected void AdjustBGSize() {
        // var bgSize = new Vector2(textElement.preferredWidth, textElement.preferredHeight);
        bgImage.rectTransform.sizeDelta = textElement.rectTransform.sizeDelta;
    }
}
