using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public class GunController : MonoBehaviour
{
    [SerializeField] ConsoleController console;
    //camera for camera shenanigins
    [SerializeField] Camera cam;
    //radarthing
    [SerializeField] GameObject Radar;
    [SerializeField] Transform radarLook;
    [SerializeField] Transform radarLookDir;
    [SerializeField] GameObject BASIC;
    [SerializeField] GameObject targetSymbol;
    [SerializeField] GameObject symbolParticle;
    [SerializeField] TextMeshProUGUI hudName;
    [SerializeField] Animator RDRVisualizer;
    public GameObject Target;

    public bool hasBasic = false;
    public bool hasAsteroid = false;
    //Lists
    [SerializeField] List<GameObject> positions;
    [SerializeField] List<GameObject> hudTrackers;
    [SerializeField] List<GameObject> hudParticles;
    //Filters out what we aim and checks for this layer.
    //if i knew i would need these many variables just for some cosmetic effects, i never would have started
    //update from the future: This was 100% worth it.
    [SerializeField] GameObject lockHud;
    [SerializeField] GameObject lockParticle;
    [SerializeField] GameObject decoyHud;
    [SerializeField] Transform spawn;
    [SerializeField] Transform particleSpawn;
    //This is just so we can physically point the gun. The player can't see it but I already programmed it in and it took some effort getting it pointed right
    //Update from the future: It became very useful later on. Because the player CAN see the laser.
    [SerializeField] Transform gun;
    [SerializeField] GameObject gunBase;

    [SerializeField] bool lockAttempted = false;
    GameObject recentScanPosition;
    GameObject recentTrack;
    GameObject recentParticle;
    //The following are public only so that ConsoleController can access them.
    public float Distance = 0f;
    public float MinRange = 25f;
    public float MaxRange = 70f;
    public int laserIntensity = 100;
    [SerializeField] float missileCooldown;
    [SerializeField] TextMeshPro missileTimer;
    [SerializeField] TextMeshPro laserIntensityText;

    [SerializeField] GameObject Laser;
    [SerializeField] GameObject missile;
    [SerializeField] Transform pylon;
    public Transform laserEnd;
    [SerializeField] Transform gunPos;
    [SerializeField] AudioSource laserSound;
    [SerializeField] AudioSource radarLockSfx;
    [SerializeField] AudioSource radarUnlockSfx;
    [SerializeField] ParticleSystem LaserParticle; //Optional.
    public bool isMining = false; //So we can check the current status of the laser
    public bool canMine = false; //Some general criteria thing
    public bool safeAngle = true;
    public bool Cool = false; //Cooldown boolean
    public bool chargingLaser = false;
    public float laserTimer = 0f; //Once we're done with the laser, keep track of how long we've been not using it
    public float laserOnTimer = 0f; // How long the laser has been on

    private float timer = 0f;
    float LOS_Timer = 0f; //A timer to keep track of Line Of Sight.
    public bool palAttempted = false;
    bool missileRearm = false;
    private Animator radarAnimator;

    //This is not for the gun. This is for reference when adding the mineable component to the asteroid.
    [SerializeField] cameraShake camShake;
    [SerializeField] Health health;
    [SerializeField] GameObject resourceHarvestedImage;
    [SerializeField] List<AudioSource> resourceHarvestedSFX;
    [SerializeField] Transform canvas;
    [SerializeField] Transform hudPosition;
    [SerializeField] GameObject asteroidHarvestedText;
    [SerializeField] GameObject bigExplosion;
    [SerializeField] GameObject smallExplosion;
    [SerializeField] GameObject PopUp;
    [SerializeField] TextMeshProUGUI PopUpText;
    [SerializeField] TextMeshPro oreCollectedText;

    [SerializeField] StoryManager storyManager;
    bool playedTutLine1 = false;
    bool playedTutLine2 = false;

    [SerializeField] AudioSource tutLine0;
    [SerializeField] AudioSource tutLine1;
    [SerializeField] AudioSource tutLine2;

    [SerializeField] Transform raycaster;
    [SerializeField] ParticleSystem debrisCloud;
    [SerializeField] GameObject laserExplosion;

    LayerMask mask;

    // Staggered update variables
    private int currentUpdateIndex = 0;
    private float staggeredUpdateTimer = 0f;
    private const float staggeredUpdateInterval = 0.05f; // Update every 0.05 seconds
    private int updatesPerFrame = 2; // Number of items to update per staggered update

    private void Start()
    {
        LaserParticle.Stop();
        debrisCloud.Stop();
        laserExplosion.SetActive(false);
        radarAnimator = RDRVisualizer.GetComponent<Animator>();
        if (console.usingMissileUpgrade)
        {
            missileTimer.text = "READY";
        }
        else if (console.usingdampDisablerUpgrade)
        {
            missileTimer.text = "DAMP ON";
        }
        else if (console.usingDashUpgrade)
        {
            missileTimer.text = "DASH";
        }
        else if (console.usingCamoUpgrade)
        {
            missileTimer.text = "VISIBLE";
        }
        else if (console.usingRadar3Upgrade)
        {
            missileTimer.text = "HILITE OFF";
        }
        else if (console.usingPalUpgrade)
        {
            missileTimer.text = "RDR2 ON";
        }

        mask = LayerMask.GetMask("Mineable", "Enemy");
    }


    //WHAT TO DO IF WE GET A SCAN HIT
    void OnTriggerEnter(Collider rdr)
    {
        hasAsteroid = false;
        hasBasic = false;
        Collider[] ColliderCollection = Physics.OverlapSphere(rdr.gameObject.transform.position, 2f);

        for (int i = 0; i < ColliderCollection.Length; i++)
        {
            if (ColliderCollection[i].gameObject.layer == LayerMask.NameToLayer("Mineable") || ColliderCollection[i].gameObject.layer == LayerMask.NameToLayer("Enemy") || ColliderCollection[i].gameObject.name.Contains("Data"))
            {
                if (ColliderCollection[i].gameObject.name.Contains("small") && console.usingPalUpgrade) //Filter out small asteroids if we have the Radar V2 Upgrade
                {
                    continue;
                }
                else
                {
                    hasAsteroid = true;
                }
            }
            if (ColliderCollection[i].gameObject.layer == LayerMask.NameToLayer("basic")) //If we find one of our invisible track objects
            {
                hasBasic = true;
            }
        }

        if (!hasBasic && hasAsteroid) //If we found an unnacounted for asteroid...
        {
            for (int i = 0; i < positions.Count; i++)
            {   //If we already have an object's position stored, do not store it again.
                if (rdr.transform.position == positions[i].transform.position)
                {
                    return;
                }
            }
            //If it isn't a physics object, don't register it.
            if (rdr.GetComponent<Rigidbody>() == null)
            {
                return;
            }
            //Register position with its velocity.
            recentScanPosition = Instantiate(BASIC.gameObject, rdr.transform.position, Quaternion.identity);
            recentScanPosition.GetComponent<Rigidbody>().linearVelocity = rdr.GetComponent<Rigidbody>().linearVelocity;
            positions.Add(recentScanPosition);
            //Create tracker.
            recentTrack = Instantiate(targetSymbol.gameObject, cam.WorldToScreenPoint(recentScanPosition.transform.position), Quaternion.identity, spawn);
            hudTrackers.Add(recentTrack);
            //Create particle
            recentParticle = Instantiate(symbolParticle, cam.ScreenToWorldPoint(recentTrack.transform.position), Quaternion.Euler(0, 0, 0), particleSpawn);
            hudParticles.Add(recentParticle);
        }
        else if (hasBasic && !hasAsteroid) //If we have a basic, but no object.
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i] == rdr.gameObject)
                {
                    Destroy(positions[i].gameObject);
                    Destroy(hudTrackers[i].gameObject);
                    Destroy(hudParticles[i].gameObject);
                    positions.Remove(positions[i]);
                    hudTrackers.Remove(hudTrackers[i]);
                    hudParticles.Remove(hudParticles[i]);
                }
            }
        }
    }

    private void OnTriggerStay(Collider rdr)
    {
        if (lockAttempted && rdr != null && rdr.gameObject.layer != LayerMask.NameToLayer("basic")) //If we hit an object after pointing our radar at an area (to lock it)
        {
            Target = rdr.gameObject;
            RDRVisualizer.SetBool("Searching", false);
            RDRVisualizer.SetBool("PAL", false);
            RDRVisualizer.SetBool("LockAttempted", false);
            RDRVisualizer.SetBool("LockSuccessful", true);
            decoyHud.SetActive(true);
            decoyHud.transform.position = cam.WorldToScreenPoint(Target.transform.position);
            //print("TARGET: " + Target.name);
            if (Target.name.Contains("Asteroid") && Target.GetComponent<Mineable>() == null)
            {
                Mineable targetRef = Target.AddComponent<Mineable>();
                targetRef.gun = this;
                targetRef.console = console;
                targetRef.cameraShake = camShake;
                targetRef.healthScript = health;
                targetRef.resourceCollectedImage = resourceHarvestedImage;
                targetRef.resourceCollectedSFX.AddRange(resourceHarvestedSFX);
                targetRef.canvas = canvas;
                targetRef.hudPos = hudPosition;
                targetRef.harvestedText = asteroidHarvestedText;
                targetRef.PopUp = PopUp;
                targetRef.PopUpText = PopUpText;
                targetRef.oreCollectedText = oreCollectedText;
                if (Target.name.Contains("big"))
                {
                    targetRef.explosion = bigExplosion;
                }
                else
                {
                    targetRef.explosion = smallExplosion;
                }
            }
            lockAttempted = false;
            palAttempted = false;
            radarLockSfx.Play();
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        checkForLockFailure();
        if (Input.GetKeyDown(KeyCode.X))
        {
            unLock();
        }
        if (Input.GetKeyDown(KeyCode.E) && !console.navMode && console.usingPalUpgrade)
        {
            if (!palAttempted)
            {
                timer = 0;
                unLock();
                RDRVisualizer.SetBool("PAL", true);
                RDRVisualizer.SetBool("Searching", false);
                palAttempted = true;
                lockAttempted = true;
            }
            else if (palAttempted && Target == null)
            {
                palAttempted = false;
                lockAttempted = false;
                RDRVisualizer.SetBool("PAL", false);
                RDRVisualizer.SetBool("Searching", true);
            }
        }
        if (!Input.GetKey(KeyCode.Mouse1))
        {
            laserIntensity += Mathf.RoundToInt((Input.GetAxis("Mouse ScrollWheel")) * 15);
        }
        if (laserIntensity <= 0)
        {
            laserIntensity = 0;
        }
        else if (laserIntensity >= 100)
        {
            laserIntensity = 100;
        }
        laserIntensityText.text = "LASER INTENSITY: " + laserIntensity + "%";
        if (console.isSeeing)
        {
            missileTimer.text = "HILITE ON";
        }
        else if (console.usingRadar3Upgrade && !console.isSeeing)
        {
            missileTimer.text = "HILITE OFF";
        }
        if (console.isCamo)
        {
            missileTimer.text = "HIDDEN";
        }
        else if (console.usingCamoUpgrade && !console.isCamo)
        {
            missileTimer.text = "VISIBLE";
        }
        if (Input.GetButton("Jump") && console.usingdampDisablerUpgrade)
        {
            missileTimer.text = "DAMP OFF";
        }
        else if (!Input.GetButton("Jump") && console.usingdampDisablerUpgrade)
        {
            missileTimer.text = "DAMP ON";
        }
        if (Target != null)
        {
            if (console.day == 0)
            {
                if (!playedTutLine1 && Target.name.Contains("Asteroid"))
                {
                    List<string> caps = new List<string>();
                    List<float> times = new List<float>();
                    caps.Add("Remember! Set your laser intensity to 55%, and DO NOT let the PTI get lower than 15%!");
                    times.Add(0f);

                    StoryMessage message = new StoryMessage();
                    message.subtitles = caps;
                    message.timestamps = times;
                    message.audio = tutLine1;
                    message.showControlText = false;
                    message.controlText = "";
                    message.bind = KeyCode.None;
                    message.customInstruction = false;
                    message.freezePlayer = false;

                    storyManager.EnqueueMessage(message);
                    playedTutLine1 = true;
                }
                //if(!playedTutLine0)
                //{
                //    StartCoroutine(storyManager.playNextStep(tutLine0, false, "[ENTER]\nTO ANALYZE ORE", false, "", KeyCode.None, true));
                //    playedTutLine0 = true;
                //}
            }
            LaserParticle.transform.LookAt(Target.transform);
            //print("LOCK SUCCESSFUL!");
            Distance = Vector3.Distance(gun.position, Target.transform.position);
            decoyHud.SetActive(false);

            if (positions.Count > 0)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    Destroy(positions[i].gameObject);
                    Destroy(hudTrackers[i].gameObject);
                    Destroy(hudParticles[i].gameObject);
                    positions.Remove(positions[i]);
                    hudTrackers.Remove(hudTrackers[i]);
                    hudParticles.Remove(hudParticles[i]);
                }
            }

            setRadarLockSystems();

            //Laser.transform.parent = gunPos.transform;
            //Laser.transform.position = gunPos.transform.position;
            miningChecklist(); //Set our canMine boolean.
            if (Input.GetMouseButton(0) && canMine && !console.isCamo) //If we are mining
            {
                Quaternion gunlookRotation = Quaternion.LookRotation((Target.transform.position - gun.position).normalized);
                laserOnTimer += Time.deltaTime;
                if (laserOnTimer < 1)
                {
                    chargingLaser = true;
                }
                Laser.SetActive(true);
                isMining = true;
                gunBase.SetActive(true);
                gunBase.GetComponent<Animator>().SetBool("isMining", true);
                gun.rotation = Quaternion.Slerp(gun.rotation, gunlookRotation, Time.deltaTime * 5f);
                if (laserOnTimer >= 1)
                {
                    RaycastHit hit;
                    if (Physics.SphereCast(raycaster.position, 0.1f, raycaster.forward, out hit, Mathf.Infinity, mask))
                    {
                        //print("HIT: " + hit.transform.name);
                        if (!Target.name.Contains("small") && hit.transform == Target.transform)
                        {
                            debrisCloud.gameObject.transform.position = hit.point;
                            laserExplosion.gameObject.transform.position = hit.point;
                            laserExplosion.SetActive(true);
                            debrisCloud.gameObject.transform.rotation = Quaternion.LookRotation(-(Target.transform.position - gun.transform.position)).normalized;
                            debrisCloud.Play();
                        }
                        else
                        {
                            laserExplosion.SetActive(false);
                            debrisCloud.Stop();
                        }

                        float gracePeriod = 1f;
                        if (hit.transform != Target.transform)
                        {
                            LOS_Timer += Time.deltaTime;
                            if (LOS_Timer >= gracePeriod)
                            {
                                LOS_Timer = 0f;
                                unLock();
                            }
                        }
                        else
                        {
                            LOS_Timer = 0f;
                        }
                    }
                    if (console.day == 0 && !playedTutLine2)
                    {
                        playedTutLine2 = true;
                        //StartCoroutine(storyManager.playNextStep(tutLine2, false, "", false, "", KeyCode.None, false));
                    }
                    chargingLaser = false;

                    if (!LaserParticle.isPlaying)
                    {
                        LaserParticle.Play();
                    }
                    ParticleSystem.MainModule temp = LaserParticle.main;
                    temp.startLifetime = Distance / 693.15f; //Using some unit conversions, this is the magic number that makes the laser particle reach its target and not overshoot it.

                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!missileRearm && console.usingMissileUpgrade)
                {
                    StartCoroutine("fireMissile");
                }
            }
            if (isMining && !canMine) //If we're mining when we're not supposed to (This plays for 1 frame)
            {
                isMining = false; //stop mining
                chargingLaser = false;
                Cool = false; //Turn on our cooldown
                laserOnTimer = 0f;
            }
            if (Input.GetMouseButtonUp(0) && isMining) //If we voluntarily stop mining (This plays for 1 frame)
            {
                chargingLaser = false;
                Cool = false; //Put on our cooldown
                laserOnTimer = 0f;
                gunBase.GetComponent<Animator>().SetBool("isMining", false); //Do the stuff to actually stop mining
                isMining = false;

            }
            if (!isMining) //Shrink the laser, and play the stop mining animation (This plays every frame when we stop mining)
            {
                laserOnTimer = 0f;
                gunBase.GetComponent<Animator>().SetBool("isMining", false);
                laserExplosion.SetActive(false);
                debrisCloud.Stop();
                LaserParticle.Stop();
                Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, 0.15f);
            }

            return;
        }

        //All of this plays if we do NOT have a target
        isMining = false;
        laserExplosion.SetActive(false);
        debrisCloud.Stop();
        LaserParticle.Stop();
        safeAngle = true;
        laserOnTimer = 0f;
        gunBase.GetComponent<Animator>().SetBool("isMining", false);
        Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, 0.15f);
        lockHud.SetActive(false);
        lockParticle.SetActive(false);

        // Update all positions and rotations every frame
        for (int i = 0; i < hudTrackers.Count; i++)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(positions[i].transform.position);
            hudTrackers[i].transform.position = screenPos;

            //Keep our particles aligned with the trackers.
            Vector3 hudTrackerPos = hudTrackers[i].transform.position;
            hudTrackerPos.z = 10f;
            hudParticles[i].transform.position = cam.ScreenToWorldPoint(hudTrackerPos);

            //Keep particles facing the player
            Quaternion particleLookDir = Quaternion.LookRotation(positions[i].transform.position - cam.transform.position).normalized;
            hudParticles[i].transform.rotation = particleLookDir;
        }

        // Staggered update for color and visibility
        staggeredUpdateTimer += Time.deltaTime;
        if (staggeredUpdateTimer >= staggeredUpdateInterval)
        {
            staggeredUpdateTimer = 0f;

            for (int updateCount = 0; updateCount < updatesPerFrame && hudTrackers.Count > 0; updateCount++)
            {
                if (currentUpdateIndex >= hudTrackers.Count)
                {
                    currentUpdateIndex = 0;
                }

                int i = currentUpdateIndex;

                //Color handling
                Button button = hudTrackers[i].GetComponent<Button>();
                ColorBlock colors = button.colors;
                Color buttonColor;

                if (button.gameObject.GetComponent<buttonHiLiteCheck>().isMouseOver)
                {
                    buttonColor = colors.highlightedColor;
                }
                else
                {
                    buttonColor = colors.normalColor;
                }

                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                mpb.SetColor("_EmissiveColor", buttonColor);
                hudParticles[i].GetComponent<ParticleSystemRenderer>().SetPropertyBlock(mpb);

                //if it is reasonable to show a lock symbol, then show it
                if (positions[i].GetComponent<Renderer>().isVisible == true && Vector3.Distance(positions[i].gameObject.transform.position, transform.position) <= MaxRange)
                {
                    hudTrackers[i].SetActive(true);
                    hudParticles[i].SetActive(true);
                }
                else
                {
                    hudTrackers[i].SetActive(false);
                    hudParticles[i].SetActive(false);
                }

                currentUpdateIndex++;
            }
        }
    }

    public void Lock(Button clickedButton)
    {
        timer = 0;
        Vector3 mousePos = clickedButton.transform.position;

        for (int i = 0; i < positions.Count; i++)
        {
            if (hudTrackers[i] == clickedButton)
            {
                mousePos.z = positions[i].transform.position.z;
            }
        }

        RDRVisualizer.SetBool("PAL", false);
        RDRVisualizer.SetBool("Searching", false);
        RDRVisualizer.SetBool("LockAttempted", true);
        Radar.transform.LookAt(cam.ScreenToWorldPoint(mousePos), Vector3.up);
        decoyHud.transform.position = mousePos;
        lockAttempted = true;
        //print("LOCK ATTEMPTED");
    }

    public void unLock()
    {
        if (!isMining || (isMining && chargingLaser))
        {
            Target = null;
            RDRVisualizer.SetBool("LockSuccessful", false);
            RDRVisualizer.SetBool("PAL", false);
            RDRVisualizer.SetBool("LockAttempted", false);
            RDRVisualizer.SetBool("Searching", true);
            Radar.transform.LookAt(radarLook);
            lockAttempted = false;
            lockHud.SetActive(false);
            lockParticle.SetActive(false);
            radarUnlockSfx.Play();
            laserOnTimer = 0f;
            chargingLaser = false;
        }
    }


    void checkTimer()
    {
        if (Cool == true)
        {
            laserTimer += Time.deltaTime;
        }

        if (laserTimer >= 2f)
        {
            Cool = false;
            laserTimer = 0f;
        }
    }

    void miningChecklist()
    {
        checkTimer();
        if (Cool == false && !isMining)
        {
            safeAngle = true;
        }
        if (Target != null && safeAngle && Cool == false)
        {
            canMine = true;
        }
        else
        {
            canMine = false;
        }
    }

    public void setRadarLockSystems()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Destroy(positions[i].gameObject);
            Destroy(hudTrackers[i].gameObject);
            Destroy(hudParticles[i].gameObject);
            positions.Remove(positions[i]);
            hudTrackers.Remove(hudTrackers[i]);
            hudParticles.Remove(hudParticles[i]);
        }

        Radar.transform.LookAt(Target.transform.position);
        lockHud.SetActive(true);
        lockParticle.SetActive(true);

        // Update lock HUD position and rotation every frame (continuous)
        Vector3 screenPos = cam.WorldToScreenPoint(Target.transform.position);
        lockHud.transform.position = screenPos;

        //Keep our particles aligned with the trackers.
        Vector3 hudTrackerPos = lockHud.transform.position;
        hudTrackerPos.z = 10f;
        lockParticle.transform.position = cam.ScreenToWorldPoint(hudTrackerPos);

        //Keep particles facing the player
        Quaternion particleLookDir = Quaternion.LookRotation(Target.transform.position - cam.transform.position).normalized;
        lockParticle.transform.rotation = particleLookDir;

        // Color handling (staggered update)
        staggeredUpdateTimer += Time.deltaTime;
        if (staggeredUpdateTimer >= staggeredUpdateInterval)
        {
            Button button = lockHud.GetComponent<Button>();
            ColorBlock colors = button.colors;
            Color buttonColor;

            if (button.gameObject.GetComponent<buttonHiLiteCheck>().isMouseOver)
            {
                buttonColor = colors.highlightedColor;
            }
            else
            {
                buttonColor = colors.normalColor;
            }

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_EmissiveColor", buttonColor);
            lockParticle.GetComponent<ParticleSystemRenderer>().SetPropertyBlock(mpb);
        }

        if (!Target.gameObject.name.Contains("Enemy"))
        {
            if (Target.GetComponent<Renderer>().isVisible == true && Vector3.Distance(Target.gameObject.transform.position, transform.position) <= MaxRange)
            {
                lockHud.SetActive(true);
            }
            else if (Vector3.Distance(Target.gameObject.transform.position, transform.position) > MaxRange)
            {
                isMining = false; //stop mining
                chargingLaser = false;
                Cool = false; //Turn on our cooldown
                laserOnTimer = 0f;
                unLock();
            }

            RaycastHit[] hit;
            bool hasTarget = false;
            hit = Physics.SphereCastAll(radarLookDir.position, 2f, radarLookDir.forward, MaxRange);
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.gameObject == Target)
                {
                    hasTarget = true;
                }
            }
            if (!hasTarget)
            {
                //print("COULDNT FIND TARGET!");
                isMining = false; //stop mining
                chargingLaser = false;
                Cool = false; //Turn on our cooldown
                laserOnTimer = 0f;
                unLock();
            }
        }
    }
    void checkForLockFailure()
    {
        if (!palAttempted)
        {
            timer += Time.deltaTime;

            if (Target == null && lockAttempted == true && timer > 1f)
            {
                //print("LOCK FAILURE!");
                RDRVisualizer.SetBool("LockAttempted", false);
                RDRVisualizer.SetBool("Searching", true);
                lockAttempted = false;
                canMine = false;
                timer = 0f;
            }
        }
    }

    IEnumerator fireMissile()
    {
        missileRearm = true;
        missileTimer.text = "REARMING...";
        GameObject spawnedMissile = Instantiate(missile, pylon.position, Quaternion.identity);
        spawnedMissile.SetActive(true);
        yield return new WaitForSeconds(missileCooldown - (console.missileRearmLvl * 0.5f));
        missileRearm = false;
        missileTimer.text = "READY";
    }

    public void closePopUp()
    {
        StartCoroutine(playCloseAnimation());
    }
    IEnumerator playCloseAnimation()
    {
        PopUp.GetComponent<Animator>().SetBool("shouldShow", false);
        yield return new WaitForSeconds(0.45f);
        {
            PopUp.SetActive(false);
        }
    }
}
