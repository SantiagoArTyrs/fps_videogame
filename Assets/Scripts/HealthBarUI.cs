using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fillBar;

    void Update()
    {
        if (playerHealth == null || fillBar == null) return;

        float fill = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        fillBar.fillAmount = fill;

        // Cambiar color según vida
        if (fill > 0.6f)
            fillBar.color = Color.green;
        else if (fill > 0.3f)
            fillBar.color = Color.yellow;
        else
            fillBar.color = Color.red;
    }
} 
