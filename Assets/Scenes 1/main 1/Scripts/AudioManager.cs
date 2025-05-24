using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Componentes de Audio")]
    [SerializeField] private AudioSource musicSource;    // Controla la reproducción de música.
    [SerializeField] private AudioSource sfxSource;      // Controla los efectos de sonido.
    [SerializeField] private AudioClip safePlatformClip; // Clip de sonido para la plataforma segura.

    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip backgroundMusic;  // Música de fondo por defecto.
    [SerializeField] private AudioClip pauseMusicClip;   // Música para la pausa.

    private float currentPitch = 1.0f;                   // Pitch actual de la música.
    private float basePitch = 1.0f;                      // Pitch base para reiniciar.
    private float masterVolume = 1.0f;                   // Volumen global.
    private bool isMuted = false;                        // Estado de muteo global.
    private bool isPlayingPauseMusic = false;            // Controla si se está reproduciendo música de pausa.

    // Instancia Singleton de AudioManager
    public static AudioManager Instance { get; private set; }

    private bool isPaused = false; // Estado de pausa

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            // Eliminar o comentar esta línea para evitar que el AudioManager persista entre escenas
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // Destruir cualquier instancia duplicada
        }
    }

    private void Start()
    {
        // Reproducir música de fondo si está configurada
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    public void PlaySafePlatformSound()
    {
        if (sfxSource == null || safePlatformClip == null) return;

        sfxSource.PlayOneShot(safePlatformClip, masterVolume);
    }

    public void ResetAudioSettings()
    {
        // Resetear el pitch de la música al valor base
        SetMusicPitch(basePitch);

        // Resetear el volumen al valor por defecto
        SetVolume(masterVolume);

        // Detener la música si es necesario
        StopMusic();

        // Opcionalmente restablecer el estado de muteo
        ToggleMute(isMuted);
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxSource == null || sfxClip == null) return;

        sfxSource.PlayOneShot(sfxClip, masterVolume);
    }

    public void PauseMusic()
    {
        if (musicSource == null) return;

        if (pauseMusicClip != null && !isPlayingPauseMusic)
        {
            musicSource.clip = pauseMusicClip;
            musicSource.loop = true;
            musicSource.Play();
            isPlayingPauseMusic = true;
        }
        else
        {
            musicSource.Pause(); // Si no hay música de pausa, simplemente pausar
        }
    }

    public void ResumeMusic()
    {
        if (musicSource == null) return;

        if (isPlayingPauseMusic && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
            isPlayingPauseMusic = false;
        }
        else
        {
            musicSource.UnPause(); // Si no se estaba usando música de pausa, reanudar
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource == null || musicClip == null) return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.pitch = basePitch;
        musicSource.volume = masterVolume;
        musicSource.mute = isMuted;
        musicSource.Play();
    }

    public void SetMusicPitch(float pitch)
    {
        currentPitch = pitch;
        if (musicSource != null)
        {
            musicSource.pitch = currentPitch;
        }
    }

    // Método para manejar el estado de pausa sin detener la música
    public void TogglePauseMusic(bool pause)
    {
        if (musicSource == null) return;

        if (pause)
        {
            // Si el juego está en pausa, reducir la velocidad de la música
            musicSource.pitch = 0.75f;
        }
        else
        {
            // Si el juego no está en pausa, restaurar la velocidad normal
            musicSource.pitch = 1.0f;
        }
    }

    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = masterVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = masterVolume;
        }
    }

    public void ToggleMute(bool mute)
    {
        isMuted = mute;

        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
