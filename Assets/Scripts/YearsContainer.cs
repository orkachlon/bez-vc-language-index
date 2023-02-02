using System;
using UnityEngine;

[ExecuteAlways]
[Serializable]
public class YearsContainer : TextContainer {

    [SerializeField] private LanguageNameContainer languageName;
    [SerializeField] private float spacing;

    protected override void Start() {
        if (languageName == null) {
            throw new Exception("A reference to language name is needed in years");
        }
        base.Start();
    }

    // protected override void Update() {
        // base.Update();
        // SetSpacingFromLanguageName();
    // }

    public override void Show() {
        gameObject.SetActive(true);
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }
}
