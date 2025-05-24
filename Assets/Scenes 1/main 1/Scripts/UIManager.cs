using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class PlayerImage
{
    public GameObject objectToDestroy;
    public string playerName;
    public Sprite playerSprite;
}

public class UIManager : MonoBehaviour
{
    [Header("Elementos de UI")]
    public TMP_Text roundText;
    public GameObject gameOverScreen;
    public GameObject pauseMenu;
    public TMP_Text timerText;
    public Slider musicSpeedSlider;

    [Header("Textos de ronda (múltiples)")]
    public List<TMP_Text> roundTexts = new List<TMP_Text>();

    [Header("Texto ganador (asignar en Inspector)")]
    public TMP_Text winnerNameText;

    [Header("Imagen del jugador ganador (asignar en Inspector)")]
    public Image winnerImage;

    [Header("Imágenes por jugador (asignar desde el Inspector)")]
    public List<PlayerImage> playerImages = new List<PlayerImage>();

    [Header("Botones del Menú (Asignar desde el Inspector)")]
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    [Header("Objetos a ocultar en Game Over (Asignar en Inspector)")]
    public List<GameObject> objectsToHideOnGameOver = new List<GameObject>();

    [Header("Objetos adicionales a destruir (asignar en Inspector)")]
    public List<GameObject> additionalObjectsToDestroy = new List<GameObject>();

    #region Funciones de Actualización de UI

    public void DisplayRoundCounter(int round)
    {
        if (roundText != null)
            roundText.text = "  " + round;

        if (roundTexts != null && roundTexts.Count > 0)
        {
            foreach (var text in roundTexts)
            {
                if (text != null)
                    text.text = "  " + round;
            }
        }
        else
        {
            Debug.LogWarning("No se han asignado textos de ronda en UIManager.");
        }
    }

    public void UpdateMusicSpeedDisplay(float musicPitch)
    {
        if (musicSpeedSlider != null)
            musicSpeedSlider.value = musicPitch;
        else
            Debug.LogWarning("MusicSpeedSlider no asignado en UIManager.");
    }

    public void ToggleSoundMute(bool isMuted)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ToggleMute(isMuted);
        else
            Debug.LogWarning("AudioManager no encontrado.");
    }

    #endregion

    #region Pantallas y Menús

    public void ShowGameOverScreen(string winnerName)
    {
        if (gameOverScreen != null)
        {
            if (winnerNameText != null)
                winnerNameText.text = "¡" + winnerName + "!";

            if (winnerImage != null)
            {
                Sprite matchedSprite = null;

                foreach (PlayerImage p in playerImages)
                {
                    if (p.playerName.Equals(winnerName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        matchedSprite = p.playerSprite;
                        break;
                    }
                }

                if (matchedSprite != null)
                {
                    winnerImage.sprite = matchedSprite;
                    winnerImage.gameObject.SetActive(true);
                }
                else
                {
                    winnerImage.gameObject.SetActive(false);
                    Debug.LogWarning("No se encontró sprite para: " + winnerName);
                }
            }

            foreach (var obj in objectsToHideOnGameOver)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            gameOverScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("GameOverScreen no asignado en UIManager.");
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        else
            Debug.LogWarning("PauseMenu no asignado en UIManager.");
    }

    public void HidePauseMenu()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        else
            Debug.LogWarning("PauseMenu no asignado en UIManager.");
    }

    #endregion

    #region Configuración de Botones del Menú

    public void OnRestartButtonPressed()
    {
        StartCoroutine(CleanUpAndLoadMainMenu());
    }

    public void OnQuitButtonPressed()
    {
        StartCoroutine(CleanUpAndLoadMainMenu());
    }

    private void ReturnToMainMenu()
    {
        StartCoroutine(CleanUpAndLoadMainMenu());
    }

    public void OnResumeButtonPressed()
    {
        PauseHandler.Instance?.TogglePauseMenu();
    }

    private IEnumerator CleanUpAndLoadMainMenu()
    {
        // Destroy additional objects assigned in the Inspector first
        foreach (var obj in additionalObjectsToDestroy)
        {
            if (obj != null)
                Destroy(obj);
        }

        // Wait one frame to ensure objects are destroyed
        yield return null;

        // Signal that the game is over before destroying the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver();
            Destroy(GameManager.Instance.gameObject);
        }

        if (AudioManager.Instance != null)
            Destroy(AudioManager.Instance.gameObject);

        if (PlatformManager.Instance != null)
            Destroy(PlatformManager.Instance.gameObject);

        // Load the "main menu" scene asynchronously
        yield return SceneManager.LoadSceneAsync("main menu");

        // Optionally unload previous scenes
        SceneManager.UnloadSceneAsync("main");
        SceneManager.UnloadSceneAsync("main menu");
    }


    #endregion

    private void Awake()
    {
        if (roundText == null) Debug.LogWarning("RoundText no asignado en UIManager.");
        if (timerText == null) Debug.LogWarning("TimerText no asignado en UIManager.");
        if (musicSpeedSlider == null) Debug.LogWarning("MusicSpeedSlider no asignado en UIManager.");
        if (gameOverScreen == null) Debug.LogWarning("GameOverScreen no asignado en UIManager.");
        if (pauseMenu == null) Debug.LogWarning("PauseMenu no asignado en UIManager.");
        if (resumeButton == null) Debug.LogWarning("ResumeButton no asignado en UIManager.");
        if (restartButton == null) Debug.LogWarning("RestartButton no asignado en UIManager.");
        if (quitButton == null) Debug.LogWarning("QuitButton no asignado en UIManager.");
        if (winnerNameText == null) Debug.LogWarning("WinnerNameText no asignado en UIManager.");
        if (winnerImage == null) Debug.LogWarning("WinnerImage no asignado en UIManager.");
    }
}
