using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class monitorManager : MonoBehaviour
{
    [SerializeField] ParticleSystem emitter;
    [SerializeField] GameObject frame;
    [SerializeField] GameObject orb;
    [SerializeField] GameObject loadingScreen;

    Animator monitorAnimator;
    Animator orbAnimatior;

    bool monitorOn = false;

    private GameObject appToLoad;
    private GameObject appLoaded;
    float timer = 0;

    bool loadingApp = false;

    //For nav menu
    [SerializeField] ConsoleController console;
    [SerializeField] TextMeshProUGUI waypointLabel;
    [SerializeField] TextMeshProUGUI fusionCellCounter;
    bool awaitingConfirmation = false;
    [SerializeField] TextMeshProUGUI warpButtonText;
    [SerializeField] GameObject errorPanel;

    //For scan menu
    [SerializeField] GunController gun;
    [SerializeField] GameObject scanButton;
    [SerializeField] Animator scanAnimator;
    [SerializeField] TextMeshProUGUI compositionText;
    [SerializeField] TextMeshProUGUI waitForScanText;
    [SerializeField] GameObject waitForScanPanel;

    //For data menu
    [SerializeField] GameObject warningText;
    [SerializeField] GameObject messagePanelText;
    [SerializeField] GameObject dataDownloaded;

    //For station menu
    [SerializeField] GameObject getCloserWarning;
    [SerializeField] GameObject stationPanel;
    [SerializeField] SpaceStationManager station;
    [SerializeField] TextMeshProUGUI quotaText;
    public bool downloadedData = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        monitorAnimator = frame.GetComponent<Animator>();
        orbAnimatior = orb.GetComponent<Animator>();
        emitter.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && timer > 1)
        {
            timer = 0;
            monitorOn = !monitorOn;
            if (monitorOn)
            {
                emitter.Play();
                frame.SetActive(true);
                orb.SetActive(true);
                monitorAnimator.SetBool("shouldShow", true);
                orbAnimatior.SetBool("shouldShow", true);
            }
            else
            {
                StartCoroutine(closeMonitor());
                appLoaded.SetActive(false);
            }
        }

        if (monitorOn && appLoaded != null && appLoaded.name == "navInfo")
        {
            handleNavMenuLogic();
        }
        if (monitorOn && appLoaded != null && appLoaded.name == "ScanPopup")
        {
            handleScanApp();
        }
        if (monitorOn && appLoaded != null && appLoaded.name == "DataDownloader")
        {
            print("HANDLING DATA");
            handleData();
        }
        if(monitorOn && appLoaded != null && appLoaded.name == "StationApp")
        {
            handleStation();
        }
    }

    public void openMonitor()
    {
        if (!monitorOn)
        {
            monitorOn = true;
            emitter.Play();
            frame.SetActive(true);
            orb.SetActive(true);
            monitorAnimator.SetBool("shouldShow", true);
            orbAnimatior.SetBool("shouldShow", true);
        }
    }

    public IEnumerator closeMonitor()
    {
        emitter.Stop();
        monitorOn = false;
        monitorAnimator.SetBool("shouldShow", false);
        orbAnimatior.SetBool("shouldShow", false);
        yield return new WaitForSeconds(0.2f);
        appLoaded.SetActive(false);
        yield return new WaitForSeconds(0.8f);
        frame.SetActive(false);
        orb.SetActive(false);
        appLoaded = null;
        appToLoad = null;
        loadingApp = false;
        downloadedData = false;
        if (!console.isWarping && console.canWarp)
        {
            console.navMode = false;
        }
    }

    public void loadApp(GameObject app)
    {
        if (!loadingApp && app != appLoaded)
        {
            appToLoad = app;
            StartCoroutine(loadApp());
        }
    }

    IEnumerator loadApp()
    {
        loadingApp = true;
        loadingScreen.SetActive(true);
        if (appLoaded != null)
        {
            appLoaded.SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);

        appToLoad.SetActive(true);

        if (appToLoad.name == "ScanPopup")
        {
            waitForScanPanel.SetActive(true);
            if (gun.Target == null)
            {
                scanButton.SetActive(false);
            }
            else
            {
                scanButton.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.4f);
        loadingScreen.SetActive(false);
        appLoaded = appToLoad;
        loadingApp = false;
    }

    //App-specific functions
    public void InitiateWarp()
    {
        if (awaitingConfirmation && !console.isWarping && console.fusionCells >= 1)
        {
            StartCoroutine(console.Warp());
        }
        else if (!awaitingConfirmation && !console.isWarping && console.fusionCells >= 1)
        {
            StartCoroutine(WarpConfirmation());
        }
    }

    void handleNavMenuLogic()
    {
        console.navMode = true;
        if(console.isWarping)
        {
            errorPanel.SetActive(true);
        }
        else
        {
            errorPanel.SetActive(false);
        }
        if(console.currentWaypoint == 0)
        {
            waypointLabel.text = "\"" + console.waypoints[console.currentWaypoint].name + "\"\n!NO-MINING ZONE!";
        }
        else if(console.currentWaypoint == 1)
        {
            waypointLabel.text = "\"" + console.waypoints[console.currentWaypoint].name + "\"\nU-TYPE MINING ZONE";
        }
        else if(console.currentWaypoint == 2)
        {
            waypointLabel.text = "\"" + console.waypoints[console.currentWaypoint].name + "\"\nA/B-TYPE MINING ZONE";
        }
        else if( console.currentWaypoint == 3)
        {
            waypointLabel.text = "\"" + console.waypoints[console.currentWaypoint].name + "\"\nG-TYPE MINING ZONE";
        }
        else
        {
            waypointLabel.text = "\"" + console.waypoints[console.currentWaypoint].name + "\"\nC-TYPE MINING ZONE";
        }

        fusionCellCounter.text = "FUSION CELLS LEFT: " + console.fusionCells;
    }
    IEnumerator WarpConfirmation()
    {
        awaitingConfirmation = true;
        warpButtonText.text = "CONFIRM WARP?";
        warpButtonText.fontSize = 0.04f;
        yield return new WaitForSeconds(1);
        awaitingConfirmation = false;
        warpButtonText.fontSize = 0.05f;
        warpButtonText.text = "WARP";
    }

    public void rightWaypoint()
    {
        console.currentWaypoint += 1;
        if (console.currentWaypoint >= console.waypoints.Count)
        {
            console.currentWaypoint = 0;
        }
    }

    public void leftWaypoint()
    {
        console.currentWaypoint -= 1;
        if (console.currentWaypoint < 0)
        {
            console.currentWaypoint = console.waypoints.Count - 1;
        }
    }

    //SCAN LOGIC
    void handleScanApp()
    {
        if (gun.Target == null)
        {
            waitForScanPanel.SetActive(true);
            waitForScanText.text = "PLEASE LOCK A TARGET";
            scanButton.SetActive(false);
        }
        else if (gun.Target != null && waitForScanPanel.activeSelf)
        {
            waitForScanText.text = "READY TO SCAN";
            scanButton.SetActive(true);
        }
        else
        {
            waitForScanPanel.SetActive(false);
        }
    }

    public void Scan()
    {
        waitForScanPanel.SetActive(false);
        scanAnimator.Play("scanPopUp", -1, 0f);
    }

    //Data downloader logic
    void handleData()
    {
        if(gun.Target == null && console.navUpgrade)
        {
            print("NO TARGET, NAV UPGRADE. WARNING ON");
            warningText.SetActive(true);
            dataDownloaded.SetActive(false);
            messagePanelText.SetActive(false);
            warningText.GetComponent<TextMeshProUGUI>().text = "LOCK TARGET TO DOWNLOAD INFO";
        }
        else if(gun.Target == null && !console.navUpgrade)
        {
            print("NO TARGET, NO UPGRADE. WARNING ON");
            warningText.SetActive(true);
            dataDownloaded.SetActive(false);
            messagePanelText.SetActive(false);
            warningText.GetComponent<TextMeshProUGUI>().text = "APPLICATION UNAVAILABLE.\nPLEASE PURCHASE HIGH-FREQ ANTENNA SUBSCRIPTION PLAN";
        }
        else if(gun.Target != null && gun.Target.name.Contains("Data") && !downloadedData && console.navUpgrade)
        {
            print("DATA POD LOCKED. NOT DOWNLOADED YET. MESSAGE ON.");
            warningText.SetActive(false);
            dataDownloaded.SetActive(false);
            messagePanelText.SetActive(true);
        }
        else if(gun.Target != null && gun.Target.name.Contains("Data") && downloadedData && console.navUpgrade)
        {
            print("DATA POD LOCKED. DOWNLOADED. COMPLETION ON.");
            warningText.SetActive(false);
            dataDownloaded.SetActive(true);
            messagePanelText.SetActive(false);
        }
    }

    string FormatQuotaLine(string resourceName, float quota, float held)
    {
        if (quota <= 0) return ""; //Skip if no quota assigned

        bool isMet = held >= quota;
        string status = isMet ? "✓ " : "X ";
        string color = isMet ? "<color=green>" : "<color=white>";
        string endColor = "</color>";

        // Format numbers appropriately
        string quotaDisplay;
        string heldDisplay;

        if (quota >= 1000f)
        {
            quotaDisplay = Mathf.Round(quota / 1000f).ToString() + "K";
            heldDisplay = Mathf.Round(held / 1000f).ToString() + "K";
        }
        else // For small amounts like Helium-3
        {
            quotaDisplay = Mathf.Round(quota).ToString();
            heldDisplay = Mathf.Round(held).ToString();
        }

        if (isMet)
        {
            return color + status + resourceName + " QUOTA COMPLETED!" + endColor + "\n";
        }
        else
        {
            return color + status + "MINE " + quotaDisplay + " " + resourceName + " (" + heldDisplay + "/" + quotaDisplay + ")" + endColor + "\n";
        }
    }

    //For the station app
    void handleStation()
    {
        if(station.isInRange)
        {
            getCloserWarning.SetActive(false);
            stationPanel.SetActive(true);
        }
        else
        {
            getCloserWarning.SetActive(true);
            stationPanel.SetActive(false);
        }
        quotaText.text = "";
        quotaText.text += FormatQuotaLine("IRON", console.ironQuota, console.ironHeld);
        quotaText.text += FormatQuotaLine("CLAY", console.clayQuota, console.clayHeld);
        quotaText.text += FormatQuotaLine("ICE", console.iceQuota, console.iceHeld);
        quotaText.text += FormatQuotaLine("MAGNESIUM", console.magnesiumQuota, console.magnesiumHeld);
        quotaText.text += FormatQuotaLine("ALUMINUM", console.aluminumQuota, console.aluminumHeld);
        quotaText.text += FormatQuotaLine("HYDROGEN", console.hydrogenQuota, console.hydrogenHeld);
        quotaText.text += FormatQuotaLine("CARBON", console.carbonQuota, console.carbonHeld);
        quotaText.text += FormatQuotaLine("COBALT", console.cobaltQuota, console.cobaltHeld);
        quotaText.text += FormatQuotaLine("NICKEL", console.nickelQuota, console.nickelHeld);
        quotaText.text += FormatQuotaLine("HELIUM-3", console.helium3Quota, console.helium3Held);

        // Remove trailing newline
        quotaText.text = quotaText.text.TrimEnd('\n');
    }
}
