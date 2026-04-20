using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject fatalObstaclePrefab;
    public GameObject groundBonusPrefab;
    public GameObject flyingBonusPrefab;

    public bool spawnFatalObstacle = true;
    public bool spawnGroundBonus = true;
    public bool spawnFlyingBonus = true;

    public float spawnDelay = 3f;

    public float minSpeed = 5f;
    public float maxSpeed = 10f;
    private List<GameObject> spawnBag = new List<GameObject>();
    private float timer;

    private float currentBagSpeed;

    void Start()
    {
        RefillAndShuffleBag();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            SpawnNextFromBag();
            timer = 0f;
        }
    }

    void SpawnNextFromBag()
    {
        if (spawnBag.Count == 0)
        {
            RefillAndShuffleBag();
            if (spawnBag.Count == 0) return;
        }

        GameObject nextPrefab = spawnBag[0];
        spawnBag.RemoveAt(0);

        GameObject newObj = Instantiate(nextPrefab, transform.position, transform.rotation);
        newObj.transform.SetParent(transform.parent);

        if (nextPrefab == flyingBonusPrefab)
        {
            newObj.transform.position += new Vector3(0, 2.5f, 0);
        }

        ObstacleMover mover = newObj.GetComponent<ObstacleMover>();
        if (mover != null)
        {
            mover.speed = currentBagSpeed;
        }
    }

    void RefillAndShuffleBag()
    {
        if (spawnFatalObstacle) spawnBag.Add(fatalObstaclePrefab);
        if (spawnGroundBonus) spawnBag.Add(groundBonusPrefab);
        if (spawnFlyingBonus) spawnBag.Add(flyingBonusPrefab);

        for (int i = 0; i < spawnBag.Count; i++)
        {
            GameObject temp = spawnBag[i];
            int randomIndex = Random.Range(i, spawnBag.Count);
            spawnBag[i] = spawnBag[randomIndex];
            spawnBag[randomIndex] = temp;
        }

        currentBagSpeed = Random.Range(minSpeed, maxSpeed);
    }
}