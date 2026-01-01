using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Hierarchy;

public class Mineable : MonoBehaviour
{

    public GunController gun;
    public ConsoleController console;
    public cameraShake cameraShake;
    public Health healthScript;
    public GameObject resourceCollectedImage;
    public List<AudioSource> resourceCollectedSFX = new List<AudioSource>();
    public Transform canvas;
    public Transform hudPos;
    public GameObject harvestedText;
    public GameObject explosion;
    private Material asteroidMat;
    public GameObject PopUp;
    public TextMeshProUGUI PopUpText;
    public TextMeshPro oreCollectedText;
    private string oreCollectedString;
    private Animator popUpAnimator;
    public float meltSpeed = 0.1f;
    float maxTickRate = 10f;
    public float tickRate = 10f;
    public float health = 500f;
    public float asteroidHealthToShare;
    public float maxHealth = 500f;
    private bool laserTouching = false;
    float totalBlend;
    public bool isDestroyed = false;

    private float timer1;

    private Camera mainCam;
    private Renderer asteroidRenderer;
    private Transform Player;

    float deathPercent;

    [SerializeField] float ironAmount;
    [SerializeField] float nickelAmount;
    [SerializeField] float clayAmount;
    [SerializeField] float cobaltAmount;
    [SerializeField] float diamondAmount;
    [SerializeField] float plutoniumAmount;
    [SerializeField] float magnesiumAmount;
    [SerializeField] float aluminumAmount;
    [SerializeField] float platiniumAmount;
    [SerializeField] float helium3Amount;
    [SerializeField] float hydrogenAmount;
    [SerializeField] float carbonAmount;
    [SerializeField] float iceAmount;

    int oreType;

    public float totalValue;
    float laseredValue;
    private float totalStuffAmount;
    private string compositionText;
    bool isToxic;

    float ogMeltSpeed;
    private float mineFractionPerTick;

    private void Start()
    {
        mainCam = Camera.main;
        asteroidRenderer = gameObject.GetComponent<Renderer>();
        Player = GameObject.Find("PlayerShip").transform;
        asteroidMat = gameObject.GetComponent<Renderer>().material;

        if (gameObject.name.Contains("small"))
        {
            int chosenHealth = Random.Range(25, 65);

            maxHealth = chosenHealth;
            health = chosenHealth;
            tickRate = 0.25f; // Much faster tick rate
            maxTickRate = tickRate;
            meltSpeed = 0.1f;
        }
        else if (gameObject.name.Contains("medium"))
        {
            int chosenHealth = Random.Range(250, 500);

            maxHealth = chosenHealth;
            health = chosenHealth;
            tickRate = 0.3f; // Much faster tick rate
            maxTickRate = tickRate;
            meltSpeed = 0.01f;
        }
        else if (gameObject.name.Contains("big"))
        {
            int chosenHealth = Random.Range(850, 1800);

            maxHealth = chosenHealth;
            health = chosenHealth;
            tickRate = 0.4f; // Much faster tick rate
            maxTickRate = tickRate;
            meltSpeed = 0.001f;
        }
        else if (gameObject.name.Contains("Debris"))
        {
            health = 26f;

            maxHealth = health;
            tickRate = 0.1f; // Much faster tick rate
            maxTickRate = tickRate;
            meltSpeed = 10f;
        }
        else
        {
            int chosenHealth = Mathf.RoundToInt(Mathf.Infinity);

            maxHealth = chosenHealth;
            health = chosenHealth;
            tickRate = 0.5f; // Much faster tick rate
            maxTickRate = tickRate;
            meltSpeed = 0.00001f;
        }

        //1 = Metallic. 2 = Soft. 3 = Gas. 4 = Carbonic. 5 = Starter (has all quotas)

        if(gameObject.name.Contains("AType"))
        {
            oreType = 0;
        }
        else if(gameObject.name.Contains("BType"))
        {
            oreType = 1;
        }
        else if (gameObject.name.Contains("GType"))
        {
            oreType = 2;
        }
        else if (gameObject.name.Contains("CType"))
        {
            oreType = 3;
        }
        else
        {
            oreType = 4;
        }

        int baseSelector = Random.Range(0, 3); //Each ore type has either one kind of base, or both of them.

        switch (oreType)
        {
            case 0: //METTALIC
                if(baseSelector == 0)
                {
                    ironAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }
                else if(baseSelector == 1)
                {
                    aluminumAmount = Random.Range(300f, 520f) * maxHealth / 10;
                }
                else
                {
                    ironAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                    magnesiumAmount = Random.Range(300f, 520f) * maxHealth / 10;
                }

                if (Random.value < 0.40f) nickelAmount = Random.Range(40f, 160f) * maxHealth / 10;
                if (Random.value <= 0.01f) platiniumAmount = Random.Range(0.5f, 3f) * maxHealth / 10;

                break;

            case 1: //SOFT
                if (baseSelector == 0)
                {
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }
                else if (baseSelector == 1)
                {
                    magnesiumAmount = Random.Range(250f, 400f) * maxHealth / 10;
                }
                else
                {
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                    magnesiumAmount = Random.Range(250f, 400f) * maxHealth / 10;
                }

                if (Random.value < 0.35f) cobaltAmount = Random.Range(32f, 150f) * maxHealth / 10; //Yes, I know cobalt isn't soft. But I can't just put all the valuable stuff in the "metallic" ore.
                if (Random.value <= 0.025f) diamondAmount = Random.Range(0.8f, 7f) * maxHealth / 10;

                break;

            case 2: //GAS
                isToxic = true;
                if (baseSelector == 0)
                {
                    iceAmount = Random.Range(95f, 400f) * maxHealth / 10;
                }
                else if (baseSelector == 1)
                {
                    hydrogenAmount = Random.Range(100f, 280f) * maxHealth / 10;
                }
                else
                {
                    iceAmount = Random.Range(95f, 400f) * maxHealth / 10;
                    hydrogenAmount = Random.Range(100f, 280f) * maxHealth / 10;
                }

                if (Random.value < 0.15f) helium3Amount = Random.Range(1f, 5f) * maxHealth / 10;
                if (Random.value <= 0.005f) plutoniumAmount = Random.Range(0.5f, 2f) * maxHealth / 10;

                break;

            case 3: //CARBONIC
                if (baseSelector == 0)
                {
                    carbonAmount = Random.Range(95f, 200f) * maxHealth / 10;
                }
                else if (baseSelector == 1)
                {
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }
                else
                {
                    carbonAmount = Random.Range(95f, 200f) * maxHealth / 10;
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }

                if (Random.value < 0.15f) hydrogenAmount = Random.Range(100f, 280f) * maxHealth / 10;
                if (Random.value < 0.025f) diamondAmount = Random.Range(0.8f, 7f) * maxHealth / 10;

                break;

            case 4: //STARTER
                if (baseSelector == 0)
                {
                    ironAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }
                else if (baseSelector == 1)
                {
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }
                else
                {
                    ironAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                    clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
                }

                //Additional Stuff that wouldn't hurt.
                if (Random.value < 0.4f) magnesiumAmount = Random.Range(300f, 520f) * maxHealth / 10;
                if (Random.value < 0.4f) aluminumAmount = Random.Range(250f, 400f) * maxHealth / 10;
                if (Random.value < 0.3f) iceAmount = Random.Range(95f, 400f) * maxHealth / 10;
                if (Random.value < 0.3f) hydrogenAmount = Random.Range(100f, 280f) * maxHealth / 10;
                if (Random.value < 0.4f) carbonAmount = Random.Range(95f, 200f) * maxHealth / 10;

                // Rarer, mid-Tier stuff
                if (Random.value < 0.25f) nickelAmount = Random.Range(40f, 160f) * maxHealth / 10;
                if (Random.value < 0.15f) cobaltAmount = Random.Range(32f, 150f) * maxHealth / 10;
                if (Random.value < 0.05f) helium3Amount = Random.Range(1f, 5f) * maxHealth / 10;

                break; 
        }


        //DEFAULT LIST OF STUFF FOR REFERENCE.
        //Common stuff
        //if (Random.value < 0.9f) ironAmount = Random.Range(1000f, 1500f) * maxHealth/10;
        //if (Random.value < 0.6f) clayAmount = Random.Range(1000f, 1500f) * maxHealth / 10;
        //if (Random.value < 0.7f) magnesiumAmount = Random.Range(300f, 520f) * maxHealth / 10;
        //if (Random.value < 0.6f) aluminumAmount = Random.Range(250f, 400f) * maxHealth / 10;
        //if (Random.value < 0.4f) iceAmount = Random.Range(95f, 400f) * maxHealth / 10;
        //if (Random.value < 0.3f) hydrogenAmount = Random.Range(100f, 280f) * maxHealth / 10;
        //if (Random.value < 0.4f) carbonAmount = Random.Range(95f, 200f) * maxHealth / 10;

        // Mid-Tier stuff
        //if (Random.value < 0.25f) nickelAmount = Random.Range(40f, 160f) * maxHealth / 10;
        //if (Random.value < 0.15f) cobaltAmount = Random.Range(32f, 150f) * maxHealth / 10;
        //if (Random.value < 0.10f) helium3Amount = Random.Range(1f, 5f) * maxHealth / 10;

        //RICHES!!!
        //if (Random.value < 0.025f) diamondAmount = Random.Range(0.8f, 7f) * maxHealth / 10;      // 2.5% chance
        //if (Random.value <= 0.005f) platiniumAmount = Random.Range(0.5f, 3f) * maxHealth / 10;  // 0.5% chance
        //if (Random.value <= 0.001f) plutoniumAmount = Random.Range(0.5f, 2f) * maxHealth / 10; // 0.1% chance

        //TOTAL VALUE CALCULATIONS
        totalValue += ironAmount * 0.8f;
        totalValue += clayAmount * 0.4f;
        totalValue += magnesiumAmount * 2f;
        totalValue += aluminumAmount * 2.5f;
        totalValue += nickelAmount * 30f;
        totalValue += cobaltAmount * 80f;
        totalValue += carbonAmount * 5f;
        totalValue += iceAmount * 1.5f;
        totalValue += hydrogenAmount * 3f;
        totalValue += helium3Amount * 3000f;
        totalValue += diamondAmount * 15000f;
        totalValue += platiniumAmount * 25000f;
        totalValue += plutoniumAmount * 50000f;

        totalStuffAmount = ironAmount + clayAmount + magnesiumAmount + aluminumAmount + nickelAmount + cobaltAmount + carbonAmount + iceAmount + hydrogenAmount + helium3Amount + diamondAmount + platiniumAmount + plutoniumAmount;

        compositionText = "ORE COMPOSITION:\n";
        if (ironAmount > 0)
        {
            compositionText += "IRON: " + (ironAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (clayAmount > 0)
        {
            compositionText += "CLAY: " + (clayAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (magnesiumAmount > 0)
        {
            compositionText += "MAGNESIUM: " + (magnesiumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (aluminumAmount > 0)
        {
            compositionText += "ALUMINUM: " + (aluminumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (nickelAmount > 0)
        {
            compositionText += "NICKEL: " + (nickelAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (cobaltAmount > 0)
        {
            compositionText += "COBALT: " + (cobaltAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (carbonAmount > 0)
        {
            compositionText += "CARBON: " + (carbonAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (iceAmount > 0)
        {
            compositionText += "ICE: " + (iceAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (hydrogenAmount > 0)
        {
            compositionText += "HYDROGEN: " + (hydrogenAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (helium3Amount > 0)
        {
            compositionText += "HELIUM-3: " + (helium3Amount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (diamondAmount > 0)
        {
            compositionText += "DIAMOND: " + (diamondAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (platiniumAmount > 0)
        {
            compositionText += "PLATINIUM: " + (platiniumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        if (plutoniumAmount > 0)
        {
            compositionText += "PLUTONIUM: " + (plutoniumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
        }
        compositionText += "\nPREDICTED OVERALL VALUE: $" + Mathf.Round(totalValue);
        deathPercent = Random.Range(0, 15f);
        asteroidHealthToShare = Mathf.Round(health / maxHealth) * 100;
        ogMeltSpeed = meltSpeed;
    }

    private void AddOre(float laserIntensity)
    {
        oreCollectedString = "";
        laserIntensity /= 100f;
        float ogValue = console.calculateCargoValue();

        float baseMiningRate = 0.05f;
        float laserBonus = 1f + (laserIntensity * 2f);
        float effectiveMiningRate = baseMiningRate * laserBonus;

        // Group 1 – Low melting point
        if (laserIntensity >= 0.0f && laserIntensity <= 0.25f)
        {
            float minedIce = Mathf.Min(iceAmount, iceAmount * effectiveMiningRate);
            float minedHydrogen = Mathf.Min(hydrogenAmount, hydrogenAmount * effectiveMiningRate);
            float minedClay = Mathf.Min(clayAmount, clayAmount * effectiveMiningRate);

            if (console.cargoHold + minedIce <= console.maxCargoHold) console.iceHeld += minedIce;
            if (console.cargoHold + minedHydrogen <= console.maxCargoHold) console.hydrogenHeld += minedHydrogen;
            if (console.cargoHold + minedClay <= console.maxCargoHold) console.clayHeld += minedClay;

            if (minedIce >= 1) oreCollectedString += "+" + Mathf.Round(minedIce * 10) / 10 + " ICE\n";
            if (minedHydrogen >= 1) oreCollectedString += "+" + Mathf.Round(minedHydrogen * 10) / 10 + " HYDROGEN\n";
            if (minedClay >= 1) oreCollectedString += "+" + Mathf.Round(minedClay * 10) / 10 + " CLAY\n";

            iceAmount -= minedIce;
            hydrogenAmount -= minedHydrogen;
            clayAmount -= minedClay;
        }

        // Group 2 – Medium melting point
        if (laserIntensity >= 0.2f && laserIntensity <= 0.55f)
        {
            float minedMagnesium = Mathf.Min(magnesiumAmount, magnesiumAmount * effectiveMiningRate);
            float minedAluminum = Mathf.Min(aluminumAmount, aluminumAmount * effectiveMiningRate);
            float minedIron = Mathf.Min(ironAmount, ironAmount * effectiveMiningRate);
            float minedIce = Mathf.Min(iceAmount, iceAmount * effectiveMiningRate);
            float minedHydrogen = Mathf.Min(hydrogenAmount, hydrogenAmount * effectiveMiningRate);
            float minedClay = Mathf.Min(clayAmount, clayAmount * effectiveMiningRate);

            if (console.cargoHold + minedMagnesium <= console.maxCargoHold) console.magnesiumHeld += minedMagnesium;
            if (console.cargoHold + minedAluminum <= console.maxCargoHold) console.aluminumHeld += minedAluminum;
            if (console.cargoHold + minedIron <= console.maxCargoHold) console.ironHeld += minedIron;

            if (minedMagnesium >= 1) oreCollectedString += "+" + Mathf.Round(minedMagnesium * 10) / 10 + " MAGNESIUM\n";
            if (minedIron >= 1) oreCollectedString += "+" + Mathf.Round(minedIron / 10) * 10 + " IRON\n";
            if (minedAluminum >= 1) oreCollectedString += "+" + Mathf.Round(minedAluminum * 10) / 10 + " ALUMINUM\n";

            magnesiumAmount -= minedMagnesium;
            aluminumAmount -= minedAluminum;
            ironAmount -= minedIron;
            iceAmount -= minedIce;
            hydrogenAmount -= minedHydrogen;
            clayAmount -= minedClay;
        }

        // Group 3 – High melting point
        if (laserIntensity >= 0.5f && laserIntensity <= 0.8f)
        {
            float minedNickel = Mathf.Min(nickelAmount, nickelAmount * effectiveMiningRate);
            float minedCobalt = Mathf.Min(cobaltAmount, cobaltAmount * effectiveMiningRate);
            float minedMagnesium = Mathf.Min(magnesiumAmount, magnesiumAmount * effectiveMiningRate);
            float minedAluminum = Mathf.Min(aluminumAmount, aluminumAmount * effectiveMiningRate);
            float minedIron = Mathf.Min(ironAmount, ironAmount * effectiveMiningRate);
            float minedIce = Mathf.Min(iceAmount, iceAmount * effectiveMiningRate);
            float minedHydrogen = Mathf.Min(hydrogenAmount, hydrogenAmount * effectiveMiningRate);
            float minedCarbon = Mathf.Min(carbonAmount, carbonAmount * effectiveMiningRate);
            float minedClay = Mathf.Min(clayAmount, clayAmount * effectiveMiningRate);

            if (console.cargoHold + minedNickel <= console.maxCargoHold) console.nickelHeld += minedNickel;
            if (console.cargoHold + minedCobalt <= console.maxCargoHold) console.cobaltHeld += minedCobalt;
            if (console.cargoHold + minedCarbon <= console.maxCargoHold) console.carbonHeld += minedCarbon;

            if (minedNickel >= 1) oreCollectedString += "+" + Mathf.Round(minedNickel * 10) / 10 + " NICKEL\n";
            if (minedCobalt >= 1) oreCollectedString += "+" + Mathf.Round(minedCobalt * 10) / 10 + " COBALT\n";
            if (minedCarbon >= 1) oreCollectedString += "+" + Mathf.Round(minedCarbon * 10) / 10 + " CARBON\n";

            magnesiumAmount -= minedMagnesium;
            aluminumAmount -= minedAluminum;
            ironAmount -= minedIron;
            iceAmount -= minedIce;
            hydrogenAmount -= minedHydrogen;
            carbonAmount -= minedCarbon;
            clayAmount -= minedClay;
            nickelAmount -= minedNickel;
            cobaltAmount -= minedCobalt;
        }

        // Group 4 – Extreme melting point
        if (laserIntensity >= 0.75f && laserIntensity <= 1.0f)
        {
            float minedNickel = Mathf.Min(nickelAmount, nickelAmount * effectiveMiningRate);
            float minedCobalt = Mathf.Min(cobaltAmount, cobaltAmount * effectiveMiningRate);
            float minedMagnesium = Mathf.Min(magnesiumAmount, magnesiumAmount * effectiveMiningRate);
            float minedAluminum = Mathf.Min(aluminumAmount, aluminumAmount * effectiveMiningRate);
            float minedIron = Mathf.Min(ironAmount, ironAmount * effectiveMiningRate);
            float minedIce = Mathf.Min(iceAmount, iceAmount * effectiveMiningRate);
            float minedHydrogen = Mathf.Min(hydrogenAmount, hydrogenAmount * effectiveMiningRate);
            float minedCarbon = Mathf.Min(carbonAmount, carbonAmount * effectiveMiningRate);
            float minedClay = Mathf.Min(clayAmount, clayAmount * effectiveMiningRate);

            //STUFF WE BE MINING
            float minedHelium3 = Mathf.Min(helium3Amount, helium3Amount * effectiveMiningRate);
            float minedDiamond = Mathf.Min(diamondAmount, diamondAmount * effectiveMiningRate);
            float minedPlatinium = Mathf.Min(platiniumAmount, platiniumAmount * effectiveMiningRate);
            float minedPlutonium = Mathf.Min(plutoniumAmount, plutoniumAmount * effectiveMiningRate);

            if (console.cargoHold + minedHelium3 <= console.maxCargoHold) console.helium3Held += minedHelium3;
            if (console.cargoHold + minedDiamond <= console.maxCargoHold) console.diamondHeld += minedDiamond;
            if (console.cargoHold + minedPlatinium <= console.maxCargoHold) console.platiniumHeld += minedPlatinium;
            if (console.cargoHold + minedPlutonium <= console.maxCargoHold) console.plutoniumHeld += minedPlutonium;

            if (minedHelium3 >= 1) oreCollectedString += "+" + Mathf.Round(minedHelium3 * 10) / 10 + " HELIUM-3\n";
            if (minedDiamond >= 1) oreCollectedString += "+" + Mathf.Round(minedDiamond * 10) / 10 + " DIAMOND\n";
            if (minedPlatinium >= 1) oreCollectedString += "+" + Mathf.Round(minedPlatinium * 10) / 10 + " PLATINIUM\n";
            if (minedPlutonium >= 1) oreCollectedString += "+" + Mathf.Round(minedPlutonium * 10) / 10 + " PLUTONIUM\n";

            //DESTROY THE WEAKER ORE!
            helium3Amount -= minedHelium3;
            diamondAmount -= minedDiamond;
            platiniumAmount -= minedPlatinium;
            plutoniumAmount -= minedPlutonium;
            magnesiumAmount -= minedMagnesium;
            aluminumAmount -= minedAluminum;
            ironAmount -= minedIron;
            iceAmount -= minedIce;
            hydrogenAmount -= minedHydrogen;
            carbonAmount -= minedCarbon;
            clayAmount -= minedClay;
            nickelAmount -= minedNickel;
            cobaltAmount -= minedCobalt;
        }

        if (console.calculateCargoValue() > ogValue)
        {
            if (console.cargoHold < console.maxCargoHold)
            {
                resourceCollectedSFX[Random.Range(0, resourceCollectedSFX.Count)].Play();
                oreCollectedText.text = oreCollectedString;
                StartCoroutine(textPopUpThing());
            }
        }
    }

    private void Update()
    {
        // Fixed tick rate calculation - higher laser intensity = faster mining
        float laserEfficiency = Mathf.Max(1f, gun.laserIntensity / 10f); // Prevent division by very small numbers
        tickRate = maxTickRate / laserEfficiency;
        tickRate = Mathf.Clamp(tickRate, 1f, 2f); // Much faster range

        if (gun.isMining && gun.laserOnTimer >= 1.5f)
        {
            laserTouching = true;
            meltSpeed = ogMeltSpeed * gun.laserIntensity / 100f;
        }

        else if (!gun.isMining)
        {
            laserTouching = false;
            meltSpeed = ogMeltSpeed;
            if (gun.Target == gameObject)
            {         
                totalValue = 0f;
                totalValue += ironAmount * 0.8f;
                totalValue += clayAmount * 0.4f;
                totalValue += magnesiumAmount * 2f;
                totalValue += aluminumAmount * 2.5f;
                totalValue += nickelAmount * 30f;
                totalValue += cobaltAmount * 80f;
                totalValue += carbonAmount * 5f;
                totalValue += iceAmount * 1.5f;
                totalValue += hydrogenAmount * 3f;
                totalValue += helium3Amount * 3000f;
                totalValue += diamondAmount * 15000f;
                totalValue += platiniumAmount * 25000f;
                totalValue += plutoniumAmount * 50000f;

                totalStuffAmount = ironAmount + clayAmount + magnesiumAmount + aluminumAmount + nickelAmount + cobaltAmount + carbonAmount + iceAmount + hydrogenAmount + helium3Amount + diamondAmount + platiniumAmount + plutoniumAmount;

                compositionText = "ORE COMPOSITION:\n";
                if (ironAmount > 0)
                {
                    compositionText += "IRON: " + (ironAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (clayAmount > 0)
                {
                    compositionText += "CLAY: " + (clayAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (magnesiumAmount > 0)
                {
                    compositionText += "MAGNESIUM: " + (magnesiumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (aluminumAmount > 0)
                {
                    compositionText += "ALUMINUM: " + (aluminumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (nickelAmount > 0)
                {
                    compositionText += "NICKEL: " + (nickelAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (cobaltAmount > 0)
                {
                    compositionText += "COBALT: " + (cobaltAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (carbonAmount > 0)
                {
                    compositionText += "CARBON: " + (carbonAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (iceAmount > 0)
                {
                    compositionText += "ICE: " + (iceAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (hydrogenAmount > 0)
                {
                    compositionText += "HYDROGEN: " + (hydrogenAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (helium3Amount > 0)
                {
                    compositionText += "HELIUM-3: " + (helium3Amount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (diamondAmount > 0)
                {
                    compositionText += "DIAMOND: " + (diamondAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (platiniumAmount > 0)
                {
                    compositionText += "PLATINIUM: " + (platiniumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                if (plutoniumAmount > 0)
                {
                    compositionText += "PLUTONIUM: " + (plutoniumAmount / totalStuffAmount * 100f).ToString("F1") + "%\n";
                }
                compositionText += "\nPREDICTED OVERALL VALUE: $" + Mathf.Round(totalValue);
                PopUpText.text = compositionText;
            }
        }

        if (health == maxHealth && asteroidMat.HasFloat("_BlendFactor"))
        {
            asteroidMat.SetFloat("_BlendFactor", 0f);
        }

        if (laserTouching)
        {
            PopUp.GetComponent<Animator>().SetBool("shouldShow", false);
            if (health < maxHealth && gameObject == gun.Target && asteroidMat.HasFloat("_BlendFactor"))
            {
                totalBlend += meltSpeed * Time.deltaTime;
                totalBlend = Mathf.Clamp01((float)totalBlend);
                asteroidMat.SetFloat("_BlendFactor", totalBlend);
            }
            timer1 += Time.deltaTime;
            if (timer1 >= tickRate && gun.Target == gameObject) //This makes sure that we only add to our balance once. Otherwise we get an extra reward for every other asteroid in the scene.
            {
                //GameObject temp = Instantiate(resourceCollectedImage.gameObject, canvas);
                //GameObject temp2 = Instantiate(harvestedText.gameObject, hudPos);
                //temp.SetActive(true);
                //temp2.SetActive(true);
                //Destroy(temp, 2f);
                //Destroy(temp2, 1.7f);
                AddOre(gun.laserIntensity);
                //laseredValue = CalculateLaseredValue(gun.laserIntensity);
                //console.cargoValue += (laseredValue * (1 + (console.laserDamage * 0.08f)));
                float damage = Mathf.Clamp(gun.laserIntensity / 10, 25f, 95f);
                health -= damage;

                asteroidHealthToShare = Mathf.Round((health / maxHealth) * 100f);
                timer1 = 0f;
            }
        }
        else if (!laserTouching && health < maxHealth)
        {
            timer1 = 0f;
            if (asteroidMat.HasFloat("_BlendFactor"))
            {
                totalBlend -= meltSpeed * Time.deltaTime;
                totalBlend = Mathf.Clamp01((float)totalBlend);
                asteroidMat.SetFloat("_BlendFactor", totalBlend);
            }
        }
        if (asteroidHealthToShare <= deathPercent && isDestroyed == false && gameObject == gun.Target)
        {
            timer1 = 0f;
            isDestroyed = true;
            gun.unLock();
            cameraShake.shakeFactor = (100f / gun.Distance);
            StartCoroutine(cameraShake.SHAKE());
            healthScript.health -= Mathf.Round((maxHealth / gun.Distance) * 3f);
            if (asteroidMat.HasFloat("_BlendFactor"))
            {
                asteroidMat.SetFloat("_BlendFactor", 0f);
            }
            GameObject temp = Instantiate(explosion.gameObject, gameObject.transform.position, Quaternion.identity);
            temp.SetActive(true);
            gameObject.GetComponent<MeshCollider>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, maxHealth);

            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(maxHealth * 5f, transform.position, maxHealth);
                }
            }
            gun.Target = null;
            gun.isMining = false;
            Destroy(gameObject, 1.1f);
        }
    }

    IEnumerator textPopUpThing()
    {
        oreCollectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.9f);
        oreCollectedText.gameObject.SetActive(false);
    }
}
