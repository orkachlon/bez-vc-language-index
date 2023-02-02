using UnityEngine;

public class InfluencesContainer : TextContainer {
    
    
    protected override void Start() {
        base.Start();
        // doesn't work yet
        // CameraController.OnCameraAnimationFinished += StretchToLeft;
    }
    
    public void StretchToLeft() {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        var textPosition = textElement.rectTransform.position;
        var leftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight / 2f,
            Camera.main.nearClipPlane));
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = leftPoint;
        GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = textElement.rectTransform.pivot;
        var width = Vector3.Distance(leftPoint, textElement.rectTransform.pivot);
        textElement.rectTransform.sizeDelta = new Vector2(width, textElement.rectTransform.sizeDelta.y);
    }
}
