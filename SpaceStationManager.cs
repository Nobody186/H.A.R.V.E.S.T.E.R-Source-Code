using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SpaceStationManager : MonoBehaviour
{
    public ConsoleController console;

    [SerializeField] GameObject spaceStationUI;
    [SerializeField] GameObject repairUI;
    [SerializeField] TextMeshProUGUI repairBalanceCounter;
    [SerializeField] TextMeshProUGUI repairHullCounter;
    [SerializeField] TextMeshProUGUI repairCostCounter;
    [SerializeField] TextMeshProUGUI repairAmountCounter;
    [SerializeField] Slider repairChooser;
    [SerializeField] GameObject dockTransition;
    [SerializeField] GameObject dockButton;
    [SerializeField] TMP_Text balanceCounter;
    [SerializeField] TMP_Text cargoCounter;
    [SerializeField] TMP_Text badNews;
    [SerializeField] GameObject Player;
    [SerializeField] Transform SpawnArea;
    [SerializeField] TMP_Text errorMessage;
    [SerializeField] GameObject errorMessageGameObject;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] Health healthScript;
    public float profit;

    public float numberOfFees = 0f;
    public float feeCost = 0f;
    public float predeterminedOweAmount = 0f;
    public float totalFeeCost = 0f;

    public List<string> fees;
    public List<string> appliedFees;
    public List<string> appliedFeesAndCosts;
    public List<float> feeCosts;
    public float timer1 = 0f;
    public float tempFee = 0f;

    public string receipt;

    public bool calculating = false;
    bool subtractedBalance = false;

    float repairCost;

    [SerializeField] TextMeshProUGUI recordText;
    [SerializeField] GameObject moneyPopup;
    [SerializeField] List<AudioSource> repairVoicelines;
    [SerializeField] Transform canvas;

    public bool isInRange;

    private void Awake()
    {
        fees.Add("NON-ESSENTIAL LABOR GRATIFICATION TRIBUTE: ");
        fees.Add("NON-REGULAR BLOCKCHAIN MAINTENANCE FEE: ");
        fees.Add("LOW-INCOME RETRIBUTION TAX: ");
        fees.Add("DAILY OXYGEN SUPPLY LIMIT EXCEEDED FINE: ");
        fees.Add("PROVISIONAL NON-COMPLIANCE DISINCENTIVE FEE: ");
        fees.Add("INVOLUNTARY VOLUNTEER SERVICE CONTRIBUTION: ");
        fees.Add("'WE DONT STARVE' MEMBERSHIP FEE: ");
        fees.Add("DAILY HOLY CONTRIBUTION: ");
        fees.Add("EXCESSIVE LASER USAGE FINE: ");
        fees.Add("FLYING DURING DARK HOURS FINE: ");
        fees.Add("NOISE COMPLAINT FEE: ");
        fees.Add("FEE FEE: ");
        fees.Add("SPACE STATION TAX: ");
        fees.Add("LOCAL MARKET TAX: ");
        fees.Add("GALACTIC MARKET TAX: ");
        fees.Add("SECTION 514&3:2 FEES: ");
        fees.Add("NOT LISTENING TO STATE-SPONSORED MEDIA FINE: ");
        fees.Add("REGULAR MARKET TAX: ");
        fees.Add("AFTER MARKET TAX: ");
        fees.Add("COMMUNAL TOOTHBRUSH INFRASTRUCTURE PROJECT TAX: ");
        fees.Add("DAMAGE CONTROL FEE: ");
        fees.Add("SPACESHIP RENT: ");
        fees.Add("NOURISHMENT FINANCE PAYMENT: ");
        fees.Add("HYDRATION FINANCE PAYMENT: ");
        fees.Add("INTEREST TAX: ");
        fees.Add("POSSIBLE ALIEN-THREAT DAMAGE FEES: ");
        fees.Add("MASS-SURRVEILENCE MAINTENANCE CHARGE: ");
        fees.Add("ENGINE RUNNING PAYMENT: ");
        fees.Add("DOCKING FEE: ");
        fees.Add("OXY+ PAYMENT PLAN: ");
        fees.Add("DESTRUCTION OF PROPERTY FINE: ");
        fees.Add("WEAPONRY FINANCE TRIBUTE: ");
        fees.Add("READING THE FEES FEE: ");
        fees.Add("INTERGALACTIC TRANSPORTATION FEES: ");
        fees.Add("IMPORTANT PERSONNEL LUXURY BUDGET BIG-INATOR CONTRIBUTION: ");
        fees.Add("UNPAID OVERTIME INCENTIVE FEE: ");
        fees.Add("LOCAL SECTOR TAX: ");
        fees.Add("INTERGALATIC SECTOR TAX: ");
        fees.Add("VOLUNTARY DONATION FOR DEFENSE PURPOSES: ");

        if (SceneManager.GetActiveScene().name == "Market")
        {
            PlayerData data = SaveData.LoadPlayer();
            console.Balance = data.money;
            console.cargoValue = data.cargoValue;
            console.fusionCells = data.fusionCells;
            console.day = data.dayNumber;
            console.navUpgrade = data.canNavUpgrade;
            console.palUpgrade = data.canPalUpgrade;
            console.laserDmgLvl = data.laserDmgUpgrade;
            console.mainThrustLvl = data.mainThrustUpgrade;
            console.sideThrustLvl = data.sideThrustUpgrade;
            console.playerHealth = data.playerHealth;
            console.maxPlayerHealth = data.maxPlayerHealth;
            console.missileDamageLvl = data.missileDamage;
            console.missileRearmLvl = data.missileRearmTime;
            console.missileThrustLvl = data.missileThrust;
            console.missileTimeLvl = data.missileTime;
            console.missileTurnLvl = data.missileTurn;
            console.missileUpgrade = data.canMissileUpgrade;
            console.usesMouseMode = data.mouseAimMode;
            console.camoUpgrade = data.canCamoUpgrade;
            console.dashUpgrade = data.canDashUpgrade;
            console.dampDisabler = data.canDampUpgrade;
            console.radar3Upgrade = data.canRadar3Upgrade;
            console.usingCamoUpgrade = data.equipCamo;
            console.usingdampDisablerUpgrade = data.equipDamp;
            console.usingDashUpgrade = data.equipDash;
            console.usingPalUpgrade = data.equipPal;
            console.usingMissileUpgrade = data.equipMissile;
            console.usingRadar3Upgrade = data.equipRadar3Upgrade;
            if (console.waypointsToSave == null)
            {
                console.waypointsToSave = new List<Vector3>();
            }
            for (int i = 0; i < data.savedWaypointsX.Count; i++)
            {
                console.waypointsToSave.Add(new Vector3(data.savedWaypointsX[i], data.savedWaypointsY[i], data.savedWaypointsZ[i]));
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if(SceneManager.GetActiveScene().name == "DemoLevel")
        {
            PlayerData data = SaveData.LoadPlayer();
            console.Balance = data.money;
            console.cargoValue = data.cargoValue;
            console.fusionCells = data.fusionCells;
            console.day = data.dayNumber;
            console.navUpgrade = data.canNavUpgrade;
            console.palUpgrade = data.canPalUpgrade;
            console.laserDmgLvl = data.laserDmgUpgrade;
            console.mainThrustLvl = data.mainThrustUpgrade;
            console.sideThrustLvl = data.sideThrustUpgrade;
            console.playerHealth = data.playerHealth;
            console.maxPlayerHealth = data.maxPlayerHealth;
            console.missileDamageLvl = data.missileDamage;
            console.missileRearmLvl = data.missileRearmTime;
            console.missileThrustLvl = data.missileThrust;
            console.missileTimeLvl = data.missileTime;
            console.missileTurnLvl = data.missileTurn;
            console.missileUpgrade = data.canMissileUpgrade;
            console.usesMouseMode = data.mouseAimMode;
            console.camoUpgrade = data.canCamoUpgrade;
            console.dashUpgrade = data.canDashUpgrade;
            console.dampDisabler = data.canDampUpgrade;
            console.radar3Upgrade = data.canRadar3Upgrade;
            console.usingCamoUpgrade = data.equipCamo;
            console.usingdampDisablerUpgrade = data.equipDamp;
            console.usingDashUpgrade = data.equipDash;
            console.usingPalUpgrade = data.equipPal;
            console.usingMissileUpgrade = data.equipMissile;
            console.usingRadar3Upgrade = data.equipRadar3Upgrade;
            if (console.waypointsToSave == null)
            {
                console.waypointsToSave = new List<Vector3>();
            }
            for (int i = 0; i < data.savedWaypointsX.Count; i++)
            {
                console.waypointsToSave.Add(new Vector3(data.savedWaypointsX[i], data.savedWaypointsY[i], data.savedWaypointsZ[i]));
            }
        }
        else if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            console.Balance = 0;
            console.cargoValue = 0;
            console.day = 0;
            console.navUpgrade = true;
            console.palUpgrade = true;
            console.usingPalUpgrade = true;
            console.usingMissileUpgrade = false;
            console.missileUpgrade = false;
            console.mainThrustLvl = 5;
            console.sideThrustLvl = 10;
            console.playerHealth = 150;
            console.maxHealthLvl = 5;
            console.maxPlayerHealth = 150;
            console.fusionCells = 13;
            console.earnedWaypoints.Clear();
        }
        else
        {
            PlayerData data = SaveData.LoadPlayer();
            console.highestDay = data.dayRecordNumber;
            recordText.text = "RECORD: " + console.highestDay + " DAYS";
        }
    }



    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 10) //If player enters
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            isInRange = false;
        }
    }

    private void Update()
    {
        balanceCounter.text = "BALANCE: $" + console.Balance;
        cargoCounter.text = "CARGO VALUE: $" + console.cargoValue;
        if (console.cargoValue == 0 && subtractedBalance == true)
        {
            calculating = false;
        }

        float upgradeMultiplierToCost = 0f;
        if(console.navUpgrade)
        {
            upgradeMultiplierToCost += 2f;
        }
        if(console.palUpgrade)
        {
            upgradeMultiplierToCost += 2f;
        }
        upgradeMultiplierToCost += console.mainThrustLvl;
        upgradeMultiplierToCost += console.sideThrustLvl;
        upgradeMultiplierToCost += console.laserDmgLvl;

        repairChooser.maxValue = console.maxPlayerHealth - console.playerHealth;
        repairChooser.minValue = 0;
        if (upgradeMultiplierToCost > 10)
        {
            repairCost = repairChooser.value * upgradeMultiplierToCost;
        }
        else
        {
            repairCost = repairChooser.value * 10f;
        }
        repairAmountCounter.text = "REPAIR AMOUNT: " +  repairChooser.value;
        repairBalanceCounter.text = "BALANCE: " + console.Balance;
        repairHullCounter.text = "HULL: " + console.playerHealth;
        repairCostCounter.text = "COST: $" + repairCost;
        
    }

    public void Dock()
    {
        if (console.quotaCompleted() && console.day != 0)
        {
            StartCoroutine(dock());
        }
        else if (console.day == 0 && healthScript.health <= 15 && console.ironHeld >= 50000)
        {
            StartCoroutine(dock());
        }
        else
        {
            StartCoroutine(showDockError());
        }
    }

    public void Repair()
    {
        if (console.Balance >= repairCost)
        {
            console.playerHealth += Mathf.RoundToInt(repairChooser.value);
            healthScript.health = console.playerHealth;
            console.Balance -= repairCost;
            SaveData.SaveInfo(console);
        }
    }

    public void Sell()
    {
        if (console.cargoValue > 0 && !calculating)
        {
            calculating = true;
            //Determine how much money we want to steal from the player.
            if (console.cargoValue > 1000000f)
            {
                predeterminedOweAmount = console.cargoValue * 0.95f;
            }
            else if(console.cargoValue < 110000f)
            {
                predeterminedOweAmount = console.cargoValue * 0.5f;
            }
            else
            {
                predeterminedOweAmount = console.cargoValue * 0.8f;
            }
            while (predeterminedOweAmount > totalFeeCost) //Create a list of values to deduct from the player until we reach the value we agreed on
            {
                tempFee = Random.Range(1000, 50000);
                feeCosts.Add(tempFee);
                totalFeeCost += tempFee;
            }

            if (appliedFees.Count > 0) //If we have any fees from a previous run, remove them.
            {
                for (int i = 0; i < appliedFees.Count; i++)
                {
                    appliedFees.Remove(appliedFees[i]);
                }
            }
            numberOfFees = feeCosts.Count; //How many fees we will need

            for (int i = 0; i < numberOfFees; i++) //For as many fees as we have, give each one a name and add it to our receipt.
            {
                appliedFees.Add(fees[Random.Range(0, fees.Count)]);
                appliedFeesAndCosts.Add(string.Join("$", appliedFees[i], feeCosts[i]));
            }

            for (int i = 0; i < appliedFeesAndCosts.Count; i++)
            {
                receipt = string.Join("\n", receipt, appliedFeesAndCosts[i]);
            }
            StartCoroutine("writeFees");
            StartCoroutine("subtractBalance");
            StartCoroutine("subtractCargoAmount");
        }
    }
    public void Leave()
    {
        if (calculating == false)
        {
            console.day += 1;
            SaveData.SaveInfo(console);
            SceneManager.LoadScene("DemoLevel");
        }
    }

    public void OpenUpgradeMenu()
    {
        if (calculating == false)
        {
            upgradeMenu.SetActive(true);
        }
    }
    
    public void OpenRepairMenu()
    {
        if (calculating == false)
        {
            spaceStationUI.GetComponent<Animator>().enabled = true;
            spaceStationUI.GetComponent<Animator>().SetBool("showRepair", true);
            spaceStationUI.GetComponent<Animator>().SetBool("showStation", false);
        }
    }

    public void CloseRepairMenu()
    {
        spaceStationUI.GetComponent<Animator>().enabled = true;
        spaceStationUI.GetComponent<Animator>().SetBool("showRepair", false);
        spaceStationUI.GetComponent<Animator>().SetBool("showStation", true);
    }

    IEnumerator writeFees()
    {
        string thingToPrint = "";
        int charPerFrame = 7;
        int charsPrinted = 0;
        foreach (char c in receipt)
        {
            thingToPrint += c;
            badNews.text = thingToPrint;
            charsPrinted++;
            if (charsPrinted >= charPerFrame)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    IEnumerator subtractCargoAmount()
    {
        while(console.cargoValue != 0)
        {
            console.cargoValue -= 10000;
            if (console.cargoValue < 0)
            {
                console.cargoValue = 0;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator subtractBalance()
    {
        float originalBalance = console.Balance;
        float targetBalance = console.Balance + (console.cargoValue-totalFeeCost);
        subtractedBalance = false;
        while (console.Balance != targetBalance)
        {
            console.Balance += 500;
            if (console.Balance >= targetBalance)
            {
                if (console.Balance < originalBalance)
                {
                    console.Balance = originalBalance + 500;
                    targetBalance = console.Balance;
                }
                else
                {
                    console.Balance = targetBalance;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        subtractedBalance = true;
    }

    IEnumerator dock()
    {
        dockTransition.SetActive(true);
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            SaveData.SaveInfo(console);
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Market");
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Menu");
        }
    }
    IEnumerator showDockError()
    {
        if(!console.quotaCompleted())
        {
            errorMessageGameObject.SetActive(true);
            errorMessage.text = "YOU HAVEN'T MET YOUR QUOTA!\nHOLD [TAB] TO CHECK YOUR QUOTAS.";
            yield return new WaitForSeconds(4f);
            errorMessageGameObject.SetActive(false);
        }
    }

    public void fastRepair()
    {
        StartCoroutine(playRepairLine());
    }

    IEnumerator playRepairLine()
    {
        if (healthScript.health == healthScript.maxHealth)
        {
            repairVoicelines[0].Play();
            yield return new WaitForSeconds(repairVoicelines[0].clip.length);
        }
        else
        {
            repairVoicelines[1].Play();
            yield return new WaitForSeconds(repairVoicelines[1].clip.length);
        }
        console.ironHeld = 0;
        console.helium3Held = 0;
        console.aluminumHeld = 0;
        console.carbonHeld = 0;
        console.clayHeld = 0;
        console.cobaltHeld = 0;
        console.diamondHeld = 0;
        console.hydrogenHeld = 0;
        console.iceHeld = 0;
        console.magnesiumHeld = 0;
        console.nickelHeld = 0;
        console.platiniumHeld = 0;
        console.plutoniumHeld = 0;
        console.Balance -= 5000;
        GameObject temp = Instantiate(moneyPopup, canvas);
        temp.SetActive(true);
        Destroy(temp, 1f);
        healthScript.health = healthScript.maxHealth;
    }

    public void buyFusionCell()
    {
        if (SceneManager.GetActiveScene().name == "Market")
        {
            console.Balance -= 1000;
            console.fusionCells += 1;
        }
        else
        {
            GameObject temp = Instantiate(moneyPopup, canvas);
            temp.SetActive(true);
            Destroy(temp, 1f);
            console.Balance -= 5000;
            console.fusionCells += 1;
        }
    }
}
