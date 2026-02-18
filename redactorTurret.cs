using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using System.Linq;

public class redactorTurret : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform aimThing;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] float damage = 35f;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float hitForce = 100f;

    [SerializeField] float engageDistance = 650f;

    float staggeredUpdateInterval;
    private float timer = 0f;
    private float fireRateTimer = 0f;

    private List<Vector3> shootDirections = new List<Vector3>();
    private float distance = 0f;

    [SerializeField] GameObject bigExplosion;
    [SerializeField] GameObject smallExplosion;
    [SerializeField] AudioSource fireSfx;
    [SerializeField] ParticleSystem fireVfx;
    [SerializeField] AudioSource impactOnPlayer;
    [SerializeField] GameObject impactOnEnvironment;

    [SerializeField] cameraShake shaker;
    private Transform player;

    void Start()
    { 
        staggeredUpdateInterval = Random.Range(0.1f, 0.9f);
        fireRate += Random.Range(-0.2f, 0.2f);
        player = shaker.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        fireRateTimer += Time.deltaTime;
        if(timer >= staggeredUpdateInterval) 
        {
            timer = 0f;
            SearchForTarget();
        }
        if(fireRateTimer >= fireRate && target != null)
        {
            Attack();
        }

        if (target != null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position, transform.up), Time.deltaTime*rotationSpeed);
        }
    }

    void SearchForTarget()
    {
        if (target == null || Vector3.Distance(target.position, aimThing.position) > engageDistance)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            target = null;
            distance = engageDistance;
            List<GameObject> potentialTargets = new List<GameObject>();
            Collider[] PotentialColliders = Physics.OverlapSphere(aimThing.position, engageDistance, ~ignoreLayer);
            for(int i = 0; i < PotentialColliders.Count(); i++)
            {
                if(Vector3.Dot(aimThing.forward, (PotentialColliders[i].transform.position - aimThing.position).normalized) > 0.3f && !PotentialColliders[i].transform.gameObject.name.Contains("small"))
                {
                    potentialTargets.Add(PotentialColliders[i].transform.gameObject);
                }
            }
            for(int i = 0; i < potentialTargets.Count(); i++)
            {
                if(Vector3.Distance(aimThing.position, potentialTargets[i].transform.position) <= distance && potentialTargets[i].GetComponent<Rigidbody>() != null)
                {
                    target = potentialTargets[i].transform;
                }
            }
        }
    }

    void Attack()
    {
        fireSfx.Play();
        fireVfx.Play();

        fireRateTimer = 0f;
        RaycastHit hit;

        if (Physics.SphereCast(aimThing.position, 1.5f, aimThing.forward, out hit, Mathf.Infinity, ~ignoreLayer))
        {
            print(hit.transform.gameObject.name);
            float distance = Vector3.Distance(hit.transform.position, player.position);
            if(distance <= 500f)
            {
                shaker.shakeFactor = (-1/500f * distance)+1f;
                StartCoroutine(shaker.SHAKE());
            }

            if (hit.transform.gameObject.GetComponent<Rigidbody>())
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce((-hit.normal * hitForce));
            }

            if (hit.transform.gameObject.GetComponent<Health>())
            {
                shaker.shakeFactor = 0.5f;
                StartCoroutine(shaker.SHAKE());
                impactOnPlayer.Play();
                hit.transform.gameObject.GetComponent<Health>().health -= damage;
            }
            else
            {
                GameObject destroyMeNOW = Instantiate(impactOnEnvironment, hit.transform.position, Quaternion.identity);
                destroyMeNOW.SetActive(true);
                Destroy(destroyMeNOW, 1f);
            }
            if (hit.transform.gameObject.GetComponent<EnemyHealth>())
            {
                hit.transform.gameObject.GetComponent<EnemyHealth>().health -= damage;
            }
            else if (hit.transform.gameObject.name.Contains("Asteroid"))
            {
                if (hit.transform.gameObject.GetComponent<Mineable>() != null)
                {
                    hit.transform.gameObject.GetComponent<Mineable>().health -= damage;
                }
                else
                {
                    if (hit.transform.gameObject.name.Contains("big"))
                    {
                        GameObject explosion = Instantiate(bigExplosion, hit.transform.position, Quaternion.identity);
                        explosion.SetActive(true);
                        Destroy(hit.transform.gameObject);
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        GameObject explosion = Instantiate(smallExplosion, hit.transform.position, Quaternion.identity);
                        explosion.SetActive(true);
                        Destroy(hit.transform.gameObject);
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
    }
}
