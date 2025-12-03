using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

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
    [SerializeField] TextMeshProUGUI hudName;
    [SerializeField] Animator RDRVisualizer;
    public GameObject Target;

    public bool hasBasic = false;
    public bool hasAsteroid = false;
    //Lists
    [SerializeField] List<GameObject> positions;
    [SerializeField] List<GameObject> hudTrackers;
    
    [SerializeField] GameObject lockHud;
    [SerializeField] GameObject decoyHud;
    [SerializeField] Transform spawn;
    //This is just so we can physically point the gun.
    [SerializeField] Transform gun;
    [SerializeField] GameObject gunBase;

    [SerializeField] bool lockAttempted = false;
    GameObject recentScanPosition;
    GameObject recentTrack;
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
    [SerializeField] ParticleSystem LaserParticle; //Optional.
    public bool isMining = false; //So we can check the current status of the laser
    public bool canMine = false; //Some general criteria thing
    public bool safeAngle = true; //The player cant fire the laser at themselves unfortunately.
    public bool Cool = false; //Cooldown boolean (deprecated)
    public bool chargingLaser = false;
    public float laserTimer = 0f; //Once we're done with the laser, keep track of how long we've been not using it (For a cooldown. This is deprecated.)
    public float laserOnTimer = 0f; // How long the laser has been on

    private float timer = 0f;
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
    //Tutorial stuff
    [SerializeField] StoryManager storyManager;
    bool playedTutLine1 = false;
    bool playedTutLine2 = false;
    bool playedTutLine0 = false;
    [SerializeField] AudioSource tutLine0;
    [SerializeField] AudioSource tutLine1;
    [SerializeField] AudioSource tutLine2;

    [SerializeField] Transform raycaster;
    [SerializeField] ParticleSystem debrisCloud;

    LayerMask mask;

    private void Start()
    {
        LaserParticle.Stop();
        debrisCloud.Stop();
        radarAnimator = RDRVisualizer.GetComponent<Animator>();
        if(console.usingMissileUpgrade)
        {
            missileTimer.text = "READY";
        }
        else if(console.usingdampDisablerUpgrade)
        {
            missileTimer.text = "DAMP ON";
        }
        else if(console.usingDashUpgrade)
        {
            missileTimer.text = "DASH";
        }
        else if(console.usingCamoUpgrade)
        {
            missileTimer.text = "VISIBLE";
        }
        else if (console.usingRadar3Upgrade)
        {
            missileTimer.text = "HILITE OFF";
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
            //Register position with its velocity.
            recentScanPosition = Instantiate(BASIC.gameObject, rdr.transform.position, Quaternion.identity);
            recentScanPosition.GetComponent<Rigidbody>().linearVelocity = rdr.GetComponent<Rigidbody>().linearVelocity;
            positions.Add(recentScanPosition);
            //Create tracker.
            recentTrack = Instantiate(targetSymbol.gameObject, cam.WorldToScreenPoint(recentScanPosition.transform.position), Quaternion.identity, spawn);
            hudTrackers.Add(recentTrack);
        }
        else if (hasBasic && !hasAsteroid) //If we have a basic, but no object.
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i] == rdr.gameObject)
                {
                    Destroy(positions[i].gameObject);
                    Destroy(hudTrackers[i].gameObject);
                    positions.Remove(positions[i]);
                    hudTrackers.Remove(hudTrackers[i]);
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
            print("TARGET: " + Target.name);
            if(Target.name.Contains("Asteroid") && Target.GetComponent<Mineable>() == null)
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
                if(Target.name.Contains("big"))
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
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        checkForLockFailure();
        if(Input.GetKeyDown(KeyCode.X)) {
            unLock();
        }
        if(Input.GetKeyDown(KeyCode.E) && !console.navMode && console.usingPalUpgrade) {
            if (!palAttempted)
            {
                timer = 0;
                unLock();
                RDRVisualizer.SetBool("PAL", true);
                RDRVisualizer.SetBool("Searching", false);
                palAttempted = true;
                lockAttempted = true;
            }
            else if(palAttempted && Target == null)
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
        else if(console.usingRadar3Upgrade && !console.isSeeing)
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
        else if(!Input.GetButton("Jump") && console.usingdampDisablerUpgrade)
        {
            missileTimer.text = "DAMP ON";
        }
        if (Target != null)
        {
            if(console.day == 0)
            {
                if (!playedTutLine1 && Target.name.Contains("Asteroid"))
                {
                    List<string> caps = new List<string>();
                    List<float> times = new List<float>();
                    times.Add(0);
                    caps.Add("And what exactly are you smiling for?");
                    times.Add(4);
                    caps.Add("THERE'S NOTHING TO SMILE ABOUT IN THIS LIFE!");
                    times.Add(7);
                    caps.Add("Get back to work!");
                    times.Add(9);
                    caps.Add("Find any asteroid. Preferably a big one. And analyze it using the SCAN app.");
                    times.Add(14);
                    caps.Add("Mine until you've met your quota of iron. You can check your quotas in the STATION (STN) app.");
                    StartCoroutine(storyManager.playNextStep(tutLine1, false, "[ENTER]\nTO ANALYZE ORE", false, "", KeyCode.None, true, caps, times));
                    playedTutLine1 = true;
                }
                //if(!playedTutLine0)
                //{
                //    StartCoroutine(storyManager.playNextStep(tutLine0, false, "[ENTER]\nTO ANALYZE ORE", false, "", KeyCode.None, true));
                //    playedTutLine0 = true;
                //}
            }
            LaserParticle.transform.LookAt(Target.transform);
            print("LOCK SUCCESSFUL!");
            Distance = Vector3.Distance(gun.position, Target.transform.position);
            decoyHud.SetActive(false);

            if (positions.Count > 0)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    Destroy(hudTrackers[i].gameObject);
                    Destroy(positions[i].gameObject);
                    positions.Remove(positions[i]);
                    hudTrackers.Remove(hudTrackers[i]);
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
                        print("HIT: " + hit.transform.name);
                        if (!Target.name.Contains("small") && hit.transform == Target.transform)
                        {
                            debrisCloud.gameObject.transform.position = hit.point;
                            debrisCloud.gameObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                            debrisCloud.Play();
                        }
                        else
                        {
                            debrisCloud.Stop();
                        }
                    }
                    if(console.day == 0 && !playedTutLine2)
                    {
                        playedTutLine2 = true;
                        //StartCoroutine(storyManager.playNextStep(tutLine2, false, "", false, "", KeyCode.None, false));
                    }
                    chargingLaser = false;
                    //float maxLength = (Vector3.Distance(gunPos.position, Target.transform.position)) * 1.1f;
                    //Laser.transform.rotation = Quaternion.Slerp(Laser.transform.rotation, gunlookRotation, Time.deltaTime * 5f);
                    //if (Laser.transform.localScale.z < maxLength && laserOnTimer < 2f)
                    //{
                       // Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, Laser.transform.localScale.z * 1.4f * (Time.deltaTime + 1));
                    //}
                    //else if (laserOnTimer > 2f)
                    //{
                        //Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, maxLength);
                    //}
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
                gunBase.GetComponent<Animator>().SetBool("isMining", false);
                debrisCloud.Stop();
                LaserParticle.Stop();
                Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, 0.15f);
            }

            return;
        }

        //All of this plays if we do NOT have a target
        isMining = false;
        debrisCloud.Stop();
        LaserParticle.Stop();
        safeAngle = true;
        laserOnTimer = 0f;
        gunBase.GetComponent<Animator>().SetBool("isMining", false);
        Laser.transform.localScale = new Vector3(Laser.transform.localScale.x, Laser.transform.localScale.y, 0.15f);
        lockHud.SetActive(false);

        for (int i = 0; i < hudTrackers.Count; i++)
        {
            hudTrackers[i].transform.position = cam.WorldToScreenPoint(positions[i].transform.position);
            //if it is reasonable to show a lock symbol, then show it
            if (positions[i].GetComponent<Renderer>().isVisible == true && Vector3.Distance(positions[i].gameObject.transform.position, transform.position) <= MaxRange)
            {
                hudTrackers[i].SetActive(true);
            }
            else
            {
                hudTrackers[i].SetActive(false);
            }
        }
    }

    public void Lock(Button clickedButton)
    {
        timer = 0;
        Vector3 mousePos = clickedButton.transform.position;
        RDRVisualizer.SetBool("PAL", false);
        RDRVisualizer.SetBool("Searching", false);
        RDRVisualizer.SetBool("LockAttempted", true);
        Radar.transform.LookAt(cam.ScreenToWorldPoint(mousePos), Vector3.up);
        decoyHud.transform.position = mousePos;
        lockAttempted = true;
        print("LOCK ATTEMPTED");
    }

    public void unLock()
    {
        if (!isMining)
        {
            Target = null;
            RDRVisualizer.SetBool("LockSuccessful", false);
            RDRVisualizer.SetBool("PAL", false);
            RDRVisualizer.SetBool("LockAttempted", false);
            RDRVisualizer.SetBool("Searching", true);
            Radar.transform.LookAt(radarLook);
            lockAttempted = false;
            lockHud.SetActive(false);
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
        if(Cool  == false && !isMining)
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
            Destroy(hudTrackers[i].gameObject);
            Destroy(positions[i].gameObject);
            positions.Remove(positions[i]);
            hudTrackers.Remove(hudTrackers[i]);
        }

        Radar.transform.LookAt(Target.transform.position);
        lockHud.SetActive(true);
        lockHud.transform.position = cam.WorldToScreenPoint(Target.transform.position);
        if (!Target.gameObject.name.Contains("Enemy"))
        {
            if (Target.GetComponent<Renderer>().isVisible == true && Vector3.Distance(Target.gameObject.transform.position, transform.position) <= MaxRange)
            {
                lockHud.SetActive(true);
            }
            else if(Vector3.Distance(Target.gameObject.transform.position, transform.position) > MaxRange)
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
            if(!hasTarget)
            {
                print("COULDNT FIND TARGET!");
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
                print("LOCK FAILURE!");
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
        yield return new WaitForSeconds(missileCooldown - (console.missileRearmLvl*0.5f));
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

