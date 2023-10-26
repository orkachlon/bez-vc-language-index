using System.Threading;
using TMPro;
using UnityEngine;

namespace Languages.LanguageItem {
    public class LanguageNameTooltip : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private float textPadding;
        [SerializeField] private Camera uiCamera;

        private static LanguageNameTooltip _instance;
        private int _disablers;
        private RectTransform _bgRectTransform;
        private Vector3 _offsetFromMouse = Vector2.one * 10;

        private void Awake() {
            _instance = this;
            _bgRectTransform = transform.Find("TooltipBG").GetComponent<RectTransform>();
            tooltipText = transform.Find("TooltipText").GetComponent<TextMeshProUGUI>();
            HideTooltip();
        }

        /// <summary>
        /// This method is used during animations in order to disable the tooltip
        /// The tooltip causes lag if activated while animating the graph
        /// </summary>
        public static void RegisterDisable() {
            Interlocked.Increment(ref _instance._disablers);
        }

        public static void UnregisterDisable() {
            if (Interlocked.CompareExchange(ref _instance._disablers, 0, 0) == 0) {
                return;
            }
            Interlocked.Decrement(ref _instance._disablers);
        }

        private void Update() {
            if (Interlocked.CompareExchange(ref _instance._disablers, 0, 0) > 0) {
                HideTooltip();
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition + _offsetFromMouse, uiCamera, out var localPoint);
            transform.localPosition = localPoint;
        }

        private void ShowTooltip(string tooltipString, int fontSize = 40) {
            if (Interlocked.CompareExchange(ref _instance._disablers, 0, 0) > 0) {
                return;
            }
            // position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition + _offsetFromMouse, uiCamera, out var localPoint);
            transform.localPosition = localPoint;
            // set text
            tooltipText.text = tooltipString;
            tooltipText.fontSize = fontSize;
            tooltipText.margin = new Vector4(textPadding * 2, 0, textPadding, textPadding);
            tooltipText.ForceMeshUpdate();
            // adjust background size to fit text
            var pref = tooltipText.GetPreferredValues();
            var bgSize = new Vector2(pref.x + textPadding * 3f, pref.y + textPadding * 2f);
            // var bgSize = tooltipText.rectTransform.sizeDelta + Vector2.one * textPadding;
            _bgRectTransform.sizeDelta = bgSize;

            gameObject.SetActive(true);
        }
        private void HideTooltip() {
            gameObject.SetActive(false);
        }

        public static void ShowTooltipStatic(string tooltipString, int fontSize = 40) {
            _instance.ShowTooltip(tooltipString, fontSize);
        }

        public static void HideTooltipStatic() {
            _instance.HideTooltip();
        }
    }
}