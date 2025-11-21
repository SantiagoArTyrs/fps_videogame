using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Life Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public Image healthBar;
    public Image damageFlashImage;
    public GameObject deathCanvas;
    public Image fadeScreen;              // ← NUEVO: Fade a negro

    [Header("Death Audio")]
    public AudioClip youDiedVoiceClip;   // Audio 1
    public AudioClip impactDeathClip;    // Audio 2 (loop)
    public AudioClip evilLaughClip;      // Audio 3

    private AudioSource audioSourceMain; // Audio 1 & 3
    private AudioSource audioSourceLoop; // Audio 2 (loop continuo)

    public bool isDead = false;
    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;

        // AUDIO MAIN
        audioSourceMain = gameObject.AddComponent<AudioSource>();
        audioSourceMain.spatialBlend = 0f;
        audioSourceMain.playOnAwake = false;

        // AUDIO LOOP
        audioSourceLoop = gameObject.AddComponent<AudioSource>();
        audioSourceLoop.spatialBlend = 0f;
        audioSourceLoop.loop = true;
        audioSourceLoop.playOnAwake = false;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        if (damageFlashImage != null)
            damageFlashImage.color = new Color(1, 0, 0, 0);

        if (fadeScreen != null)
            fadeScreen.color = new Color(0, 0, 0, 0); // transparente

        UpdateHealthUIImmediate();
    }

    // ===========================================
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        UpdateHealthUI();
        DamageFlash();

        if (currentHealth <= 0)
            Die();
    }

    // ===========================================
    void Die()
    {
        if (isDead) return;
        isDead = true;

        DisablePlayerMovement();
        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        StartCoroutine(PlayDeathAudioSequence());
    }

    // ===========================================
    IEnumerator PlayDeathAudioSequence()
    {
        // 🔊 AUDIO 2 — Loop continuo
        if (impactDeathClip != null)
        {
            audioSourceLoop.clip = impactDeathClip;
            audioSourceLoop.Play();
        }

        // 🔊 AUDIO 1 — YOU DIED
        if (youDiedVoiceClip != null)
        {
            audioSourceMain.PlayOneShot(youDiedVoiceClip);
            yield return new WaitForSeconds(youDiedVoiceClip.length);
        }

        // 🔊 AUDIO 3 — Risa malévola
        if (evilLaughClip != null)
        {
            audioSourceMain.PlayOneShot(evilLaughClip);
            yield return new WaitForSeconds(evilLaughClip.length);
        }

        // 🎬 FADE A NEGRO ANTES DE IR AL MENÚ
        yield return StartCoroutine(FadeToBlack());

        // Espera 3 segundos en pantalla negra
        yield return new WaitForSeconds(3f);

        audioSourceLoop.Stop();

        SceneManager.LoadScene("MainMenu");
    }

    // ===========================================
    IEnumerator FadeToBlack()
    {
        float t = 0f;
        float duration = 2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / duration);

            if (fadeScreen != null)
                fadeScreen.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }

    // ===========================================
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            float target = (float)currentHealth / maxHealth;
            StartCoroutine(AnimateHealthBar(target));
        }
    }

    void UpdateHealthUIImmediate()
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    IEnumerator AnimateHealthBar(float target)
    {
        float start = healthBar.fillAmount;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            healthBar.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }

    // ===========================================
    void DamageFlash()
    {
        if (!isFlashing)
            StartCoroutine(DamageFlashEffect());
    }

    IEnumerator DamageFlashEffect()
    {
        isFlashing = true;

        for (float a = 0; a <= 0.6f; a += Time.deltaTime * 4f)
        {
            if (damageFlashImage != null)
                damageFlashImage.color = new Color(1, 0, 0, a);
            yield return null;
        }

        for (float a = 0.6f; a >= 0; a -= Time.deltaTime * 2f)
        {
            if (damageFlashImage != null)
                damageFlashImage.color = new Color(1, 0, 0, a);
            yield return null;
        }

        isFlashing = false;
    }

    // ===========================================
    void DisablePlayerMovement()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }
    }
}
