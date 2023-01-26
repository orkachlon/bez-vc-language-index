using System;
using JetBrains.Annotations;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    private static LanguageNode _selectedLanguage;
    
    private void Start() {
        LanguageNode.OnLangNodeClicked += SetSelectedLanguage;
        AncestryConnection.OnConnectionClicked += SetSelectedLanguage;
    }

    [CanBeNull]
    public static LanguageNode GetSelectedLanguage() {
        return _selectedLanguage;
    }

    private static void SetSelectedLanguage(LanguageNode langNode) {
        _selectedLanguage = langNode;
    }
}
