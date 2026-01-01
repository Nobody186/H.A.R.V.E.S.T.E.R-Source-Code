using System.Collections.Generic;
using UnityEngine;

public class storyTrigger : MonoBehaviour
{
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

    [SerializeField] bool freezePlayer;
    float timer = 0f;
    float clipTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        messageToPlay = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!alreadyTriggered && other.gameObject.name == "PlayerShip" && !dayIntro.activeSelf)
        {
            alreadyTriggered = true;
            StartCoroutine(storyManager.playNextStep(messageToPlay, showKeybindMessage, keybindMessage, showObjectiveMessage, objectiveName, keybind, longKeybindName, caps, times, freezePlayer));
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
