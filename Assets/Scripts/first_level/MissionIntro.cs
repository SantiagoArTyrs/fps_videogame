using UnityEngine;
using TMPro;
using System.Collections; 

public class MissionIntro : MonoBehaviour
{
    public TextMeshProUGUI missionText;
    public float fadeInTime = 1.5f;
    public float stayTime = 3f;
    public float fadeOutTime = 1.5f;

    private void Start()
    {
        StartCoroutine(ShowMission());
    }

    IEnumerator ShowMission()
    {
        missionText.text = "";
        Color c = missionText.color;

        // Fade IN
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeInTime);
            missionText.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        // Fade OUT
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeOutTime);
            missionText.color = c;
            yield return null;
        }
    }
}
