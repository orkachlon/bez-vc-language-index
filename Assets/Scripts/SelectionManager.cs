using System;
using JetBrains.Annotations;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    private static LanguageNode _selectedLanguage = null;
    
    private void Start() {
        _selectedLanguage = null;
        LanguageNode.OnLangNodeClicked += SetSelectedLanguage;
        AncestryConnection.OnConnectionClicked += SetSelectedLanguage;
        BackArrowClickReceiver.OnBackArrowClicked += ResetSelectedLanguage;
    }

    private void OnDestroy() {
        LanguageNode.OnLangNodeClicked -= SetSelectedLanguage;
        AncestryConnection.OnConnectionClicked -= SetSelectedLanguage;
        BackArrowClickReceiver.OnBackArrowClicked -= ResetSelectedLanguage;
    }

    [CanBeNull]
    public static LanguageNode GetSelectedLanguage() {
        return _selectedLanguage;
    }

    private static void SetSelectedLanguage(LanguageNode langNode) {
        _selectedLanguage = langNode;
    }

    private static void ResetSelectedLanguage() {
        SetSelectedLanguage(null);
    }
}
