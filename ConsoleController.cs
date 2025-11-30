using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] GameObject waypointMarker;
    [SerializeField] GameObject waypointDirFinder;
    [SerializeField] TextMeshProUGUI WayPointText;
    public List<Vector3> waypointsToSave;
    [SerializeField] List<GameObject> allWaypoints; //A ref to all waypoints in game.
    public List<GameObject> waypoints;
    [SerializeField] List<GameObject> defaultWaypoints;
    public List<GameObject> earnedWaypoints;
    [SerializeField] List<Transform> normalizedWaypoints;
    public GameObject waypointToAdd;
    public int currentWaypoint;
    private float rangeToWaypoint;
    bool gotDecoded = false;

    [SerializeField] Camera cam;

    public TextMeshPro screenText;
    public TextMeshPro predictedHealth;
    public TextMeshPro angleWarner;
    public TextMeshPro shiftTimer;
    public TextMeshPro lifeClock;
    public TextMeshPro cargoValueText;
    [SerializeField] PlayerController playerController;
    [SerializeField] GunController gunController;
    [SerializeField] Health health;
    [SerializeField] HazardSpawn hazard;
    [SerializeField] AudioSource alignWarning;
    [SerializeField] AudioSource asteroidWarning;
    [SerializeField] GameObject redWarningLight1;
    [SerializeField] GameObject redWarningLight2;
    public GameObject lightToFlash; //[HIDEININSPECTOR] DID NOT WORK
    public AudioSource warnToPlay; //DO NOT TOUCH THESE IN INSPECTOR!!!!
    private float flashTime = 1.5f;
    private float timer = 0f;
    private float timer2 = 0f;

    private bool warning1;
    private bool warning2;

    public float Balance = 0;
    public float cargoValue = 0;
    public int day = 0;
    public int highestDay = 0;

    public bool navMode = false;

    public float lifeSupportDuration;
    public float hour;
    private float shiftCounter = 0f; //This is a value of seconds (0-60) that gets converted into minutes every 60 second cycle

    public float requiredOre;

    public bool deathTime = false;
    public bool canDock = false;

    [SerializeField] AudioSource oxygenWarning;
    [SerializeField] GameObject oxygenLight;
    bool warningOxygen = false;

    //Upgrade stuff
    public bool palUpgrade = false;
    public bool radar3Upgrade = false;
    public bool navUpgrade = false;
    public bool missileUpgrade = false;
    public bool dashUpgrade = false;
    public bool camoUpgrade = false;
    public bool dampDisabler = false;

    public bool usingPalUpgrade = false;
    public bool usingRadar3Upgrade = true;
    public bool usingMissileUpgrade = false;
    public bool usingDashUpgrade = false;
    public bool usingCamoUpgrade = false;
    public bool usingdampDisablerUpgrade = false;

    public int mainThrustLvl = 0;
    public int sideThrustLvl = 0;
    public int laserDmgLvl = 0;
    public int maxHealthLvl = 0;

    public int missileDamageLvl = 0;
    public int missileThrustLvl = 0;
    public int missileTimeLvl = 0;
    public int missileTurnLvl = 0;
    public int missileRearmLvl = 0;

    //Loadout Preferences

    //RepairStuff
    public int playerHealth = 100;
    public int maxPlayerHealth = 100;

    [SerializeField] int baseLaserDmg = 25;
    public int laserDamage;

    public bool beingAmbushed;
    public bool beingAttacked;
    public bool beingPirateAttacked;

    public bool usesMouseMode;
    [SerializeField] ParticleSystem warpEffect;
    [SerializeField] AudioSource warpSound;
    [SerializeField] ParticleSystem tunnel;
    [SerializeField] cameraShake shake;
    [SerializeField] GameObject warpConfirmation;
    [SerializeField] AudioSource warpConfirmSound;
    public bool canWarp = true;
    public bool isWarping = false;
    private float initialRange = 0f;

    [SerializeField] List<Cubemap> skyboxes;
    [SerializeField] Volume hdriVol;
    float idealExposure = 0;

    private List<Vector3> constructedPositions = new List<Vector3>();
    [SerializeField] Button decoderButton;

    public bool isCamo;
    public bool isSeeing;
    float maxCamoTime = 60;
    float camoTimer = 0;
    [SerializeField] AudioSource cloakSound;
    [SerializeField] float camoCooldown = 2;
    [SerializeField] Renderer frameRenderer;
    [SerializeField] Renderer seatRenderer;
    private Material frameMat;
    private Material seatMat;
    float camoLerper;
    private float camoCoolTimer = 0;
    [SerializeField] GameObject radar3Panel;
    [SerializeField] AudioSource rdr3Sound;

    public bool tutorialButton = false;
    bool playedTutMessage2 = false;
    [SerializeField] AudioSource tutorialMessage1;
    [SerializeField] AudioSource tutorialMessage2;
    [SerializeField] StoryManager storyManager;
    public bool alreadyPressed = false;
    public int tutPodNum = 1;

    public float ironHeld = 0;
    public float nickelHeld = 0;
    public float clayHeld = 0;
    public float cobaltHeld = 0;
    public float diamondHeld = 0;
    public float plutoniumHeld = 0;
    public float magnesiumHeld = 0;
    public float aluminumHeld = 0;
    public float platiniumHeld = 0;
    public float helium3Held = 0;
    public float hydrogenHeld = 0;
    public float carbonHeld = 0;
    public float iceHeld = 0;

    public float iceValue = 1.5f;
    public float hydrogenValue = 3f;
    public float clayValue = 0.4f;

    public float magnesiumValue = 2f;
    public float aluminumValue = 2.5f;
    public float ironValue = 0.8f;

    public float nickelValue = 30f;
    public float cobaltValue = 80f;
    public float carbonValue = 5f;

    public float helium3Value = 3000f;
    public float diamondValue = 15000f;
    public float platiniumValue = 25000f;
    public float plutoniumValue = 50000f;

    public float cargoHold = 0f;
    public float maxCargoHold = 1000000f;

    [SerializeField] TextMeshPro fcell;
    public int fusionCells = 10;

    public float ironQuota;
    public float nickelQuota;
    public float clayQuota;
    public float iceQuota;
    public float cobaltQuota;
    public float magnesiumQuota;
    public float aluminumQuota;
    public float hydrogenQuota;
    public float carbonQuota;
    public float helium3Quota;

    [SerializeField] monitorManager monitor;

    // Start is called before the first frame update
    void Start()
    {
        if(waypointsToSave == null)
        {
            waypointsToSave = new List<Vector3>();
        }
        waypointToAdd = new GameObject();
        for (int i = 0; i < defaultWaypoints.Count; i++)
        {
            waypoints.Add(defaultWaypoints[i]);
        }
        if(earnedWaypoints == null)
        {
            earnedWaypoints = new List<GameObject>();
        }
        for(int i = 0; i < allWaypoints.Count; i++)
        {
            for(int j = 0; j < waypointsToSave.Count; j++)
            {
                if (Vector3.Distance(waypointsToSave[j], allWaypoints[i].transform.position) <= 50)
                {
                    earnedWaypoints.Add(allWaypoints[i]);
                }
            }
        }
        if (earnedWaypoints != null)
        {
            for (int i = 0; i < earnedWaypoints.Count; i++)
            {
                waypoints.Add(earnedWaypoints[i]);
            }
        }
        health.health = playerHealth;
        if(day == 0 && SceneManager.GetActiveScene().name != "Tutorial")
        {
            day += 1;
            lifeSupportDuration = 25;
        }
        
        else if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            lifeSupportDuration = 100;
        }

        else
        {
            lifeSupportDuration = 20;
        }
        requiredOre = 250000 * day;
        laserDamage = baseLaserDmg + (25*laserDmgLvl);
        playerController.useMouseAim = usesMouseMode;


        //SET DAILY QUOTAS
        // Quota system: Max 4 quotas per day, no ultra-rare materials
        // Ultra-rare materials (plutonium, platinum, diamond) are bonus rewards only
        if (day != 0)
        {
            List<System.Action> potentialQuotas = new List<System.Action>();
            int currentQuotaCount = 0;

            // TIER 1: COMMON RESOURCES (High priority, almost always included)

            // IRON - Most reliable resource
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.95f && currentQuotaCount < 4)
                {
                    float baseIronQuota = Random.Range(80000f, 200000f);
                    ironQuota = baseIronQuota * (1f + (day * 0.15f));
                    currentQuotaCount++;
                }
            });

            // CLAY - Common alternative to iron
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.7f && currentQuotaCount < 4)
                {
                    float baseClayQuota = Random.Range(80000, 140000f);
                    clayQuota = baseClayQuota * (1f + (day * 0.12f));
                    currentQuotaCount++;
                }
            });

            // ICE - Critical survival resource
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.8f && currentQuotaCount < 4)
                {
                    float baseIceQuota = Random.Range(8000f, 15000f);
                    iceQuota = baseIceQuota * Mathf.Pow(day, 1.2f); // Grows with urgency
                    currentQuotaCount++;
                }
            });

            // TIER 2: MID-TIER RESOURCES (Moderate challenge)

            // MAGNESIUM - Requires selective asteroid choice
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.5f && currentQuotaCount < 4)
                {
                    float baseMagQuota = Random.Range(25000f, 45000f);
                    magnesiumQuota = baseMagQuota * (1f + (day * 0.2f));
                    currentQuotaCount++;
                }
            });

            // ALUMINUM - Mid-tier alternative
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.5f && currentQuotaCount < 4)
                {
                    float baseAlumQuota = Random.Range(20000f, 35000f);
                    aluminumQuota = baseAlumQuota * (1f + (day * 0.2f));
                    currentQuotaCount++;
                }
            });

            // HYDROGEN - Fuel/survival resource
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.4f && currentQuotaCount < 4)
                {
                    float baseHydroQuota = Random.Range(6000f, 12000f);
                    hydrogenQuota = baseHydroQuota * (1f + (day * 0.25f));
                    currentQuotaCount++;
                }
            });

            // CARBON - Mid-tier resource (available after day 2)
            if (day >= 2)
            {
                potentialQuotas.Add(() =>
                {
                    if (Random.value < 0.35f && currentQuotaCount < 4)
                    {
                        float baseCarbonQuota = Random.Range(1000f, 2500f);
                        carbonQuota = baseCarbonQuota * (1f + (day * 0.25f));
                        currentQuotaCount++;
                    }
                });
            }

            // TIER 3: CHALLENGING RESOURCES (Require strategy and luck)

            // COBALT - Forces players to hunt specific asteroids
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.3f && currentQuotaCount < 4)
                {
                    float baseCobaltQuota = Random.Range(2500f, 6000f);
                    cobaltQuota = baseCobaltQuota * (1f + (day * 0.3f));
                    currentQuotaCount++;
                }
            });

            // NICKEL - Selective mining required
            potentialQuotas.Add(() =>
            {
                if (Random.value < 0.25f && currentQuotaCount < 4)
                {
                    float baseNickelQuota = Random.Range(1200f, 3500f);
                    nickelQuota = baseNickelQuota * (1f + (day * 0.35f));
                    currentQuotaCount++;
                }
            });

            // HELIUM-3 - Requires max laser intensity (late game)
            if (day >= 3)
            {
                potentialQuotas.Add(() =>
                {
                    if (Random.value < 0.12f && currentQuotaCount < 4)
                    {
                        float baseHeliumQuota = Random.Range(25f, 60f);
                        helium3Quota = baseHeliumQuota * (day - 2);
                        currentQuotaCount++;
                    }
                });
            }

            // Shuffle the list to randomize quota selection order
            for (int i = 0; i < potentialQuotas.Count; i++)
            {
                System.Action temp = potentialQuotas[i];
                int randomIndex = Random.Range(i, potentialQuotas.Count);
                potentialQuotas[i] = potentialQuotas[randomIndex];
                potentialQuotas[randomIndex] = temp;
            }

            // Execute quota assignments until we have 4 or run out of options
            foreach (var quotaAction in potentialQuotas)
            {
                if (currentQuotaCount >= 4) break;
                quotaAction();
            }

            // SAFETY NET: Ensure at least 2 quotas are assigned
            if (currentQuotaCount < 2)
            {
                // Force iron quota if we don't have enough
                if (ironQuota == 0)
                {
                    float baseIronQuota = Random.Range(180000f, 350000f);
                    ironQuota = baseIronQuota * (1f + (day * 0.15f));
                    currentQuotaCount++;
                }

                // Force ice quota if we still need more
                if (currentQuotaCount < 2 && iceQuota == 0)
                {
                    float baseIceQuota = Random.Range(7000f, 12000f);
                    iceQuota = baseIceQuota * Mathf.Pow(day, 1.2f);
                    currentQuotaCount++;
                }
            }

            // BONUS REWARDS ONLY (No quotas for these ultra-rare materials):
            // - Plutonium: Bonus credits but radiation damage risk
            // - Platinum: Pure profit bonus
            // - Diamond: Massive credit bonus
            // These materials should give substantial credit rewards when found,
            // but never be required for survival
        }
        else
        {
            ironQuota = 50000;
        }

        foreach (GameObject t in waypoints)
        {
            GameObject newWaypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(newWaypointObject.GetComponent<Collider>());
            newWaypointObject.GetComponent<MeshRenderer>().enabled = false;
            normalizedWaypoints.Add(newWaypointObject.transform);
        }
        frameMat = frameRenderer.material;
        seatMat = seatRenderer.material;
    }

    public bool quotaCompleted()
    {
        bool check;
        if (day != 0)
        {
            if ((ironQuota == 0 || ironHeld >= ironQuota) && (nickelQuota == 0 || nickelHeld >= nickelQuota) && (iceQuota == 0 || iceHeld >= iceQuota) && (cobaltQuota == 0 || cobaltHeld >= cobaltQuota) && (clayQuota == 0 || clayHeld >= clayQuota) && (magnesiumQuota == 0 || magnesiumHeld >= magnesiumQuota) && (aluminumQuota == 0 || aluminumHeld >= aluminumQuota) && (hydrogenQuota == 0 || hydrogenHeld >= hydrogenQuota) && (carbonQuota == 0 || carbonHeld >= carbonQuota) && (helium3Quota == 0 || helium3Held >= helium3Quota))
            {
                check = true;
            }
            else
            {
                check = false;
            }
        }
        else
        {
            if (ironQuota >= 50000)
            {
                check = true;
            }
            else
            {
                check = false;
            }
        }
        return check;
    }

    public float calculateCargoValue()
    {
        float value = 0f;

        value += iceHeld * 1.5f;
        value += hydrogenHeld * 3f;
        value += clayHeld * 0.4f;

        value += magnesiumHeld * 2f;
        value += aluminumHeld * 2.5f;
        value += ironHeld * 0.8f;

        value += nickelHeld * 30f;
        value += cobaltHeld * 80f;
        value += carbonHeld * 5f;

        value += helium3Held * 3000f;
        value += diamondHeld * 15000f;
        value += platiniumHeld * 25000f;
        value += plutoniumHeld * 50000f;

        cargoHold = Mathf.Round(iceHeld + hydrogenHeld + clayHeld + magnesiumHeld + aluminumHeld + ironHeld + nickelHeld + cobaltHeld + carbonHeld + helium3Held + diamondHeld + platiniumHeld + plutoniumHeld);

        return Mathf.Round(value);
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = Mathf.RoundToInt(health.health);
        shiftCounter += Time.deltaTime;

        if(shiftCounter >= 60)
        {
            hour += 1;
            lifeSupportDuration -= 1;
            shiftCounter = 0;
        }
        lifeClock.text = "LF: " + Mathf.Round(lifeSupportDuration);
        if (cargoHold < maxCargoHold)
        {
            shiftTimer.text = "CARGO HOLD: " + Mathf.Round((cargoHold / maxCargoHold)*100f) + "%";
        }
        else
        {
            shiftTimer.text = "CARGO HOLD: FULL";
        }
        cargoValue = calculateCargoValue();

        if (cargoValue < 1000)
        {
            cargoValueText.text = "VALUE\n " + Mathf.Round(cargoValue/100f)*100f;
        }
        else if (cargoValue >= 1000 && cargoValue < 1000000)
        {
            cargoValueText.text = "VALUE\n " + Mathf.Round(cargoValue / 100f) * 100f / 1000f + "K";
        }
        else if(cargoValue >= 1000000)
        {
            cargoValueText.text = "VALUE\n " + Mathf.Round(cargoValue / 100f) * 100f / 1000000f + "M";
        }

        if(lifeSupportDuration <= 1f && deathTime == false)
        {
            deathTime = true;
            lifeSupportDuration = 60;
        }

        if(deathTime)
        {
            if(!warningOxygen && lifeSupportDuration <= 50f)
            {
                StartCoroutine("OxygenWarn");
            }
            if (!oxygenWarning.isPlaying && lifeSupportDuration <= 50f)
            {
                oxygenWarning.Play();
            }

            lifeSupportDuration -= Time.deltaTime;
            if(lifeSupportDuration <= 0f)
            {
                health.health = 0f;
            }
        }

        if (warpSound.isPlaying && !isWarping && canWarp == true)
        {
            warpSound.volume -= Time.deltaTime;
            if (warpSound.volume <= 0f)
            {
                warpSound.Stop();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && usingCamoUpgrade && camoTimer < maxCamoTime && camoCoolTimer >= camoCooldown)
        {
            isCamo = !isCamo;
            cloakSound.Play();
            camoCoolTimer = 0;
        }
        if(Input.GetKeyDown(KeyCode.Space) && usingRadar3Upgrade)
        {
            isSeeing = !isSeeing;
            rdr3Sound.Play();
        }
        if(isSeeing)
        {
            radar3Panel.SetActive(true);
            radar3Panel.GetComponent<Animator>().SetBool("radar3On", true);
        }
        else if (!isSeeing)
        {
            radar3Panel.GetComponent<Animator>().SetBool("radar3On", false);
        }
        if (camoCoolTimer < camoCooldown)
        {
            camoCoolTimer += Time.deltaTime;
        }
        if (isCamo)
        {
            camoTimer += Time.deltaTime;
            if (camoLerper < 1)
            {
                camoLerper += Time.deltaTime;
            }
            if (camoLerper > 1)
            {
                camoLerper = 1;
            }
            if (camoTimer >= maxCamoTime)
            {
                isCamo = false;
            }
            angleWarner.text = "CAMO LEFT:\n " + (60f-Mathf.Round(camoTimer));
            frameMat.SetFloat("_cloakFactor", camoLerper);
            seatMat.SetFloat("_cloakFactor", camoLerper);
        }
        if(!isCamo && camoLerper != 0)
        {
            camoLerper -= Time.deltaTime;
            if(camoLerper < 0)
            {
                camoLerper = 0;
            }
            frameMat.SetFloat("_cloakFactor", camoLerper);
            seatMat.SetFloat("_cloakFactor", camoLerper);
        }

        if (Input.GetKeyDown(KeyCode.M) && canWarp == true)
        {
            navMode = !navMode;
            if(Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) > 15000) //If our current waypoint is not the waypoint we are at right now,
            {
                for(int i = 0; i < waypoints.Count; i++)
                {
                    if(Vector3.Distance(transform.position, waypoints[i].transform.position) < 15000)
                    {
                        currentWaypoint = i;
                    }
                }
            }
        }

        if (!navMode)
        {
            waypointMarker.SetActive(false);
            waypointDirFinder.SetActive(false);
            gunController.enabled = true;

            if (gunController.isMining)
            {
                if (gunController.Distance > gunController.MaxRange && timer < flashTime)
                {
                    timer += Time.deltaTime;
                    screenText.text = "MAX RANGE!";
                }
                //After 1.5 seconds, show speed again, and wait another 1.5 seconds to run above if statement.
                else if (gunController.Distance > gunController.MaxRange && timer > flashTime)
                {
                    timer2 += Time.deltaTime;
                    screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
                    if (timer2 >= flashTime)
                    {
                        timer = 0f;
                        timer2 = 0f;
                    }
                }
                else
                {
                    screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
                }
            }
            else if (!gunController.safeAngle && !warning1)
            {
                warning1 = true;
                lightToFlash = redWarningLight1;
                warnToPlay = alignWarning;
                StartCoroutine("flashLight");
            }
            if (hazard.asteroidLaunched && !warning2)
            {
                hazard.asteroidLaunched = false;
                warning2 = true;
                lightToFlash = redWarningLight2;
                warnToPlay = asteroidWarning;
                StartCoroutine("flashLight");
            }
            if (gunController.Target != null)
            {
                screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
                if (gunController.Target.name.Contains("Asteroid"))
                {
                    predictedHealth.text = "PTI: " + gunController.Target.GetComponent<Mineable>().asteroidHealthToShare.ToString() + "%";
                }
                else if (gunController.Target.name.Contains("Pirate"))
                {
                    predictedHealth.text = "TARGET HULL: " + gunController.Target.GetComponent<EnemyHealth>().health;
                }
                else if(gunController.Target.name.Contains("EnemyMining"))
                {
                    predictedHealth.text = "TARGET HULL: " + gunController.Target.GetComponent<EnemyHealth>().health;
                }
                else if(gunController.Target.name.Contains("Enemy"))
                {
                    predictedHealth.text = "TARGET HULL: " + gunController.Target.GetComponent<EnemyHealth>().health;
                }
                else if(gunController.Target.name.Contains("dummy"))
                {
                    predictedHealth.text = "TARGET HULL: " + gunController.Target.GetComponent<EnemyHealth>().health;
                }
                else if(gunController.Target.name.Contains("Data"))
                {
                    predictedHealth.text = "TARGET: HRV - DCSD";
                }
                else
                {
                    predictedHealth.text = "TARGET: UNKNOWN";
                }

            }
            if (gunController.Target == null && !gunController.palAttempted)
            {
                screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
                predictedHealth.text = ("SCAN");
            }
            else if(gunController.Target == null && gunController.palAttempted)
            {
                screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
                predictedHealth.text = "PAL";
            }
            fcell.gameObject.SetActive(false);
        }
        else //IF WE ARE IN NAV MODE
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Transform real = waypoints[i].transform;
                Transform proxy = normalizedWaypoints[i];

                Vector3 dir = (real.position - transform.position).normalized;
                float displayDist = 500f;

                proxy.position = transform.position + dir * displayDist;
            }
            if (gunController.Target != null)
            {
                gunController.canMine = false;
                gunController.isMining = false;
                gunController.unLock();
            }

                if (canWarp)
                {
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
                    Bounds bounds = normalizedWaypoints[currentWaypoint].GetComponent<Renderer>().bounds;

                    bool isVisible = GeometryUtility.TestPlanesAABB(planes, bounds);

                    waypointDirFinder.SetActive(!isVisible);
                    waypointDirFinder.transform.LookAt(waypoints[currentWaypoint].transform);
                    waypointMarker.SetActive(isVisible);
                }
                else
                {
                    waypointDirFinder.SetActive(false);
                    waypointMarker.SetActive(false);
                }

                if(Input.GetKeyDown(KeyCode.Z) && warpConfirmation.activeSelf == true && !isWarping && canWarp && fusionCells >= 1)
                {
                    StartCoroutine("Warp");
                }
                else if(Input.GetKeyDown(KeyCode.Z) && warpConfirmation.activeSelf == false && !isWarping && canWarp && Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) > 10000f)
                {
                    StartCoroutine(askForWarpConfirmation());
                }
                if(!isWarping && !canWarp)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(waypoints[currentWaypoint].transform.position-transform.position), Time.deltaTime);
                }
                if(isWarping)
                {
                    if(Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position) > 500f)
                    {
                        if (!shake.isShaking)
                        {
                            shake.shakeFactor = .5f;
                            shake.shakeTime = Vector3.Distance(transform.position, waypoints[currentWaypoint].transform.position)/2000;
                            StartCoroutine(shake.SHAKE());
                        }
                        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, Time.deltaTime*3200f);
                        if(deathTime && warpSound.isPlaying)
                        {
                            warpSound.volume -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

                        for (int i = 0; i < allObjects.Length; i++)
                        {
                            if (allObjects[i].transform.parent == null && allObjects[i].transform != transform)
                            {
                                allObjects[i].transform.position = allObjects[i].transform.position + (-transform.position);
                            }
                        }
                        transform.position = Vector3.zero;
                        if (hdriVol.profile.TryGet<HDRISky>(out var hdriSky))
                        {
                            hdriSky.hdriSky.Override(skyboxes[currentWaypoint]);

                            switch(currentWaypoint)
                            {
                                case 0:
                                    idealExposure = -4.2f;
                                    break;
                                case 1:
                                    idealExposure = -4.2f;
                                    break;
                                case 2:
                                    idealExposure = -1.26f;
                                    break;
                                case 3:
                                    idealExposure = -3.8f;
                                    break;
                                case 5:
                                    idealExposure = -3.8f;
                                    break;
                                default:
                                    break;
                            }
                            // hdriSky.exposure.Override(1f);

                            // Force sky update
                            var hdPipeline = RenderPipelineManager.currentPipeline as HDRenderPipeline;
                            hdPipeline.RequestSkyEnvironmentUpdate();
                        }
                        StartCoroutine(exposureLerp());
                        tunnel.Stop();
                        fusionCells -= 1;
                        shake.shakeTime = 1f;
                        StartCoroutine(monitor.closeMonitor());
                        navMode = false;
                        playerController.canMove = true;
                        isWarping = false;
                        canWarp = true;
                    }
                    
                }
                if(warpSound.isPlaying && !isWarping && canWarp == true)
                {
                    warpSound.volume -= Time.deltaTime;
                    if(warpSound.volume <= 0f)
                    {
                        warpSound.Stop();
                    }
                }

                if (Input.GetKeyDown(KeyCode.RightBracket) && canWarp)
                {
                    currentWaypoint += 1;
                    if (currentWaypoint >= waypoints.Count)
                    {
                        currentWaypoint = 0;
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftBracket) && canWarp)
                {
                    currentWaypoint -= 1;
                    if (currentWaypoint < 0)
                    {
                        currentWaypoint = waypoints.Count - 1;
                    }
                }

                rangeToWaypoint = Vector3.Distance(gameObject.transform.position, waypoints[currentWaypoint].transform.position);
                predictedHealth.text = "NAV: " + currentWaypoint;
                fcell.gameObject.SetActive(true);
                fcell.text = "F-CELLS:\n" + fusionCells;
                Vector3 VectorToTarget = (waypoints[currentWaypoint].transform.position - cam.transform.position).normalized;
                Vector3 screenPoint = (cam.transform.position + VectorToTarget * 2f);

                waypointMarker.transform.position = cam.WorldToScreenPoint(screenPoint);

                WayPointText.text = waypoints[currentWaypoint].name + "\n" + Mathf.Round(rangeToWaypoint) + "M";
            if (!isWarping)
            {
                screenText.text = (Mathf.Round(playerController.VehicleSpeed) + " KM/H");
            }
            else
            {
                screenText.text = "ERROR!";
            }

                if (hazard.asteroidLaunched && !warning2)
                {
                    hazard.asteroidLaunched = false;
                    warning2 = true;
                    lightToFlash = redWarningLight2;
                    warnToPlay = asteroidWarning;
                    StartCoroutine("flashLight");
                }
        }

        if(day == 0 && ironHeld > 50000 && !playedTutMessage2)
        {
            List<string> caps = new List<string>();
            List<float> times = new List<float>();
            times.Add(0);
            caps.Add("That's enough iron, you can stop mining now.");
            times.Add(2);
            caps.Add("I've added some coordinates into you navigation system labeled 'THE FIRING RANGE'.");
            times.Add(6);
            caps.Add("Head over there as soon as you can. Please.");
            StartCoroutine(storyManager.playNextStep(tutorialMessage2, false, "", true, "GO TO THE FIRING RANGE", KeyCode.None, false, caps, times));
            playedTutMessage2 = true;
            waypointToAdd = allWaypoints[2];
            AddPoint();
        }

    }

    public IEnumerator exposureLerp()
    {
        if (hdriVol.profile.TryGet<HDRISky>(out var hdriSky))
        {
            for (int i = 0; i < 30; i++)
            {
                float exposureForFrame = Mathf.Lerp(hdriSky.exposure.value, idealExposure, 10f*Time.deltaTime);
                hdriSky.exposure.Override(exposureForFrame);
                yield return new WaitForEndOfFrame();
            }
            hdriSky.exposure.Override(idealExposure);
        }
    }

    public IEnumerator flashLight()
    {
        float duration = 0f;
        if (lightToFlash == null || warnToPlay == null)
        {
            yield return null;
        }
        GameObject lightIFlash = lightToFlash;
        warnToPlay.Play();
        while (duration < 2f)
        {
            duration += 0.3f;
            lightIFlash.SetActive(true);
            yield return new WaitForSeconds(.15f);
            lightIFlash.SetActive(false);
            yield return new WaitForSeconds(.15f);
        }
        lightIFlash.SetActive(false);
        warning1 = false;
        warning2 = false;
        yield return null;
    }

    IEnumerator OxygenWarn()
    {
        warningOxygen = true;
        oxygenLight.SetActive(true);
        yield return new WaitForSeconds(1.065f);
        oxygenLight.SetActive(false);
        yield return new WaitForSeconds(0.35f);
        warningOxygen = false;
    }

    public IEnumerator Warp()
    {
        initialRange = rangeToWaypoint;
        playerController.canMove = false;
        canWarp = false;
        warpConfirmation.SetActive(false);
        warpEffect.Play();
        if (!deathTime)
        {
            warpSound.volume = 1f;
            warpSound.Play();
        }
        shake.shakeFactor = 1f;
        StartCoroutine(shake.SHAKE());
        yield return new WaitForSeconds(1.02f);
        shake.shakeFactor = 1f;
        StartCoroutine(shake.SHAKE());
        yield return new WaitForSeconds(1.1f);
        if (hdriVol.profile.TryGet<HDRISky>(out var hdriSky))
        {
            hdriSky.exposure.Override(-10f);
        }
        isWarping = true;
        tunnel.Play();
        beingAttacked = false;
        beingAmbushed = false;
    }

    IEnumerator askForWarpConfirmation()
    {
        warpConfirmation.SetActive(true);
        warpConfirmation.GetComponent<TextMeshProUGUI>().text = "CONFIRM WARP TO\n" + waypoints[currentWaypoint].name + "?";
        warpConfirmSound.Play();
        yield return new WaitForSeconds(2f);
        warpConfirmation.SetActive(false);
    }

    IEnumerator hideUI()
    {
        decoderButton.GetComponent<Animator>().SetBool("shouldShow", false);
        yield return new WaitForSeconds(1.95f);
        decoderButton.gameObject.SetActive(false);
    }

    public void AddPoint()
    {
        if (earnedWaypoints != null && !earnedWaypoints.Contains(waypointToAdd))
        {
            GameObject newWaypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(newWaypointObject.GetComponent<Collider>());
            newWaypointObject.GetComponent<MeshRenderer>().enabled = false;
            normalizedWaypoints.Add(newWaypointObject.transform);
            earnedWaypoints.Add(waypointToAdd);
            waypoints.Add(waypointToAdd);
            waypointsToSave.Add(waypointToAdd.transform.position);
        }
        else if(earnedWaypoints == null)
        {
            earnedWaypoints = new List<GameObject>();
            GameObject newWaypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(newWaypointObject.GetComponent<Collider>());
            newWaypointObject.GetComponent<MeshRenderer>().enabled = false;
            normalizedWaypoints.Add(newWaypointObject.transform);
            earnedWaypoints.Add(waypointToAdd);
            waypoints.Add(waypointToAdd);
            waypointsToSave.Add(waypointToAdd.transform.position);
        }
        if(tutorialButton && tutPodNum == 1 && !alreadyPressed)
        {
            List<string> caps = new List<string>();
            List<float> times = new List<float>();
            times.Add(0);
            caps.Add("Good job! You can listen to my instructions.");
            times.Add(3);
            caps.Add("Open the navigation app, and then choose T-ZONE 1 as your selected waypoint.");
            times.Add(8);
            caps.Add("After that, initiate the warp.");
            StartCoroutine(storyManager.playNextStep(tutorialMessage1, false, "[M]\nFOR NAVIGATION MODE", false, "WARP TO T-ZONE 1", KeyCode.None, false, caps, times));
        }
        monitor.downloadedData = true;
        alreadyPressed = true;
    }
}
