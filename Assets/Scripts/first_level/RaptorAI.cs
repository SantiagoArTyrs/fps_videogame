using UnityEngine;
using UnityEngine.AI;

public class RaptorAI : MonoBehaviour
{
    public Transform target;               // Player o el objetivo
    public float attackDistance = 2f;      // Distancia para atacar
    public float chaseDistance = 20f;      // Desde cuanto empieza a perseguir

    NavMeshAgent agent;
    Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // MUY IMPORTANTE: no usar RootMotion para IA
        anim.applyRootMotion = false;

        // El NavMesh controla la rotación
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    void Update()
    {
        if (target == null)
            return;

        float dist = Vector3.Distance(transform.position, target.position);

        // Si está muy lejos, Idle
        if (dist > chaseDistance)
        {
            agent.isStopped = true;
            anim.SetFloat("speed", 0f);
            return;
        }

        // Si está dentro del rango de ataque
        if (dist <= attackDistance)
        {
            agent.isStopped = true;         // Se detiene para atacar
            anim.SetFloat("speed", 0f);
            anim.SetTrigger("attack");      // Tackle o Bite
            return;
        }

        // Si está persiguiendo
        agent.isStopped = false;
        agent.SetDestination(target.position);

        // Actualizar velocidad para animaciones
        float currentSpeed = agent.velocity.magnitude;
        anim.SetFloat("speed", currentSpeed);
    }
}
