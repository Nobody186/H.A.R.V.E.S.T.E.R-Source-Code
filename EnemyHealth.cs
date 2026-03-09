using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] EnemyMovement enemyMov;
    [SerializeField] GunController gun;
    [SerializeField] cameraShake shake;
    [SerializeField] ConsoleController console;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] List<AudioSource> minerDeathSounds;
    [SerializeField] List<AudioSource> hostileDeathSounds;
    public float health;
    public float maxHealth;
    [SerializeField] float damageIncrements;

    private float timer = 0f;
    private float deathTimer = 0f;
    private float timeToDeath = 1f;

    private AudioSource deathSound;

    public bool isDying = false;

    [SerializeField] AudioSource tutLine;
    [SerializeField] StoryManager storyManager;

    private float cobaltHeld;
    private float ironHeld;
    private float nickelHeld;
    private float iceHeld;

    public List<AudioSource> resourceCollectedSFX = new List<AudioSource>();
    public TextMeshPro oreCollectedText;
    private string oreCollectedString;

    public static event Action<string> OnEnemyDeath;

    private void Start()
    {
        if (enemyMov.hostile)
        {
            nickelHeld = UnityEngine.Random.Range(0, 1000);
            oreCollectedString += "+" + Mathf.Round(nickelHeld * 10) / 10 + " NICKEL\n";
            ironHeld = UnityEngine.Random.Range(500, 5000);
            oreCollectedString += "+" + Mathf.Round(ironHeld * 10) / 10 + " IRON\n";
        }
        else
        {
            cobaltHeld = UnityEngine.Random.Range(0, 1500);
            oreCollectedString += "+" + Mathf.Round(cobaltHeld * 10) / 10 + " COBALT\n";
            iceHeld = UnityEngine.Random.Range(0, 500);
            oreCollectedString += "+" + Mathf.Round(iceHeld * 10) / 10 + " ICE\n";
        }
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(gun.Target != null && gun.Target.transform == transform && gun.isMining && gun.laserOnTimer >= 1)
        {
            timer += Time.deltaTime;
            if(timer >= damageIncrements)
            {
                health -= console.laserDamage;
                timer = 0f;
            }
        }
        if(health <= 0f)
        {
            deathTimer += Time.deltaTime;
            if(deathSound == null)
            {
                if (!enemyMov.hostile)
                {
                    deathSound = minerDeathSounds[UnityEngine.Random.Range(0, minerDeathSounds.Count)];
                }
                else if(enemyMov.hostile)
                {
                    deathSound = hostileDeathSounds[UnityEngine.Random.Range(0, hostileDeathSounds.Count)];
                }
                deathSound.Play();
                timeToDeath = deathSound.clip.length - 0.15f;
            }
        }
        if(deathTimer >= timeToDeath && !isDying)
        {
            OnEnemyDeath?.Invoke(gameObject.name);
            isDying = true;
            if (gun.Target == gameObject)
            {
                gun.unLock();
                resourceCollectedSFX[UnityEngine.Random.Range(0, resourceCollectedSFX.Count)].Play();
                oreCollectedText.text = oreCollectedString;
                gun.StartCoroutine(gun.textPopUpThing());
                console.ironHeld += ironHeld;
                console.nickelHeld += nickelHeld;
                console.cobaltHeld += cobaltHeld;
                console.iceHeld += iceHeld;
            }
            GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
            explosion.SetActive(true);
            shake.shakeFactor = (100f / gun.Distance);
            shake.shakeTime = 1f;
            shake.StartCoroutine(shake.SHAKE());
            if (console.day != 0)
            {
                Destroy(gameObject, 0.1f);
            }
            else
            {
                List<string> caps = new List<string>();
                List<float> times = new List<float>();

                times.Add(0f);
                caps.Add("Now's a good time to note that you have a life support clock on your HUD, labeled \"LF\"");
                times.Add(6f);
                caps.Add("You cannot dock until you've met your daily quota, so take some of my trademarked friendly advice.");
                times.Add(11f);
                caps.Add("Don't slack off unless suffocating and exploding is a hobby of yours.");
                times.Add(15f);
                caps.Add("Again, your cargo hold is of no use to us if the delivery guy is dead.");
                times.Add(20f);
                caps.Add("Head back to your station and go ahead and dock. This would conclude today's training session.");
                times.Add(24.5f);
                caps.Add("Thank you for your time, and I wish you luck on your future and only endeavor. Working... with.... us. [RADIO DIES OUT]");
                times.Add(33f);
                caps.Add("YOUR RADIO+ SUBSCRIPTION PLAN IS AWAITING PAYMENT. ALL INCOMMING TRANSMITIONS WILL BE CEASED UNTIL A PAYMENT IS MADE.");
                times.Add(37f);
                caps.Add("WITH THE BASIC RADIO SUBCRIPTION PLAN, YOU WILL RETAIN FREE ACCESS TO INCOMING ADVERTISING TRANSMISSIONS.");
                times.Add(42f);
                caps.Add("JERRY: ARE YOU ALWAYS EATING MISSILES TO THE FACE? TIRED OF BEING STUCK IN A BURNING UP COCKPIT?");
                times.Add(46f);
                caps.Add("Repent! For Jerry's Repair has come to our salvation!");
                times.Add(49f);
                caps.Add("Wait. [Flips page] Wait no, REJOICE*. Sorry I read that wrong.");
                times.Add(52f);
                caps.Add("Now operating at 80% of Shergeo owned stations, Jerry's repair will personally send their patented repair nanobots to go after your ship, and fix it up instantly!");
                times.Add(59f);
                caps.Add("You won't even need to leave the warmth of your nice cockpit!");
                times.Add(62f);
                caps.Add("JERRY'S REPAIR! WE REPAIR, WITH PRICES FAIR.");


                GameObject audioSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position, Quaternion.identity);
                audioSphere.GetComponent<MeshRenderer>().enabled = false;
                audioSphere.GetComponent<SphereCollider>().enabled = false;
                audioSphere.AddComponent<AudioSource>();
                audioSphere.GetComponent<AudioSource>().clip = tutLine.clip;

                StoryMessage message = new StoryMessage();
                message.subtitles = caps;
                message.timestamps = times;
                message.audio = audioSphere.GetComponent<AudioSource>();
                message.showControlText = false;
                message.controlText = "";
                message.bind = KeyCode.None;
                message.customInstruction = false;
                message.freezePlayer = false;

                storyManager.EnqueueMessage(message);

                Destroy(gameObject, 0.1f);
            }
        }
    }
}
