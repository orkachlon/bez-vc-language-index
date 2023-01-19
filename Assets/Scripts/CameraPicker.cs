using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraPicker : MonoBehaviour {

    public enum ECameraType {
        Perspective,
        Orthographic
    }
    
    public ECameraType selectedCameraType;

    private Dictionary<ECameraType, Camera> _cameras;

    void Update() {
        if (Camera.main is not null) Camera.main.orthographic = ECameraType.Orthographic.Equals(selectedCameraType);
    }
}
