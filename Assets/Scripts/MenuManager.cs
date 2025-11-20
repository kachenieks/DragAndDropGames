using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject Canvas_Windows;
    public GameObject Canvas_Android;

    [Header("Editor Test Mode")]
    public bool ForceAndroidUIInEditor = false;

    void Awake()
    {
        bool isAndroid = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        isAndroid = true;
#endif

#if UNITY_EDITOR
        if (ForceAndroidUIInEditor)
            isAndroid = true;
#endif

        if (Canvas_Windows != null)
            Canvas_Windows.SetActive(!isAndroid);
        if (Canvas_Android != null)
            Canvas_Android.SetActive(isAndroid);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGame2()
    {
        SceneManager.LoadScene(2);
    }

    // ✅ Atgriešanās atpakaļ uz MainMenu
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ✅ Aizver spēli vai apstādina Editoru
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
