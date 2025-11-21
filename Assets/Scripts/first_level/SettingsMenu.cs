using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle musicToggle;

    void Start()
    {
        // Cargar estado guardado previamente
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        musicToggle.isOn = isMusicOn;

        // Aplicar estado inicial
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMusic(isMusicOn);

        // Escuchar cambios del toggle
        musicToggle.onValueChanged.AddListener(delegate {
            ToggleMusic();
        });
    }

    public void ToggleMusic()
    {
        bool state = musicToggle.isOn;

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMusic(state);

        // Guardar preferencia
        PlayerPrefs.SetInt("MusicOn", state ? 1 : 0);
    }
}
