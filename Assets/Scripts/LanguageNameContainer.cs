using System.Text.RegularExpressions;
using UnityEngine;

public class LanguageNameContainer : TextContainer {

    [SerializeField] private int maxCharactersPerLine;

    protected override void Start() {
        base.Start();
        // insert new lines if text is too long
        InsertNewLines();
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
        return textElement.rectTransform.sizeDelta;
    }
    
}
