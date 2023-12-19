using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objects {

public class RoomBehavior : MonoBehaviour {
  [SerializeField]
  GameObject[] walls;
  // public GameObject[] doors;

  public void UpdateRoom(bool[] status) {
    for (int i = 0; i < status.Length; i++) {
      // doors[i].SetActive(status[i]);
      walls[i].SetActive(!status[i]);
    }
  }
}
}
