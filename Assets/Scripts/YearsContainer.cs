using System;
using UnityEngine;

[ExecuteAlways]
[Serializable]
public class YearsContainer : TextContainer {
    
    protected override void Awake() {
        base.Awake();
    }

    public override void ToItem() {
        gameObject.SetActive(true);
        // textElement.ForceMeshUpdate();
        // SetSize(textElement.GetPreferredValues());
    }
    
    public override void ToItemRelative() {
        gameObject.SetActive(false);
    }

    public override void ToNode() {
        gameObject.SetActive(false);
    }
}
