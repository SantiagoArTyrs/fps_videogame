using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            print ("Player Dead");
            // Game over
            // Respawn Player
        }
        else
        {
            print ("Player Hit");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwordSkeleton"))
        {
            TakeDamage(other.gameObject.GetComponent<SwordSkeleton>().damage);
        }
    }
}
