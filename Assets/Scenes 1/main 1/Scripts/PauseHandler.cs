using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    public static PauseHandler Instance;

    [Header("Estado y Configuración")]
    public bool isPaused = false;
    public float resumeCountdownTime = 3f;

    [Header("Referencias a Otros Sistemas")]
    public GameManager gameManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
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

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Adjust music pitch when paused
        if (audioManager != null)
        {
            audioManager.TogglePauseMusic(true);  // Slow down the music when paused
        }

        if (uiManager != null)
        {
            uiManager.ShowPauseMenu();
        }
        else
        {
            Debug.LogWarning("UIManager no asignado en PauseHandler.");
        }

        // Show and unlock cursor when paused
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (uiManager != null)
        {
            uiManager.HidePauseMenu();
        }

        StartCoroutine(ResumeCountdown());
    }

    private IEnumerator ResumeCountdown()
    {
        float countdown = resumeCountdownTime;

        while (countdown > 0f)
        {
            countdown -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        isPaused = false;

        // Adjust music pitch back to normal speed when unpaused
        if (audioManager != null)
        {
            audioManager.TogglePauseMusic(false);  // Return music to normal speed when unpaused
        }

        // Hide and lock cursor when game resumes
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            Debug.LogWarning("GameManager no asignado en PauseHandler.");
        }
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
