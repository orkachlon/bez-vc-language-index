using UnityEngine;

public static class Utils {
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T: Component {
        var t = parent.transform;
        foreach(Transform tr in t) {
            if(tr.CompareTag(tag) && tr.GetComponent<T>()) {
                return tr.GetComponent<T>();
            }

            var componentFound = tr.gameObject.FindComponentInChildWithTag<T>(tag);
            if (componentFound) {
                return componentFound;
            }
        }
        return null;
    }
    
    
    public static T GetLanguageComponent<T>(this GameObject gameObject, T component, string requestedTag) where T: Component {
        if (!component) {
            component = gameObject.FindComponentInChildWithTag<T>(requestedTag);
        } else {
            return component;
        }
        if (!component) {
            throw new MissingComponentException($"LanguageNode is missing {requestedTag} {typeof(T)} component!");
        }

        return component;
    }
}
