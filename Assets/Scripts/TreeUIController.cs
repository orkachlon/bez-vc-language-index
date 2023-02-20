using UnityEngine;


public class TreeUIController : MonoBehaviour {
    void Start() {
        BackClickReceiver.OnBackArrowClicked += ShowUI;
        LanguageNode.OnLangNodeClicked += HideUI;
        AncestryConnection.OnConnectionClicked += HideUI;
    }

    private void OnDestroy() {
        BackClickReceiver.OnBackArrowClicked -= ShowUI;
        LanguageNode.OnLangNodeClicked -= HideUI;
        AncestryConnection.OnConnectionClicked -= HideUI;
    }

    private void ShowUI() {
        gameObject.SetActive(true);
    }

    private void HideUI(LanguageNode langNode = null) {
        gameObject.SetActive(false);
    }
}
