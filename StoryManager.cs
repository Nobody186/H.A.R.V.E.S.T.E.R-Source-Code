using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.VisualScripting;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    //For objectives
    [SerializeField] ConsoleController console;
    [SerializeField] TextMeshProUGUI objectiveTextbox;
    [SerializeField] Animator objectiveTextBoxAnimator;
    [SerializeField] List<string> objectives;
    public List<string> objectivesToPrint;
    private string allObjectives;

    //For pause menu
    [SerializeField] GameObject pauseMenu;
    bool isPaused = false;

    [SerializeField] GameObject settingsMenu;
    public Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionChooser;
    [SerializeField] Toggle fullScreenBox;

    [SerializeField] Toggle mouseMode;

    [SerializeField] GameObject tipsMenu;

    Resolution[] AllResolutions;
    bool fullScreened;
    int selectedRes;
    List<Resolution> resolutionsForScript = new List<Resolution>();
    private bool hasInitializedResolution = false;


    //For dialog
    [SerializeField] TextMeshProUGUI captions;
    [SerializeField] List<AudioSource> pirateChatters;
    [SerializeField] List<AudioSource> pirateAmbush;
    [SerializeField] AudioSource chaseMusic;
    [SerializeField] AudioSource lastMinuteMessage;
    [SerializeField] List<AudioSource> tutorialMessages;

    [SerializeField] GameObject appForTutorial;

    [SerializeField] GameObject objectiveAddedTip;
    [SerializeField] GameObject controlTip;
    private TextMeshProUGUI controlTipText;

    bool recentPirateAttackQuip = false;
    bool recentPirateAmbushQuip = false;

    bool alreadyDisabledCursor = false;

    [SerializeField] PlayerController player;
    bool isMoused;

    bool playingStoryMessage = false;

    bool showedWarpTip = false;
    bool showedWarpTip2 = false;
    bool showedWarpTip3 = false;

    bool showedBookTip = false;

    Vector2 OGSize = new Vector2();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        fullScreened = true;
        AllResolutions = Screen.resolutions;
        List<string> availableResolutions = new List<string>();
        string modifiedResolution;

        foreach(Resolution res in AllResolutions)
        {
            modifiedResolution = res.width.ToString() + "x " + res.height.ToString();
            if (!availableResolutions.Contains(modifiedResolution))
            {
                availableResolutions.Add(modifiedResolution);
                resolutionsForScript.Add(res);
            }
        }
        resolutionChooser.AddOptions(availableResolutions);

        if (console.ironQuota > 0)
        {
            objectivesToPrint.Add("MINE " + Mathf.Round(console.ironQuota / 1000f)+ "K IRON ORE");
        }
        if (console.cobaltQuota > 0)
        {
            objectivesToPrint.Add("MINE " + Mathf.Round(console.cobaltQuota / 1000f) + "K COBALT ORE");
        }
        if (console.iceQuota > 0)
        {
            objectivesToPrint.Add("MINE " + Mathf.Round(console.iceQuota / 1000f) + "K ICE");
        }
        if (console.nickelQuota > 0)
        {
            objectivesToPrint.Add("MINE " + Mathf.Round(console.nickelQuota / 1000f) + "K NICKEL ORE");
        }
        if (objectives.Count > 0)
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                objectivesToPrint.Add(objectives[i]);
            }
        }
        for(int i = 0; i < objectivesToPrint.Count; i++)
        {
            allObjectives += (objectivesToPrint[i] + "\n");
        }

        mouseMode.isOn = console.usesMouseMode;
        controlTipText = controlTip.GetComponentInChildren<TextMeshProUGUI>();
        Vector2 OGSize = controlTip.GetComponent<RectTransform>().sizeDelta;
    }

    void changeObjectiveColors()
    {
        for(int i = 0; i < objectives.Count; i++)
        {
            //IRON
            if (objectives[i].Contains("IRON") && console.ironHeld >= console.ironQuota)
            {
                objectives[i] = "IRON QUOTA MET!";
                allObjectives = allObjectives.Replace("MINE " + Mathf.Round(console.ironQuota / 1000f) + "K IRON ORE", "IRON QUOTA MET!");
            }
            else if (objectives[i].Contains("IRON") && console.ironHeld < console.ironQuota)
            {
                objectives[i] = "MINE " + Mathf.Round(console.ironQuota / 1000f) + "K IRON ORE";
                if (allObjectives.Contains("IRON QUOTA MET"))
                {
                    allObjectives = allObjectives.Replace("IRON QUOTA MET!", "MINE " + Mathf.Round(console.ironQuota / 1000f) + "K IRON ORE");
                }
            }

            // COBALT
            if (objectives[i].Contains("COBALT") && console.cobaltHeld >= console.cobaltQuota)
            {
                objectives[i] = "COBALT QUOTA MET!";
                allObjectives = allObjectives.Replace("MINE " + Mathf.Round(console.cobaltQuota / 1000f) + "K COBALT ORE", "COBALT QUOTA MET!");
            }
            else if (objectives[i].Contains("COBALT") && console.cobaltHeld < console.cobaltQuota)
            {
                objectives[i] = "MINE " + Mathf.Round(console.cobaltQuota / 1000f) + "K COBALT ORE";
                if (allObjectives.Contains("COBALT QUOTA MET"))
                {
                    allObjectives = allObjectives.Replace("COBALT QUOTA MET!", "MINE " + Mathf.Round(console.cobaltQuota / 1000f) + "K COBALT ORE");
                }
            }

            // ICE
            if (objectives[i].Contains("ICE") && console.iceHeld >= console.iceQuota)
            {
                objectives[i] = "ICE QUOTA MET!";
                allObjectives = allObjectives.Replace("MINE " + Mathf.Round(console.iceQuota / 1000f) + "K ICE", "ICE QUOTA MET!");
            }
            else if (objectives[i].Contains("ICE") && console.iceHeld < console.iceQuota)
            {
                objectives[i] = "MINE " + Mathf.Round(console.iceQuota / 1000f) + "K ICE";
                if (allObjectives.Contains("ICE QUOTA MET"))
                {
                    allObjectives = allObjectives.Replace("ICE QUOTA MET!", "MINE " + Mathf.Round(console.iceQuota / 1000f) + "K ICE");
                }
            }

            // NICKEL
            if (objectives[i].Contains("NICKEL") && console.nickelHeld >= console.nickelQuota)
            {
                objectives[i] = "NICKEL QUOTA MET!";
                allObjectives = allObjectives.Replace("MINE " + Mathf.Round(console.nickelQuota / 1000f) + "K NICKEL ORE", "NICKEL QUOTA MET!");
            }
            else if (objectives[i].Contains("NICKEL") && console.nickelHeld < console.nickelQuota)
            {
                objectives[i] = "MINE " + Mathf.Round(console.nickelQuota / 1000f) + "K NICKEL ORE";
                if (allObjectives.Contains("NICKEL QUOTA MET"))
                {
                    allObjectives = allObjectives.Replace("NICKEL QUOTA MET!", "MINE " + Mathf.Round(console.nickelQuota / 1000f) + "K NICKEL ORE");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        audioMixer.SetFloat("Music", Mathf.Log10(volumeSlider.value) * 20);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            settingsMenu.SetActive(isPaused);
            if (isPaused)
            {
                alreadyDisabledCursor = false;
                AudioListener.pause = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                if (!alreadyDisabledCursor)
                {
                    Cursor.visible = false;
                    alreadyDisabledCursor = true;
                }
                tipsMenu.SetActive(false);
                AudioListener.pause = false;
                Time.timeScale = 1f;
            }
        }

        objectiveTextbox.text = allObjectives;
        if (Input.GetKey(KeyCode.Tab))
        {
            objectiveTextbox.gameObject.SetActive(true);
            objectiveTextBoxAnimator.enabled = true;
            objectiveTextBoxAnimator.SetBool("holdingTab", true);
        }
        else
        {
            objectiveTextBoxAnimator.SetBool("holdingTab", false);
        }
        // Build objective string every frame cuz efficiency is for nerds
        string currentObjectives = "";

        // Add mining quotas
        if (console.ironQuota > 0)
        {
            if (console.ironHeld >= console.ironQuota)
                currentObjectives += "IRON QUOTA MET!\n";
            else
                currentObjectives += "MINE " + Mathf.Round(console.ironQuota / 1000f) + "K IRON ORE\n";
        }
        if (console.cobaltQuota > 0)
        {
            if (console.cobaltHeld >= console.cobaltQuota)
                currentObjectives += "COBALT QUOTA MET!\n";
            else
                currentObjectives += "MINE " + Mathf.Round(console.cobaltQuota / 1000f) + "K COBALT ORE\n";
        }

        if (console.iceQuota > 0)
        {
            if (console.iceHeld >= console.iceQuota)
                currentObjectives += "ICE QUOTA MET!\n";
            else
                currentObjectives += "MINE " + Mathf.Round(console.iceQuota / 1000f) + "K ICE\n";
        }

        if (console.nickelQuota > 0)
        {
            if (console.nickelHeld >= console.nickelQuota)
                currentObjectives += "NICKEL QUOTA MET!\n";
            else
                currentObjectives += "MINE " + Mathf.Round(console.nickelQuota / 1000f) + "K NICKEL ORE\n";
        }
        // Similar for other quotas...

        // Add other objectives
        for (int i = 0; i < objectives.Count; i++)
        {
            currentObjectives += objectives[i] + "\n";
        }

        objectiveTextbox.text = currentObjectives;

        if (console.beingAttacked)
        {
            if (!chaseMusic.isPlaying)
            {
                chaseMusic.volume = 0;
                chaseMusic.Play();
            }
            if (chaseMusic.volume != 1)
            {
                chaseMusic.volume += Time.deltaTime;
                if (chaseMusic.volume >= 1)
                {
                    chaseMusic.volume = 1;
                }
            }
        }
        else
        {
            if (chaseMusic.volume != 0)
            {
                chaseMusic.volume -= Time.deltaTime;
                if (chaseMusic.volume <= 0)
                {
                    chaseMusic.volume = 0;
                    chaseMusic.Stop();
                }
            }
        }

        //For pirate dialog
        if (console.beingPirateAttacked && !recentPirateAttackQuip)
        {
            pirateAmbush[Random.Range(0, pirateAmbush.Count)].Play();
            recentPirateAttackQuip = true;
        }
        else if(console.beingAmbushed && !recentPirateAmbushQuip)
        {
            pirateChatters[Random.Range(0, pirateChatters.Count)].Play();
            recentPirateAmbushQuip = true;
        }

        if(!console.beingPirateAttacked && recentPirateAttackQuip)
        {
            recentPirateAttackQuip = false;
        }
        if(!console.beingAmbushed && recentPirateAmbushQuip)
        {
            recentPirateAmbushQuip = false;
        }

        //For the tutorial.
        //if (Input.GetKeyDown(KeyCode.M) && !showedWarpTip)
        //{
        //    showedWarpTip = true;
        //    StartCoroutine(playNextStep(tutorialMessages[1], true, "[LEFT BRACKET]\nTO CYCLE WAYPOINT!", false, "", KeyCode.LeftBracket, true));
        //}
        //if((showedWarpTip2 == false && showedWarpTip == true) && (Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.RightBracket)))
        //{
        //    showedWarpTip2 = true;
        //    StartCoroutine(playNextStep(tutorialMessages[0], true, "[Z]\nTO WARP!", false, "", KeyCode.Z, false));
        //}
        if(!showedBookTip && appForTutorial.activeSelf)
        {
            showedBookTip = true;
            List<string> caps = new List<string>();
            List<float> times = new List<float>();
            times.Add(0);
            caps.Add("Okay. I got you a 24 hour subscription so you may familarize yourself with more of your ship's systems.");
            times.Add(6);
            caps.Add("We've placed an empty ship directly in front of you. Sometimes, wrecks like to broadcast data...");
            times.Add(12);
            caps.Add("Remember. It is SUPER DUPER ILLEGAL to download unauthorized signals. This one is company approved.");
            times.Add(20);
            caps.Add("Here are the instructions: MOVE FORWARDS UNTIL YOU SEE A TRACKING ICON.");
            times.Add(24);
            caps.Add("CLICK ON THE ICON.");
            times.Add(27);
            caps.Add("OPEN THE MONITOR. OPEN THE DATA APP, AND THEN PRESS 'DOWNLOAD'.");

            StartCoroutine(playNextStep(tutorialMessages[2], true, "[SHIFT]\nTO MOVE FORWARD", false, "", KeyCode.LeftShift, true, caps, times));
        }
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void openSettingsMenu()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void returnToPauseMenu()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void changeRes()
    {
        if (!hasInitializedResolution)
        {
            hasInitializedResolution = true;
            return;
        }
        selectedRes = resolutionChooser.value;
        Screen.SetResolution(resolutionsForScript[selectedRes].width, resolutionsForScript[selectedRes].height, fullScreened);
    }

    public void setFullScreen()
    {
        fullScreened = fullScreenBox.isOn;
        Screen.SetResolution(resolutionsForScript[selectedRes].width, resolutionsForScript[selectedRes].height, fullScreened);
    }

    public void setMouseMode()
    {
        isMoused = mouseMode.isOn;
        player.useMouseAim = isMoused;
        console.usesMouseMode = isMoused;
    }

    //A generic function to play an audio source, and show some text on the player's screen.
    public IEnumerator playNextStep(AudioSource message, bool showControlText, string controlText, bool showObjectiveNotif, string objectiveText, KeyCode bindToPress, bool customInstruction, List<string> subtitles, List<float> timestamps) //Timestamps should be like : 4.2 seconds
    {
        captions.gameObject.SetActive(true);
        float timer = 0;
        int index = 0;
        while(playingStoryMessage) //WE RUN THIS LOOP TO ENSURE WE DONT OVERLAP WITH ALREADY PLAYING MESSAGES.
        {
            yield return null;
        }
        playingStoryMessage = true;
        if (showControlText)
        {
            player.canMove = false;
        }
        message.Play();
        float soundLength = message.clip.length;
        bool finishedMessage = false;
        while(!finishedMessage)
        {
            captions.text = subtitles[index];
            timer = message.time;
            if (index < subtitles.Count - 1 && timer >= timestamps[index+1])
            {
                index += 1;
            }
            if(timer >= soundLength)
            {
                finishedMessage = true;
            }
            yield return null;
        }
        if(showControlText )
        {
            controlTip.SetActive(true);
            controlTipText.text = controlText;
        }
        if(showObjectiveNotif)
        {
            objectiveAddedTip.SetActive(true);
            objectives.Add(objectiveText);
        }
        Vector2 currentSize = controlTip.GetComponent<RectTransform>().sizeDelta;
        if (customInstruction)
        {
            controlTip.GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x*5f, currentSize.y);
        }
        while(!Input.GetKeyDown(bindToPress) && showControlText)
        {
            yield return null;
        }
        player.canMove = true;
        controlTip.GetComponent<RectTransform>().sizeDelta = currentSize;
        controlTip.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        objectiveAddedTip.SetActive(false);
        playingStoryMessage = false;
        captions.gameObject.SetActive(false);
    }
}
