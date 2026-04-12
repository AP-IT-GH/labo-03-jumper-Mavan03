using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject fatalObstaclePrefab;
    public GameObject groundBonusPrefab;
    public GameObject flyingBonusPrefab;

    public bool spawnFatalObstacle = true;
    public bool spawnGroundBonus = false;
    public bool spawnFlyingBonus = false;

    public float minSpawnDelay = 2.5f;
    public float maxSpawnDelay = 4.0f;

    public float minSpeed = 5f;
    public float maxSpeed = 7f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        // lijst van wat mag spawnen
        List<GameObject> allowedPrefabs = new List<GameObject>();

        if (spawnFatalObstacle) allowedPrefabs.Add(fatalObstaclePrefab);
        if (spawnGroundBonus) allowedPrefabs.Add(groundBonusPrefab);
        if (spawnFlyingBonus) allowedPrefabs.Add(flyingBonusPrefab);

        // niks aangeduid niet spawnen
        if (allowedPrefabs.Count == 0) return;

        // random 1 kiezen
        int randomIndex = Random.Range(0, allowedPrefabs.Count);
        GameObject prefabToSpawn = allowedPrefabs[randomIndex];

        Vector3 spawnPosition = transform.position;

        if (prefabToSpawn == flyingBonusPrefab)
        {
            spawnPosition.y += 2.5f;
        }

        // spawnen als child van de TrainingArea zodat ze automatisch worden opgeruimd aan het einde van een episode
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, transform.rotation, transform.parent);

        ObstacleMover mover = spawnedObject.GetComponent<ObstacleMover>();
        if (mover != null)
        {
            mover.speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}