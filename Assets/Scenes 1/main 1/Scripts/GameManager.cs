using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración de Rondas")]
    public float roundDelay = 6f;
    public float delayMultiplier = 0.75f;
    public int currentRound = 1;

    // Make private and expose via getter
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    public PlatformManager platformManager;
    public UIManager uiManager;
    public AudioManager audioManager;

    [Header("Jugadores (configuración manual)")]
    public List<PlayerManager> players = new List<PlayerManager>();

    private GameObject safePlatform;
    private string lastEliminatedPlayer = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioManager?.SetMusicPitch(1.0f);
        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        while (!isGameOver)
        {
            yield return StartRound();
            yield return new WaitForSeconds(roundDelay);

            platformManager.DropUnsafePlatforms(5f + (currentRound * 0.5f));

            if (CheckGameOver())
                break;

            yield return platformManager.WaitForPlatformsToReset();

            currentRound++;
            UpdateRoundInfo();
        }
    }

    IEnumerator StartRound()
    {
        yield return new WaitForSeconds(3f);

        safePlatform = platformManager.SelectSafePlatform();
        platformManager.ApplyReferenceMaterial();
        audioManager?.PlaySafePlatformSound();

        NotifyAllAI(safePlatform);
    }

    private void NotifyAllAI(GameObject safePlatform)
    {
        if (safePlatform == null) return;

        AIController[] aiControllers = FindObjectsOfType<AIController>();
        foreach (AIController ai in aiControllers)
        {
            ai.SetNewTarget(safePlatform.transform);
        }
    }

    bool CheckGameOver()
    {
        List<PlayerManager> alivePlayers = GetAlivePlayers();
        Debug.Log("Alive players count: " + alivePlayers.Count);

        if (alivePlayers.Count == 0)
        {
            string winner = lastEliminatedPlayer != "" ? lastEliminatedPlayer : "Nadie";
            uiManager?.ShowGameOverScreen(winner);
            SetGameOver(); // <- Use this instead of direct assignment

            // Stop the timer when the game is over
            GameTimerManager.Instance?.StopTimer();

            return true;
        }

        return false;
    }

    public void SetGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("[GameManager] Juego terminado.");
        }
    }

    public void OnCharacterEliminated(GameObject eliminatedCharacter)
    {
        Debug.Log(eliminatedCharacter.name + " ha sido eliminado.");

        lastEliminatedPlayer = eliminatedCharacter.name;

        PlayerManager player = eliminatedCharacter.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.OnPlayerEliminated();
            RemovePlayer(player);
        }

        if (!isGameOver)
        {
            CheckGameOver();
        }
    }

    public void NotifyAIElimination(AIController eliminatedAI)
    {
        Debug.Log("NPC eliminado: " + eliminatedAI.name);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }

    public void RestartGame()
    {
        audioManager?.ResetAudioSettings();
        PlayerManager.ResetAllPlayers();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateRoundInfo()
    {
        uiManager?.DisplayRoundCounter(currentRound);
        roundDelay *= delayMultiplier;
        audioManager?.SetMusicPitch(1.0f + ((currentRound - 1) * 0.1f));
    }

    #region Métodos para Gestión de Jugadores (Manual)
    public void AddPlayer(PlayerManager playerManager)
    {
        if (!players.Contains(playerManager))
        {
            players.Add(playerManager);
            Debug.Log("Jugador añadido: " + playerManager.player.name);
        }
    }

    public void RemovePlayer(PlayerManager playerManager)
    {
        if (players.Contains(playerManager))
        {
            players.Remove(playerManager);
            Debug.Log("Jugador removido: " + playerManager.player.name);
        }
    }

    public List<PlayerManager> GetAlivePlayers()
    {
        return players.FindAll(p => !p.isEliminated);
    }
    #endregion
}
