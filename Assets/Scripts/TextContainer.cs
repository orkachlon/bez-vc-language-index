using System;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

[ExecuteAlways]
[Serializable]
public class TextContainer : MonoBehaviour, IItemContainer {

    // [SerializeField] private Canvas uiCanvas;
    [SerializeField] protected TextMeshProUGUI textElement;
    [SerializeField] protected Image bgImage;
    [SerializeField] protected float textPadding;

    [SerializeField] [HideInInspector] protected RectTransform rectTransform;
    
    // [SerializeField] [HideInInspector] private Camera mainCam;
    
    protected virtual void Awake() {
        // mainCam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) {
            print($"no rect transform on {name}");
        }

        var nodeMargins = GetNodeMargins();
        SetSize(textElement.GetPreferredValues() + new Vector2(nodeMargins.x + nodeMargins.z, nodeMargins.y + nodeMargins.w));
    }

    public virtual Vector2 GetSize() {
        return rectTransform.sizeDelta;
    }

    public virtual void SetWidth(float width) {
        SetSize(new Vector2(width, textElement.rectTransform.sizeDelta.y));
    }

    public virtual void SetSize(Vector2 newSize) {
        textElement.rectTransform.sizeDelta = newSize;
        // bgImage.rectTransform.sizeDelta = newSize;
        rectTransform.sizeDelta = newSize;
    }

    public virtual void ToItem() {
        SetOpacity(1f);
        MoveToLayer("UI");
    }

    public virtual void ToItemRelative() {
        SetOpacity(1f);
        MoveToLayer("Default");
    }

    public virtual void ToNode() {
        SetOpacity(1f);
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

    public RectTransform GetTextRectTransform() {
        return textElement.rectTransform;
    }

    public Vector3 GetBotLeft() {
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
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues();
    }
    
    public Vector4 GetItemMargins() {
        return new(textPadding * 0.5f, textPadding * 0.5f, 0, textPadding);
    }
    
    public Vector4 GetNodeMargins() {
        return new(textPadding, 0, textPadding * 0.5f, textPadding * 0.5f);
    }
    
    public Vector4 GetItemRelativeMargins() {
        return new(textPadding * 0.5f, 0, textPadding * 0.5f, 0);
    }

    public float GetOpacity() {
        return bgImage.color.a;
    }

    public void SetOpacity(float percent) {
        var bgColor = bgImage.color;
        bgImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, percent);
        var textColor = textElement.color;
        textElement.color = new Color(textColor.r, textColor.g, textColor.b, percent);
    }
    
    // public void RotateTowardsCamera() {
    //     var canvasPos = uiCanvas.transform.position;
    //     uiCanvas.transform.LookAt(new Vector3(canvasPos.x, canvasPos.y, mainCam.transform.position.z));
    // }
}
