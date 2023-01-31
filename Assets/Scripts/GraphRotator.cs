using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRotator : MonoBehaviour {
    [SerializeField] private float sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    private bool _disableRotation = false;
    [SerializeField]
    private float rotationDuration = .5f;

    private Coroutine _graphRotateCoroutine;

    // Start is called before the first frame update
    void Start() {
        sensitivity = 0.1f;
        _rotation = Vector3.zero;
        LanguageNode.OnLangNodeClicked += RotateTowards;
        AncestryConnection.OnConnectionClicked += RotateTowards;
        BackArrowClickReceiver.OnBackArrowClicked += EnableRotation;
    }

    // Update is called once per frame
    void Update() {
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
        _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * sensitivity;
             
        // rotate
        transform.Rotate(_rotation);
             
        // store mouse
        _mouseReference = Input.mousePosition;
    }
    
    void OnMouseDown() {
        if (_disableRotation) {
            return;
        }
        // rotating flag
        _isRotating = true;
         
        // store mouse
        _mouseReference = Input.mousePosition;
    }
     
    void OnMouseUp() {
        if (_disableRotation) {
            return;
        }
        // rotating flag
        _isRotating = false;
    }

    private void EnableRotation() {
        _disableRotation = false;
    }

    private void RotateTowards(LanguageNode langNode) {
        var mainCam = Camera.main;
        if (mainCam == null) {
            throw new Exception("Main Camera can't be null!");
        }
        // graph center on camera height
        var mainCamPos = mainCam.transform.position;
        var langNodePos = langNode.transform.position;
        // we only rotate around y so
        // zero out all y values to not mess with the rotation angle
        var graphCenter = new Vector3(mainCamPos.x ,0, 0);
        var graphToNode = langNodePos - graphCenter;
        graphToNode.y = 0;
        var graphToCam = mainCamPos - graphCenter;
        graphToCam.y = 0;
        var angleToRotate = Vector3.SignedAngle(graphToNode, graphToCam, Vector3.up);

        // get angle between V3fwd and transform.fwd
        var fwdOffset = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up);
        // add angle between graphToNode and graphToCam
        var endAngle = angleToRotate + fwdOffset;

        var eulerRotation = new Vector3(0, endAngle, 0);
        var endRotation = Quaternion.Euler(eulerRotation);

        if (_graphRotateCoroutine != null) {
            StopCoroutine(_graphRotateCoroutine);
        }
        _graphRotateCoroutine = StartCoroutine(LerpGraphRotation(endRotation));
        _disableRotation = true;
    }

    private IEnumerator LerpGraphRotation(Quaternion endRotation) {
        LanguageNameTooltip.RegisterDisable();
        // rotate without animation
        // transform.Rotate(Vector3.up, angleToRotate);
        var time = 0f;
        var startRotation = transform.rotation;
        while (time < rotationDuration) {
            // lerp graph rotation
            var t = time / rotationDuration;
            t = t * t * (3f - 2f * t); // ease animation
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            // increment
            time += Time.deltaTime;
            if (t > .9f) { 
                LanguageNameTooltip.UnregisterDisable();
            }
            yield return null;
        }

        transform.rotation = endRotation;
    }
}
