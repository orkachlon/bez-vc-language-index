
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using UnityEngine;

public class AlphabetContainer : TextContainer {

    [SerializeField] [HideInInspector] private List<string> unicodeList;
    
    public override void ToItem() {
        if (textElement.text.Length == 0) {
            return;
        }
        gameObject.SetActive(true);
        textElement.rectTransform.sizeDelta = textElement.GetPreferredValues();
        AdjustBGSize();
    }
    
    public override void ToItemRelative() {
        gameObject.SetActive(false);
    }

    public override void ToNode() {
        gameObject.SetActive(false);
    }

    public override Vector2 GetTextBoxSize() {
        return textElement.text.Length > 0 ? base.GetTextBoxSize() : Vector2.zero;
    }

    // might still need this
    private List<string> GetUnicodeList() {
        if (unicodeList.Count > 0) {
            return unicodeList;
        }
        var url = "https://www.unicode.org/Public/15.0.0/ucd/UnicodeData.txt";

        var query = from record in new WebClient().DownloadString(url).Split('\n')
            where !string.IsNullOrEmpty(record)
            let properties = record.Split(';')
            where properties[4] == "R" || properties[4] == "AL"
            select int.Parse(properties[0], NumberStyles.AllowHexSpecifier);

        unicodeList = query.Select(codepoint => codepoint.ToString("X4")).ToList();
        return unicodeList;
    }

    //  don't know if this works
    private bool IsRandALCat(int codePoint) {
        return GetUnicodeList().Contains($"{codePoint}");
    }
    
    private bool IsAnyCharacterRightToLeft(string s) {
        for (var i = 0; i < s.Length; i += char.IsSurrogatePair(s, i) ? 2 : 1) {
            var codepoint = char.ConvertToUtf32(s, i);
            if (IsRandALCat(codepoint)) {
                return true;
            }
        }
        return false;
    }
}