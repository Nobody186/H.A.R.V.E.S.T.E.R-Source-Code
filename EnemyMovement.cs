using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //The nightmare begins by creating a bunch of states for our state machine
    public bool hostile;
    public bool mining;
    public bool harvesting;
    public bool avoiding;
    public bool pathfinding;
    public bool chase;
    public bool sneaking;
    public bool ramming;
    public bool patrol;
    public bool enRoute = false; //For patrol purposes only
    public bool attacking = false;
    //Numbers we can tweak to modify behavior.
    [SerializeField] float attackCooldown = 8f;

    [SerializeField] float rotSpeed = 10f;
    [SerializeField] float posSpeed = 10f;

    [SerializeField] float chaseRotSpeed = 10f;
    [SerializeField] float chasePosSpeed = 10f;

    [SerializeField] float avoidRange = 20f;
    [SerializeField] float asteroidScanDistance = 100f;
    //A reference we need for raycasting
    [SerializeField] Transform pathfinder;
    //References we need for immersion.
    [SerializeField] GameObject Missile;
    [SerializeField] Transform missileRack;
    [SerializeField] Transform Player;
    [SerializeField] ConsoleController console;
    [SerializeField] GunController gun;
    [SerializeField] cameraShake camShake;

    [SerializeField] GameObject RWRLight;
    [SerializeField] AudioSource RWRSound;
    [SerializeField] AudioSource gunDeploySound;
    [SerializeField] AudioSource gunFireSound;
    [SerializeField] List<ParticleSystem> muzzleFlash;
    //Some stuff for some pathfinding 
    private float distance = 0f;
    public Vector3 patrolPosition;
    public Vector3 previousPatrolPoint;
    //Some variables to further influence our states
    [SerializeField] float viewRange = 100f;
    [SerializeField] float chaseViewRange = 500f;
    [SerializeField] float camoViewRange = 25f;
    [SerializeField] float engageDistance = 50f;
    [SerializeField] float stopDistance = 10f;
    [SerializeField] float ambushDistance = 180f;
    [SerializeField] float gunRange = 200f;
    private float patrolDistance;
    //duh
    private Rigidbody rb;

    //Stuff for different ship variants
    [SerializeField] GameObject TurretBase;
    [SerializeField] GameObject TurretBase2;
    [SerializeField] GameObject TurretSphere;
    [SerializeField] GameObject TurretSphere2;
    [SerializeField] Transform laserLookAt;
    [SerializeField] ParticleSystem laserEmission;
    [SerializeField] List<AudioSource> gunImpacts;
    [SerializeField] Renderer mainRenderer;

    [SerializeField] List<ParticleSystem> exhausts;
    [SerializeField] ParticleSystem mainThruster; //For ships with 1 main engine
    [SerializeField] List<ParticleSystem> mainThrusters; //For ships with multiple main engines
    [SerializeField] bool isDrone; //Different characteristics for different ships
    [SerializeField] bool isDisc;

    [SerializeField] Animator gunAnimator;
    //We use this stuff for obstacle avoidance.
    private List<Vector3> possibleRoutes;
    private List<float> ranges;
    public Vector3 BestRoute;
    Vector3 direction;

    public Vector3 Target;
    public Vector3 secondaryTarget; //A reminder of the main objective during obstacle avoidance.
    public Vector3 temporaryTarget; //1: We only use this to handle the avoidance logic, and make sure we dont look for a new route every frame. Ctrl + F for more details.
    
    Vector3 vectorToTarget;
    Quaternion directionToTarget;

    private float defaultViewRange;

    GameObject closestAsteroid; //Note: This is not used for obstacle avoidance. This is for miner ships.
    GameObject prevAsteroid;

    bool firingThrusters = false;
    //Stuff for enemy encounters
    [SerializeField] GameObject threatPointer;
    [SerializeField] Transform threatPos;
    [SerializeField] EnemyHealth enemHealth;
    private GameObject refPointer;

    [SerializeField] GameObject AlertText;
    [SerializeField] Transform canvas;

    bool firstFrameAttack = false;

    //OPTIMIZATION VARIABLES
    private float updateTimer = 0f;
    private float updateInterval;
    private bool isVisible;
    private float visibilityCheckTimer = 0f;
    private const float visibilityCheckInterval = 0.8f; //Check for camouflage 1 time per 0.8 seconds
    private float asteroidScanTimer = 0f;
    private const float asteroidScanInterval = 1f; //Scan for asteroids once per second
    private float stateCheckTimer = 0f;
    private const float stateCheckInterval = 0.2f; //The interval in which we check the bot's current state
    private const float avoidanceCheckInterval = 0.7f; //Look for obstacles every 0.7 seconds.
    private float avoidanceCheckTimer = 0f;

    //Cached references
    private Camera mainCamera;
    private static readonly int maxRaycastsPerFrame = 10; //Limited amount of raycasts for asteroid scanning
    private int currentRaycastIndex = 0;
    private List<Vector3> scanDirections;

    void Awake()
    {
        //Stagger update intervals to spread CPU load
        updateInterval = Random.Range(0.02f, 0.05f);

        if (hostile)
        {
            patrol = true; //This automatically causes problems if the enemy spawns close to the player. Luckily, that isn't possible.
        }
        else if (!hostile)
        {
            mining = true;
        }

        defaultViewRange = viewRange;

        Target = Vector3.zero;
        secondaryTarget = Vector3.zero;
        temporaryTarget = Vector3.zero;

        closestAsteroid = null;
        prevAsteroid = null;

        if (gunAnimator != null)
        {
            gunAnimator.enabled = false;
        }

        rb = gameObject.GetComponent<Rigidbody>();
        possibleRoutes = new List<Vector3>();
        ranges = new List<float>();
        BestRoute = Vector3.zero;

        // Cache main camera reference
        mainCamera = Camera.main;

        //We used to generate a bunch of pathfinding vectors every frame. Instead, we now generate the directions for pathfinding at the start, and cache them for later.
        scanDirections = new List<Vector3>();
        for (int i = 0; i < 100; i++)
        {
            scanDirections.Add(Random.onUnitSphere * asteroidScanDistance);
        }
    }

    void Update()
    {
        // Staggered updates - not every enemy updates every frame
        updateTimer += Time.deltaTime;
        if (updateTimer < updateInterval)
        {
            return;
        }
        updateTimer = 0f;

        //See if we're visible. Our complex polygons don't need to be rendered if we're not.
        CheckVisibility();

        //If the player is camouflaged, then we gotta reduce how far we can see.
        UpdateViewRange();

        //Check if we're dead
        if (enemHealth.health <= 0)
        {
            HandleDeath();
            return;
        }

        //Core movement logic. All it says is to move towards the target, and play your particle/sound effects.
        HandleMovement();

        //State management (less frequent)
        stateCheckTimer += updateInterval;
        if (stateCheckTimer >= stateCheckInterval)
        {
            stateCheckTimer = 0f;
            UpdateAIState();
        }

        //Avoidance management (least frequent). The logic here is that if there is an obstacle, we have some time to point in a new direction before we try to generate a new path.
        avoidanceCheckTimer += updateInterval;
        if(avoidanceCheckTimer >= avoidanceCheckInterval)
        {
            //We shoot a raycast to make sure we aren't doing anything stupid. This sets the avoiding bool.
            HandleObstacleAvoidance();
            avoidanceCheckTimer = 0f;
        }

        //Behavior execution
        ExecuteCurrentBehavior();
    }

    private void CheckVisibility()
    {
        visibilityCheckTimer += updateInterval;
        if (visibilityCheckTimer >= visibilityCheckInterval)
        {
            visibilityCheckTimer = 0f;

            if (Vector3.Distance(Player.position, transform.position) < 800f && mainCamera != null)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
                Bounds bounds = mainRenderer.bounds;
                isVisible = GeometryUtility.TestPlanesAABB(planes, bounds);
            }
            else
            {
                isVisible = false;
            }
        }
    }

    private void UpdateViewRange()
    {
        if (console.isCamo)
        {
            viewRange = camoViewRange;
        }
        else
        {
            if (chase)
            {
                viewRange = chaseViewRange;
            }
            else if (sneaking)
            {
                viewRange = defaultViewRange + 2000f;
            }
            else
            {
                viewRange = defaultViewRange;
            }
        }
    }

    private void HandleDeath()
    {
        console.beingAttacked = false;
        if (isDisc)
        {
            console.beingPirateAttacked = false;
        }
        if (refPointer != null)
        {
            Destroy(refPointer);
        }
    }

    private void HandleMovement() //Note: We are always moving towards the target. The secondary target is not a place to move to, it's used for comparisons.
    {
        //Get us some numbers we need to move the ship.
        if (Target != Vector3.zero)
        {
            distance = (transform.position - Target).magnitude;

            vectorToTarget = (Target - transform.position).normalized; //We use this stuff to know where to go.
            directionToTarget = Quaternion.LookRotation(vectorToTarget, Vector3.up);

            if (isDrone && !firingThrusters && !gameObject.name.Contains("Mining"))
            {
                StartCoroutine(shootThrusters());
            }
        }

        //We move.
        if (distance > stopDistance)
        {
            rb.linearDamping = 1f - (Mathf.Clamp01(Vector3.Dot(transform.forward, vectorToTarget))) * 5f; //This should keep our linear damping at 1 when we're on target, and make it higher when we're not (so we can slow down and adjust)
            if (chase)
            {
                rb.AddRelativeForce(0f, 0f, chasePosSpeed * updateInterval);
            }
            else
            {
                rb.AddRelativeForce(0f, 0f, posSpeed * updateInterval);
            }

            if (isVisible && isDrone && !mainThruster.isPlaying)
            {
                mainThruster.Play();
            }
            else if (isVisible && isDisc && !mainThrusters[2].isPlaying && !sneaking)
            {
                for (int i = 0; i < mainThrusters.Count; i++)
                {
                    mainThrusters[i].Play();
                }
            }
        }

        //Clamp damping
        if (rb.linearDamping < 0.8f)
        {
            rb.linearDamping = 0.8f;
        }

        //Rotation
        if(avoiding)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, directionToTarget, updateInterval * chaseRotSpeed*2f); //If we have an obstacle, we gotta SWERVE!
        }
        if (chase)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, directionToTarget, updateInterval * chaseRotSpeed);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, directionToTarget, updateInterval * rotSpeed);
        }

        //Stop movement when close
        if (distance < stopDistance)
        {
            if (isDrone)
            {
                mainThruster.Stop();
            }
            rb.linearDamping = 3f;
        }
    }

    private void HandleObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(pathfinder.position, pathfinder.forward, out hit, avoidRange)) //If there's something in front of us and it's not the player, get out of the way.
        {
            if (hit.transform.name != "PlayerShip" && hit.transform.position != Target && !hit.transform.gameObject.GetComponent<Collider>().isTrigger)
            {
                print(gameObject.name + " HAS DETECTED A " + hit.transform.name + "! AVOIDANCE SYSTEM ACTIVATED!");
                avoiding = true;

                if (Vector3.Distance(secondaryTarget, Vector3.zero) < 1f) //If haven't cached our current target yet, do so.
                {
                    print("SETTING SECONDARY TARGET TO TARGET!");
                    secondaryTarget = Target; //First thing we do is store the target we originally wanted to go to.
                }
                print(gameObject.name + " IS FETCHING AN AVOIDANCE ROUTE!");
                findAvoidanceRoute();
            }
        }
        else
        {
            print(gameObject.name + " REPORTS: MY VIEW IS CLEAR!");
            if (secondaryTarget != Vector3.zero) //If there's nothing ahead of us, but we're still avoiding (secondaryTarget is always Vector3.zero if not avoiding), then get back on task and turn off the avoiding state.
            {
                print(gameObject.name + " HAS EXITED ITS AVOIDANCE MODE!");
                Target = secondaryTarget;
                secondaryTarget = Vector3.zero;
                avoiding = false;
                BestRoute = Vector3.zero;
                temporaryTarget = Vector3.zero; //4: Rest our temporary target for future use.
            }
        }
    }

    private void UpdateAIState() //Based on what we can see, we gotta set our states.
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (isDrone)
        {
            if (distanceToPlayer < viewRange && hostile)
            {
                patrol = false;
                chase = true;
            }
            else if (distanceToPlayer > viewRange && hostile && !chase)
            {
                patrol = true;
            }
            else if (!hostile)
            {
                mining = true;
            }
        }
        else if (isDisc)
        {
            // If we are further than our ambushDistance but closer than our stalkDistance, then turn on the Sneak state.
            if (distanceToPlayer > ambushDistance && distanceToPlayer <= viewRange && !chase)
            {
                patrol = false;
                chase = false;
                sneaking = true;
            }
            if (distanceToPlayer < ambushDistance)
            {
                patrol = false;
                sneaking = false;
                chase = true;
            }
            // If player escapes the viewRange, then give up the fight
            else if (distanceToPlayer > viewRange && !chase)
            {
                patrol = true;
                sneaking = false;
            }
        }
    }

    private void ExecuteCurrentBehavior() //Based on our state, we do stuff.
    {
        if (patrol)
        {
            ExecutePatrol();
        }
        else if (chase)
        {
            ExecuteChase();
        }
        else if (sneaking)
        {
            ExecuteSneaking();
        }
        else if (mining)
        {
            ExecuteMining();
        }
    }

    private void ExecutePatrol()
    {
        float graceDistance = 100f;//How close the ship can be to the patrol point until we switch
        rb.linearDamping = 0.8f;
        if (!enRoute) //If we're not currently going anywhere, find somewhere to go.
        {
            patrolDistance = Random.Range(0f, Vector3.Distance(transform.position, Player.position));
            if (avoiding)
            {
                Vector3 possiblePoint = transform.position + (Random.onUnitSphere * patrolDistance);

                if (Vector3.Distance(Player.position, transform.position) > Vector3.Distance(Player.position, possiblePoint)) //If the possible point gets us closer to the player than we currently are, we accept it.
                {
                    secondaryTarget = possiblePoint;
                    enRoute = true;
                }
            }
            else
            {
                Vector3 possiblePoint = transform.position + (Random.onUnitSphere * patrolDistance);

                if (Vector3.Distance(Player.position, transform.position) > Vector3.Distance(Player.position, possiblePoint)) //If the possible point gets us closer to the player than we currently are, we accept it.
                {
                    Target = possiblePoint;
                    enRoute = true;
                }
            }
        }
        else //If we are going somewhere...
        {
            if (!avoiding)
            {
                if (Vector3.Distance(transform.position, Target) <= graceDistance) //If we reach our lcation, turn enroute false. (This will result in a loop to the code above, where we find a new point to go.)
                {
                    enRoute = false;
                    return;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, secondaryTarget) <= graceDistance)
                {
                    enRoute = false;
                    return;
                }
            }
        }
    }

    private void ExecuteChase()
    {
        if (isDisc && enemHealth.health > 0) //This allows us to play a pirate voice line through the console script
        {
            console.beingAmbushed = false;
            console.beingPirateAttacked = true;
        }

        if (!avoiding)
        {
            Target = Player.position;
        }
        else
        {
            secondaryTarget = Player.position;
        }

        enRoute = false;

        if (!console.isCamo)
        {
            viewRange = chaseViewRange;
        }

        if (Vector3.Distance(transform.position, Player.position) < engageDistance && !attacking)
        {
            if (refPointer == null && enemHealth.health > 0)
            {
                refPointer = Instantiate(threatPointer, threatPos);
            }
            if (!firstFrameAttack)
            {
                GameObject alert = Instantiate(AlertText, canvas);
                alert.SetActive(true);
                Destroy(alert, 3f);
                firstFrameAttack = true;
            }
            StartCoroutine(ATTACK());
        }

        if (enemHealth.health <= 0)
        {
            if (refPointer != null)
                Destroy(refPointer);
            console.beingAttacked = false;
            if (isDisc)
            {
                console.beingPirateAttacked = false;
            }
        }

        if (refPointer != null && enemHealth.health > 0 && !isVisible)
        {
            refPointer.SetActive(true);
            refPointer.transform.LookAt(transform);
        }
        else if (refPointer != null)
        {
            refPointer.SetActive(false);
        }

        // When the player escapes...
        if (Vector3.Distance(transform.position, Player.position) > viewRange)
        {
            firstFrameAttack = false;
            if (refPointer != null)
                refPointer.SetActive(false);

            if (!console.isCamo)
            {
                viewRange = defaultViewRange;
            }
            else
            {
                viewRange = camoViewRange;
            }

            previousPatrolPoint = Vector3.zero;
            if (isDisc)
            {
                console.beingPirateAttacked = false;
            }
            chase = false;
            console.beingAttacked = false;
        }
    }

    private void ExecuteSneaking()
    {
        console.beingAmbushed = true;
        if (!console.isCamo)
        {
            viewRange = defaultViewRange + 2000f;
        }
        else
        {
            viewRange = camoViewRange;
        }

        findHiddenPoint();

        if (mainThrusters != null && mainThrusters.Count > 0 && mainThrusters[0].isPlaying)
        {
            for (int i = 0; i < mainThrusters.Count; i++)
            {
                mainThrusters[i].Stop();
            }
        }
    }

    private void ExecuteMining()
    {
        if ((Target == Vector3.zero && !avoiding) || (secondaryTarget == Vector3.zero && avoiding) || closestAsteroid == null) //If we don't have a target.
        {
            //Do a scan.
            asteroidScanTimer += updateInterval;
            if (asteroidScanTimer >= asteroidScanInterval)
            {
                asteroidScanTimer = 0f;
                ScanForAsteroidsOptimized();
            }
        }

        if (closestAsteroid != null && (!avoiding && Target != Vector3.zero|| avoiding && secondaryTarget != Vector3.zero)) //If we know the closest asteroid.
        {
            if (avoiding)
            {
                secondaryTarget = closestAsteroid.transform.position;
            }
            else
            {
                Target = closestAsteroid.transform.position;
            }

            if (Target != Vector3.zero && distance < stopDistance + 40f) //If we have a target and we've reached it...
            {
                RaycastHit hit2;
                if (!Physics.Raycast(pathfinder.position, vectorToTarget, out hit2, stopDistance + 40f) && harvesting) //Let's check if it's still there. If not, get out.
                {
                    Target = Vector3.zero;
                    secondaryTarget = Vector3.zero;
                    harvesting = false;
                    laserEmission.Stop();
                    closestAsteroid = null;
                }
            }

            if (distance <= stopDistance + 40f && Target != Vector3.zero && !avoiding) //Start mining if we're close to the target.
            {
                Vector3 localPos = transform.InverseTransformPoint(Target);

                if (localPos.y >= 0)
                {
                    harvesting = true;
                    laserEmission.gameObject.transform.position = TurretSphere.transform.position;
                    TurretSphere.transform.LookAt(Target);
                    laserEmission.Play();
                }
                else if (localPos.y < 0)
                {
                    harvesting = true;
                    laserEmission.gameObject.transform.position = TurretSphere2.transform.position;
                    TurretSphere2.transform.LookAt(Target);
                    laserEmission.Play();
                }
                laserEmission.gameObject.transform.LookAt(Target);
                ParticleSystem.MainModule temp = laserEmission.main;
                temp.startLifetime = distance / 693.15f; //This magic number makes sure the laser only goes as far as the target is.
            }
            else
            {
                harvesting = false;
                laserEmission.Stop();
            }
        }
    }

    private void ScanForAsteroidsOptimized() //This is for the mining bots to know about their surroundings.
    {
        laserEmission.Stop();

        int raycastsThisFrame = 0;
        while (raycastsThisFrame < maxRaycastsPerFrame && currentRaycastIndex < scanDirections.Count)
        {
            Vector3 scanDirection = scanDirections[currentRaycastIndex];
            RaycastHit hit2;

            if (Physics.Raycast(pathfinder.position, scanDirection, out hit2, asteroidScanDistance))
            {
                if (hit2.transform.name.Contains("Asteroid"))
                {
                    if (prevAsteroid != null && Vector3.Distance(hit2.transform.position, transform.position) < Vector3.Distance(prevAsteroid.transform.position, transform.position)) //If we find an asteroid that's closer than our previous one, USE IT!
                    {
                        closestAsteroid = hit2.transform.gameObject;
                        prevAsteroid = closestAsteroid;
                    }
                    else
                    {
                        closestAsteroid = hit2.transform.gameObject; //We only run this if we don't have a "previous asteroid" reference
                        prevAsteroid = closestAsteroid;
                    }
                }
            }

            currentRaycastIndex++;
            raycastsThisFrame++;
        }

        //Reset index when we've completed a full scan
        if (currentRaycastIndex >= scanDirections.Count)
        {
            currentRaycastIndex = 0;
        }

        if (closestAsteroid != null)
        {
            if (avoiding)
            {
                secondaryTarget = closestAsteroid.transform.position;
            }
            else
            {
                Target = closestAsteroid.transform.position; //Set our targets.
            }
        }
    }

    void findAvoidanceRoute() //This is only for pathfinding purposes. This will give us a temporary target that moves us away from danger.
    {
        pathfinding = true;
        if (possibleRoutes != null)
        {
            possibleRoutes.Clear();
            ranges.Clear();
        }
        BestRoute = Vector3.zero;

        //Generate a bunch of possible directions to go
        for (int i = 0; i < 100; i++)
        {
            direction = Random.onUnitSphere;
            //if (Vector3.Dot(gameObject.transform.forward, direction) < 0f) //Make sure we keep moving forward (in a 180 degree cone).
            //{
                //direction = -direction;
            //}
            RaycastHit hit;
            bool theHit = Physics.Raycast(pathfinder.position, direction, out hit, avoidRange+20f); //Shoot our raycasts as far as we need, with a little bit of grace.

            possibleRoutes.Add(direction);
            ranges.Add(theHit ? hit.distance : avoidRange+20f); //We will use these ranges to find the least obstructed path.

        }

        float previousScore = 0f;

        for (int i = 0; i < possibleRoutes.Count; i++)
        {
            float alignment = Vector3.Dot(gameObject.transform.forward, possibleRoutes[i]); //Closer to 1 for routes that are close to where we're already pointing.
            float deviation = Vector3.Distance(transform.position + possibleRoutes[i]*avoidRange, secondaryTarget); //We want a point that still keeps us focused on where we ACTUALLY want to go.
            float score = alignment * ranges[i] * (500 / deviation); //This gives better routes a higher score, with emphasis on low obscurity and ease of access.
            if (score > previousScore)
            {
                BestRoute = possibleRoutes[i];
                previousScore = score;
            }
        }
        Target = (BestRoute.normalized * 50f) + transform.position;
        temporaryTarget = Target; //2: We need something to compare Target to for reference. Look below
        pathfinding = false;
    }

    private IEnumerator ATTACK()
    {
        attacking = true;
        console.beingAttacked = true;

        if (isDrone)
        {
            GameObject spawnedMissile = Instantiate(Missile, missileRack.position, Quaternion.identity);
            spawnedMissile.SetActive(true);
            console.lightToFlash = RWRLight;
            console.warnToPlay = RWRSound;
            StartCoroutine(console.flashLight());
        }
        else if (isDisc)
        {
            float originalChasePosSpeed = chasePosSpeed;
            float originalChaseRotSpeed = chaseRotSpeed;

            chasePosSpeed = 80f;
            chaseRotSpeed = 5f;

            float gunDuration = 0f;
            float gunDeployTime = 3.375f;
            float gunFireTime = gunFireSound.clip.length;
            gunAnimator.enabled = true;
            gunAnimator.SetTrigger("Deploy");
            gunDeploySound.Play();
            yield return new WaitForSeconds(gunDeployTime);
            gunAnimator.SetTrigger("Firing");
            gunDeploySound.Stop();
            gunFireSound.Play();
            for (int i = 0; i < muzzleFlash.Count; i++)
            {
                muzzleFlash[i].Play();
            }
            while (gunDuration < gunFireTime)
            {
                gunAnimator.SetTrigger("Firing");
                float tickTime = Random.Range(0.2f, 0.625f);
                gunDuration += Time.deltaTime;
                RaycastHit hit;
                if (Physics.Raycast(pathfinder.position, pathfinder.forward, out hit, gunRange))
                {
                    Health playerHealth = hit.transform.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        camShake.shakeFactor = 1;
                        StartCoroutine(camShake.SHAKE());
                        gunImpacts[Random.Range(0, gunImpacts.Count)].Play();
                        playerHealth.health -= 5f;
                    }
                }
                yield return new WaitForSeconds(tickTime);
                gunDuration += tickTime;
            }
            for (int i = 0; i < muzzleFlash.Count; i++)
            {
                muzzleFlash[i].Stop();
            }
            chasePosSpeed = originalChasePosSpeed;
            chaseRotSpeed = originalChaseRotSpeed;
            gunFireSound.Stop();
            gunAnimator.SetTrigger("Retract");
            yield return new WaitForSeconds(gunDeployTime);
            gunAnimator.enabled = false;
        }

        yield return new WaitForSeconds(attackCooldown);
        attacking = false;
        yield return null;
    }

    void findHiddenPoint()
    {
        if (Vector3.Distance(Target, Player.position) > viewRange || distance < 60f)
        {
            Vector3 idealPoint = Vector3.zero;
            Vector3 prevPoint = Vector3.zero;

            //Step 1: Find closest asteroid to player
            float distanceToAsteroid;
            Collider[] ColliderCollection = Physics.OverlapSphere(Player.position, 200f);
            for (int i = 0; i < ColliderCollection.Length; i++)
            {
                if (ColliderCollection[i].gameObject.name.Contains("Asteroid"))
                {
                    distanceToAsteroid = Vector3.Distance(transform.position, ColliderCollection[i].transform.position);
                    if (prevAsteroid != null)
                    {
                        if (distanceToAsteroid < Vector3.Distance(transform.position, prevAsteroid.transform.position))
                        {
                            closestAsteroid = ColliderCollection[i].transform.gameObject;
                            prevAsteroid = closestAsteroid;
                        }
                    }
                    else
                    {
                        closestAsteroid = ColliderCollection[i].transform.gameObject;
                        prevAsteroid = closestAsteroid;
                    }
                }
            }

            if (closestAsteroid == null) //If there are no asteroids, then just go for the player and exit this function.
            {
                if (!avoiding)
                {
                    Target = Player.position;
                }
                else if (avoiding)
                {
                    secondaryTarget = Player.position;
                }
                return;
            }

            //Step 2: Find a point near that asteroid (above avoidance range) that is the furthest from the player
            for (int i = 0; i < 50; i++)
            {
                idealPoint = closestAsteroid.transform.position + (Random.onUnitSphere * 140f);
                //Make sure we are not moving to another asteroid.
                Collider[] sphereAroundPoint = Physics.OverlapSphere(idealPoint, 150f);
                if (sphereAroundPoint.Length < 1)
                {
                    float distanceToPlayer = Vector3.Distance(idealPoint, Player.position);
                    if ((prevPoint != Vector3.zero) && (Vector3.Distance(prevPoint, Player.position) < distanceToPlayer)) //If our previous point is closer to the player than our new (ideal) point, then set our previous point to the ideal point.
                    {
                        prevPoint = idealPoint;
                    }
                    else if (prevPoint != Vector3.zero && (Vector3.Distance(prevPoint, Player.position) > distanceToPlayer)) //Dont really know why we have this here exactly
                    {
                        idealPoint = prevPoint;
                    }
                    else if (prevPoint == Vector3.zero)
                    {
                        prevPoint = idealPoint;
                    }
                }
            }
            closestAsteroid = null;
            prevAsteroid = null;
            if (prevPoint != Vector3.zero)
            {
                if (!avoiding)
                {
                    Target = prevPoint;
                }
                else if (avoiding)
                {
                    secondaryTarget = prevPoint;
                }
            }
            else
            {
                if (!avoiding)
                {
                    Target = Player.position;
                }
                else if (avoiding)
                {
                    secondaryTarget = Player.position;
                }
            }
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
                exhausts[i].Play();
                print("FIRING THRUSTER " + exhausts[i].name);
                yield return new WaitForSeconds(0.005f);
            }
            else if (num == 0 && exhausts[i].isPlaying)
            {
                print("STOPPING THRUSTER " + exhausts[i].name);
                exhausts[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
                yield return new WaitForSeconds(0.005f);
            }
        }
        firingThrusters = false;
    }
}
