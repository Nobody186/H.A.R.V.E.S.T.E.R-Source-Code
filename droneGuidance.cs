using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering;

public class droneGuidance : MonoBehaviour
{
    private Rigidbody rb;
    private Transform home;
    bool firingThrusters = false;

    [SerializeField] cameraShake shake;
    Transform player;

    [SerializeField] List<ParticleSystem> exhausts;
    [SerializeField] GameObject explosion;
    [SerializeField] Transform fwd;
    [SerializeField] float thrustSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float damage;
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRadius;
    [SerializeField] float viewRange;
    [SerializeField] float avoidDistance;
    [SerializeField] float maxStrayDistance;

    [SerializeField] float minPatrolDistance;
    [SerializeField] float maxPatrolDistance;

    public Transform target;

    bool patrolling;
    bool avoiding = false;
    int pointAttempts = 20;
    Vector3 patrolPoint;
    Vector3 avoidPoint;

    List<avoidancePoint> points = new List<avoidancePoint>();

    bool calculatingAcceleration = false;
    Vector3 targetAcceleration = Vector3.zero;
    Vector3 myAcceleration = Vector3.zero;

    void Start()
    {
        home = transform.parent;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.maxLinearVelocity = 250f;
        player = shake.transform;
    }

    void Update()
    {
        if (!calculatingAcceleration)
        {
            StartCoroutine(computeAcceleration()); //Compute acceleration of stuff all the time.
        }
        LookForTarget(); //A function to handle target searching.
        checkAvoidance(); //A function that SHOULD override all other movement to prevent collisions.
        if (!firingThrusters) //Cosmetic effect.
        {
            StartCoroutine(shootThrusters());
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void LookForTarget()
    {
        if (target == null)
        {
            int possibleEnemies = 0;
            Collider[] potentialTargets = Physics.OverlapSphere(transform.position, viewRange);
            foreach (Collider Target in potentialTargets)
            {
                if (Target.gameObject.name.Contains("Player") || Target.gameObject.name.Contains("Enemy"))
                {
                    target = Target.transform;
                    possibleEnemies++;
                }
            }
            if (possibleEnemies == 0)
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        if (!patrolling)
        {
            for (int i = 0; i < pointAttempts; i++)
            {
                Vector3 possiblePoint = transform.position + (Random.onUnitSphere * Random.Range(minPatrolDistance, maxPatrolDistance));
                Collider[] overlapCheck = Physics.OverlapSphere(possiblePoint, 10f);
                if (!(overlapCheck.Length > 0))
                {
                    if (Vector3.Distance(home.position, possiblePoint) <= maxStrayDistance)
                    {
                        patrolPoint = possiblePoint;
                        patrolling = true;
                        break;
                    }
                }
            }
        }
        else
        {
            if (!avoiding)
            {
                Vector3 vectorToTarget = (patrolPoint - transform.position);
                rb.linearDamping = (1f - Mathf.Clamp01(Vector3.Dot(rb.linearVelocity.normalized, vectorToTarget))) * 2f;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(vectorToTarget), Time.deltaTime * rotSpeed);
                if (Vector3.Distance(patrolPoint, transform.position) <= 50f)
                {
                    patrolling = false;
                }
            }
        }
    }

    void checkAvoidance()
    {
        points.Clear();

        RaycastHit hit;
        if (!avoiding && Physics.Raycast(fwd.position, fwd.forward, out hit, avoidDistance)) //If we need to begin an avoidance.
        {
            avoiding = true;
            for (int i = 0; i < pointAttempts; i++) //Create a bunch of possible points.
            {
                float range = 0f;
                Vector3 possiblePoint = transform.position + (Random.onUnitSphere * 50f);

                if (Physics.OverlapSphere(possiblePoint, 20f).Length > 0)
                {
                    continue;
                }

                if (target != null)
                {
                    range = Vector3.Distance(avoidPoint, target.position);
                }
                else range = 1f;
                avoidancePoint avPoint = new avoidancePoint();
                avPoint.point = possiblePoint;
                avPoint.range = range;
                points.Add(avPoint);
            }

            float prevScore = 0f;

            for (int i = 0; i < points.Count; i++)
            {
                float score = 0f; //Higher score, the better the point.
                //We want an avoidance point that gets us closest to where our target is currently going, and will also align us with the target.
                score = Vector3.Distance(PredictedTargetPoint(), points[i].point);
                Vector3 vectorToAvoidPoint = points[i].point - transform.position;
                Vector3 vectorToTarget = PredictedTargetPoint() - transform.position;
                score += Vector3.Dot(vectorToAvoidPoint, vectorToTarget) * 500f;
                if (score >= prevScore)
                {
                    avoidPoint = points[i].point;
                    prevScore = score;
                }
            }
        }
        if (avoiding)
        {
            Vector3 vectorToTarget = avoidPoint - transform.position;
            rb.linearDamping = (1f - Mathf.Clamp01(Vector3.Dot(rb.linearVelocity.normalized, vectorToTarget))) * 3f;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(vectorToTarget), Time.deltaTime * rotSpeed);
        }
        if (!Physics.Raycast(fwd.position, fwd.forward, out hit, avoidDistance))
        {
            avoiding = false;
        }
    }

    void Move()
    {
        if (!avoiding)
        {
            if (target != null)
            {
                Vector3 vectorToTarget = PredictedTargetPoint() - transform.position;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(vectorToTarget), Time.fixedDeltaTime * rotSpeed);
                rb.linearDamping = (1f - Mathf.Clamp01(Vector3.Dot(rb.linearVelocity.normalized, vectorToTarget))) * 4f;
            }
        }
        rb.AddRelativeForce(0, 0, thrustSpeed * Time.fixedDeltaTime);
    }

    Vector3 PredictedTargetPoint()
    {
        if (target == null) return transform.position;

        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetPos = target.position;
        Vector3 targetVel = targetRb.linearVelocity;

        float missileSpeed = rb.linearVelocity.magnitude;

        if (missileSpeed < 0.01f) return targetPos; //Prevent a division by 0.

        //This is unfortunately vibe coded. I did not have the formal education to solve this problem.
        Vector3 predictedPos = targetPos;
        for (int i = 0; i < 5; i++)
        {
            float dist = Vector3.Distance(transform.position, predictedPos);
            float a = myAcceleration.magnitude * 0.5f;
            float v = missileSpeed;

            float timeToReach;
            if (a < 0.01f)
            {
                timeToReach = dist / v; // Simple linear time estimate
            }
            else
            {
                float discriminant = v * v + 4 * a * dist;
                timeToReach = (-v + Mathf.Sqrt(discriminant)) / (2 * a);
            }

            predictedPos = targetPos + targetVel * timeToReach + 0.5f * targetAcceleration * timeToReach * timeToReach;
        }
        return predictedPos;
    }

    IEnumerator computeAcceleration()
    {
        calculatingAcceleration = true;
        Vector3 oldVelocity = rb.linearVelocity;
        Vector3 oldTargetVelocity = Vector3.zero;
        if (target != null)
        {
            oldTargetVelocity = target.GetComponent<Rigidbody>().linearVelocity;
        }
        yield return new WaitForEndOfFrame();
        myAcceleration = (rb.linearVelocity - oldVelocity) / Time.deltaTime;
        if (target != null)
        {
            targetAcceleration = (target.GetComponent<Rigidbody>().linearVelocity - oldTargetVelocity) / Time.deltaTime;
        }
        calculatingAcceleration = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.name.Contains("Redactor") && !collision.gameObject.name.Contains("Drone") && !collision.gameObject.name.Contains("STATION"))
        {
            StartCoroutine(beginExplosion(collision));
        }
    }

    IEnumerator shootThrusters()
    {
        firingThrusters = true;
        for (int i = 0; i < exhausts.Count; i++)
        {
            int num = Random.Range(0, 2);
            if (num == 1 && !exhausts[i].isEmitting)
            {
                exhausts[i].Play(); //Firing thruster
                yield return new WaitForSeconds(0.005f);
            }
            else if (num == 0 && exhausts[i].isPlaying)
            {
                exhausts[i].Stop(true, ParticleSystemStopBehavior.StopEmitting); //Stopping thruster
                yield return new WaitForSeconds(0.005f);
            }
        }
        firingThrusters = false;
    }

    IEnumerator beginExplosion(Collider collider)
    {
        float distance = Vector3.Distance(collider.transform.position, transform.position);
        float distanceLastFrame = distance;
        bool readyToExplode = false;

        while (!readyToExplode)
        {
            if (collider == null) //If our target dies while we're about to detonate, then just explode too.
            {
                Explode();
            }
            distance = Vector3.Distance(collider.transform.position, transform.position);
            if (distance > distanceLastFrame)
            {
                readyToExplode = true;
                Explode();
            }
            else
            {
                distanceLastFrame = distance;
                yield return new WaitForEndOfFrame();
            }
        }

    }

    void Explode()
    {
        GameObject boom = Instantiate(explosion, transform.position, Quaternion.identity);
        boom.SetActive(true);
        shake.shakeFactor = (100f / Vector3.Distance(transform.position, player.position));
        shake.shakeTime = 1f;
        shake.StartCoroutine(shake.SHAKE());
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < hitObjects.Length; i++)
        {
            Health playerHealth = hitObjects[i].transform.gameObject.GetComponent<Health>();
            EnemyHealth enemHealth = hitObjects[i].transform.gameObject.GetComponent<EnemyHealth>();
            Mineable rockHealth = hitObjects[i].transform.gameObject.GetComponent<Mineable>();
            Rigidbody rigidbody = hitObjects[i].transform.gameObject.GetComponent<Rigidbody>();
            float Distance = Vector3.Distance(transform.position, hitObjects[i].transform.position);

            if (playerHealth != null)
            {
                playerHealth.health -= Mathf.Round(damage / Distance);
            }
            if (enemHealth != null)
            {
                enemHealth.health -= damage / Distance;
            }
            if (rockHealth != null)
            {
                rockHealth.health -= damage / Distance;
            }
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        Destroy(gameObject);
    }
}
public class avoidancePoint
{
    public Vector3 point = Vector3.zero;
    public float range = 0f;
}