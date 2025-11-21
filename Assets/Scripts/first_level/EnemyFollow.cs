using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyFollow : MonoBehaviour
{
    public Transform player;         // referencia al jugador
    public float followRange = 15f;  // rango en el que empieza a perseguir
    public float attackRange = 2f;   // rango para atacar

    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.updateRotation = false;

        // busca automáticamente al player por tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // si está dentro del rango de persecución
        if (distance <= followRange)
        {
            agent.SetDestination(player.position);

            // si está fuera del rango de ataque → caminar/correr
            if (distance > attackRange)
            {
                agent.isStopped = false;
                anim.ResetTrigger("Attack");
                anim.SetFloat("Speed", agent.velocity.magnitude); // activa el blend tree
            }
            else // si está dentro del rango → atacar
            {
                agent.isStopped = true;
                anim.SetFloat("Speed", 0);
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            // si el jugador está lejos, se detiene
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
        }
    }
}
