using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;

namespace Objects {

public class Chest : MonoBehaviour, IInteractable {
  // [SerializeField]
  // float minGain = 0.1f;
  // [SerializeField]
  // float maxGain = 0.3f;
  [SerializeField]
  int ammoGain = 6;
  [SerializeField]
  string prompt;
  [SerializeField]
  GameObject panel;
  [SerializeField]
  GameObject text;
  [SerializeField]
  AudioClip lootAudio;

  float timeLastInteract;
  AudioSource audioSource;

  // Start is called before the first frame update
  void Start() {
    text.GetComponent<Text>().text = prompt;
    panel.SetActive(false);
    timeLastInteract = -5f;
    audioSource = GetComponent<AudioSource>();
  }

  bool isDisplay = false;

  public bool IsDisplay {
    get => isDisplay;
    set { isDisplay = value; }
  }

  public void SetUpUI() {
    if (Time.time - timeLastInteract >= 5f && !IsDisplay) {
      panel.SetActive(true);
      isDisplay = true;
    }
  }
  public void CloseUI() {
    if (IsDisplay) {
      panel.SetActive(false);
      isDisplay = false;
    }
  }
  public string InteractionPrompt { get => prompt; }

  public bool Interact(Interactor interactor) {
    if (interactor.gameObject.name == "Player" &&
        Time.time - timeLastInteract >= 5f) {
      Game game = GameObject.Find("Game").GetComponent<Game>();
      // game.playerHealth += Random.Range(minGain, maxGain);
      game.ammoCount = Mathf.Min(30, game.ammoCount + ammoGain);
      timeLastInteract = Time.time;
      CloseUI();
      audioSource.PlayOneShot(lootAudio, 1f);
      return true;
    } else {
      return false;
    }
  }
}

}
