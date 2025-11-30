using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSoundManager : MonoBehaviour
{
    [SerializeField] GunController gun;
    [SerializeField] AudioSource laserChargeSFX;
    [SerializeField] AudioSource laserLoop;
    [SerializeField] AudioSource laserBlast;

    [SerializeField] float maxVolume = 0.5f;

    bool playedBlast;

    // Update is called once per frame
    void Update()
    {
        if (gun.chargingLaser && !laserChargeSFX.isPlaying)
        {
            laserChargeSFX.Play();
        }
        else if (!gun.chargingLaser && gun.isMining && !laserLoop.isPlaying)
        {
            if(!playedBlast)
            {
                laserBlast.volume = 1.0f;
                laserBlast.Play();
                playedBlast = true;
            }
            laserLoop.volume = maxVolume;
            laserLoop.Play();
        }

        if (gun.isMining == false)
        {
            playedBlast = false;
            laserLoop.Stop();
        }
    }
}
