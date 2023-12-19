using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class CrosshairHandler : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {}

  // Update is called once per frame
  void Update() {}

  public void ChangeColor(Color color) {
    this.GetComponent<Image>().color = color;
  }
}
}
