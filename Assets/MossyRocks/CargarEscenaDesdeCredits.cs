using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscenaDesdeCredits : MonoBehaviour
{
    public string escenaSiguiente = "JuegoPrincipal"; // Cambia esto por el nombre de tu siguiente escena
    public float tiempoMinimo = 5f; // segundos que dura la pantalla de créditos

    void Start()
    {
        StartCoroutine(CargarSiguiente());
    }

    IEnumerator CargarSiguiente()
    {
        float tiempoInicio = Time.time;

        AsyncOperation carga = SceneManager.LoadSceneAsync(escenaSiguiente);
        carga.allowSceneActivation = false;

        while (!carga.isDone)
        {
            if (carga.progress >= 0.9f && Time.time - tiempoInicio >= tiempoMinimo)
            {
                carga.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
