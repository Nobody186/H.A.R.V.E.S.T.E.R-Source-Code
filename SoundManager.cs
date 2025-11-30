using System.Net;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] ConsoleController console;
    [SerializeField] AudioSource ambience1;
    [SerializeField] AudioSource ambience2;
    [SerializeField] AudioSource finalHour;

    bool alreadyPlayed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1 && !ambience1.isPlaying && !console.deathTime && !console.navMode && (console.currentWaypoint == 0 || console.currentWaypoint == 1))
        {
            ambience1.Play();
            ambience2.Stop();
        }
        else if(Time.timeScale == 1 && !ambience2.isPlaying && !console.deathTime && !console.navMode && (console.currentWaypoint == 2 || console.currentWaypoint == 4))
        {
            ambience1.Stop();
            ambience2.Play();
        }
        else if(Time.timeScale == 1 && !finalHour.isPlaying && console.deathTime && !alreadyPlayed)
        {
            alreadyPlayed = true;
            ambience1.Stop();
            ambience2.Stop();
            finalHour.Play();
        }
        if(console.isWarping || console.beingAttacked)
        {
            ambience1.Pause();
            ambience2.Pause();
        }
        else if(!console.isWarping && !console.beingAttacked && !console.deathTime)
        {
            if ((console.currentWaypoint == 0 || console.currentWaypoint == 1 || console.currentWaypoint == 3) && !ambience1.isPlaying)
            {
                ambience1.UnPause();
            }
            else if((console.currentWaypoint == 2 || console.currentWaypoint == 4) && !ambience2.isPlaying)
            {
                ambience2.UnPause();
            }
        }
    }
}
