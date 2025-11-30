using System.Reflection;
using System;
using UnityEngine;
using NUnit.Framework;

public class tutorialBotScript : MonoBehaviour
{
    [SerializeField] GameObject Missile;
    [SerializeField] Transform missileRack;
    [SerializeField] ConsoleController console;
    [SerializeField] GameObject RWRLight;
    [SerializeField] AudioSource RWRSound;
    [SerializeField] AudioSource triggerSound;
    [SerializeField] Health playerHealth;

    bool firedMissile = false;
    bool sequenceStarted = false;

    // Update is called once per frame
    void Update()
    {
        if(triggerSound.isPlaying)
        {
            sequenceStarted = true;
        }
        if (!firedMissile && !triggerSound.isPlaying && sequenceStarted)
        {
            GameObject spawnedMissile = Instantiate(Missile, missileRack.position, Quaternion.identity);
            spawnedMissile.SetActive(true);
            console.lightToFlash = RWRLight;
            console.warnToPlay = RWRSound;
            StartCoroutine(console.flashLight());
            firedMissile = true;
        }
    }
}
