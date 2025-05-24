using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject containerMainMenu;             // "container main menu"
    public GameObject howToPlay;                     // "how to play"
    public GameObject containerCharacterSelection;   // "container character selection"

    private void Start()
    {
        if (containerMainMenu != null)
            containerMainMenu.SetActive(true);

        if (howToPlay != null)
            howToPlay.SetActive(false);

        if (containerCharacterSelection != null)
            containerCharacterSelection.SetActive(false);
    }

    public void ShowInstructions()
    {
        containerMainMenu.SetActive(false);
        howToPlay.SetActive(true);
    }

    public void BackToMainMenu()
    {
        howToPlay.SetActive(false);
        containerCharacterSelection.SetActive(false);
        containerMainMenu.SetActive(true);
    }

    public void GoToCharacterSelection()
    {
        containerMainMenu.SetActive(false);
        howToPlay.SetActive(false);
        containerCharacterSelection.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("main"); // Asegúrate de que la escena se llama "main"
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Solo funciona en la build
    }
}
