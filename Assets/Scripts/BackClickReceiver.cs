using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackClickReceiver : MonoBehaviour, IPointerClickHandler {

    public static event Action OnBackArrowClicked;
    
    public void OnPointerClick(PointerEventData eventData) {
        if (!LanguageNode.IsClickEnabled()) {
            return;
        }
        OnBackArrowClicked?.Invoke();
    }
}
