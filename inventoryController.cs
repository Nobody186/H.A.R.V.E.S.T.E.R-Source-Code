using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class inventoryController : MonoBehaviour
{

    [SerializeField] ConsoleController console;
    [SerializeField] PlayerController player;
    [SerializeField] GunController gun;

    [SerializeField] GameObject cell;
    [SerializeField] GameObject inventory;
    [SerializeField] Transform gridContent;

    [SerializeField] monitorManager monitor;

    string[] oreNames = {
    "iron", "nickel", "clay", "cobalt", "diamond", "plutonium",
    "magnesium", "aluminum", "platinium", "helium3", "hydrogen",
    "carbon", "ice"};

    List<GameObject> activeCells;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cell.SetActive(false);
    }

    float GetOreHeld(string ore)
    {
        switch (ore)
        {
            case "iron": return console.ironHeld;
            case "nickel": return console.nickelHeld;
            case "clay": return console.clayHeld;
            case "cobalt": return console.cobaltHeld;
            case "diamond": return console.diamondHeld;
            case "plutonium": return console.plutoniumHeld;
            case "magnesium": return console.magnesiumHeld;
            case "aluminum": return console.aluminumHeld;
            case "platinium": return console.platiniumHeld;
            case "helium3": return console.helium3Held;
            case "hydrogen": return console.hydrogenHeld;
            case "carbon": return console.carbonHeld;
            case "ice": return console.iceHeld;
            default: return 0;
        }
    }

    float GetOreMass(string ore)
    {
        switch (ore)
        {
            case "iron": return player.ironMass;
            case "nickel": return player.nickelMass;
            case "clay": return player.clayMass;
            case "cobalt": return player.cobaltMass;
            case "diamond": return player.diamondMass;
            case "plutonium": return player.plutoniumMass;
            case "magnesium": return player.magnesiumMass;
            case "aluminum": return player.aluminumMass;
            case "platinium": return player.platiniumMass;
            case "helium3": return player.helium3Mass;
            case "hydrogen": return player.hydrogenMass;
            case "carbon": return player.carbonMass;
            case "ice": return player.iceMass;
            default: return 0;
        }
    }

    float GetOreValue(string ore)
    {
        switch (ore)
        {
            case "iron": return console.ironValue;
            case "nickel": return console.nickelValue;
            case "clay": return console.clayValue;
            case "cobalt": return console.cobaltValue;
            case "diamond": return console.diamondValue;
            case "plutonium": return console.plutoniumValue;
            case "magnesium": return console.magnesiumValue;
            case "aluminum": return console.aluminumValue;
            case "platinium": return console.platiniumValue;
            case "helium3": return console.helium3Value;
            case "hydrogen": return console.hydrogenValue;
            case "carbon": return console.carbonValue;
            case "ice": return console.iceValue;
            default: return 0;
        }
    }

    bool CellExists(string ore)
    {
        foreach (GameObject c in activeCells)
        {
            if (c == null) continue;

            string id = c.transform.Find("OreIdentifier").GetComponent<TextMeshProUGUI>().text.ToLower();
            if (id == ore.ToLower())
                return true;
        }
        return false;
    }

    GameObject GetExistingCell(string ore)
    {
        foreach (GameObject c in activeCells)
        {
            if (c == null) continue;

            string id = c.transform.Find("OreIdentifier").GetComponent<TextMeshProUGUI>().text.ToLower();
            if (id == ore.ToLower())
                return c;
        }
        return null;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            monitor.openMonitor();
            monitor.loadApp(inventory);
        }

        if (inventory.activeSelf)
        {
            if (activeCells == null)
                activeCells = new List<GameObject>();

            foreach (string ore in oreNames)
            {
                float held = GetOreHeld(ore);
                if (held <= 1) continue;

                GameObject existingCell = GetExistingCell(ore);
                if (existingCell != null)
                {
                    // Update data if cell already exists
                    existingCell.transform.Find("inCargo").GetComponent<TextMeshProUGUI>().text = "CARGO HOLD:\n" + Mathf.Round(held/1000) + "K ORE";
                    existingCell.transform.Find("Mass").GetComponent<TextMeshProUGUI>().text = "MASS:\n" + Mathf.Round((held * GetOreMass(ore))/1000) + "K LBS";
                    existingCell.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = "VALUE:\n$" + Mathf.Round((held * GetOreValue(ore))/1000) + "K";
                }
                else
                {
                    // Create new cell
                    GameObject temp = Instantiate(cell, gridContent);
                    temp.SetActive(true);
                    activeCells.Add(temp);

                    temp.transform.Find("OreIdentifier").GetComponent<TextMeshProUGUI>().text = ore.ToUpper();
                    temp.transform.Find("inCargo").GetComponent<TextMeshProUGUI>().text = Mathf.Round(held).ToString();
                    temp.transform.Find("Mass").GetComponent<TextMeshProUGUI>().text = Mathf.Round((held * GetOreMass(ore))).ToString();
                    temp.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = Mathf.Round((held * GetOreValue(ore))).ToString();

                    Button jettisonButton = temp.transform.Find("JettisonOre").GetComponent<Button>();
                    jettisonButton.onClick.AddListener(() => Jettison(temp));
                }
            }
        }
    }

    public void Jettison(GameObject cell)
    {
        string ore = cell.transform.Find("OreIdentifier").GetComponent<TextMeshProUGUI>().text.ToLower();
        float amount = GetOreHeld(ore);

        switch (ore)
        {
            case "iron": console.ironHeld -= amount; break;
            case "nickel": console.nickelHeld -= amount; break;
            case "clay": console.clayHeld -= amount; break;
            case "cobalt": console.cobaltHeld -= amount; break;
            case "diamond": console.diamondHeld -= amount; break;
            case "plutonium": console.plutoniumHeld -= amount; break;
            case "magnesium": console.magnesiumHeld -= amount; break;
            case "aluminum": console.aluminumHeld -= amount; break;
            case "platinium": console.platiniumHeld -= amount; break;
            case "helium3": console.helium3Held -= amount; break;
            case "hydrogen": console.hydrogenHeld -= amount; break;
            case "carbon": console.carbonHeld -= amount; break;
            case "ice": console.iceHeld -= amount; break;
            default: Debug.LogWarning("Unknown ore type: " + ore); return;
        }

        Destroy(cell); // bring the wrath upon the UI element
    }

    public void closeInventoryFunc()
    {
        StartCoroutine(closeInventory());
    }

    IEnumerator closeInventory()
    {
        inventory.GetComponent<Animator>().SetBool("shouldShow", false);
        yield return new WaitForSeconds(1f);
        inventory.SetActive(false);
    }
}
