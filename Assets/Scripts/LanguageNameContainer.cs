using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class LanguageNameContainer : TextContainer {

    [SerializeField] private int maxCharactersPerLine;
    [SerializeField] [HideInInspector] private string phonetic;
    [SerializeField] [HideInInspector] private string languageName;

    protected override void Start() {
        base.Start();
        // insert new lines if text is too long
        // InsertNewLines();
    }

    public override void Show() {
    }

    public override void Hide() {
    }

    public void SetName(string languageNameString) {
        languageName = languageNameString;
    }

    public void SetPhonetic([NotNull] string phoneticString) {
        phonetic = phoneticString;
    }

    private void MultilineText() {
        var text = textElement.text;
        // remove newlines and tags
        text = text.Replace("\n", " ");
        text = Regex.Replace(text, "<\\w=[0-9A-Za-z -]+>>", "");
        text = Regex.Replace(text, "</\\w>", "");
        if (text.Length <= maxCharactersPerLine) {
            return;
        }

        var spaceIndex = text.IndexOf(" ", StringComparison.Ordinal);
        if (spaceIndex >= 0 && spaceIndex <= maxCharactersPerLine) {
            text = text.Insert(spaceIndex, "\n");
            text = text.Remove(spaceIndex + 1, 1);
        }
    }
    
    private void InsertNewLines() {
        var text = textElement.text;
        text = text.Replace("\n", " ");
        var matches = Regex.Matches(text, " ");
        Match prevMatch = null;
        var currCharsInLine = 0;
        foreach (Match match in matches) {
            if (currCharsInLine > maxCharactersPerLine && prevMatch != null) {
                // insert newline
                text = text.Insert(prevMatch.Index, "\n");
                // remove old space
                text = text.Remove(prevMatch.Index + 1, 1);
                // reset the amount of chars in current line
                currCharsInLine = match.Index - prevMatch.Index;
            }

            // count the amount of chars in current line
            currCharsInLine += prevMatch == null ? match.Index : match.Index - prevMatch.Index;
            // keep reference to previous match
            prevMatch = match;
        }

        // check num of chars until end of string
        if (prevMatch != null && prevMatch.Index < text.Length) {
            currCharsInLine += text.Length - prevMatch.Index;
            
            // if we exceed limit - insert newline
            if (currCharsInLine > maxCharactersPerLine) {
                // insert newline
                text = text.Insert(prevMatch.Index, "\n");
                // remove old space
                text = text.Remove(prevMatch.Index + 1, 1);
            }
        }
        
        textElement.text = text;
        AdjustTextBoxSize();
    }

    public Vector2 GetSize() {
        return bgImage.rectTransform.sizeDelta;
    }

    public void ToItemRelative() {
        SetFontSize(0.5f);
    }
    
    public void ToItem() {
        SetFontSize(0.5f);
        textElement.alignment = TextAlignmentOptions.Left;
        textElement.text += $"<size=60%>\n<font=NotoSerif-Italic SDF>({phonetic})</font></size>";
    }

    public void ToNode() {
        SetFontSize(0.8f);
        textElement.alignment = TextAlignmentOptions.Center;
        textElement.text = languageName;
    }
}
