using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enemyMining : MonoBehaviour
{
    [SerializeField] EnemyMovement mov;

    [SerializeField] cameraShake camShake;
    [SerializeField] Health health;
    [SerializeField] GameObject resourceHarvestedImage;
    [SerializeField] List<AudioSource> resourceHarvestedSFX;
    [SerializeField] Transform canvas;
    [SerializeField] Transform hudPosition;
    [SerializeField] GameObject asteroidHarvestedText;
    [SerializeField] GameObject bigExplosion;
    [SerializeField] GameObject smallExplosion;
    [SerializeField] GameObject PopUp;
    [SerializeField] TextMeshProUGUI PopUpText;
    [SerializeField] TextMeshPro oreCollectedText;
    [SerializeField] ConsoleController console;
    [SerializeField] GunController gun;

    public float staggeredUpdateInterval = 0.2f;
    private float timer = 0f;

    GameObject Target;
    [SerializeField] GameObject laserSmoke;
    [SerializeField] GameObject laserExplosion;
    [SerializeField] AudioSource laserBlast;
    [SerializeField] AudioSource laserLoop;

    bool firstFrameLaser = false;

    // Update is called once per frame
    void Update()
    {
        Target = mov.closestAsteroid;
        timer += Time.deltaTime;
        if(timer >= staggeredUpdateInterval)
        {
            timer = 0f;
            checkForTarget();
            Mine();
        }
    }

    void checkForTarget()
    {
        if (Target != null)
        {
            if (Target.GetComponent<Mineable>() == null)
            {
                Mineable targetRef = Target.AddComponent<Mineable>();
                targetRef.gun = gun;
                targetRef.console = console;
                targetRef.cameraShake = camShake;
                targetRef.healthScript = health;
                targetRef.resourceCollectedImage = resourceHarvestedImage;
                targetRef.resourceCollectedSFX.AddRange(resourceHarvestedSFX);
                targetRef.canvas = canvas;
                targetRef.hudPos = hudPosition;
                targetRef.harvestedText = asteroidHarvestedText;
                targetRef.PopUp = PopUp;
                targetRef.PopUpText = PopUpText;
                targetRef.oreCollectedText = oreCollectedText;
                if (Target.name.Contains("big"))
                {
                    targetRef.explosion = bigExplosion;
                }
                else
                {
                    targetRef.explosion = smallExplosion;
                }
            }
        }
    }

    void Mine()
    {
        if (Target != null)
        {
            if (Target.GetComponent<Mineable>() != null || Target.GetComponent<Health>() != null)
            {
                if (mov.harvesting)
                {
                    if(Target.GetComponent<Mineable>() != null) Target.GetComponent<Mineable>().enemyLaserHitting = true;
                    else
                    {
                        Target.GetComponent<Health>().health -= 2f;
                        camShake.shakeFactor = 0.5f;
                        StartCoroutine(camShake.SHAKE());
                    }

                    laserSmoke.transform.position = mov.hitPoint.position;
                    laserExplosion.transform.position = mov.hitPoint.position;

                    Quaternion lookAtRotation = Quaternion.LookRotation(mov.hitDirection);
                    laserSmoke.transform.rotation = lookAtRotation;

                    if(!firstFrameLaser)
                    {
                        firstFrameLaser = true;
                        laserBlast.Play();
                        laserLoop.Play();
                        laserSmoke.SetActive(true);
                        laserExplosion.SetActive(true);
                    }
                }
                else
                {
                    Target.GetComponent<Mineable>().enemyLaserHitting = false;
                    firstFrameLaser = false;
                    laserLoop.Stop();
                    laserSmoke.SetActive(false);
                    laserExplosion.SetActive(false);
                }
            }
        }
        else
        {
            firstFrameLaser = false;
            laserLoop.Stop();
            laserSmoke.SetActive(false);
            laserExplosion.SetActive(false);
        }
    }

    
}
