using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;

namespace Objects {

public class FinishZone : MonoBehaviour {

  [SerializeField]
  GameObject spotlight;
  Game game;

  void Start() {
    game = GameObject.Find("Game").GetComponent<Game>();
    spotlight.GetComponent<Light>().color = Color.green;
  }

  // Update is called once per frame
  void Update() {
    if (game.hasWon) {
      spotlight.GetComponent<Light>().color = Color.cyan;
    } else {
      spotlight.GetComponent<Light>().color = Color.green;
    }
  }

  private void OnTriggerEnter(Collider other) {
    GameObject player = GameObject.Find("Player");
    if (other.gameObject == player) {
      game.hasWon = true;
    }
  }
}
}
