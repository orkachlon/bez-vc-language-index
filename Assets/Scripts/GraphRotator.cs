using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRotator : MonoBehaviour {
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    private bool _isZoomed = false;
    [SerializeField]
    private float rotationDuration = .5f;

    // Start is called before the first frame update
    void Start() {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
        LanguageNode.OnLangNodeClicked += RotateTowards;
    }

    // Update is called once per frame
    void Update() {
        if (_isZoomed) {
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
        _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;
             
        // rotate
        transform.Rotate(_rotation);
             
        // store mouse
        _mouseReference = Input.mousePosition;
    }
    
    void OnMouseDown() {
        if (_isZoomed) {
            return;
        }
        // rotating flag
        _isRotating = true;
         
        // store mouse
        _mouseReference = Input.mousePosition;
    }
     
    void OnMouseUp() {
        if (_isZoomed) {
            return;
        }
        // rotating flag
        _isRotating = false;
    }

    private void RotateTowards(LanguageNode langNode) {
        print($"clicked {langNode.GetName()}");
        var mainCam = Camera.main;
        if (mainCam is null) {
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
        StartCoroutine(LerpGraphRotation(endRotation, rotationDuration));

        // mainCam.transform.position = langNode.transform.position + Vector3.back * 5;
        // var nodeRadius = graphToNode.magnitude;
        // mainCam.transform.position = graphCenter + Vector3.back * nodeRadius + Vector3.back * 5 + Vector3.up * langNodePos.y;
        // _isZoomed = true;
    }

    private IEnumerator LerpGraphRotation(Quaternion endRotation, float duration) {
        // rotate without animation
        // transform.Rotate(Vector3.up, angleToRotate);
        var time = 0f;
        var startRotation = transform.rotation;
        while (time < duration) {
            // lerp graph rotation
            var t = time / duration;
            t = t * t * (3f - 2f * t); // ease animation
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            // increment
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }
    
    private IEnumerator LerpGraphRotation(Vector3 endLookDirection, float duration) {
        var time = 0f;
        // Vector3.Lerp(transform.forward, Quaternion.)
        while (time < duration) {
            // lerp graph rotation
            // transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, time / duration);
            // increment
            time += Time.deltaTime;
            yield return null;
        }
    
        // transform.rotation = endRotation;
    }

    private IEnumerator MoveCamera(Camera lerpCam, Vector3 cameraEndPos, float duration) {
        var time = 0f;
        while (time < duration) {
            // lerp camera translation
            lerpCam.transform.position = Vector3.Lerp(lerpCam.transform.position, cameraEndPos, time / duration);
            // increment
            time += Time.deltaTime;
            yield return null;
        }
    }
}
