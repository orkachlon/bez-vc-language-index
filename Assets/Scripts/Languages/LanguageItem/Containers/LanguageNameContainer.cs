using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Utils.UI;

namespace Languages.LanguageItem.Containers {
    public class LanguageNameContainer : TextContainer, IFadable {

        [SerializeField] private float nodeFontSize = 0.8f;
        [SerializeField] private float itemFontSize = 0.5f;
        [SerializeField] private float itemRelativeFontSize = 0.3f;

        [SerializeField][HideInInspector] private string phonetic;
        [SerializeField][HideInInspector] private string languageName;

        protected override void Awake() {
            base.Awake();
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
            textElement.margin = GetItemRelativeMargins();
            textElement.text = languageName;
            textElement.ForceMeshUpdate();
            SetSize(textElement.GetPreferredValues());
            base.ToItemRelative();
        }

        public override void ToItem() {
            SetFontSize(itemFontSize);
            textElement.alignment = TextAlignmentOptions.Left;
            textElement.margin = GetItemMargins();
            textElement.text = $"{languageName}<size=60%>\n<font=NotoSerif-Italic SDF>/{phonetic}/</font></size>";
            textElement.ForceMeshUpdate();
            SetSize(textElement.GetPreferredValues() + Vector2.right * (2 * textPadding));
            base.ToItem();
        }

        public override void ToNode() {
            SetFontSize(nodeFontSize);
            textElement.alignment = TextAlignmentOptions.Center;
            textElement.margin = GetNodeMargins();
            textElement.text = languageName;
            textElement.ForceMeshUpdate();
            SetSize(textElement.GetPreferredValues());
            base.ToNode();
        }
    }
}