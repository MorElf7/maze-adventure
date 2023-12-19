using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;

namespace Objects {
public class Pills : MonoBehaviour {
  [SerializeField]
  float minGain = 0.1f;
  [SerializeField]
  float maxGain = 0.3f;
  [SerializeField]
  AudioClip pickUpAudio;

  void OnTriggerEnter(Collider other) {
    if (other.gameObject.name == "Player") {
      GameObject.Find("Game").GetComponent<Game>().playerHealth +=
          Random.Range(minGain, maxGain);
      GameObject.Find("First Person Camera")
          .GetComponent<AudioSource>()
          .PlayOneShot(pickUpAudio, 1f);
      Destroy(gameObject);
    }
  }
}
}
