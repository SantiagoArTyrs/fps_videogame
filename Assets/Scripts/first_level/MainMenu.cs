using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Mainmenu;   // Panel principal del menú
    public GameObject Settings;   // Panel de ajustes

    public void OpenSettings()
    {
        Mainmenu.SetActive(false);
        Settings.SetActive(true);
    }

    public void BackToMain()
    {
        Settings.SetActive(false);
        Mainmenu.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("first_level");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
