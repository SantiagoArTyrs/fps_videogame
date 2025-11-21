using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int checkpointsCollected = 0;
    public int totalCheckpoints = 4;

    [Header("UI")]
    public TMP_Text checkpointText;

    [Header("Audio de victoria")]
    public AudioClip victoryMusic;
    private AudioSource audioSource;

    private bool levelCompleted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar UI solo si existe en la escena
        GameObject uiObj = GameObject.FindWithTag("CheckpointText");

        if (uiObj != null)
        {
            checkpointText = uiObj.GetComponent<TMP_Text>();
            UpdateCheckpointUI();
        }
        else
        {
            // No hay UI en esta escena → no pasa nada
            checkpointText = null;
        }
    }

    private void Start()
    {
        UpdateCheckpointUI();
    }

    public void AddCheckpoint()
    {
        if (levelCompleted) return;

        checkpointsCollected++;
        Debug.Log("Checkpoints: " + checkpointsCollected);

        UpdateCheckpointUI();

        if (checkpointsCollected >= totalCheckpoints)
        {
            CompleteLevel();
        }
    }

    void UpdateCheckpointUI()
    {
        if (checkpointText != null)
        {
            checkpointText.text = $"Cubos: {checkpointsCollected} / {totalCheckpoints}";
        }
        else
        {
            // Evita errores en escenas sin UI
            Debug.Log("No hay texto de checkpoint en esta escena.");
        }
    }

    void CompleteLevel()
    {
        levelCompleted = true;

        if (victoryMusic != null)
            audioSource.PlayOneShot(victoryMusic);

        Invoke("LoadNextLevel", 3f);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
