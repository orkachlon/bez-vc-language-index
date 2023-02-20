using System.Collections;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    private static Vector3 _cameraStartPos;

    private static Camera _mainCamera;

    [SerializeField]
    private AnimationCurve durationByDistance;
    [SerializeField] private float distanceModifier = 0.06f;
    
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

    private void ResetCamera() {
        if (transform.position == _cameraStartPos) {
            return;
        }
        var distance = Vector3.Distance(transform.position, _cameraStartPos);
        var duration = durationByDistance.Evaluate(distance * distanceModifier);
        StartCoroutine(MoveCamera(_cameraStartPos, duration));
    }

    private void MoveCameraToLanguageNode(LanguageNode langNode) {
        // move camera
        var newPos = langNode.GetEndPosition() + Vector3.back * cameraDistanceFromNode;
        var distance = Vector3.Distance(transform.position, newPos);
        var duration = durationByDistance.Evaluate(distance * distanceModifier);
        StartCoroutine(MoveCamera(newPos, duration));
    }

    private IEnumerator MoveCamera(Vector3 cameraEndPos, float duration) {
        LanguageNameTooltip.RegisterDisable();
        LanguageNode.RegisterClickDisabler();
        var time = 0f;
        var startPosition = _mainCamera.transform.position;
        while (time < duration) {
            // lerp camera translation
            var t = time / duration;
            // t = t * t * (3f - 2f * t); // ease animation
            _mainCamera.transform.position = Vector3.Lerp(startPosition, cameraEndPos, Mathf.SmoothStep(0, duration, time));
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
