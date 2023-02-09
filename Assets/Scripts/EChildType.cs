using System;
using UnityEngine;

public enum EChildType {
    Replace,
    Add,
    Revive
}

public static class ChildTypeExtensions {

    private static readonly Material ReplaceMaterial = Resources.Load<Material>("ReplaceChildTypeMaterial");
    private static readonly Material AddMaterial = Resources.Load<Material>("AddChildTypeMaterial");
    private static readonly Material ReviveMaterial = Resources.Load<Material>("ReviveChildTypeMaterial");
    
    public static readonly float LineWide = 0.2f;
    public static readonly float LineNarrow = 0.1f;

    public static Material GetMaterial(this EChildType type) {
        return type switch {
            EChildType.Replace => ReplaceMaterial,
            EChildType.Add => AddMaterial,
            EChildType.Revive => ReviveMaterial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static void SetLineType(this EChildType childType, LineRenderer lineRenderer) {
        // lineRenderer.widthMultiplier = LineWide;
        lineRenderer.material = childType.GetMaterial();
        // was used to scale dotted/ dashed line properly
        // lineRenderer.sharedMaterial.mainTextureScale = EChildType.Revive.Equals(childType)
        //     ? new Vector2(1f / lineRenderer.widthMultiplier, 1f)
        //     : new Vector2(1f, 1f);
        // lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.sortingLayerName = "Lines";
        switch (childType) {
            case EChildType.Replace:
                lineRenderer.startWidth = LineNarrow;
                lineRenderer.endWidth = LineWide;
                break;
            case EChildType.Add:
                lineRenderer.widthMultiplier = LineWide;
                break;
            case EChildType.Revive:
                lineRenderer.endWidth = LineNarrow;
                lineRenderer.startWidth = LineWide;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(childType), childType, null);
        }
    }
}