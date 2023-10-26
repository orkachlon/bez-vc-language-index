using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectionLegendButton : MonoBehaviour, IPointerClickHandler {

    [SerializeField] private Image connectionLegend;

    private void Awake() {
        if (!connectionLegend) {
            throw new MissingComponentException("Legend button must have link to legend");
        }
        
        connectionLegend.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {
        // toggle the legend
        connectionLegend.gameObject.SetActive(!connectionLegend.gameObject.activeInHierarchy);
    }
}