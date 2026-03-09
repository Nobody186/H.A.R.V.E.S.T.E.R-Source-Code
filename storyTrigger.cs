using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class storyTrigger : MonoBehaviour
{
    [SerializeField] ConsoleController consoleController;
    [SerializeField] GameObject dayIntro;
    [SerializeField] StoryManager storyManager;
    [SerializeField] string keybindMessage;
    [SerializeField] KeyCode keybind;
    [SerializeField] bool showKeybindMessage;
    [SerializeField] string objectiveName; //example: "Get close to waypoint 5"
    [SerializeField] bool showObjectiveMessage;
    [SerializeField] bool longKeybindName;
    private AudioSource messageToPlay;
    private bool alreadyTriggered = false;
    public bool finishedTrigger = false;

    [SerializeField] List<string> caps;
    [SerializeField] List<float> times;

    [SerializeField] List<GameObject> triggersToDestroy;
    [SerializeField] List<GameObject> triggersToEnable;
    [SerializeField] List<int> daysICanLive;

    [SerializeField] bool freezePlayer;
    [SerializeField] bool interrupt;
    float timer = 0f;
    float clipTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messageToPlay = GetComponent<AudioSource>();
        if(!daysICanLive.Contains(consoleController.day))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!alreadyTriggered && other.gameObject.name == "PlayerShip" && !dayIntro.activeSelf)
        {
            alreadyTriggered = true;

            StoryMessage message = new StoryMessage();
            message.subtitles = caps;
            message.timestamps = times;
            message.audio = messageToPlay;
            message.showControlText = showKeybindMessage;
            message.controlText = keybindMessage;
            message.bind = keybind;
            message.customInstruction = longKeybindName;
            message.freezePlayer = freezePlayer;
            message.whiteOut = false;
            message.interrupt = interrupt;

            storyManager.EnqueueMessage(message);

            for(int i = 0; i < triggersToDestroy.Count; i++)
            {
                Destroy(triggersToDestroy[i]);
            }
            for(int i = 0; i < triggersToEnable.Count; i++)
            {
                triggersToEnable[i].SetActive(true);
            }

            print("TRIGGER FIRED!");
            clipTime = messageToPlay.clip.length;
        }
    }

    private void Update()
    {
        if (alreadyTriggered)
        {
            timer += Time.deltaTime;
            if(timer >= clipTime)
            {
                finishedTrigger = true;
            }
        }
    }
}
