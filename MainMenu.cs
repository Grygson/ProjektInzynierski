using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro namespace

public class MainMenu : MonoBehaviour
{
    public TMP_InputField nodeCountInput; // Pole wejściowe do liczby wierzchołków
    public TMP_Dropdown difficultyDropdown; // Dropdown do wyboru poziomu trudności

    private void Start()
    {
        // Ustaw domyślną wartość w polu wejściowym
        if (nodeCountInput != null)
        {
            nodeCountInput.text = PlayerPrefs.GetInt("NodeCount", 20).ToString();
            nodeCountInput.onValueChanged.AddListener(OnNodeCountChanged);
        }
        else
        {
            Debug.LogError("Node Count Input is not assigned in the Inspector. Please assign it to a TMP_InputField object.");
        }
    }

    public void StartVsPlayer()
    {
        if (!ValidateInputField()) return;

        SaveNodeCount();
        PlayerPrefs.SetInt("VsAI", 0);
        SceneManager.LoadScene("GameSceneVsPlayer");
    }

    public void StartVsAI()
    {
        if (!ValidateInputField()) return;

        int selectedDifficulty = difficultyDropdown.value;
        PlayerPrefs.SetInt("Difficulty", selectedDifficulty);
        SaveNodeCount();
        PlayerPrefs.SetInt("VsAI", 1);
        SceneManager.LoadScene("GameSceneVsAI");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }

    private void SaveNodeCount()
    {
        if (nodeCountInput == null)
        {
            Debug.LogError("Node Count Input is not assigned. Cannot save node count.");
            return;
        }

        if (int.TryParse(nodeCountInput.text, out int nodeCount))
        {
            PlayerPrefs.SetInt("NodeCount", Mathf.Max(5, nodeCount)); // Minimalna liczba wierzchołków to 5
        }
        else
        {
            Debug.LogWarning("Invalid node count input. Using default value: 20");
            PlayerPrefs.SetInt("NodeCount", 20);
        }
    }

    private void OnNodeCountChanged(string newValue)
    {
        if (int.TryParse(newValue, out int nodeCount))
        {
            PlayerPrefs.SetInt("NodeCount", Mathf.Max(5, nodeCount));
        }
        else
        {
            Debug.LogWarning("Invalid input for node count.");
        }
    }

    private bool ValidateInputField()
    {
        if (nodeCountInput == null)
        {
            Debug.LogError("Node Count Input is not assigned in the Inspector. Please assign it to a TMP_InputField object.");
            return false;
        }

        return true;
    }
}
