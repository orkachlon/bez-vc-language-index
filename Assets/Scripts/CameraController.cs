using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public enum ECameraType {
        Perspective,
        Orthographic
    }
    
    public ECameraType selectedCameraType;

    private static Camera _mainCamera;
    [SerializeField]
    private float translationDuration = 1.5f;
    [SerializeField]
    private float cameraDistanceFromNode = 5f;

    private Coroutine _cameraMoveCoroutine;

    private void Start() {
        _mainCamera = Camera.main;
        LanguageNode.OnLangNodeClicked += MoveCamera;
    }

    private void Update() {
        if (_mainCamera is not null) _mainCamera.orthographic = ECameraType.Orthographic.Equals(selectedCameraType);
    }

    private void MoveCamera(LanguageNode langNode) {
        var langNodePos = langNode.transform.position;
        var camHeight = langNodePos.y;
        var camZ = Mathf.Sqrt(langNodePos.x * langNodePos.x + langNodePos.z * langNodePos.z) + cameraDistanceFromNode;

        if (_cameraMoveCoroutine is not null) {
            StopCoroutine(_cameraMoveCoroutine);
        }
        _cameraMoveCoroutine = StartCoroutine(MoveCamera(new Vector3(0, camHeight, -camZ)));
    }
    
    private IEnumerator MoveCamera(Vector3 cameraEndPos) { 
        var time = 0f; 
        while (time < translationDuration) {
            // lerp camera translation
            var t = time / translationDuration;
            t = t * t * (3f - 2f * t); // ease animation
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, cameraEndPos, t);
            // increment
            time += Time.deltaTime;
            yield return null;
        }

        _mainCamera.transform.position = cameraEndPos;
    }
}
