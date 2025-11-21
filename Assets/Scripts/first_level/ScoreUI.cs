using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text resultText;    // "Moriste" o "Nivel completado"
    public TMP_Text barrelsText;   // "Barriles: X / Y"
    public TMP_Text timeText;      // "Tiempo: 12.3s"

    private int lastLevelIndex;

    void Start()
    {
        // Si no existe ScoreManager, mostramos error
        if (ScoreManager.Instance == null)
        {
            if (resultText != null)
                resultText.text = "<color=red>Error: No ScoreManager.</color>";
            return;
        }

        var score = ScoreManager.Instance;

        // Guardamos el nivel del que venimos
        lastLevelIndex = score.lastLevelIndex;

        // ============================
        //      MENSAJE PRINCIPAL
        // ============================
        if (score.playerDied)
        {
            resultText.text = "<color=red>❌ Moriste</color>";
        }
        else if (score.levelCompleted)
        {
            resultText.text = "<color=green>✔ Nivel completado</color>";
        }
        else
        {
            resultText.text = "<color=yellow>Resultado desconocido</color>";
        }

        // ============================
        //      BARRILES
        // ============================
        barrelsText.text =
            "Barriles: " +
            score.barrelsCollected +
            " / " +
            score.barrelsTotal;

        // ============================
        //      TIEMPO
        // ============================
        timeText.text = "Tiempo: " + score.levelTimer.ToString("F1") + " s";
    }

    // ============================================================
    //                    LÓGICA DEL BOTÓN CONTINUE
    // ============================================================
    public void Continue()
    {
        if (ScoreManager.Instance == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        var score = ScoreManager.Instance;

        // Si murió → reintentar el nivel desde el que vino
        if (score.playerDied)
        {
            ScoreManager.Instance.ResetScore();

            // Volver al nivel original
            SceneManager.LoadScene(lastLevelIndex);
            return;
        }

        // Si ganó → ir al siguiente nivel
        if (score.levelCompleted)
        {
            ScoreManager.Instance.ResetScore();

            int nextIndex = lastLevelIndex + 1;

            // Si no existe el siguiente nivel, ir al menú
            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }

            SceneManager.LoadScene(nextIndex);
        }
    }
}
