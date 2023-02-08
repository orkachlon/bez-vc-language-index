using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour {

    public enum ECameraType {
        Perspective,
        Orthographic
    }
    
    public static event Action<Vector3> OnCameraAnimationFinished;
    
    public ECameraType selectedCameraType;

    private static Vector3 _cameraStartPos;

    private static Camera _mainCamera;
    [SerializeField]
    private float translationDuration = 1.5f;
    [SerializeField]
    private float cameraDistanceFromNode = 5f;

    private Coroutine _cameraMoveCoroutine;

    private void Start() {
        _mainCamera = Camera.main;
        if (!_mainCamera) {
            throw new MissingComponentException("No main camera found!");
        }
        _cameraStartPos = _mainCamera.transform.position;
        LanguageNode.OnLangNodeClicked += MoveCameraToLanguageNode;
        AncestryConnection.OnConnectionClicked += MoveCameraToLanguageNode;
        BackClickReceiver.OnBackArrowClicked += ResetCamera;
    }

    private void OnDestroy() {
        LanguageNode.OnLangNodeClicked -= MoveCameraToLanguageNode;
        AncestryConnection.OnConnectionClicked -= MoveCameraToLanguageNode;
        BackClickReceiver.OnBackArrowClicked -= ResetCamera;
    }

    private void Update() {
        if (_mainCamera != null) _mainCamera.orthographic = ECameraType.Orthographic.Equals(selectedCameraType);
    }

    private void ResetCamera() {
        StartCameraMoveCoroutine(_cameraStartPos.y, _cameraStartPos.z);
    }

    private void MoveCameraToLanguageNode(LanguageNode langNode) {
        // calculate position
        var langNodePos = langNode.transform.position;
        var camHeight = langNodePos.y;
        var camZ = -(Mathf.Sqrt(langNodePos.x * langNodePos.x + langNodePos.z * langNodePos.z) + cameraDistanceFromNode);
        // var camZ = 0f;

        // move camera
        StartCameraMoveCoroutine(camHeight, camZ);
    }

    private void StartCameraMoveCoroutine(float camHeight, float camZ) {
        if (_cameraMoveCoroutine != null) {
            StopCoroutine(_cameraMoveCoroutine);
        }
        _cameraMoveCoroutine = StartCoroutine(MoveCamera(new Vector3(0, camHeight, camZ)));
    }

    private IEnumerator MoveCamera(Vector3 cameraEndPos) { 
        LanguageNameTooltip.RegisterDisable();
        var time = 0f; 
        while (time < translationDuration) {
            // lerp camera translation
            var t = time / translationDuration;
            t = t * t * (3f - 2f * t); // ease animation
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, cameraEndPos, t);
            // increment
            time += Time.deltaTime;
            if (t > .9f) { 
                LanguageNameTooltip.UnregisterDisable();
            }
            yield return null;
        }

        _mainCamera.transform.position = cameraEndPos;
        OnCameraAnimationFinished?.Invoke(cameraEndPos);
    }
}
