using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance;

    [Header("UI")]
    public TMP_Text globalTimerText;   // Texto que muestra el tiempo en vivo
    public TMP_Text finalTimeText;     // Texto que muestra el tiempo final (solo al detener)

    [Header("Players")]
    public List<GameObject> playerObjects;

    private bool timerStarted = false;
    private bool timerStopped = false;
    private float startTime;
    private float currentTime;

    private Dictionary<GameObject, Vector3> playerLastPositions = new Dictionary<GameObject, Vector3>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var player in playerObjects)
        {
            if (player != null)
                playerLastPositions[player] = player.transform.position;
        }
    }

    void Update()
    {
        if (timerStopped) return;

        if (!timerStarted)
        {
            foreach (var player in playerObjects)
            {
                if (player == null) continue;

                Vector3 lastPos = playerLastPositions[player];
                Vector3 currentPos = player.transform.position;

                if (Vector3.Distance(currentPos, lastPos) > 0.01f)
                {
                    StartTimer();
                    break;
                }
                else
                {
                    playerLastPositions[player] = currentPos;
                }
            }
        }
        else
        {
            currentTime = Time.time - startTime;

            if (globalTimerText != null)
                globalTimerText.text = $"{currentTime:F1}s";
        }
    }

    private void StartTimer()
    {
        timerStarted = true;
        startTime = Time.time;
        Debug.Log("Timer started!");
    }

    // Método público para detener timer y mostrar tiempo final
    public void StopTimer()
    {
        timerStopped = true;
        Debug.Log("Timer stopped at: " + currentTime.ToString("F2") + "s");

        if (finalTimeText != null)
        {
            finalTimeText.text = $"Tiempo final: {currentTime:F2}s";
        }
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}
