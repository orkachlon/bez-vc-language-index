using System;
using UnityEngine;

public enum EChildType {
    Replace,
    Add,
    Revive
}

public static class ChildTypeExtensions {

    private static readonly Material ReplaceMaterial = Resources.Load<Material>("Materials/ReplaceChildTypeMaterial");
    private static readonly Material AddMaterial = Resources.Load<Material>("Materials/AddChildTypeMaterial");
    private static readonly Material ReviveMaterial = Resources.Load<Material>("Materials/ReviveChildTypeMaterial");

    public static Material GetMaterial(this EChildType type) {
        return type switch {
            EChildType.Replace => ReplaceMaterial,
            EChildType.Add => AddMaterial,
            EChildType.Revive => ReviveMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}