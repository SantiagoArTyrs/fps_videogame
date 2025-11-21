using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject player;

    [Header("Mission UI")]
    public TextMeshProUGUI missionText;        // ← Texto de misión dentro del PauseMenu
    public string currentMission = "MISIÓN: Encuentra la salida...";

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Asegurar que la misión no se muestre antes del pause
        if (missionText != null)
            missionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
            player.SetActive(true);

        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
            player.SetActive(false);

        // Mostrar misión dentro del menú de pausa
        if (missionText != null)
        {
            missionText.text = currentMission;
            missionText.gameObject.SetActive(true);
        }

        isPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
