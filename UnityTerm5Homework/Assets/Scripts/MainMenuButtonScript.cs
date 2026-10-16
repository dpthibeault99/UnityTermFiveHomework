using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{
    private void Start()
    {
        // Cursor state survives scene changes
        // dosent interfere with mouse look
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
    }

    public void startGame()
    {
        Debug.Log("start game");
        SceneManager.LoadScene("GameScene");
    }

    public void endGame()
    {
        Debug.Log("quit app");
        Application.Quit();
    }
}
