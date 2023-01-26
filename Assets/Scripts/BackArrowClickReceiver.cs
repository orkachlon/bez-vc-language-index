using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackArrowClickReceiver : MonoBehaviour, IPointerClickHandler {

    public static event Action OnBackArrowClicked;
    
    public void OnPointerClick(PointerEventData eventData) {
        OnBackArrowClicked?.Invoke();
    }
}
