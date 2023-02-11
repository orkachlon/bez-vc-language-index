using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class LanguageNameContainer : TextContainer {

    [SerializeField] private float nodeFontSize = 0.8f;
    [SerializeField] private float itemFontSize = 0.5f;
    [SerializeField] private float itemRelativeFontSize = 0.3f;

    [SerializeField] [HideInInspector] private string phonetic;
    [SerializeField] [HideInInspector] private string languageName;
    
    protected override void Start() {
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues() + Vector2.right * (textPadding * 2);
        AdjustBGSize();
        // textElement.margin = new Vector4(textPadding, 0, textPadding, 0);
    }

    public void SetName([NotNull] string languageNameString) {
        languageName = languageNameString;
    }

    public void SetPhonetic([NotNull] string phoneticString) {
        phonetic = phoneticString;
    }
    
    public override void ToItemRelative() {
        SetFontSize(itemRelativeFontSize);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.margin = new Vector4(0, 0, 0, 0);
        textElement.text = languageName;
        textElement.ForceMeshUpdate();
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues();
        AdjustBGSize();
        MoveToLayer("UI");
    }
    
    public override void ToItem() {
        SetFontSize(itemFontSize);
        textElement.alignment = TextAlignmentOptions.Left;
        textElement.margin = new Vector4(textPadding, 0, 0, 0);
        textElement.text = $"{languageName}<size=60%>\n<font=NotoSerif-Italic SDF>/{phonetic}/</font></size>";
        textElement.ForceMeshUpdate();
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues() + Vector2.right * (2 * textPadding);
        AdjustBGSize();
        MoveToLayer("UI");
    }

    public override void ToNode() {
        SetFontSize(nodeFontSize);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.margin = new Vector4(0, 0, 0, 0);
        textElement.text = languageName;
        textElement.ForceMeshUpdate();
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues() + Vector2.right * (textPadding * 2);
        AdjustBGSize();
        MoveToLayer("Default");
    }
}
