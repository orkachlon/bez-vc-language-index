using System;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

[ExecuteAlways]
[Serializable]
public class TextContainer : MonoBehaviour, IItemContainer {

    [SerializeField] protected TextMeshProUGUI textElement;
    [SerializeField] protected Image bgImage;
    [SerializeField] private float textPadding;

    // not actually in use
    public static readonly Vector3 StartPos = new(0, 0, 21.5f);

    [SerializeField] [HideInInspector] protected float manualWidth;
    [SerializeField] [HideInInspector] protected float fontStartSize;
    [SerializeField] [HideInInspector] protected Vector2 textBoxStartSize;


    protected virtual void Start() {
        fontStartSize = textElement.fontSize;
        
        AdjustTextBoxSize();

        // adjust background size to fit text
        AdjustBGSize();
        textBoxStartSize = textElement.rectTransform.sizeDelta;
    }

    protected virtual void Update() {
        if (!textElement || !bgImage) {
            return;
        }
        // resize bg to text bounding box
        AdjustBGSize();
    }

    public Vector2 GetTextBoxSize() {
        return textElement.GetPreferredValues();
    }

    public Vector2 GetBGSize() {
        if (manualWidth > 0) {
            return new Vector2(manualWidth, textElement.rectTransform.sizeDelta.y);
        }
        return textElement.rectTransform.sizeDelta + Vector2.right * (textPadding * 2f);
    }

    public void SetBGWidth(float width) {
        manualWidth = width;
    }

    public virtual void ToItem() {
        MoveToLayer("UI");
    }

    public virtual void ToItemRelative() {
        MoveToLayer("Default");
    }

    public virtual void ToNode() {
        MoveToLayer("Default");
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        textElement.gameObject.layer = layerID;
        bgImage.gameObject.layer = layerID;
    }

    public void SetPosition(Vector3 newPos) {
        transform.position = newPos;
    }

    public virtual void SetText(string text) {
        textElement.text = text;
    }

    public bool IsEmpty() {
        return textElement.text.Length == 0;
    }

    public void SetFontSize(float newSize) {
        textElement.fontSize = newSize;
        AdjustTextBoxSize();
    }

    public void ResetFontSize() {
        textElement.rectTransform.sizeDelta = textBoxStartSize;
        textElement.fontSize = fontStartSize;
    }

    public RectTransform GetTextRectTransform() {
        return textElement.rectTransform;
    }

    public Vector3 GetBottomLeft() {
        var corners = new Vector3[4];
        bgImage.rectTransform.GetWorldCorners(corners);
        return corners[0];
    }
    
    public Vector3 GetTopRight() {
        var corners = new Vector3[4];
        bgImage.rectTransform.GetWorldCorners(corners);
        return corners[2];
    }
    
    protected void AdjustTextBoxSize() {
        textElement.ForceMeshUpdate(true);
        var textRect = textElement.GetRenderedValues(false);
        textElement.rectTransform.sizeDelta = textRect;
    }

    protected void AdjustBGSize() {
        bgImage.rectTransform.sizeDelta = GetBGSize();
    }
}
