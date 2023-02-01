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

    [SerializeField] protected TextMeshProUGUI textElement;
    [SerializeField] private int maxCharacters;
    [SerializeField] protected Image bgImage;

    public static readonly Vector3 startPos = new Vector3(0, 0, 21.5f);

    [SerializeField] [HideInInspector] private Material textMaterial;


    private void Start() {
        BackArrowClickReceiver.OnBackArrowClicked += ResetMaterial;
        // LanguageNode.OnLangNodeClicked += SetMaterial;
        // AncestryConnection.OnConnectionClicked += SetMaterial;
        CameraController.OnCameraAnimationFinished += SetMaterial;
    }

    // Update is called once per frame
    void Update() {
        if (!textElement || !bgImage) {
            return;
        }

        textMaterial = new Material(textElement.fontMaterial);
        textElement.fontMaterial = textMaterial;
        // resize bg to text bounding box
        bgImage.rectTransform.sizeDelta = textElement.rectTransform.sizeDelta;
    }

    public void SetMaterial(Vector3 newPos) {
        if (!gameObject.activeSelf) {
            return;
        }
        textMaterial.SetVector("FrontOfGraph", newPos);
        textElement.fontMaterial = new Material(textMaterial);
        // GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = newPos;
    }
    
    public void ResetMaterial() {
        textElement.fontSharedMaterial.SetVector("FrontOfGraph", startPos);
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        // gameObject.layer = layerID;  // not necessary
        textElement.gameObject.layer = layerID;
        bgImage.gameObject.layer = layerID;
    }

    public void SetText(string text) {
        textElement.text = text;
    }
}
