using System;
using UnityEngine;

public enum EChildType {
    Replace,
    Add,
    Revive
}

public static class ChildTypeExtensions {
    
    public static readonly Color ReplaceColor = new Color32(0xE7, 0xC2, 0x4C, 0xFF);
    public static readonly Color AddColor = new Color32(0xA6, 0x79, 0x4C, 0x2B);  
    public static readonly Color ReviveColor = new Color32(0xA1, 0x3E, 0x00, 0xFF);

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