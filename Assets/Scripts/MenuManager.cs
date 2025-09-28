using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame1()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGame2()
    {
        SceneManager.LoadScene(2);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    public void QuitGame()
    {
        Debug.Log("Game closed!");
        Application.Quit();
    }
}
