using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

public class SkeletonSpawnerController : MonoBehaviour
{
    public int initialSkeletonsPerWave = 5;
    public int currentSkeletonsPerWave;

    public float spawnDelay = 0.05f;

    public int currentWave = 0;
    public float waveCooldown = 10.0f;

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentSkeletonsAlive;

    public GameObject skeletonPrefab;

    private void Start()
    {
        currentSkeletonsPerWave = initialSkeletonsPerWave;

        StartNextWave();


    }
    private void StartNextWave()
    {
        currentSkeletonsAlive.Clear();
        currentWave++;

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentSkeletonsPerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            var skeleton = Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = skeleton.GetComponent<Enemy>();

            currentSkeletonsAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        List<Enemy> skeletonsToRemove = new List<Enemy>();
        foreach (Enemy skeleton in currentSkeletonsAlive)
        {
            if (skeleton.isDead)
            {
                skeletonsToRemove.Add(skeleton);
            }
        }


        foreach (Enemy skeleton in skeletonsToRemove)
        {
            currentSkeletonsAlive.Remove(skeleton);
        }

        skeletonsToRemove.Clear();

        if (currentSkeletonsAlive.Count == 0 && inCooldown == false)
        {
            StartCoroutine(WaveCooldown());
        }

        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }

    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;

        currentSkeletonsPerWave *= 1;

        StartNextWave();
    }
}
