using System;
using UnityEngine;

public class YearsContainer : TextContainer {

    [SerializeField] private LanguageNameContainer languageName;

    protected override void Start() {
        if (languageName == null) {
            throw new Exception("A reference to language name is needed in years");
        }
        base.Start();

        textElement.rectTransform.sizeDelta = languageName.GetSize();
    }
}
