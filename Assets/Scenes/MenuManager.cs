using UnityEngine;
using UnityEngine.SceneManagement;   // Lets us load other scenes

// Handles the buttons on the title screen.
public class MenuManager : MonoBehaviour {
    public GameObject titlePanel;          // The page with the title and Start button
    public GameObject instructionsPanel;   // The page with the how-to-play text

    void Start() {
        titlePanel.SetActive(true);           // Show the title first
        instructionsPanel.SetActive(false);   // Hide the instructions
    }

    // Called by the Start button
    public void ShowInstructions() {
        titlePanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    // Called by the Let's Go! button
    public void StartGame() {
        SceneManager.LoadScene("SampleScene");   // Load the garden
    }
}