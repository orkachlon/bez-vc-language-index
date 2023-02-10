using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class LanguageNameContainer : TextContainer {

    [SerializeField] private float nodeFontSize = 0.8f;
    [SerializeField] private float itemFontSize = 0.5f;
    [SerializeField] private float itemRelativeFontSize = 0.3f;
    [SerializeField] [HideInInspector] private string phonetic;
    [SerializeField] [HideInInspector] private string languageName;

    protected override void Start() {
        base.Start();
        // insert new lines if text is too long
        // InsertNewLines();
    }

    public void SetName(string languageNameString) {
        languageName = languageNameString;
    }

    public void SetPhonetic([NotNull] string phoneticString) {
        phonetic = phoneticString;
    }
    
    public Vector2 GetSize() {
        return bgImage.rectTransform.sizeDelta;
    }

    public override void ToItemRelative() {
        SetFontSize(itemRelativeFontSize);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.text = languageName;
        textElement.ForceMeshUpdate();
        MoveToLayer("UI");
    }
    
    public override void ToItem() {
        SetFontSize(itemFontSize);
        textElement.alignment = TextAlignmentOptions.Left;
        textElement.text = $"{languageName}<size=60%>\n<font=NotoSerif-Italic SDF>/{phonetic}/</font></size>";
        textElement.ForceMeshUpdate();
        MoveToLayer("UI");
    }

    public override void ToNode() {
        SetFontSize(nodeFontSize);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.text = languageName;
        textElement.ForceMeshUpdate();
        MoveToLayer("Default");
    }
}
