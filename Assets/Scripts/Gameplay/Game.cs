using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gameplay {
public class Game : MonoBehaviour {
  [SerializeField]
  GameObject healthBar;
  [SerializeField]
  GameObject ammoText;
  [SerializeField]
  GameObject notification;
  [SerializeField]
  GameObject tutorialText;
  [SerializeField]
  GameObject[] panels;
  [SerializeField]
  AudioClip levelCompleteAudio;
  [SerializeField]
  AudioClip deathAudio;

  internal float playerHealth = 1.0f;
  internal bool hasWon = false;
  internal int ammoCount = 30;

  float lastUpdate;
  bool gameStarted = false;
  int prevPanel;
  DungeonGenerator generator;
  GameObject menuCam;
  AudioSource audioSource;
  bool startNew;
  // Start is called before the first frame update
  void Start() {
    Application.targetFrameRate = 60;
    Time.timeScale = 0f;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    menuCam = GameObject.Find("MenuCam");
    menuCam.SetActive(true);
    audioSource = GetComponent<AudioSource>();
    generator = GetComponent<DungeonGenerator>();
    TogglePanel(0);
  }

  void ResetGameState() {
    gameStarted = true;
    playerHealth = 1f;
    ammoCount = 30;
    hasWon = false;
    lastUpdate = Time.time;
    UpdateNotification("Find your way out!");
  }

  public void StartGame() {
    menuCam.SetActive(false);
    generator.GenerateWorld();
    ChangeState();
    TogglePanel(1);
    ResetGameState();
  }

  public void PlayAgain(bool startNew) {
    this.startNew = startNew;
    StartCoroutine("ResetGame");
  }

  IEnumerator ResetGame() {
    if (startNew) {
      generator.CleanGameWorld();
      yield return null;
      StartGame();
    } else {
      generator.ResetWorld();
      yield return null;
      ChangeState();
      TogglePanel(1);
      ResetGameState();
    }
  }

  public void QuitGame() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  // Update is called once per frame
  void Update() {
    if (gameStarted) {
      // Game ended, player won
      if (hasWon) {
        ChangeState();
        UpdateNotification("Congratulations, you have won! ");
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (var audio in audios) {
          audio.Pause();
        }
        audioSource.Play();
        TogglePanel(3);
        gameStarted = false;
        audioSource.PlayOneShot(levelCompleteAudio);
        return;
      }

      // Player lose
      playerHealth = Mathf.Max(0f, playerHealth);
      if (playerHealth == 0f) {
        ChangeState();
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (var audio in audios) {
          audio.Pause();
        }
        audioSource.Play();
        UpdateNotification("You died! Try your luck next time.");
        TogglePanel(3);
        gameStarted = false;
        audioSource.PlayOneShot(deathAudio);
        return;
      }

      // Pause game
      if (Input.GetButtonDown("Cancel")) {
        ChangeState();
        if (IsPause()) {
          ToggleSettings();
        } else {
          TogglePanel(1);
        }
        return;
      }

      if (playerHealth <= 0.4f) {
        UpdateNotification("Careful! You are low health!");
      }

      if (!IsPause() && Time.time - lastUpdate >= 5.0f) {
        UpdateNotification("Find your way out!");
      }
      healthBar.GetComponent<Slider>().value = playerHealth;
      ammoText.GetComponent<Text>().text = ammoCount + "/30";
    }
  }

  bool IsPause() { return Time.timeScale == 0f; }
  void ChangeState() {
    if (Time.timeScale == 0f) {
      Time.timeScale = 1f;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    } else {
      Time.timeScale = 0f;
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
  }

  public void ToggleTutorial() {
    tutorialText.SetActive(!tutorialText.activeSelf);
  }

  public void ToggleSettings() {
    TogglePanel(2);
    AudioSource[] audios = FindObjectsOfType<AudioSource>();
    foreach (var audio in audios) {
      audio.Pause();
    }
    audioSource.Play();
    notification.GetComponent<Text>().text = "";
  }

  public void ToggleBack() {
    if (prevPanel == 1) {
      ChangeState();
    }
    TogglePanel(prevPanel);
  }

  void TogglePanel(int index) {
    // 0: Start, 1: Game, 2: Settings, 3: End
    if (panels[index].activeSelf)
      return;
    for (int i = 0; i < 4; i++) {
      if (panels[i].activeSelf)
        prevPanel = i;
      panels[i].SetActive(i == index);
    }
  }

  internal void UpdateNotification(string noti) {
    notification.GetComponent<Text>().text = noti;
    lastUpdate = Time.time;
  }
}
}
