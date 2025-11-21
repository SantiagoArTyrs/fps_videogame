using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Rangos")]
    public float detectionRange = 20f;   // distancia donde empieza a seguir
    public float runRange = 15f;          // distancia donde empieza a correr
    public float attackRange = 3f;       // distancia donde ataca

    [Header("Daño")]
    public int damage = 20;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = 0;

    [Header("Sonidos de caminar")]
    public AudioClip[] pasos;
    public float pasoIntervalo = 0.5f;
    private float pasoTimer = 0f;

    [Header("Sonidos de ataque")]
    public AudioClip[] sonidosAtaque;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        agent.updatePosition = true;
        agent.updateRotation = true;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // ==========================================
        //                ATAQUE
        // ==========================================
        if (distance <= attackRange)
        {
            agent.isStopped = true;

            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);

            // mira al jugador
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 6f);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                DoDamage();
                lastAttackTime = Time.time;
            }

            return;
        }

        // ==========================================
        //              CORRER (RUNNING)
        // ==========================================
        if (distance <= runRange)
        {
            agent.isStopped = false;
            agent.speed = 4f;  // velocidad de correr

            animator.SetBool("Walking", false);
            animator.SetBool("Running", true);

            agent.SetDestination(player.position);

            RotateTowardsMovement();

            return;
        }

        // ==========================================
        //             CAMINAR (WALKING)
        // ==========================================
        if (distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.speed = 1.5f;  // velocidad de caminar

            animator.SetBool("Walking", true);
            animator.SetBool("Running", false);

            agent.SetDestination(player.position);

            RotateTowardsMovement();
            ReproducirPasos();

            return;
        }

        // ==========================================
        //                  IDLE
        // ==========================================
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }


    // ==================================================
    //                  ROTACIÓN SUAVE
    // ==================================================
    void RotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            direction.y = 0;

            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 6f);
        }
    }


    // ==================================================
    //                    ATAQUE
    // ==================================================
    void DoDamage()
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health != null)
            health.TakeDamage(damage);

        if (sonidosAtaque.Length > 0)
        {
            int index = Random.Range(0, sonidosAtaque.Length);
            audioSource.pitch = Random.Range(0.9f, 1.2f);
            audioSource.PlayOneShot(sonidosAtaque[index]);
        }
    }


    // ==================================================
    //            SONIDOS DE PASOS
    // ==================================================
    void ReproducirPasos()
    {
        if (!animator.GetBool("Walking")) return;
        if (pasos.Length == 0 || audioSource == null) return;

        pasoTimer += Time.deltaTime;

        if (pasoTimer >= pasoIntervalo)
        {
            int index = Random.Range(0, pasos.Length);

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(pasos[index]);

            pasoTimer = 0f;
        }
    }
}
