using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class asteroidSpawner : MonoBehaviour
{
    private GameObject asteroid;
    [SerializeField] Transform Player;
    [SerializeField] List<GameObject> asteroidExamples; //The different asteroid values we can use
    [SerializeField] float minSpawnRadius, maxSpawnRadius; //How far should the asteroids be able to spawn from the origin?
    [SerializeField] Camera cam; //camera
    [SerializeField] float minSpawnTime, maxSpawnTime; //How long should it take to spawn in these asteroids?
    [SerializeField] float minMaxHeath, maxMaxHeath; //What health ranges should the asteroids be?
    [SerializeField] float minValue, maxValue;
    [SerializeField] float interferenceRadius = 50f; //How much space is needed for the asteroid to spawn?

    [SerializeField] int saturationAmount = 5000;
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool playerAttached = false;
    [SerializeField] float smallDistance;
    [SerializeField] float midDistance;
    [SerializeField] float largeDistance;

    private List<GameObject> childrenAsteroids = new List<GameObject>();

    bool waiting = false;

    private float timer = 0f;
    private float timeToSpawn = 0f;

    bool canSpawn = false;

    static readonly int baseColorID = Shader.PropertyToID("_BaseColor"); //I'm pretty sure I did this stuff while playing around with GPU instancing. Don't touch it, I'm not fully sure what it does.
    static MaterialPropertyBlock mpb;

    private void Awake()
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        if (!playerAttached)
        {
            asteroid = asteroidExamples[Random.Range(0, asteroidExamples.Count)];
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
                    float xRot = Random.Range(0, 360);
                    float yRot = Random.Range(0, 360);
                    float zRot = Random.Range(0, 360);
                    float xScale = Random.Range(1f, 1.25f);
                    float yScale = Random.Range(1f, 1.25f);
                    float zScale = Random.Range(1f, 1.25f);  //Set up a bunch of stuff to give some more variation in the asteroids


                    Quaternion Rotation = Quaternion.Euler(xRot, yRot, zRot);
                    GameObject newAsteroid = Instantiate(asteroid, spawnPos, Rotation);
                    newAsteroid.transform.parent = transform;
                    Mineable mineable = newAsteroid.GetComponent<Mineable>();
                    Rigidbody rb = newAsteroid.GetComponent<Rigidbody>();
                    float xTor = Random.Range(0, .1f);
                    float yTor = Random.Range(0, .1f);
                    float zTor = Random.Range(0, .1f);
                    if (mineable != null)
                    {
                        mineable.maxHealth = Random.Range(minMaxHeath, maxMaxHeath); //This line is probably deprecated. By default, no asteroids should be given the mineable script.
                    }
                    newAsteroid.transform.localScale = new Vector3(newAsteroid.transform.localScale.x * xScale, newAsteroid.transform.localScale.y * yScale, newAsteroid.transform.localScale.z * zScale);
                    var hsv = Random.ColorHSV(0f, 1f, 0.1f, 0.4f, 0.6f, 1f);
                    mpb.SetColor(baseColorID, hsv);
                    newAsteroid.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
                    newAsteroid.SetActive(true);
                    rb.angularVelocity = new Vector3(xTor, yTor, zTor);
                    childrenAsteroids.Add(newAsteroid);
                    asteroid = asteroidExamples[Random.Range(0, asteroidExamples.Count)];
                    canSpawn = false;
                    waiting = false;
                }
            }
        }
        else 
        {
            float distance = Vector3.Distance(transform.position, spawnPoint.position);
            if (distance > smallDistance && distance < midDistance)
            {
                asteroid = asteroidExamples[Random.Range(0, 5)]; //0 to 5 will always include the small asteroids
            }
            else if (distance > midDistance && distance < largeDistance)
            {
                asteroid = asteroidExamples[Random.Range(0, 11)]; // This includes the medium sized asteroids
            }
            else if (distance > largeDistance)
            {
                asteroid = asteroidExamples[Random.Range(0, 17)]; //This includes the large asteroids. 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = Vector3.Distance(transform.position, Player.position);
        if(Distance < 20000f && childrenAsteroids[0].activeSelf == false)
        {
            StartCoroutine(loadAsteroids());
        }
        else if(Distance > 20000f && childrenAsteroids[childrenAsteroids.Count-1].activeSelf == true)
        {
            foreach (GameObject obj in childrenAsteroids)
            {
                obj.SetActive(false);
            }
        }
        //Commented all of this out because as of right now, we don't need to spawn asteroids after the start() method. But things may change, who knows?
        //if (!waiting)
        //{
        //    timeToSpawn = Random.Range(minSpawnTime, maxSpawnTime);
        //    timer = 0f;
        //    waiting = true;
        //}

        //if (waiting)
        //{
        //    timer += Time.deltaTime;
        //    if(timer >= timeToSpawn)
        //    {
        //        float spawnRadius = Random.Range(minSpawnRadius, maxSpawnRadius);
        //        Vector3 spawnPos = transform.position + (Random.onUnitSphere * spawnRadius);
        //        Collider[] ColliderCollection = Physics.OverlapSphere(spawnPos, interferenceRadius);
        //        if(ColliderCollection.Length < 1)
        //        {
        //            if (playerAttached)
        //            {
        //                canSpawn = true;
        //            }
        //            else
        //            {
        //                if (Vector3.Dot(cam.transform.forward, spawnPos) < 0)
        //                {
        //                    canSpawn = true;
        //                    print(gameObject.name + " CAN SPAWN AN ASTEROID!");
        //                }
        //                else
        //                {
        //                    print(gameObject.name + " IS TOO SHY!");
        //                    canSpawn = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            canSpawn = false;
        //            print(gameObject.name + " IS TOO CROWDED!");
        //        }
        //        if(canSpawn)
        //        {
        //            print("SPAWNING AN ASTEROID!");
        //            float xRot = Random.Range(0, 360);
        //            float yRot = Random.Range(0, 360);
        //            float zRot = Random.Range(0, 360);
        //            Quaternion Rotation = Quaternion.Euler(xRot, yRot, zRot);
        //            GameObject newAsteroid = Instantiate(asteroid, spawnPos, Rotation);
        //            Mineable mineable = newAsteroid.GetComponent<Mineable>();
        //            if (mineable != null)
        //            {
        //                mineable.maxHealth = Random.Range(minMaxHeath, maxMaxHeath);
        //                mineable.value = Random.Range(minValue, maxValue);
        //            }
        //            newAsteroid.SetActive(true);
        //            if (!playerAttached)
        //            {
        //                asteroid = asteroidExamples[Random.Range(0, asteroidExamples.Count)];
        //            }
        //            else
        //            {
        //                float distance = Vector3.Distance(transform.position, spawnPoint.position);
        //                if(distance > smallDistance && distance < midDistance)
        //                {
        //                    asteroid = asteroidExamples[Random.Range(0, 5)];
        //                }
        //                else if(distance > midDistance && distance < largeDistance)
        //                {
        //                    asteroid = asteroidExamples[Random.Range(0, 11)];
        //                }
        //                else if(distance > largeDistance)
        //                {
        //                    asteroid = asteroidExamples[Random.Range(0, 17)];
        //                }
        //            }
        //            canSpawn = false;
        //            waiting = false;
        //        }
        //    }
        //}
    }

    IEnumerator loadAsteroids()
    {
        int batchSize = 5;
        int i = 0;

        while (i < childrenAsteroids.Count)
        {
            for (int j = 0; j < batchSize && i < childrenAsteroids.Count; j++, i++)
            {
                if (!childrenAsteroids[i].activeSelf)
                {
                    childrenAsteroids[i].SetActive(true);
                }
            }

            // Give the GPU & CPU a breather
            yield return null;
        }
    }
}
