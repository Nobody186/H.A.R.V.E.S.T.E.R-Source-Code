using UnityEngine;
using System.Collections.Generic;
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

    [SerializeField] Transform normal;
    [SerializeField] bool upsideDown = false;

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

        if (target != null && Vector3.Dot(normal.forward, (target.position - normal.position).normalized) <= 0.4f)
        {
            target = null;
        }

        if (fireRateTimer >= fireRate && target != null)
        {
            Attack();
        }

        if (target != null)
        {
            if(!upsideDown) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position, transform.up), Time.deltaTime*rotationSpeed);
            else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-(target.position - transform.position), transform.up), Time.deltaTime * rotationSpeed);
        }
        if (!upsideDown) normal.localRotation = Quaternion.Euler(0f, 0f, 0f);
        else normal.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void SearchForTarget()
    {
        if (target == null || Vector3.Distance(target.position, aimThing.position) > engageDistance)
        {
            target = null;
            distance = engageDistance;
            List<GameObject> potentialTargets = new List<GameObject>();
            Collider[] PotentialColliders = Physics.OverlapSphere(normal.position, engageDistance, ~ignoreLayer);
            for(int i = 0; i < PotentialColliders.Count(); i++)
            {
                if(Vector3.Dot(normal.forward, (PotentialColliders[i].transform.position - normal.position).normalized) > 0.4f && !PotentialColliders[i].transform.gameObject.name.Contains("small") && !PotentialColliders[i].isTrigger && !PotentialColliders[i].transform.gameObject.name.Contains("BASIC"))
                {
                    potentialTargets.Add(PotentialColliders[i].transform.gameObject);
                }
            }
            for(int i = 0; i < potentialTargets.Count(); i++)
            {
                if(Vector3.Distance(aimThing.position, potentialTargets[i].transform.position) <= distance && potentialTargets[i].GetComponent<Rigidbody>() != null)
                {
                    distance = (Vector3.Distance(aimThing.position, potentialTargets[i].transform.position));
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
                    }
                    else
                    {
                        GameObject explosion = Instantiate(smallExplosion, hit.transform.position, Quaternion.identity);
                        explosion.SetActive(true);
                        Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }
}
