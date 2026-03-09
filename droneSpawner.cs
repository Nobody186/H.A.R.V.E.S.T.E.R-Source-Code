using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class droneSpawner : MonoBehaviour
{
    [SerializeField] GameObject redactor;
    [SerializeField] GameObject drone;
    [SerializeField] float spawnAmount;
    [SerializeField] float spawnCooldown;

    [SerializeField] ParticleSystem smoke;
    [SerializeField] AudioSource buzzer;

    [SerializeField] List<Transform> spawnersToShootFrom;

    float timer = 50f;

    Quaternion rotToLaunchAt;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnCooldown)
        {
            timer = 0f;
            StartCoroutine(spawnDrones());
        }
    }

    IEnumerator spawnDrones()
    {
        smoke.Play();
        buzzer.Play();
        for (int i = 0; i < spawnAmount; i++)
        {
            int spawnerToShootFrom = Random.Range(0, 3);
            rotToLaunchAt = Quaternion.LookRotation(spawnersToShootFrom[spawnerToShootFrom].forward);
            GameObject newDrone = Instantiate(drone, spawnersToShootFrom[spawnerToShootFrom].position, rotToLaunchAt, redactor.GetComponent<RedactorPathfinder>().home);
            newDrone.SetActive(true);
            newDrone.GetComponent<Rigidbody>().AddForce(spawnersToShootFrom[spawnerToShootFrom].forward*200f);
            yield return new WaitForSeconds(1f);
        }
        smoke.Stop();
    }
}
