using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[ExecuteAlways]
public class TextContainer : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI textElement;
    [SerializeField] private int maxCharacters;
    [SerializeField] private Image bgImage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (!textElement || !bgImage) {
            return;
        }
        // resize bg to text bounding box
        bgImage.rectTransform.sizeDelta = textElement.rectTransform.sizeDelta;
    }

    public void SetText(string text) {
        textElement.text = text;
    }
}
