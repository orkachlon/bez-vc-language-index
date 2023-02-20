using System;
using UnityEngine;

public class DescriptionContainer : TextContainer {

    [SerializeField] [Range(5, 7)] private float minWidth;
    [SerializeField] [Range(7, 10)] private float maxWidth;
    [SerializeField] [Range(0, 3)] private float maxHeight;
    
    protected override void Awake() {
        base.Awake();
        textElement.margin = new Vector4(textPadding * 0.5f, textPadding * 0.5f, textPadding, textPadding);
        var initialSize = textElement.GetPreferredValues(minWidth - textPadding * 1.5f, 0);
        SetSize(initialSize);
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var pref = textElement.GetPreferredValues(textElement.rectTransform.sizeDelta.x - textPadding * 2, 0).y;
        var worldPivot = rectTransform.position;
        Gizmos.DrawLine(worldPivot, worldPivot + Vector3.down * pref);
    }
    
    public override void ToItem() {
        gameObject.SetActive(true);
    }
    
    public override void ToItemRelative() {
        gameObject.SetActive(false);
    }

    public override void ToNode() {
        gameObject.SetActive(false);
    }

    public override void SetSize(Vector2 newSize) {
        var newWidth = newSize.x; // Mathf.Max(newSize.x, minWidth);
        var prefHeight = textElement.GetPreferredValues(newWidth - textPadding * 1.5f, 0).y;
        base.SetSize(new Vector2(newWidth, prefHeight));
    }
}
