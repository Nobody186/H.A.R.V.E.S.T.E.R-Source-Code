using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class shipSpawner : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] ConsoleController console;
    [SerializeField] GameObject minerShip;
    [SerializeField] GameObject pirateShip;
    [SerializeField] GameObject missileShip;
    [SerializeField] float minSpawnTime, maxSpawnTime;
    [SerializeField] float minSpawnRadius, maxSpawnRadius;
    [SerializeField] int minSpawnChance, maxSpawnChance; //The closer the 2 numbers are to 1, the more likely for a hostile to spawn.
    [SerializeField] bool pirateSpawn;
    [SerializeField] float saturationAmount = 25;
    [SerializeField] float interferenceLimit = 50f;
    [SerializeField] float minDistanceBetweenShips = 800f; // NEW: Prevent clustering
    [SerializeField] int maxSpawnAttempts = 20; // NEW: Limit spawn attempts

    private List<GameObject> childrenShips = new List<GameObject>();
    [SerializeField] Transform Player;
    private bool canSpawn = false;

    private Vector3 spawnPosition;

    float timer = 0f;
    float spawnTime = 0f;
    bool preparingASpawn = false;
    bool hostileShip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < saturationAmount + (console.day * 5); i++)
        {
            // Try multiple times to find a good spawn position
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                Vector3 spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);

                // Check environment collision
                Collider[] ColliderCollection = Physics.OverlapSphere(spawnPos, interferenceLimit);
                if (ColliderCollection.Length >= 1)
                {
                    continue; // Try again
                }

                // Check camera view
                if (Vector3.Dot(cam.transform.forward, spawnPos) >= 0)
                {
                    continue; // Try again
                }

                // NEW: Check distance from existing ships to prevent clustering
                bool tooCloseToOtherShip = false;
                foreach (GameObject existingShip in childrenShips)
                {
                    if (existingShip != null)
                    {
                        float distanceToExisting = Vector3.Distance(spawnPos, existingShip.transform.position);
                        if (distanceToExisting < minDistanceBetweenShips)
                        {
                            tooCloseToOtherShip = true;
                            break;
                        }
                    }
                }

                if (tooCloseToOtherShip)
                {
                    continue; // Try again
                }

                // Found a good position, spawn the ship
                canSpawn = true;
                break;
            }

            if (canSpawn)
            {
                float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                Vector3 spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);

                // Re-validate the final position (using the last valid position from loop)
                for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                {
                    spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                    spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);

                    Collider[] ColliderCollection = Physics.OverlapSphere(spawnPos, interferenceLimit);
                    if (ColliderCollection.Length < 1 && Vector3.Dot(cam.transform.forward, spawnPos) < 0)
                    {
                        bool tooClose = false;
                        foreach (GameObject existingShip in childrenShips)
                        {
                            if (existingShip != null && Vector3.Distance(spawnPos, existingShip.transform.position) < minDistanceBetweenShips)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (!tooClose) break;
                    }
                }

                if (hostileShip)
                {
                    int shipType = Random.Range(1, 3);
                    if (shipType == 1 || !pirateSpawn)
                    {
                        GameObject newShip = Instantiate(missileShip, spawnPos, Quaternion.identity);
                        newShip.transform.parent = transform;
                        childrenShips.Add(newShip);
                        newShip.SetActive(true);
                        print(gameObject.name + " SPAWNED A MISSILE SHIP!");
                    }
                    else
                    {
                        GameObject newShip = Instantiate(pirateShip, spawnPos, Quaternion.identity);
                        newShip.transform.parent = transform;
                        childrenShips.Add(newShip);
                        newShip.SetActive(true);
                        print(gameObject.name + " SPAWNED A PIRATE SHIP!");
                    }
                }
                else
                {
                    GameObject newShip = Instantiate(minerShip, spawnPos, Quaternion.identity, null);
                    childrenShips.Add(newShip);
                    newShip.transform.parent = transform;
                    newShip.SetActive(true);
                    print(gameObject.name + " SPAWNED A NEUTRAL SHIP!");
                }

                // Determine next ship type - MOVED THIS TO AFTER SPAWNING for better randomization
                int hostileSpawnChance = Random.Range(minSpawnChance, maxSpawnChance);
                if (hostileSpawnChance == 1)
                {
                    hostileShip = true;
                }
                else
                {
                    hostileShip = false;
                }
                canSpawn = false;
            }
        }

        // Set initial spawn timer
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = Vector3.Distance(transform.position, Player.position);

        // Clean up null references
        childrenShips.RemoveAll(ship => ship == null);

        // Handle ship activation/deactivation based on distance
        if (Distance < 15000f && childrenShips.Count > 0)
        {
            // Check if any ships are inactive and activate them
            bool anyInactive = false;
            foreach (GameObject ship in childrenShips)
            {
                if (ship != null && !ship.activeSelf)
                {
                    anyInactive = true;
                    break;
                }
            }

            if (anyInactive)
            {
                StartCoroutine(loadShips());
            }
        }
        else if (Distance > 15000f)
        {
            foreach (GameObject obj in childrenShips)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // Handle continuous spawning when player is in range
        if (Distance < 15000f)
        {
            timer += Time.deltaTime;

            if (preparingASpawn)
            {
                if (timer >= spawnTime)
                {
                    // Try to spawn a new ship
                    Vector3 validSpawnPos = Vector3.zero;
                    bool foundValidPosition = false;

                    for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                    {
                        float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
                        Vector3 spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);

                        Collider[] ColliderCollection = Physics.OverlapSphere(spawnPos, interferenceLimit);
                        if (ColliderCollection.Length >= 1) continue;

                        if (Vector3.Dot(cam.transform.forward, spawnPos) >= 0) continue;

                        // Check distance from existing ships
                        bool tooClose = false;
                        foreach (GameObject existingShip in childrenShips)
                        {
                            if (existingShip != null && Vector3.Distance(spawnPos, existingShip.transform.position) < minDistanceBetweenShips)
                            {
                                tooClose = true;
                                break;
                            }
                        }

                        if (!tooClose)
                        {
                            validSpawnPos = spawnPos;
                            foundValidPosition = true;
                            break;
                        }
                    }

                    if (foundValidPosition)
                    {
                        if (hostileShip)
                        {
                            int shipType = Random.Range(1, 3);
                            if (shipType == 1 || !pirateSpawn)
                            {
                                GameObject newShip = Instantiate(missileShip, validSpawnPos, Quaternion.identity);
                                childrenShips.Add(newShip);
                                newShip.transform.parent = transform;
                                newShip.SetActive(true);
                                print(gameObject.name + " SPAWNED A MISSILE SHIP!");
                            }
                            else
                            {
                                GameObject newShip = Instantiate(pirateShip, validSpawnPos, Quaternion.identity);
                                childrenShips.Add(newShip);
                                newShip.transform.parent = transform;
                                newShip.SetActive(true);
                                print(gameObject.name + " SPAWNED A PIRATE SHIP!");
                            }
                        }
                        else
                        {
                            GameObject newShip = Instantiate(minerShip, validSpawnPos, Quaternion.identity, null);
                            childrenShips.Add(newShip);
                            newShip.transform.parent = transform;
                            newShip.SetActive(true);
                            print(gameObject.name + " SPAWNED A NEUTRAL SHIP!");
                        }

                        preparingASpawn = false;
                    }
                    else
                    {
                        // Couldn't find valid position, try again later
                        timer = 0;
                        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
                    }
                }
            }
            else
            {
                timer = 0;
                spawnTime = Random.Range(minSpawnTime, maxSpawnTime);

                // Determine if next spawn should be hostile - INCREASED HOSTILE CHANCE
                int hostileSpawnChance = Random.Range(minSpawnChance, maxSpawnChance);
                if (hostileSpawnChance == 1)
                {
                    hostileShip = true;
                }
                else
                {
                    hostileShip = false;
                }
                preparingASpawn = true;
            }
        }
    }

    IEnumerator loadShips()
    {
        foreach (GameObject obj in childrenShips)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}