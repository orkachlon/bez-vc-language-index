using Languages.Graph.Elements;
using UnityEngine;

namespace Languages.LanguageItem {
    public class ItemUIController : MonoBehaviour {

        void Start() {
            BackClickReceiver.OnBackArrowClicked += HideUI;
            LanguageNode.OnLangNodeClicked += ShowUI;
            AncestryConnection.OnConnectionClicked += ShowUI;

            HideUI();
        }

        private void OnDestroy() {
            BackClickReceiver.OnBackArrowClicked -= HideUI;
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
}