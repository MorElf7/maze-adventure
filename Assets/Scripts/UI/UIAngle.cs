using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
public class UIAngle : MonoBehaviour {
  Camera mainCam;
  // Start is called before the first frame update
  void Start() { mainCam = Camera.main; }

  void LateUpdate() {
    mainCam = Camera.main;
    if (mainCam != null) {
      var rotation = mainCam.transform.rotation;
      transform.LookAt(transform.position + rotation * Vector3.forward,
                       rotation * Vector3.up);
    }
  }
}
}
