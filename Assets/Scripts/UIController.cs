using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

    [SerializeField]
    private Image backArrow;
    
    // Start is called before the first frame update
    void Start() {
        if (backArrow) 
            return;
        backArrow = GetComponentInChildren<Image>();
        if (!backArrow) {
            throw new MissingComponentException("UIController must have a child with TextMeshProUGUI component");
        }

        BackArrowClickReceiver.OnBackArrowClicked += HideUI;
        LanguageNode.OnLangNodeClicked += ShowUI;
        AncestryConnection.OnConnectionClicked += ShowUI;
        
        HideUI();
    }

    private void OnDestroy() {
        BackArrowClickReceiver.OnBackArrowClicked -= HideUI;
        LanguageNode.OnLangNodeClicked -= ShowUI;
        AncestryConnection.OnConnectionClicked -= ShowUI;
    }

    private void ShowUI(LanguageNode langNode) {
        gameObject.SetActive(true);
    }

    private void HideUI() {
        gameObject.SetActive(false);
    }
}
