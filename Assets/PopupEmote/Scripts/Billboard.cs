using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {

    [Tooltip("The main camera of the scene. It will be populated by default with Camera.main")]
    [SerializeField]
    private Camera _camera;

    void Start() {
        if (_camera == null) {
            _camera = Camera.main;
        }
    }

    void Update() {
        transform.LookAt(_camera.transform.position, Vector3.up);
    }
}
