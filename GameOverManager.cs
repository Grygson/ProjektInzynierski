using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Namespace dla TextMeshPro

public class GameOverManager : MonoBehaviour
{
    public TMP_Text winnerText; // Pole tekstowe dla zwycięzcy

    private void Start()
    {
        // Pobierz nazwę zwycięzcy z PlayerPrefs
        string winner = PlayerPrefs.GetString("Winner", "No winner");

        // Ustaw tekst wyświetlany w UI
        winnerText.text = $"{winner} wins!";
    }

public void TryAgain()
{
    // Sprawdź tryb gry na podstawie PlayerPrefs
    int isVsAI = PlayerPrefs.GetInt("VsAI", 0); // 0 - gra vs Player, 1 - gra vs AI

    // Wybierz odpowiednią scenę na podstawie trybu gry
    string sceneToLoad = isVsAI == 1 ? "GameSceneVsAI" : "GameSceneVsPlayer";

    // Debugowanie
    Debug.Log("Restarting the game in mode: " + (isVsAI == 1 ? "VsAI" : "VsPlayer"));
    Debug.Log("Loading scene: " + sceneToLoad);

    // Załaduj odpowiednią scenę
    SceneManager.LoadScene(sceneToLoad);
}

    public void MainMenu()
    {
        // Wczytaj scenę menu głównego
        SceneManager.LoadScene("StartScene");
    }
}
