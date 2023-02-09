using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    public enum ECameraType {
        Perspective,
        Orthographic
    }
    
    
    public ECameraType selectedCameraType;

    private static Vector3 _cameraStartPos;

    private static Camera _mainCamera;
    [SerializeField]
    private float translationDuration = 1.5f;
    [SerializeField]
    private float cameraDistanceFromNode = 5f;

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
        if (transform.position == _cameraStartPos) {
            return;
        }
        StartCoroutine(MoveCamera(_cameraStartPos));
    }

    private void MoveCameraToLanguageNode(LanguageNode langNode) {
        // move camera
        StartCoroutine(MoveCamera(langNode.GetEndPosition() + Vector3.back * cameraDistanceFromNode));
    }

    private IEnumerator MoveCamera(Vector3 cameraEndPos) {
        LanguageNameTooltip.RegisterDisable();
        LanguageNode.RegisterClickDisabler();
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
        LanguageNode.UnregisterClickDisabler();
    }
}
