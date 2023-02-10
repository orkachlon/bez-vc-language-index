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
    [SerializeField] private SpriteShapeController spriteShape;

    protected override void Start() {
        base.Start();
        if (!spriteShape) {
            spriteShape = GetComponentInChildren<SpriteShapeController>();
        }
    }

    public void SetName(string languageNameString) {
        languageName = languageNameString;
    }

    public void SetPhonetic([NotNull] string phoneticString) {
        phonetic = phoneticString;
    }
    
    public Vector2 GetSize() {
        var rightPos = spriteShape.spline.GetPosition(0);
        var leftPos = spriteShape.spline.GetPosition(1);
        return new Vector2((rightPos - leftPos).magnitude, spriteShape.spline.GetHeight(0));
        // return bgImage.rectTransform.sizeDelta;
    }

    protected override void AdjustBGSize() {
        var rx = transform.position + Vector3.right * (textElement.GetRenderedValues().x / 2f);
        // var ry = spriteShape.spline.GetPosition(0).y;
        // var rz = spriteShape.spline.GetPosition(0).z;
        var lx = transform.position + Vector3.left * (textElement.GetRenderedValues().x / 2f);
        // var ly = spriteShape.spline.GetPosition(1).y;
        // var lz = spriteShape.spline.GetPosition(1).z;
        var h = textElement.GetRenderedValues().y / 2f;
        if (!spriteShape) {
            spriteShape = GetComponentInChildren<SpriteShapeController>();
            if (!spriteShape) {
                return;
            }
        }
        print($"L: {lx}\tR: {rx}\tH: {h}");
        // spriteShape.spline.SetPosition(0, lx);
        // spriteShape.spline.SetPosition(1, rx);
        // spriteShape.spline.SetHeight(0, h);
        // spriteShape.spline.SetHeight(1, h);
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
        AdjustBGSize();
        MoveToLayer("UI");
    }

    public override void ToNode() {
        SetFontSize(nodeFontSize);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.text = languageName;
        textElement.ForceMeshUpdate();
        AdjustBGSize();
        MoveToLayer("Default");
    }
}
