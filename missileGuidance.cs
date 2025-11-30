using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class missileGuidance : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] ConsoleController console;
    [SerializeField] cameraShake shake;
    [SerializeField] Renderer missileRenderer;
    [SerializeField] Transform Player;
    [SerializeField] GunController gun;
    [SerializeField] float baseRotSpeed = 0.9f;
    [SerializeField] float baseDamage = 1000f;
    [SerializeField] float baseThrusterForce;
    [SerializeField] float baseThrustTime;

    private float rotSpeed = 3f;
    private float damage = 380f;
    private float thrusterForce = 6500;
    private float thrustTime = 3.5f;

    [SerializeField] ParticleSystem exhaust;
    [SerializeField] GameObject explosionEffect;
    Rigidbody rb;

    [SerializeField] bool isPlayerMissile;

    float distance;
    float previousDistance;

    private float timer = 0f;
    private GameObject lastKnownPosition;

    bool gonnaImpact = false;
    bool exploding;

    [SerializeField] StoryManager storyManager;
    [SerializeField] AudioSource engineNoise;
    [SerializeField] AudioSource tutorialLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastKnownPosition = new GameObject();
        lastKnownPosition.AddComponent<Rigidbody>();
        if (isPlayerMissile)
        {
            thrustTime = 2.5f + console.missileTimeLvl;
            thrusterForce = 2000 + (500f * console.missileThrustLvl);
            damage = 200f + (console.missileDamageLvl * 200f);
            rotSpeed = 0.8f + (console.missileTurnLvl / 2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!isPlayerMissile)
        {

            distance = Vector3.Distance(gameObject.transform.position, Player.position);
            Vector3 dirToPlayer = Player.position - gameObject.transform.position;
            Quaternion dirLookAt = Quaternion.LookRotation(dirToPlayer);
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, dirLookAt, baseRotSpeed * Time.deltaTime);
            if (timer < thrustTime)
            {
                rb.AddRelativeForce(0f, 0f, baseThrusterForce * Time.deltaTime);
                if (!exhaust.isPlaying)
                {
                    exhaust.Play();
                }
            }
            else
            {
                exhaust.Stop();
            }
            if (distance <= 10f && gonnaImpact == false)
            {
                gonnaImpact = true;
                previousDistance = distance;
            }
            if (gonnaImpact == true)
            {
                if (distance > previousDistance && !exploding)
                {
                    Explode();
                }
                else
                {
                    previousDistance = distance;
                }
            }

            if (timer >= 15)
            {
                Explode();
            }
        }

        else
        {
            if (gun.Target != null)
            {
                distance = Vector3.Distance(gameObject.transform.position, gun.Target.transform.position);
                Vector3 dirToPlayer = gun.Target.transform.position - gameObject.transform.position;
                Quaternion dirLookAt = Quaternion.LookRotation(dirToPlayer);
                gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, dirLookAt, rotSpeed * Time.deltaTime);
                lastKnownPosition.transform.position = gun.Target.transform.position;
                lastKnownPosition.GetComponent<Rigidbody>().linearVelocity = gun.Target.GetComponent<Rigidbody>().linearVelocity;
            }
            else
            {
                distance = Vector3.Distance(gameObject.transform.position, lastKnownPosition.transform.position);
                Vector3 dirToPlayer = lastKnownPosition.transform.position - gameObject.transform.position;
                Quaternion dirLookAt = Quaternion.LookRotation(dirToPlayer);
            }
            if (timer < thrustTime)
            {
                rb.AddRelativeForce(0f, 0f, thrusterForce * Time.deltaTime);
                if (!exhaust.isPlaying)
                {
                    exhaust.Play();
                }
            }
            else
            {
                exhaust.Stop();
            }
            if (distance <= 10f && gonnaImpact == false)
            {
                gonnaImpact = true;
                previousDistance = distance;
            }
            if (gonnaImpact == true)
            {
                if (distance > previousDistance && !exploding)
                {
                    Explode();
                }
                else
                {
                    previousDistance = distance;
                }
            }

            if (timer >= 15)
            {
                Explode();
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (timer >= 2f && console.day != 0)
        {
            Explode();
        }
        if(collider.gameObject.name == "PlayerShip")
        {
            Explode();
        }
    }

    void Explode()
    {
        if (!exploding)
        {
            exploding = true;
            missileRenderer.enabled = false;
            shake.shakeFactor = 0.5f;
            StartCoroutine(shake.SHAKE());
            if (!isPlayerMissile)
            {
                if (console.day != 0)
                {
                    if (distance < 70)
                    {
                        health.health -= Mathf.Round(damage / distance);
                    }
                }
                else
                {
                    health.health = 15;
                }
            }
            else
            {
                Collider[] collection = Physics.OverlapSphere(transform.position, 150f);
                foreach(Collider thing in collection)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, thing.transform.position);
                    Mineable asteroidScript = thing.gameObject.GetComponent<Mineable>();
                    EnemyHealth enemyScript = thing.gameObject.GetComponent<EnemyHealth>();
                    if(asteroidScript != null)
                    {
                        asteroidScript.health -= Mathf.Round(damage / distanceToTarget);
                    }
                    if(enemyScript != null)
                    {
                        enemyScript.health -= Mathf.Round(damage / distanceToTarget);
                    }
                    Rigidbody RB = thing.gameObject.GetComponent<Rigidbody>();
                    if(RB != null)
                    {
                        RB.AddExplosionForce(2f, transform.position, 8f);
                    }
                }
            }
            GameObject explosion = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            explosion.SetActive(true);
            engineNoise.Stop();
            explosion.GetComponent<ParticleSystem>().Play(true);
            if (console.day != 0)
            {
                Destroy(gameObject, 1.2f);
            }
            else
            {
                List<string> caps = new List<string>();
                List<float> times = new List<float>();
                StartCoroutine(storyManager.playNextStep(tutorialLine, true, "[E]\nTO TOGGLE PAL MODE", true, "DESTROY THE BOT", KeyCode.E, false, caps, times));
            }
        }
    }
}
