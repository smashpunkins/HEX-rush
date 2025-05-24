using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [Header("Estado del Jugador")]
    public bool isEliminated = false;
    public GameObject player;

    [Header("Efectos y Sonidos")]
    public AudioClip eliminationSound;
    [Range(0f, 1f)] public float eliminationVolume = 1.0f; // Volume control
    public ParticleSystem eliminationEffect;

    [Header("Referencias Opcionales")]
    public GameManager gameManager;

    private bool isEliminating = false;

    public static List<GameObject> playersList = new List<GameObject>();

    public GameObject lavaObject;

    private void Awake()
    {
        if (player == null)
            player = gameObject;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        // Solo agregamos si no existe aún
        if (!playersList.Contains(player))
        {
            playersList.Add(player);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isEliminated && collision.gameObject == lavaObject && !isEliminating)
        {
            EliminateCharacter();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isEliminated && hit.gameObject == lavaObject && !isEliminating)
        {
            EliminateCharacter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEliminated && other.gameObject == lavaObject && !isEliminating)
        {
            EliminateCharacter();
        }
    }

    private void EliminateCharacter()
    {
        if (isEliminated || isEliminating) return;

        isEliminating = true;

        PlayEliminationEffects(player.transform.position);
        OnPlayerEliminated();

        if (gameManager != null)
        {
            gameManager.OnCharacterEliminated(player);
        }

        isEliminating = false;
    }

    private void PlayEliminationEffects(Vector3 position)
    {
        if (eliminationEffect != null)
        {
            ParticleSystem ps = Instantiate(eliminationEffect, position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }

        if (eliminationSound != null)
        {
            // Create a temporary GameObject to play 2D audio
            GameObject tempAudio = new GameObject("Temp2DAudio");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            // Configure the audio source for 2D playback
            audioSource.clip = eliminationSound;
            audioSource.volume = eliminationVolume;
            audioSource.spatialBlend = 0f; // 0 = 2D sound
            audioSource.Play();

            // Destroy the temporary GameObject after the sound finishes playing
            Destroy(tempAudio, eliminationSound.length);
        }
    }


    public void OnPlayerEliminated()
    {
        if (isEliminated) return;

        isEliminated = true;

        DisablePlayer();

        if (playersList.Contains(player))
        {
            playersList.Remove(player);
        }
    }

    private void DisablePlayer()
    {
        if (player != null)
        {
            player.SetActive(false);
        }
    }

    public void ResetPlayerManager()
    {
        isEliminated = false;

        if (player != null)
        {
            player.SetActive(true);
        }

        if (eliminationEffect != null)
        {
            eliminationEffect.Stop();
        }
    }

    public static void ResetAllPlayers()
    {
        foreach (GameObject playerObj in playersList)
        {
            PlayerManager pm = playerObj.GetComponent<PlayerManager>();
            if (pm != null)
            {
                pm.ResetPlayerManager();
            }
        }
    }
}
