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
        // study this
    }

    public void startWeek1()
    {
        Debug.Log("start week1");
        SceneManager.LoadScene("GameScene");
    }
    public void startWeek2()
    {
        Debug.Log("start week2");
        SceneManager.LoadScene("PuzzleScene");
    }

    public void endGame()
    {
        Debug.Log("quit app");
        Application.Quit();
    }
}
