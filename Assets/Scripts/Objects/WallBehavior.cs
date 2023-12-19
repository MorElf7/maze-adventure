using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects {

public class WallBehavior : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    Collider[] colliders = Physics.OverlapSphere(transform.position, .001f);

    foreach (Collider collider in colliders) {
      if (collider.tag == "Wall") {
        Destroy(gameObject);
        return;
      }
    }

    GetComponent<Collider>().enabled = true;
  }
}
}
