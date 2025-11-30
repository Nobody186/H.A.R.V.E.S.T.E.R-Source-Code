using UnityEngine;
using TMPro;

public class upgradeManager : MonoBehaviour
{

    //READ ME!!!!
    //When adding upgrades, make sure to do all the necessary paperwork in the PlayerData script, the loading script (for new game), and the Space Station Manager
    [SerializeField] ConsoleController console;
    [SerializeField] Health healthScript;
    [SerializeField] TextMeshProUGUI mainThrusterText;
    [SerializeField] TextMeshProUGUI laserDmgText;
    [SerializeField] TextMeshProUGUI sideThrusterText;

    [SerializeField] TextMeshProUGUI navModeText;
    [SerializeField] TextMeshProUGUI PALModeText;
    [SerializeField] TextMeshProUGUI DashText;
    [SerializeField] TextMeshProUGUI camoText;
    [SerializeField] TextMeshProUGUI dampText;
    [SerializeField] TextMeshProUGUI radar3Text;

    [SerializeField] TextMeshProUGUI maxHealthText;

    [SerializeField] TextMeshProUGUI missileText;
    [SerializeField] TextMeshProUGUI missileTurnText;
    [SerializeField] TextMeshProUGUI missileDamageText;
    [SerializeField] TextMeshProUGUI missileRearmText;
    [SerializeField] TextMeshProUGUI missileThrustText;
    [SerializeField] TextMeshProUGUI missileTimeText;

    [SerializeField] int baseCost = 5000;
    [SerializeField] TextMeshProUGUI balanceShower;
    [SerializeField] TextMeshProUGUI balanceShower2;

    [SerializeField] bool isPirateStation = false;
    [SerializeField] GameObject pirateStation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if (balanceShower != null)
            {
                balanceShower.text = "BAlANCE: $" + console.Balance;
            }
        if (balanceShower2 != null)
        {
            balanceShower2.text = "BALANCE: $" + console.Balance;
        }
            if (mainThrusterText != null)
            {
                mainThrusterText.text = "THRUSTERS\n$" + baseCost * (1 + console.mainThrustLvl) + "\nLVL" + console.mainThrustLvl;
            }
            if (sideThrusterText != null)
            {
                sideThrusterText.text = "AXIS THRUSTERS\n$" + baseCost * (1 + console.sideThrustLvl) + "\nLVL" + console.sideThrustLvl;
            }
            if (laserDmgText != null)
            {
                laserDmgText.text = "LASER DMG\n$" + baseCost * (1 + console.laserDmgLvl) + "\nLVL" + console.laserDmgLvl;
            }
            if (maxHealthText != null)
            {
                maxHealthText.text = "HULL INTEGRITY\n$" + baseCost * (1 + console.maxHealthLvl) + "\nLVL" + console.maxHealthLvl;
            }
            if (missileDamageText != null)
            {
                missileDamageText.text = "MISSILE DAMAGE\n$" + baseCost * (1 + console.missileDamageLvl) + "\nLVL" + console.missileDamageLvl;
            }
            if (missileRearmText != null)
            {
                missileRearmText.text = "MISSILE REARM TIME\n$" + baseCost * (1 + console.missileRearmLvl) + "\nLVL" + console.missileRearmLvl;
            }
            if (missileThrustText != null)
            {
                missileThrustText.text = "MISSILE PROPELLENT\n$" + baseCost * (1 + console.missileThrustLvl) + "\nLVL" + console.missileThrustLvl;
            }
            if (missileTimeText != null)
            {
                missileTimeText.text = "MISSILE BURN TIME\n$" + baseCost * (1 + console.missileTimeLvl) + "\nLVL" + console.missileTimeLvl;
            }
            if (missileTurnText != null)
            {
                missileTurnText.text = "MISSILE TURN SPEED\n$" + baseCost * (1 + console.missileTurnLvl) + "\nLVL" + console.missileTurnLvl;
            }

            if (console.navUpgrade && navModeText != null)
            {
                navModeText.text = "HIGH-FREQ ANTENNA\n PURCHASED!";
            }
            else if (!console.navUpgrade && navModeText != null)
            {
                navModeText.text = "HIGH-FREQ ANTENNA\n$5000";
            }
            if (console.palUpgrade && PALModeText != null && console.usingPalUpgrade)
            {
                PALModeText.text = "RADAR V2\n(EQUIPPED)";
            }
            else if (console.palUpgrade && PALModeText != null && !console.usingPalUpgrade)
            {
                PALModeText.text = "RADAR V2\n(UNEQUIPPED)";
            }
            else if (!console.palUpgrade && PALModeText != null)
            {
                PALModeText.text = "RADAR V2\n$10000";
            }
            if (console.missileUpgrade && missileText != null && console.usingMissileUpgrade)
            {
                missileText.text = "MISSILES\n(EQUIPPED)";
            }
            else if (console.missileUpgrade && missileText != null && !console.usingMissileUpgrade)
            {
                missileText.text = "MISSILES\n(UNEQUIPPED)";
            }
            else if (!console.missileUpgrade && missileText != null)
            {
                missileText.text = "MISSILES\n$25000";
            }
            if (console.dashUpgrade && DashText != null && console.usingDashUpgrade)
            {
                DashText.text = "SPACE DASH\n(EQUIPPED)";
            }
            else if (console.dashUpgrade && DashText != null && !console.usingDashUpgrade)
            {
                DashText.text = "SPACE DASH\n(UNEQUIPPED)";
            }
            else if (!console.dashUpgrade && DashText != null)
            {
                DashText.text = "SPACE DASH\n$25000";
            }
            if (console.camoUpgrade && camoText != null && console.usingCamoUpgrade)
            {
                camoText.text = "CAMOUFLAGE\n(EQUIPPED)";
            }
            else if (console.camoUpgrade && camoText != null && !console.usingCamoUpgrade)
            {
                camoText.text = "CAMOUFLAGE\n(UNEQUIPPED)";
            }
            else if (!console.camoUpgrade && camoText != null)
            {
                camoText.text = "CAMOUFLAGE\n$50000";
            }
            if (console.dampDisabler && dampText != null && console.usingdampDisablerUpgrade)
            {
                dampText.text = "DAMP DISABLER\n(EQUIPPED)";
            }
            else if (console.dampDisabler && dampText != null && !console.usingdampDisablerUpgrade)
            {
                dampText.text = "DAMP DISABLER\n(UNEQUIPPED)";
            }
            else if (!console.dampDisabler && dampText != null)
            {
                dampText.text = "DAMP DISABLER\n$50000";
            }
            if (console.radar3Upgrade && radar3Text != null && console.usingRadar3Upgrade)
            {
                radar3Text.text = "RADAR V3\n(EQUIPPED)";
            }
            else if (console.radar3Upgrade && radar3Text != null && !console.usingRadar3Upgrade)
            {
                radar3Text.text = "RADAR V3\n(UNEQUIPPED)";
            }
            else if (!console.radar3Upgrade && radar3Text != null)
            {
                radar3Text.text = "RADAR V3\n$50000";
            }
    }

    public void BuyUpgrade(string upgradeType)
    {
        if (upgradeType == "axis")
        {
            int requiredCost = baseCost * (1 + console.sideThrustLvl);
            if (console.Balance > requiredCost && console.sideThrustLvl < 10)
            {
                console.sideThrustLvl += 1;
                console.Balance -= requiredCost;
                SaveData.SaveInfo(console);
            }
        }
        else if (upgradeType == "main")
        {
            int requiredCost = baseCost * (1 + console.mainThrustLvl);
            if (console.Balance > requiredCost && console.mainThrustLvl < 10)
            {
                console.mainThrustLvl += 1;
                console.Balance -= requiredCost;
                SaveData.SaveInfo(console);
            }
        }
        else if (upgradeType == "laser")
        {
            int requiredCost = baseCost * (1 + console.laserDmgLvl);
            if (console.Balance > requiredCost && console.laserDmgLvl < 10)
            {
                console.laserDmgLvl += 1;
                console.Balance -= requiredCost;
                SaveData.SaveInfo(console);
            }
        }
        else if (upgradeType == "nav")
        {
            if (console.Balance > 5000 && !console.navUpgrade)
            {
                console.navUpgrade = true;
                console.Balance -= 5000;
                SaveData.SaveInfo(console);
            }
        }
        else if (upgradeType == "pal") //This is an equippable secondary, so it gets custom logic.
        {
            if (console.Balance > 10000 && !console.palUpgrade)
            {
                console.palUpgrade = true;
                console.Balance -= 10000;
                SaveData.SaveInfo(console);
            }
            if (console.palUpgrade)
            {
                if (console.usingPalUpgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = true;
                    console.usingMissileUpgrade = false;
                    console.usingDashUpgrade = false;
                    console.usingdampDisablerUpgrade = false;
                    console.usingCamoUpgrade = false;
                    console.usingRadar3Upgrade = false;
                    SaveData.SaveInfo(console);
                }
            }
        }
        else if (upgradeType == "health")
        {
            int requiredCost = baseCost * (1 + console.maxHealthLvl);
            if (console.Balance > requiredCost && console.maxHealthLvl < 10)
            {
                console.maxHealthLvl += 1;
                healthScript.health += 10;
                console.maxPlayerHealth += 10;
                console.playerHealth += 10;
                console.Balance -= requiredCost;
                SaveData.SaveInfo(console);
            }
        }
        else if (upgradeType == "missileTurn")
        {
            int requiredCost = baseCost * (1 + console.missileTurnLvl);
            if (console.Balance > requiredCost && console.missileTurnLvl < 10 && console.missileUpgrade)
            {
                console.missileTurnLvl += 1;
                console.Balance -= requiredCost;
            }
        }
        else if (upgradeType == "missile")
        {
            if (console.Balance > 25000 && !console.missileUpgrade)
            {
                console.missileUpgrade = true;
                console.Balance -= 25000;
                SaveData.SaveInfo(console);
            }
            if (console.palUpgrade)
            {
                if (console.usingMissileUpgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = false;
                    console.usingMissileUpgrade = true;
                    console.usingDashUpgrade = false;
                    console.usingdampDisablerUpgrade = false;
                    console.usingCamoUpgrade = false;
                }
            }
        }
        else if (upgradeType == "missileDamage")
        {
            int requiredCost = baseCost * (1 + console.missileDamageLvl);
            if (console.Balance > requiredCost && console.missileDamageLvl < 10 && console.missileUpgrade)
            {
                console.missileDamageLvl += 1;
                console.Balance -= requiredCost;
            }
        }
        else if (upgradeType == "missileRearm")
        {
            int requiredCost = baseCost * (1 + console.missileRearmLvl);
            if (console.Balance > requiredCost && console.missileRearmLvl < 10 && console.missileUpgrade)
            {
                console.missileRearmLvl += 1;
                console.Balance -= requiredCost;
            }
        }
        else if (upgradeType == "missileThrust")
        {
            int requiredCost = baseCost * (1 + console.missileThrustLvl);
            if (console.Balance > requiredCost && console.missileThrustLvl < 10 && console.missileUpgrade)
            {
                console.missileThrustLvl += 1;
                console.Balance -= requiredCost;
            }
        }
        else if (upgradeType == "missileTime")
        {
            int requiredCost = baseCost * (1 + console.missileTimeLvl);
            if (console.Balance > requiredCost && console.missileTimeLvl < 10 && console.missileUpgrade)
            {
                console.missileTimeLvl += 1;
                console.Balance -= requiredCost;
            }
        }
        else if(upgradeType == "camo")
        {
            if (console.Balance > 50000 && !console.camoUpgrade)
            {
                console.camoUpgrade = true;
                console.Balance -= 50000;
                SaveData.SaveInfo(console);
            }
            if (console.camoUpgrade)
            {
                if (console.usingCamoUpgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = false;
                    console.usingMissileUpgrade = false;
                    console.usingDashUpgrade = false;
                    console.usingdampDisablerUpgrade = false;
                    console.usingCamoUpgrade = true;
                    console.usingRadar3Upgrade = false;
                }
            }
        }
        else if (upgradeType == "damp")
        {
            if (console.Balance > 50000 && !console.dampDisabler)
            {
                console.dampDisabler = true;
                console.Balance -= 50000;
            }
            if (console.dampDisabler)
            {
                if (console.usingdampDisablerUpgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = false;
                    console.usingMissileUpgrade = false;
                    console.usingDashUpgrade = false;
                    console.usingdampDisablerUpgrade = true;
                    console.usingCamoUpgrade = false;
                    console.usingRadar3Upgrade = false;
                }
            }
        }
        else if (upgradeType == "radar3")
        {
            if (console.Balance > 50000 && !console.radar3Upgrade)
            {
                console.radar3Upgrade = true;
                console.Balance -= 50000;
            }
            if (console.radar3Upgrade)
            {
                if (console.usingRadar3Upgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = true;
                    console.usingMissileUpgrade = false;
                    console.usingDashUpgrade = false;
                    console.usingdampDisablerUpgrade = false;
                    console.usingCamoUpgrade = false;
                    console.usingRadar3Upgrade = true;
                }
            }
        }
        else if (upgradeType == "dash")
        {
            if (console.Balance > 25000 && !console.dashUpgrade)
            {
                console.dashUpgrade = true;
                console.Balance -= 25000;
            }
            if (console.dashUpgrade)
            {
                if (console.usingDashUpgrade)
                {
                    return;
                }
                else
                {
                    console.usingPalUpgrade = false;
                    console.usingMissileUpgrade = false;
                    console.usingDashUpgrade = true;
                    console.usingdampDisablerUpgrade = false;
                    console.usingCamoUpgrade = false;
                    console.usingRadar3Upgrade = false;
                    SaveData.SaveInfo(console);
                }
            }
        }
    }
}
