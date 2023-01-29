using UnityEngine;

public static class Utils {
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T: Component {
        var t = parent.transform;
        foreach(Transform tr in t) {
            if(tr.CompareTag(tag) && tr.GetComponent<T>()) {
                return tr.GetComponent<T>();
            }

            tr.gameObject.FindComponentInChildWithTag<T>(tag);
        }
        return null;
    }
}
