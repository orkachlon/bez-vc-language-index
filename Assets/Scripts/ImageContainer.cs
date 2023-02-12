using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class ImageContainer : MonoBehaviour, IItemContainer, IPointerEnterHandler, IPointerExitHandler {
    
    [SerializeField] protected Image image;
    [SerializeField] [HideInInspector] private string picTooltip;
    
    protected virtual void Awake() {
        if (image) 
            return;
        
        image = gameObject.GetComponentInChildren<Image>();
        if (!image) {
            throw new Exception("Image container isn't linked to an image component");
        }
    }
    
    public virtual void LoadImage(string languageName, string newPicTooltip = "") {
        if (!File.Exists(Directory.GetCurrentDirectory() + $"/Assets/Resources/Pictures/{languageName}.jpg")) {
            return;
        }
        image.sprite = Resources.Load<Sprite>($"Pictures/{languageName}");
        picTooltip = newPicTooltip;
    }
    
    public Vector2 GetSize() {
        return image.rectTransform.sizeDelta;
    }
    
    public void ToNode() {
        gameObject.SetActive(false);
        MoveToLayer("Default");
    }

    public void ToItem() {
        gameObject.SetActive(true);
        MoveToLayer("UI");
    }

    public void ToItemRelative() {
        gameObject.SetActive(false);
        MoveToLayer("Default");
    }

    public void MoveToLayer(string layerName) {
        var layerID = LayerMask.NameToLayer(layerName);
        if (layerID == -1) {
            throw new ArgumentOutOfRangeException($"Couldn't find Layer {layerName}!");
        }

        image.gameObject.layer = layerID;
    }

    public void SetPosition(Vector3 newPos) {
        transform.position = newPos;
    }

    public Vector3 GetTopRight() {
        var corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        return corners[2];
    }

    public Vector3 GetBotLeft() {
        var corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        return corners[0];
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (picTooltip.Length == 0) {
            return;
        }
        LanguageNameTooltip.ShowTooltipStatic(picTooltip);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (picTooltip.Length == 0) {
            return;
        }
        LanguageNameTooltip.HideTooltipStatic();
    }
}