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
    [SerializeField] GameObject confirmBackToMenu;
    [SerializeField] GameObject confirmExitApp;

    //For pause menu
    [SerializeField] GameObject pauseMenu;
    bool isPaused = false;

    [SerializeField] GameObject settingsMenu;
    public Slider volumeSlider;
    public Slider dialogSlider;
    public Slider sfxSlider;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionChooser;
    [SerializeField] Toggle fullScreenBox;

    [SerializeField] Toggle mouseMode;


    Resolution[] AllResolutions;
    bool fullScreened;
    int selectedRes;
    List<Resolution> resolutionsForScript = new List<Resolution>();
    private bool hasInitializedResolution = false;


    //For dialog
    [SerializeField] TextMeshProUGUI captions;
    [SerializeField] List<AudioSource> pirateChatters;
    [SerializeField] List<AudioSource> pirateAmbush;

    bool needToInterrupt = false;

    [SerializeField] List<AudioSource> tutorialMessages;

    [SerializeField] List<AdSet> ads;

    int quotasMet = 0;

    bool quota1met = false;
    bool quota2met = false;
    bool quota3met = false;
    bool quota4met = false;
    bool quota5met = false;
    bool quota6met = false;
    bool quota7met = false;
    bool quota8met = false;
    bool quota9met = false;
    bool quota10met = false;

    [SerializeField] GameObject appForTutorial;

    [SerializeField] GameObject objectiveAddedTip;
    [SerializeField] GameObject controlTip;
    [SerializeField] GameObject whiteout;
    [SerializeField] GameObject radioLight;
    private TextMeshProUGUI controlTipText;

    bool recentPirateAttackQuip = false;
    bool recentPirateAmbushQuip = false;


    [SerializeField] PlayerController player;

    private Queue<StoryMessage> storyQueue = new Queue<StoryMessage>(); //A queue that stores all of our storymessages.
    public bool playingStoryMessage = false;

    bool showedBookTip = false;

    [SerializeField] GameObject Redactor;
    [SerializeField] Slider jammerSlider;

    float staggeredUpdateTimer = 0f;
    float staggeredUpdateInterval = .5f;

    public static bool isPlaying = false;

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

        radioLight.SetActive(false);

        mouseMode.isOn = console.usesMouseMode;
        controlTipText = controlTip.GetComponentInChildren<TextMeshProUGUI>();
        Vector2 OGSize = controlTip.GetComponent<RectTransform>().sizeDelta;

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        dialogSlider.onValueChanged.AddListener(OnDialogChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    // Update is called once per frame
    void Update()
    {
        staggeredUpdateTimer += Time.deltaTime;
        if(staggeredUpdateTimer >= staggeredUpdateInterval)
        {
            checkQuotaProgression(); //A lot of if conditions here, so let's not check it every frame.
            staggeredUpdateTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //Pause menu handling
        {
            pauseToggle();
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

        if(appForTutorial != null && !showedBookTip && appForTutorial.activeSelf && !playingStoryMessage)
        {
            showedBookTip = true;
            List<string> caps = new List<string>();
            List<float> times = new List<float>();
            
            caps.Add("Okay. I got you a 24 hour subscription so you may familarize yourself with more of your systems.");
            times.Add(0f);
            caps.Add("I've placed an empty ship directly in front of you. Sometimes, they broadcast data...");
            times.Add(6.6f);
            caps.Add("Remember. It is very ILLEGAL to download unauthorized signals, but I've approved this one for you.");
            times.Add(12.8f);
            caps.Add("Here are your instructions: MOVE FORWARDS UNTIL YOU SEE A TRACKING ICON.");
            times.Add(19f);
            caps.Add("CLICK ON THE ICON.");
            times.Add(24f);
            caps.Add("OPEN YOUR MONITOR. OPEN THE DATA APP, AND THEN PRESS 'DOWNLOAD'.");
            times.Add(26.8f);

            StoryMessage message = new StoryMessage();
            message.subtitles = caps;
            message.timestamps = times;
            message.audio = tutorialMessages[2];
            message.showControlText = true;
            message.controlText = "[SHIFT]\nTO MOVE FORWARD";
            message.bind = KeyCode.LeftShift;
            message.customInstruction = true;
            message.freezePlayer = true;

            EnqueueMessage(message);
        }
    }

    void OnVolumeChanged(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
    }

    void OnDialogChanged(float value)
    {
        audioMixer.SetFloat("Dialog", Mathf.Log10(value) * 20);
    }

    void OnSFXChanged(float value)
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(value) * 20);
    }

    public void pauseToggle()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
        }
        if(confirmBackToMenu.activeSelf)
        {
            confirmBackToMenu.SetActive(false);
        }
        if(confirmExitApp.activeSelf)
        {
            confirmExitApp.SetActive(false);
        }
        if (isPaused)
        {
            AudioListener.pause = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (mouseMode.isOn)
            {
                Cursor.visible = false;
            }
            AudioListener.pause = false;
            Time.timeScale = 1f;
        }
    }

    public void returnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void maybe()
    {
        if(Random.value >= 0.5f)
        {
            confirmBackToMenu.SetActive(false);
        }
        else
        {
            returnToMenu();
        }
    }

    public void exitGame()
    {
        Application.Quit();
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
        player.useMouseAim = mouseMode.isOn;
        console.usesMouseMode = mouseMode.isOn;
    }

    //A generic function to play an audio source, and show some text on the player's screen.
    public void EnqueueMessage(StoryMessage message)
    {
        if (!message.interrupt)
        {
            storyQueue.Enqueue(message);
            if (!playingStoryMessage)
            {
                StartCoroutine(queueProcessor());
            }
        }
        else
        {
            storyQueue.Clear();
            storyQueue.Enqueue(message);
            needToInterrupt = true;
            if (!playingStoryMessage)
            {
                StartCoroutine(queueProcessor());
            }
        }
    }

    public IEnumerator queueProcessor() 
    {
        playingStoryMessage = true;

        while(storyQueue.Count > 0)
        {
            StoryMessage message = storyQueue.Dequeue(); //Take the first thing in the queue...
            yield return playNextStep(message); //Play that message, and don't restart the while loop until we're done.
        }

        playingStoryMessage = false;
    }

    public IEnumerator playNextStep(StoryMessage msg) //Timestamps should be like : 4.2 seconds
    {
        isPlaying = true;
        captions.gameObject.SetActive(true);
        float timer = 0f;
        float whiteoutTimer = 0f;
        int index = 0;
        playingStoryMessage = true;
        player.canMove = !msg.freezePlayer;
        msg.audio.Play();
        float soundLength = msg.audio.clip.length;
        bool finishedMessage = false;
        bool didWhiteout = false;

        radioLight.SetActive(true);
        captions.text = msg.subtitles[0];
        if(msg.redactorSpawn)
        {
            console.warpJammed = true;
            jammerSlider.maxValue = 150f;
        }
        while (!finishedMessage) //While we're playing our message, keep the captions updated.
        {
            if (needToInterrupt)
            {
                finishedMessage = true;
                isPlaying = false;
                msg.audio.Pause();
                if (console.canWarp)
                {
                    player.canMove = true;
                }
                captions.gameObject.SetActive(false);
                needToInterrupt = false;
                yield break;
            }
            if (console.canWarp)
            {
                player.canMove = !msg.freezePlayer; //If we're not currently warping, freeze the player if the message asks us to.
            }
            captions.text = msg.subtitles[index];
            timer += Time.deltaTime;
            if(didWhiteout && whiteoutTimer < 3f && msg.whiteOut)
            {
                whiteoutTimer += Time.deltaTime;
            }
            else if(didWhiteout && whiteoutTimer >= 3f)
            {
                whiteout.SetActive(false);
            }
            if(timer >= msg.timeToWhiteOut && !didWhiteout && msg.whiteOut)
            {
                whiteout.SetActive(true);
                didWhiteout = true;
                if (msg.redactorSpawn)
                {
                    GameObject newRedactor = GameObject.Instantiate(Redactor, player.transform.position + (player.transform.forward * 1000f), Quaternion.identity, msg.homePoint);
                    newRedactor.SetActive(true);
                    newRedactor.GetComponent<RedactorPathfinder>().home = msg.homePoint;
                }
            }
            if (index < msg.subtitles.Count - 1 && timer >= msg.timestamps[index+1])
            {
                index += 1;
            }
            if(timer >= soundLength)
            {
                finishedMessage = true;
            }

            yield return null;
        }
        if(msg.showControlText)
        {
            controlTip.SetActive(true);
            controlTipText.text = msg.controlText;
        }

        Vector2 currentSize = controlTip.GetComponent<RectTransform>().sizeDelta;
        if (msg.customInstruction)
        {
            controlTip.GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x*5f, currentSize.y);
        }
        while(!Input.GetKeyDown(msg.bind) && msg.showControlText) //If we're ought to show a keybind hint, don't leave before we get it.
        {
            yield return null;
        }
        if (console.canWarp) //just a small little anti-game breaking guardrail
        {
            player.canMove = true;
        }
        if (msg.showControlText)
        {
            controlTip.GetComponent<RectTransform>().sizeDelta = currentSize;
            controlTip.SetActive(false);
        }
        radioLight.SetActive(false);
        captions.gameObject.SetActive(false);
        isPlaying = false;
    }

    void checkQuotaProgression()
    {
        if (console.day != 0)
        {
            if (console.ironQuota > 0 && console.ironHeld*2f >= console.ironQuota && quota1met == false)
            {
                quotasMet += 1;
                quota1met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet-1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.aluminumQuota > 0 && console.aluminumHeld >= console.aluminumQuota && quota2met == false)
            {
                quotasMet += 1;
                quota2met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.carbonQuota > 0 && console.carbonHeld >= console.carbonQuota && quota3met == false)
            {
                quotasMet += 1;
                quota3met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.clayQuota > 0 && console.clayHeld*2f >= console.clayQuota && quota4met == false)
            {
                quotasMet += 1;
                quota4met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.cobaltQuota > 0 && console.cobaltHeld >= console.cobaltQuota && quota5met == false)
            {
                quotasMet += 1;
                quota5met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.helium3Quota > 0 && console.helium3Held >= console.helium3Quota && quota6met == false)
            {
                quotasMet += 1;
                quota6met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.hydrogenQuota > 0 && console.hydrogenHeld >= console.hydrogenQuota && quota7met == false)
            {
                quotasMet += 1;
                quota7met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.iceQuota > 0 && console.iceHeld * 2f >= console.iceQuota && quota8met == false)
            {
                quotasMet += 1;
                quota8met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.magnesiumQuota > 0 && console.magnesiumHeld >= console.magnesiumQuota && quota9met == false)
            {
                quotasMet += 1;
                quota9met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet - 1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
            if (console.nickelQuota > 0 && console.nickelHeld >= console.nickelQuota && quota10met == false)
            {
                quotasMet += 1;
                quota10met = true;
                if (quotasMet <= ads[console.day - 1].audioClips.Count)
                {
                    StoryMessage message = new StoryMessage();
                    message.audio = ads[console.day - 1].audioClips[quotasMet-1];
                    message.subtitles = ads[console.day - 1].captions[quotasMet - 1].captions;
                    message.timestamps = ads[console.day - 1].timestamps[quotasMet - 1].timestamps;
                    EnqueueMessage(message);
                }
            }
        }
    }
}

[System.Serializable] //Yes I know my poor choice of naming has made this completely unreadable. I pray that I will never have to touch this again.
public class StoryMessage
{
    public AudioSource audio;
    public List<string> subtitles;
    public List<float> timestamps;
    public bool showControlText;
    public bool customInstruction = false;
    public string controlText;
    public KeyCode bind;
    public bool freezePlayer;

    public bool interrupt = false;

    public bool whiteOut = false;
    public float timeToWhiteOut = 0f;

    public bool redactorSpawn = false;
    public Transform homePoint;
}

[System.Serializable]
public class AdSet
{
    public List<AudioSource> audioClips;
    public List<Captions> captions;
    public List<Timestamps> timestamps;
}

[System.Serializable]
public class Captions
{
    public List<string> captions;
}
[System.Serializable]
public class Timestamps
{
    public List<float> timestamps;
}
