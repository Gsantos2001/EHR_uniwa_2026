using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuAudio : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        float volumeDB = Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat("MasterVolume", volumeDB);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}