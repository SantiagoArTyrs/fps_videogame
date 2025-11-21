using UnityEngine;

public class CheckpointCube : MonoBehaviour
{
    private bool activated = false; // para evitar que se sume 2 veces

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            // Llamamos al GameManager
            GameManager.Instance.AddCheckpoint();
        }
    }
}
