using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class GraphRotator : MonoBehaviour {
    [SerializeField] private float sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    private bool _disableRotation = false;

    [SerializeField] private float distanceModifier = 0.1f;
    [SerializeField] private AnimationCurve durationByDistance;

    private static GraphRotator _instance;
    private static Quaternion _endRotation;
    private static readonly object EndRotationLock = new();

    public static event Action<LanguageNode> OnGraphFinishedRotating;

    private  void Start() {
        _instance = this;
        sensitivity = 0.1f;
        _rotation = Vector3.zero;
        LanguageNode.OnLangNodeClicked += RotateTowards;
        LanguageNode.OnLangNodeClicked += DisableRotation;
        AncestryConnection.OnConnectionClicked += RotateTowards;
        AncestryConnection.OnConnectionClicked += DisableRotation;
        BackClickReceiver.OnBackArrowClicked += EnableRotation;
    }

    private void OnDestroy() {
        LanguageNode.OnLangNodeClicked -= RotateTowards;
        LanguageNode.OnLangNodeClicked -= DisableRotation;
        AncestryConnection.OnConnectionClicked -= RotateTowards;
        AncestryConnection.OnConnectionClicked -= DisableRotation;
        BackClickReceiver.OnBackArrowClicked -= EnableRotation;
    }

    // Update is called once per frame
    private void Update() {
        if (_disableRotation) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            // rotating flag
            _isRotating = true;
         
            // store mouse
            _mouseReference = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(0)) {
            // rotating flag
            _isRotating = false;
            return;
        }
        
        if (!_isRotating) return;
        
        // offset
        _mouseOffset = Input.mousePosition - _mouseReference;
             
        // apply rotation
        _rotation.y = -_mouseOffset.x * sensitivity;
             
        // rotate
        transform.Rotate(_rotation);
             
        // store mouse
        _mouseReference = Input.mousePosition;
    }
    
    private void EnableRotation() {
        _disableRotation = false;
    }

    private void DisableRotation([CanBeNull] LanguageNode node) {
        _disableRotation = true;
    }

    public static void SetEndRotation(LanguageNode langNode) {
        SetEndRotation(_instance.transform, langNode);
    }

    private static void SetEndRotation(Transform graphTransform, LanguageNode langNode) {
        lock (EndRotationLock) {
            var angle = Vector3.SignedAngle(-langNode.transform.forward, graphTransform.forward, Vector3.up);
            _endRotation = Quaternion.AngleAxis(angle, Vector3.up);
        }
    }

    private static Quaternion GetEndRotation() {
        lock (EndRotationLock) {
            return _endRotation;
        }
    }

    private void RotateTowards(LanguageNode langNode) {
        SetEndRotation(transform, langNode);
        var angleDiff = Quaternion.Angle(transform.rotation, GetEndRotation());
        var duration = durationByDistance.Evaluate(angleDiff * distanceModifier);
        StartCoroutine(LerpGraphRotation(GetEndRotation(), duration, langNode));
    }

    private IEnumerator LerpGraphRotation(Quaternion endRotation, float duration, LanguageNode langNode) {
        LanguageNameTooltip.RegisterDisable();
        LanguageNode.RegisterClickDisabler();
        var time = 0f;
        var startRotation = transform.rotation;
        while (time < duration) {
            // lerp graph rotation
            var t = time / duration;
            // t = t * t * (3f - 2f * t); // ease animation
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, Mathf.SmoothStep(0, duration, time));
            // increment
            time += Time.deltaTime;
            if (t > .9f) { 
                LanguageNameTooltip.UnregisterDisable();
            }
            yield return null;
        }

        transform.rotation = endRotation;
        LanguageNode.UnregisterClickDisabler();
        OnGraphFinishedRotating?.Invoke(langNode);
    }
}
