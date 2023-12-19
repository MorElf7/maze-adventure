using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Gameplay {

public class SoundSettings : MonoBehaviour {
  [SerializeField]
  Slider soundSlider;
  [SerializeField]
  AudioMixer masterMixer;
  [SerializeField]
  GameObject musicToggle;

  // Start is called before the first frame update
  void Start() {
    musicToggle.GetComponent<Toggle>().isOn = true;
    SetVolume(PlayerPrefs.GetFloat("SavedMasterVolume", 100));
  }

  public void SetVolume(float value) {
    if (value < 1) {
      value = .001f;
    }

    RefreshSlider(value);
    PlayerPrefs.SetFloat("SavedMasterVolume", value);
    masterMixer.SetFloat("MasterVolume", Mathf.Log10(value / 100) * 20f);
  }

  public void SetVolumeFromSlider() { SetVolume(soundSlider.value); }

  public void RefreshSlider(float value) { soundSlider.value = value; }

  public void ToggleMusic() {
    AudioSource audio = GameObject.Find("Game").GetComponent<AudioSource>();
    if (musicToggle.GetComponent<Toggle>().isOn) {
      audio.Play();
    } else {
      audio.Stop();
    }
  }
}

}
