using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class SliderText : MonoBehaviour {
  [SerializeField]
  GameObject sliderText;
  // Start is called before the first frame update
  void Start() {}

  // Update is called once per frame
  void Update() {
    float sliderValue = GetComponent<Slider>().value;
    sliderText.GetComponent<Text>().text = Mathf.RoundToInt(sliderValue) + "%";
  }
}
}
