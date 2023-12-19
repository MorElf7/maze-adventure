using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects {

public class Arrow : MonoBehaviour {
  [SerializeField]
  float damage = 0.17f;
  [SerializeField]
  AudioClip impactAudio;
  float birth_time;
  AudioSource audioSource;
  // Start is called before the first frame update
  void Start() {
    audioSource = GetComponent<AudioSource>();
    birth_time = Time.time;
  }

  // Update is called once per frame
  void Update() {
    if (Time.time - birth_time > 10.0f) // arrow live for 10 sec
    {
      Destroy(transform.gameObject);
    }
  }

  void OnCollisionEnter(Collision other) {
    if (other.gameObject.name.Contains("Enemy")) {
      other.gameObject.GetComponent<Enemy>().health -= damage;
    }
    GameObject.Find("First Person Camera")
        .GetComponent<AudioSource>()
        .PlayOneShot(impactAudio, 1f);
    Destroy(transform.gameObject);
  }
}
}
