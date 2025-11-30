using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour
{
    [SerializeField] ConsoleController console;

    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gameSelect;
    [SerializeField] GameObject resumeGame;

    bool showResumeGame = true;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData data = SaveData.LoadPlayer();
        if(data.dayNumber == 0)
        {
            showResumeGame = false;
        }
    }

    public void GameSelect()
    {
        gameSelect.SetActive(true);
        if(showResumeGame )
        {
            resumeGame.SetActive(true);
        }
        else
        {
            resumeGame.SetActive(false);
        }
        mainMenu.SetActive(false);
    }

    public void Credits()
    {
        creditsScreen.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ReturnToMenu()
    {
        creditsScreen.SetActive(false);
        gameSelect.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void NewGame()
    {
        console.Balance = 0;
        console.cargoValue = 0;
        console.day = 0;
        console.navUpgrade = false;
        console.palUpgrade = false;
        console.missileUpgrade = false;
        console.mainThrustLvl = 0;
        console.sideThrustLvl = 0;
        console.laserDmgLvl = 0;
        console.maxPlayerHealth = 100;
        console.playerHealth = 100;
        console.maxHealthLvl = 0;
        console.missileDamageLvl = 0;
        console.missileRearmLvl = 0;
        console.missileThrustLvl = 0;
        console.missileTimeLvl = 0;
        console.missileTurnLvl = 0;
        console.earnedWaypoints.Clear();
        console.usingCamoUpgrade = false;
        console.usingdampDisablerUpgrade = false;
        console.usingDashUpgrade = false;
        console.usingMissileUpgrade = false;
        console.usingPalUpgrade = false;
        console.usingRadar3Upgrade = false;
        console.dampDisabler = false;
        console.dashUpgrade = false;
        console.camoUpgrade = false;
        console.radar3Upgrade = false;
        console.fusionCells = 10;
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
        SaveData.SaveInfo(console);
        StartCoroutine("LoadScene");
    }

    public void launchTutorial()
    {
        StartCoroutine("LoadTutorial");
    }

    public void LoadGame()
    {
        StartCoroutine("LoadScene");
    }

    IEnumerator LoadScene()
    {
        loadingScreen.SetActive(true);
        Time.timeScale = 0f; // freeze game (optional if scene is additive)
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
        operation.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(1f);

        // loading done — enable scene
        operation.allowSceneActivation = true;

        // re-enable game
        Time.timeScale = 1f;
        yield return null;
    }

    IEnumerator LoadTutorial()
    {
        loadingScreen.SetActive(true);
        Time.timeScale = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Tutorial"); // Load by name
        operation.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(1f);

        operation.allowSceneActivation = true;
        Time.timeScale = 1f;
        yield return null;
    }
}
