using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public Transform player;
    public float followRange = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public float patrolRadius = 20f;

    public int damageAmount = 30;

    // -------- VIDA --------
    public int maxHealth = 200;
    private int currentHealth;

    // -------- AUDIO --------
    public AudioClip roarClip;
    public AudioClip attackClip;
    public AudioClip superRoarClip;
    public AudioClip deathClip;

    public float roarInterval = 6f;
    public float superRoarInterval = 40f;
    public float maxHearingDistance = 100f;

    private float nextRoarTime = 0f;
    private float nextSuperRoarTime = 0f;

    private AudioSource roarSource;
    private AudioSource attackSource;
    private AudioSource superRoarSource;
    private AudioSource deathSource;

    private NavMeshAgent agent;
    private Animator anim;
    private float nextAttackTime = 0f;

    private PlayerHealth playerHealth;

    private Vector3 patrolTarget;
    private bool hasPatrolTarget = false;

    // -------- SCREEN SHAKE --------
    private CameraShake camShake;
    public float shakeDistance = 20f;


    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        camShake = Camera.main.GetComponent<CameraShake>();

        nextSuperRoarTime = Time.time + superRoarInterval;

        // AUDIO SOURCES
        roarSource = gameObject.AddComponent<AudioSource>();
        attackSource = gameObject.AddComponent<AudioSource>();
        superRoarSource = gameObject.AddComponent<AudioSource>();
        deathSource = gameObject.AddComponent<AudioSource>();

        ConfigureAudio(roarSource, roarClip, maxHearingDistance);
        ConfigureAudio(attackSource, attackClip, maxHearingDistance);
        ConfigureAudio(superRoarSource, superRoarClip, 200f);
        ConfigureAudio(deathSource, deathClip, 200f);

        // PLAYER
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        agent.updateRotation = false;
    }

    void ConfigureAudio(AudioSource src, AudioClip clip, float maxDist)
    {
        src.clip = clip;
        src.spatialBlend = 1f;
        src.playOnAwake = false;
        src.loop = false;
        src.maxDistance = maxDist;
        src.volume = 1f;
    }

    void Update()
    {
        if (currentHealth <= 0)
            return;

        if (player == null || playerHealth == null || playerHealth.isDead)
        {
            IdleOnPlayerDeath();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        HandleCameraShake(dist);
        HandleSuperRoar();
        HandleRoar(dist);

        if (dist <= attackRange)
        {
            StopNormalRoar();
            AttackPlayer();
            return;
        }

        if (dist <= followRange)
        {
            ChasePlayer();
            return;
        }

        Patrol();
    }

    void IdleOnPlayerDeath()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);
    }

    // -------- CAMERA SHAKE --------
    void HandleCameraShake(float dist)
    {
        if (dist <= shakeDistance)
        {
            float intensity = Mathf.Clamp01((shakeDistance - dist) / shakeDistance);
            camShake.Shake(intensity * 0.4f, 0.1f);
        }
    }

    // -------- ROAR --------
    void HandleRoar(float distance)
    {
        if (Time.time >= nextRoarTime && roarClip != null)
        {
            if (!agent.isStopped)
                roarSource.Play();

            nextRoarTime = Time.time + roarInterval;
        }

        roarSource.volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
    }

    void StopNormalRoar()
    {
        if (roarSource.isPlaying)
            roarSource.Stop();
    }

    // -------- SUPER ROAR --------
    void HandleSuperRoar()
    {
        if (Time.time >= nextSuperRoarTime && superRoarClip != null)
        {
            superRoarSource.PlayOneShot(superRoarClip);
            nextSuperRoarTime = Time.time + superRoarInterval;
        }
    }

    // -------- ATTACK --------
    void AttackPlayer()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);
        LookAtPlayer();

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;

            if (attackClip != null)
                attackSource.PlayOneShot(attackClip);

            if (playerHealth != null && !playerHealth.isDead)
                playerHealth.TakeDamage(damageAmount);
        }
    }

    // -------- CHASE --------
    void ChasePlayer()
    {
        hasPatrolTarget = false;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        LookAtPlayer();

        anim.SetFloat("Speed", agent.desiredVelocity.magnitude > 0.2f ? 1 : 0);
    }

    // -------- PATROL --------
    void Patrol()
    {
        if (!hasPatrolTarget || Vector3.Distance(transform.position, patrolTarget) < 2f)
        {
            patrolTarget = GetRandomPoint();
            hasPatrolTarget = true;
        }

        agent.isStopped = false;
        agent.SetDestination(patrolTarget);

        anim.SetFloat("Speed", agent.desiredVelocity.magnitude > 0.2f ? 1 : 0);
    }

    Vector3 GetRandomPoint()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, patrolRadius, NavMesh.AllAreas);
        return hit.position;
    }

    void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
    }

    // =====================================================
    //                    RECIBIR DAÑO
    // =====================================================
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Monster hit! HP: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    // -------- MUERTE --------
    void Die()
    {
        Debug.Log("Monster_Mutant murió");

        agent.isStopped = true;
        agent.enabled = false;
        this.enabled = false;

        anim.SetTrigger("Die");

        if (deathClip)
            deathSource.PlayOneShot(deathClip);

        StartCoroutine(FadeAfterDeath());
    }

    IEnumerator FadeAfterDeath()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeAndDisappear());
    }

    IEnumerator FadeAndDisappear()
    {
        SkinnedMeshRenderer[] meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        float duration = 3f;
        float t = 0f;

        Material[] mats = new Material[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            mats[i] = meshes[i].material;
            mats[i].SetFloat("_Surface", 1);
            mats[i].SetFloat("_ZWrite", 0);
            mats[i].EnableKeyword("_ALPHABLEND_ON");
            mats[i].renderQueue = 3000;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);

            foreach (var m in mats)
            {
                Color c = m.color;
                c.a = alpha;
                m.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
