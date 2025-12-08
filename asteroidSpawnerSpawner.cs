using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class asteroidSpawnerSpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> spawners;
    [SerializeField] float minSpawnRadius, maxSpawnRadius;
    [SerializeField] float saturationAmount;
    [SerializeField] float interferenceRadius;

    private GameObject spawnerRef;
    private bool canSpawn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnerRef = spawners[Random.Range(0, spawners.Count)];
        for (int i = 0; i < saturationAmount; i++)
        {
            float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);
            Collider[] ColliderCollection = Physics.OverlapSphere(spawnPos, interferenceRadius);
            if (ColliderCollection.Length < 1)
            {
                canSpawn = true;
            }
            else
            {
                canSpawn = false;
            }
            if (canSpawn)
            {
                GameObject newSpawn = Instantiate(spawnerRef, spawnPos, Quaternion.identity);
                newSpawn.transform.parent = transform;
                spawnerRef.SetActive(true);
                spawnerRef = spawners[Random.Range(0, spawners.Count)];
                canSpawn = false;
            }
        }
    }

}
