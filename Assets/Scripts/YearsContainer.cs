using System;
using UnityEngine;

[ExecuteAlways]
[Serializable]
public class YearsContainer : TextContainer {

    public override void ToItem() {
        gameObject.SetActive(true);
        AdjustBGSize();
    }
    
    public override void ToItemRelative() {
        gameObject.SetActive(false);
    }

    public override void ToNode() {
        gameObject.SetActive(false);
    }
}
