using UnityEngine;

public class FootstepSimple : MonoBehaviour
{
    public CharacterController controller;   // Tu CharacterController
    public AudioSource audioSource;          // Sonido para las pisadas
    public AudioClip[] footstepClips;        // Varias pisadas aleatorias

    public float stepInterval = 0.4f;        // Tiempo entre pasos
    private float stepTimer = 0f;
    

    void Update()
    {
        // jugador caminando
        bool isMoving = controller.velocity.magnitude > 0.15f && controller.isGrounded;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval; // Reinicia el tiempo
            }
        }
        else
        {
            stepTimer = 0f; // si está quieto, reiniciar timer
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
        Debug.Log("Velocidad player: " + controller.velocity);
    }
    
}
