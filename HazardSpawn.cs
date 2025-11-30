using System.Collections;
using System.Collections.Generic;
using Unity.XR.GoogleVr;
using UnityEngine;

public class HazardSpawn : MonoBehaviour
{
    [SerializeField] ConsoleController console;

    [SerializeField] GameObject Hazard;
    [SerializeField] Transform cam;
    [SerializeField] Transform PlayerShip;
    [SerializeField] float radius;
    [SerializeField] PlayerController controller;
    [SerializeField] int difficulty; //0 = Peaceful
    [SerializeField] int asteroidSpeed = 10;

    private Vector3 PlayerLook;
    private Vector3 spawnPosition;
    private float angle;
    private Vector3 directionToPlayer;
    private float timer = 0;
    private float timeToSpawn;

    public bool asteroidLaunched = false; //Returns true the frame an asteroid is spawned and launched towards the player

    private void Start()
    {
        difficulty = console.day;
        if (difficulty != 0)
        {
            timeToSpawn = Random.Range(60f / difficulty, 180f / difficulty);
        }
        else
        {
            timeToSpawn = float.MaxValue; // Ensure no immediate spawning
        }
    }

    void Update()
    {
        if (difficulty != console.day)
        {
            difficulty = console.day;
            if (difficulty != 0)
            {
                timeToSpawn = Random.Range(60f / difficulty, 180f / difficulty);
                timer = 0f; // Reset timer when difficulty changes
            }
        }
        timer += Time.deltaTime;
        if (timer > timeToSpawn && difficulty != 0)
        {
            asteroidLaunched = true;
            spawnPosition = Random.onUnitSphere;
            PlayerLook = cam.forward;
            angle = Vector3.Dot(spawnPosition, PlayerLook);
            if (angle > 0)
            {
                spawnPosition = -spawnPosition;
            }
            spawnPosition = PlayerShip.position + spawnPosition * radius;
            directionToPlayer = PlayerShip.position - spawnPosition;
            print("Instantiating an aseteroid");
            GameObject hazard = Instantiate(Hazard, spawnPosition, Quaternion.identity);

            hazard.SetActive(true);
            if (hazard.GetComponent<Rigidbody>().linearVelocity == Vector3.zero)
            {
                Vector3 predictedPos = PlayerShip.position + controller.playerDirection * (directionToPlayer.magnitude / asteroidSpeed);
                Vector3 leadDir = (predictedPos - spawnPosition).normalized;
                hazard.GetComponent<Rigidbody>().linearVelocity = leadDir * asteroidSpeed;
                hazard.GetComponent<Rigidbody>().AddTorque(Random.Range(0, 15), Random.Range(0, 15), Random.Range(0, 15));
            }
            timer = 0f;
            timeToSpawn = Random.Range(60f / difficulty, 180f/difficulty);
        }
    }
}
