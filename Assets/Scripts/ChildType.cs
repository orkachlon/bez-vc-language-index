using System;
using UnityEngine;

internal enum EChildType {
    Replace,
    Add,
    Revive
}

internal static class ChildTypeExtensions {

    private static readonly Material ReplaceMaterial = Resources.Load<Material>("ReplaceChildTypeMaterial");
    private static readonly Material AddMaterial = Resources.Load<Material>("AddChildTypeMaterial");
    private static readonly Material ReviveMaterial = Resources.Load<Material>("ReviveChildTypeMaterial");

    public static Material GetMaterial(this EChildType type) {
        return type switch {
            EChildType.Replace => ReplaceMaterial,
            EChildType.Add => AddMaterial,
            EChildType.Revive => ReviveMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}