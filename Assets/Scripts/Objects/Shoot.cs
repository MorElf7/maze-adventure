using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;

namespace Objects {

public class Shoot : MonoBehaviour {
  [SerializeField]
  GameObject projectile;
  [SerializeField]
  AudioClip projectileAudio;

  // Start is called before the first frame update
  float projectileSpeed = 2000f;
  void Start() {}

  // Update is called once per frame
  void Update() {
    if (Time.timeScale != 0 && Input.GetButtonDown("Fire1")) {
      Game game = GameObject.Find("Game").GetComponent<Game>();
      if (game.ammoCount > 0) {
        GameObject shootThis =
            Instantiate(projectile, transform.position, transform.rotation);
        shootThis.name = "Arrow";
        shootThis.transform.Rotate(90f, 0f, 0f);
        shootThis.GetComponent<Rigidbody>().AddRelativeForce(
            new Vector3(0f, 50f, projectileSpeed));
        game.ammoCount--;
        GameObject.Find("First Person Camera")
            .GetComponent<AudioSource>()
            .PlayOneShot(projectileAudio, 1f);
      } else {
        game.UpdateNotification("Out of ammo!");
      }
    }
  }
}
}
