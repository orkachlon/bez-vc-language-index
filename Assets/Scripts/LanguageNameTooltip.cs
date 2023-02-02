using System;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LanguageNameTooltip : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private float textPadding;
    [SerializeField] private Camera uiCamera;

    private static LanguageNameTooltip _instance;
    private int _disableQueue;
    private RectTransform _bgRectTransform;
    
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
        Interlocked.Increment(ref _instance._disableQueue);
    }

    public static void UnregisterDisable() {
        if (Interlocked.CompareExchange(ref _instance._disableQueue, 0, 0) == 0) {
            return;
        }
        Interlocked.Decrement(ref _instance._disableQueue);
    }

    private void Update() {
        if (Interlocked.CompareExchange(ref _instance._disableQueue, 0, 0) > 0) {
            HideTooltip();
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition, uiCamera, out var localPoint);
        transform.localPosition = localPoint;
    }

    private void ShowTooltip(string tooltipString) {
        if (Interlocked.CompareExchange(ref _instance._disableQueue, 0, 0) > 0) {
            return;
        }
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        // adjust background size to fit text
        var bgSize = new Vector2(tooltipText.preferredWidth + textPadding * 2f, tooltipText.preferredHeight + textPadding * 2f);
        _bgRectTransform.sizeDelta = bgSize;
    }
    private void HideTooltip() {
        gameObject.SetActive(false);
    }

    public static void ShowTooltipStatic(string tooltipString) {
        _instance.ShowTooltip(tooltipString);
    }
    
    public static void HideTooltipStatic() {
        _instance.HideTooltip();
    }
}
