using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;
    public Slider volumeSlider;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Player")]
    public GameObject crosshair;
    public PlayerCam playerCam;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            SetVolume(volumeSlider.value);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;

        if (crosshair != null)
            crosshair.SetActive(false);

        if (playerCam != null)
            playerCam.inputEnabled = false;

        // Ελευθερώνει το mouse για το Pause Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        if (crosshair != null)
            crosshair.SetActive(true);

        if (playerCam != null)
            playerCam.inputEnabled = true;

        StartCoroutine(LockCursorNextFrame());
    }

    private System.Collections.IEnumerator LockCursorNextFrame()
    {
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
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