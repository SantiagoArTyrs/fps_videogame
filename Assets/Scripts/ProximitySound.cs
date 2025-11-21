using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    public Transform player;
    public float maxDistance = 10f;
    public float minDistance = 1.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("❌ NO hay AudioSource en el barril.");
            return;
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null)
        {
            Debug.LogError("❌ NO se encontró player con TAG Player.");
        }

        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        Debug.Log("✔ Sonido de proximidad iniciado.");
    }

    void Update()
    {
        if (player == null || audioSource == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist > maxDistance)
        {
            audioSource.volume = 0f;
            return;
        }

        float t = Mathf.InverseLerp(maxDistance, minDistance, dist);
        audioSource.volume = Mathf.Lerp(0f, 1f, 1f - t);
    }
}
