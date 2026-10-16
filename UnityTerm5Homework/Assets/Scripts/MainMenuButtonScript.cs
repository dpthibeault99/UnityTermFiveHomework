using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{
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
