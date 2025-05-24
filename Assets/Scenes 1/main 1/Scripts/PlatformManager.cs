using System.Collections;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    // Singleton
    public static PlatformManager Instance { get; private set; }

    [Header("Plataformas")]
    public GameObject[] platforms;               // Las 7 plataformas en la escena
    public Material[] platformMaterials;         // Un material único para cada plataforma
    public Transform referencePlatform;          // Objeto visual que imita el material de la plataforma segura

    [Header("Configuración")]
    public float dropDistance = 20f;             // Cuánto caen las plataformas
    public float resetDelay = 2f;                // Tiempo antes de restaurar plataformas tras caída
    public float riseSpeed = 2f;                 // Velocidad de subida de las plataformas

    [Header("Audio")]
    [SerializeField] private AudioClip safePlatformClip;  // AudioClip para plataforma segura
    [SerializeField] private AudioSource audioSource;     // AudioSource para controlar el volumen

    private int safePlatformIndex = -1;          // Índice de plataforma segura actual
    private Vector3[] originalPositions;         // Posiciones iniciales

    private void Awake()
    {
        // Configurar Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: si quieres que no se destruya al cambiar de escena
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StoreOriginalPositions();
    }

    private void StoreOriginalPositions()
    {
        originalPositions = new Vector3[platforms.Length];
        for (int i = 0; i < platforms.Length; i++)
        {
            originalPositions[i] = platforms[i].transform.position;
        }
    }

    public GameObject GetSafePlatform()
    {
        if (safePlatformIndex >= 0 && safePlatformIndex < platforms.Length)
            return platforms[safePlatformIndex];

        return null;
    }

    public GameObject SelectSafePlatform()
    {
        safePlatformIndex = Random.Range(0, platforms.Length);
        PlaySafePlatformSound();
        return platforms[safePlatformIndex];
    }

    private void PlaySafePlatformSound()
    {
        if (safePlatformClip != null && audioSource != null)
        {
            audioSource.clip = safePlatformClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Safe platform sound or audio source is not set.");
        }
    }

    public void ApplyReferenceMaterial()
    {
        if (safePlatformIndex < 0 || safePlatformIndex >= platforms.Length)
        {
            Debug.LogWarning("Plataforma segura no válida al aplicar material.");
            return;
        }

        Transform safeCylinder = platforms[safePlatformIndex].transform.Find("Cylinder.001");
        if (safeCylinder == null)
        {
            Debug.LogWarning("No se encontró 'Cylinder.001' en la plataforma segura.");
            return;
        }

        Renderer safeCylinderRenderer = safeCylinder.GetComponent<Renderer>();
        if (safeCylinderRenderer == null)
        {
            Debug.LogWarning("El objeto 'Cylinder.001' no tiene un Renderer.");
            return;
        }

        Transform referenceCylinder = referencePlatform.transform.Find("Cylinder.001");
        if (referenceCylinder == null)
        {
            Debug.LogWarning("No se encontró 'Cylinder.001' en la plataforma de referencia.");
            return;
        }

        Renderer referenceCylinderRenderer = referenceCylinder.GetComponent<Renderer>();
        if (referenceCylinderRenderer == null)
        {
            Debug.LogWarning("El objeto 'Cylinder.001' en la plataforma de referencia no tiene un Renderer.");
            return;
        }

        referenceCylinderRenderer.material = safeCylinderRenderer.material;
    }

    public void DropUnsafePlatforms(float fallSpeed)
    {
        StartCoroutine(DropPlatformsRoutine(fallSpeed));
    }

    private IEnumerator DropPlatformsRoutine(float fallSpeed)
    {
        bool[] isFalling = new bool[platforms.Length];
        Vector3[] targetPositions = new Vector3[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            if (i == safePlatformIndex) continue;

            isFalling[i] = true;
            targetPositions[i] = platforms[i].transform.position + Vector3.down * dropDistance;
        }

        bool anyMoving = true;
        while (anyMoving)
        {
            anyMoving = false;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (!isFalling[i]) continue;

                platforms[i].transform.position = Vector3.MoveTowards(
                    platforms[i].transform.position,
                    targetPositions[i],
                    fallSpeed * Time.deltaTime
                );

                if (Vector3.Distance(platforms[i].transform.position, targetPositions[i]) > 0.01f)
                {
                    anyMoving = true;
                }
                else
                {
                    isFalling[i] = false;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(resetDelay);

        StartCoroutine(RisePlatformsRoutine(fallSpeed));
    }

    private IEnumerator RisePlatformsRoutine(float riseSpeed)
    {
        bool[] isRising = new bool[platforms.Length];
        Vector3[] targetPositions = new Vector3[platforms.Length];

        for (int i = 0; i < platforms.Length; i++)
        {
            isRising[i] = true;
            targetPositions[i] = originalPositions[i];
        }

        bool anyMoving = true;
        while (anyMoving)
        {
            anyMoving = false;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (!isRising[i]) continue;

                platforms[i].transform.position = Vector3.MoveTowards(
                    platforms[i].transform.position,
                    targetPositions[i],
                    riseSpeed * Time.deltaTime
                );

                if (Vector3.Distance(platforms[i].transform.position, targetPositions[i]) > 0.01f)
                {
                    anyMoving = true;
                }
                else
                {
                    isRising[i] = false;
                }
            }

            yield return null;
        }

        ResetAllPlatforms();
    }

    public void ResetAllPlatforms()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].transform.position = originalPositions[i];
            platforms[i].SetActive(true);
        }

        safePlatformIndex = -1;
    }

    public IEnumerator WaitForPlatformsToReset()
    {
        bool allPlatformsReset = false;

        while (!allPlatformsReset)
        {
            allPlatformsReset = true;
            for (int i = 0; i < platforms.Length; i++)
            {
                if (Vector3.Distance(platforms[i].transform.position, originalPositions[i]) > 0.01f)
                {
                    allPlatformsReset = false;
                    break;
                }
            }
            yield return null;
        }
    }

    public void ResetPlatformManager()
    {
        ResetAllPlatforms();
        safePlatformIndex = -1;
        // Aquí puedes añadir más lógica si lo necesitas
    }
}
