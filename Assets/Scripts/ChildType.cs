using UnityEngine;

public enum ChildType {
    Replace,
    Add,
    Revive
}

static class ChildTypeExtensions {
    public static Material GetMaterial(this ChildType type) {
        return null;
    }
}