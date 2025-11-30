using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.CompilerServices;

public class Health : MonoBehaviour
{
    [SerializeField] bool rogueMode = false;
    public float maxHealth = 100f;
    public float health = 100f;
    [SerializeField] ConsoleController consoleController;
    [SerializeField] TextMeshPro hull;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem fire1;
    [SerializeField] ParticleSystem fire2;
    [SerializeField] ParticleSystem deathExplosion;
    [SerializeField] GameObject whitePanel;

    [SerializeField] AudioSource collision1;
    [SerializeField] AudioSource collision2;
    [SerializeField] AudioSource collision3;
    [SerializeField] AudioSource collision4;
    [SerializeField] AudioSource collision5;

    private float randomNum;

    [SerializeField] AudioListener listener;

    public float force = 0f;

    private float Timer = 0f;

    private void Start()
    {
        fire1.Stop();
        fire2.Stop();
        smoke.Stop();
        deathExplosion.Stop();
        whitePanel.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        force = collision.impulse.magnitude / Time.fixedDeltaTime;
        randomNum = Random.Range(1, 6);
        switch (randomNum)
        {
            case 1:
                collision1.Play();
                break;
            case 2:
                collision2.Play();
                break;
            case 3:
                collision3.Play();
                break;
            case 4:
                collision4.Play();
                break;
            case 5:
                collision5.Play();
                break;
            default:
                break;
        }

        print("FORCE: " + force);
        if (collision.gameObject.name.Contains("Debris"))
        {
            health -= Mathf.Round(Mathf.Pow(force, 2f)/120000f);
        }
        else
        {
            health -= Mathf.Round(Mathf.Pow(force, 2f) / 250000f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hull.text = "HULL: " + health;
        if(health <= 0f)
        {
            if (!deathExplosion.isPlaying)
            {
                deathExplosion.Play();
                whitePanel.SetActive(true);
            }
            Timer += Time.deltaTime;
            
            if (Timer >= 2f)
            {
                if (rogueMode)
                {
                    if (consoleController.highestDay >= consoleController.day)
                    {
                        consoleController.highestDay = consoleController.day;
                    }
                    consoleController.Balance = 0;
                    consoleController.cargoValue = 0;
                    consoleController.day = 0;
                    consoleController.navUpgrade = false;
                    consoleController.palUpgrade = false;
                    consoleController.missileUpgrade = false;
                    consoleController.mainThrustLvl = 0;
                    consoleController.sideThrustLvl = 0;
                    consoleController.laserDmgLvl = 0;
                    consoleController.maxPlayerHealth = 100;
                    consoleController.playerHealth = 100;
                    consoleController.maxHealthLvl = 0;
                    consoleController.missileDamageLvl = 0;
                    consoleController.missileRearmLvl = 0;
                    consoleController.missileThrustLvl = 0;
                    consoleController.missileTimeLvl = 0;
                    consoleController.missileTurnLvl = 0;
                    consoleController.earnedWaypoints.Clear();
                    consoleController.usingCamoUpgrade = false;
                    consoleController.usingdampDisablerUpgrade = false;
                    consoleController.usingDashUpgrade = false;
                    consoleController.usingMissileUpgrade = false;
                    consoleController.usingPalUpgrade = false;
                    consoleController.usingRadar3Upgrade = false;
                    consoleController.dampDisabler = false;
                    consoleController.dashUpgrade = false;
                    consoleController.camoUpgrade = false;
                    consoleController.radar3Upgrade = false;
                    consoleController.fusionCells = 10;
                    consoleController.ironHeld = 0;
                    consoleController.helium3Held = 0;
                    consoleController.aluminumHeld = 0;
                    consoleController.carbonHeld = 0;
                    consoleController.clayHeld = 0;
                    consoleController.cobaltHeld = 0;
                    consoleController.diamondHeld = 0;
                    consoleController.hydrogenHeld = 0;
                    consoleController.iceHeld = 0;
                    consoleController.magnesiumHeld = 0;
                    consoleController.nickelHeld = 0;
                    consoleController.platiniumHeld = 0;
                    consoleController.plutoniumHeld = 0;
                    SaveData.SaveInfo(consoleController);
                    SceneManager.LoadScene("Menu");
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
        if(health <= 10f && !fire2.isPlaying)
        {
            fire2.Play();
        }
        if(health <= 25f && !fire1.isPlaying)
        {
            fire1.Play();
        }
        if(health <= 50f && !smoke.isPlaying)
        {
            smoke.Play();
        }

        if(health > 10f && fire2.isPlaying)
        {
            fire2.Stop();
        }
        if(health > 25f && fire1.isPlaying)
        {
            fire1.Stop();
        }
        if(health > 50 && smoke.isPlaying)
        {
            smoke.Stop();
        }
    }
}
