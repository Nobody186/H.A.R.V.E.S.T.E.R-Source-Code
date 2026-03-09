using UnityEngine;
using System.Collections.Generic;

public class redactorSpawner : MonoBehaviour
{
    [SerializeField] ConsoleController console;
    [SerializeField] StoryManager storyManager;
    [SerializeField] AudioSource arrivalTrack;
    [SerializeField] GameObject navigationApp;
    [SerializeField] GameObject whiteout;
    [SerializeField] List<GameObject> otherRedactorSpawners;
    public List<int> daysCanSpawn = new List<int>();
    public float spawnChance = 0.001f;
    private float timer = 0f;
    private float timeCheckIntervals = 30f;

    bool tryingToSpawn = false;
    bool hasRedactor = false;

    [SerializeField] Transform player;
    float distance = 0f;

    [Tooltip("This is for developer purposes only. Check this to override a spawn for the redactor")]
    public bool spawn = false;
    bool alreadySpawned = false;

    private void Start()
    {
        if (!daysCanSpawn.Contains(console.day))
        {
            Destroy(gameObject); //We don't need this if we're never using it.
        }
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.position, transform.position);

        if(transform.childCount > 0 && Vector3.Distance(transform.position, player.position) > 15000)
        {
            transform.Find("Redactor(Clone)").gameObject.SetActive(false);
        }

        if(navigationApp.activeSelf)
        {
            return; //Don't risk a spawn if the player is in the warp menu. Can break immersion or a terrible edge case might occur.
        }
        timer += Time.deltaTime;
        if(timer >= timeCheckIntervals)
        {
            timer = 0f;
            if(Random.value <= spawnChance && !alreadySpawned)
            {
                Spawn();
            }
        }

        if(spawn)
        {
            spawn = false;
            Spawn();
        }

        if(tryingToSpawn && whiteout.activeSelf) //The whiteout will be enabled once The Redactor spawns.
        {
            tryingToSpawn = false;
        }
    }

    void Spawn()
    {
        for(int i = 0; i < otherRedactorSpawners.Count; i++)
        {
            otherRedactorSpawners[i].SetActive(false);
        }

        List<string> caps = new List<string>();
        List<float> times = new List<float>();
        caps.Add("");
        times.Add(0f);
        caps.Add("Attention Shergeo Family.");
        times.Add(2f);
        caps.Add("The fact that you are hearing this message means you are in a compromised mining zone due to hostile signatures.");
        times.Add(4.4f);
        caps.Add("A Redactor is enroute. Please evaccuate immediately.");
        times.Add(12f);
        caps.Add("");
        times.Add(15.6f);

        StoryMessage message = new StoryMessage();
        message.audio = arrivalTrack;
        message.subtitles = caps;
        message.timestamps = times;
        message.whiteOut = true;
        message.timeToWhiteOut = 32.1f;
        message.freezePlayer = false;
        message.showControlText = false;
        message.customInstruction = false;
        message.redactorSpawn = true;
        message.homePoint = transform;

        storyManager.EnqueueMessage(message);
        tryingToSpawn = true;
    }
}
