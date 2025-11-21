using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Datos del nivel")]
    public int lastLevelIndex;       // ← Nivel donde estabas antes de morir / ganar
    public bool playerDied;          // ← si el jugador murió
    public bool levelCompleted;      // ← si se completó el nivel

    [Header("Puntuación")]
    public int barrelsCollected;     // barriles recogidos
    public int barrelsTotal;         // barriles totales del nivel
    public float levelTimer;         // tiempo total en segundos

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Solo contar tiempo si el jugador ni murió ni ganó
        if (!playerDied && !levelCompleted)
            levelTimer += Time.deltaTime;
    }

    // ============================================================
    //                 MÉTODOS PARA ACTUALIZAR DATOS
    // ============================================================
    public void RegisterLevelStart(int levelIndex, int totalBarrels)
    {
        lastLevelIndex = levelIndex;
        barrelsTotal = totalBarrels;
        barrelsCollected = 0;
        levelTimer = 0;
        playerDied = false;
        levelCompleted = false;
    }

    public void AddBarrel()
    {
        barrelsCollected++;
    }

    public void MarkDeath()
    {
        playerDied = true;
    }

    public void MarkLevelComplete()
    {
        levelCompleted = true;
    }

    // ============================================================
    //               RESET PARA SIGUIENTE NIVEL
    // ============================================================
    public void ResetScore()
    {
        barrelsCollected = 0;
        barrelsTotal = 0;
        levelTimer = 0;

        playerDied = false;
        levelCompleted = false;
    }
}
