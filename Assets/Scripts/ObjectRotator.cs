using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour {
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotating;
    
    // Start is called before the first frame update
    void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {

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
        // rotating flag
        _isRotating = true;
         
        // store mouse
        _mouseReference = Input.mousePosition;
    }
     
    void OnMouseUp() {
        // rotating flag
        _isRotating = false;
    }
}
