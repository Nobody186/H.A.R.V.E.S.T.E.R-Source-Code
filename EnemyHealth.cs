using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] EnemyMovement enemyMov;
    [SerializeField] GunController gun;
    [SerializeField] cameraShake shake;
    [SerializeField] ConsoleController console;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] List<AudioSource> minerDeathSounds;
    [SerializeField] List<AudioSource> hostileDeathSounds;
    public float health;
    [SerializeField] float damageIncrements;

    private float timer = 0f;
    private float deathTimer = 0f;
    private float timeToDeath = 1f;

    private AudioSource deathSound;

    public bool isDying = false;

    [SerializeField] AudioSource tutLine; 


    // Update is called once per frame
    void Update()
    {
        if(gun.Target != null && gun.Target.transform == transform && gun.isMining && gun.laserOnTimer >= 1)
        {
            timer += Time.deltaTime;
            if(timer >= damageIncrements)
            {
                health -= console.laserDamage;
                timer = 0f;
            }
        }
        if(health <= 0f)
        {
            deathTimer += Time.deltaTime;
            if(deathSound == null)
            {
                if (!enemyMov.hostile)
                {
                    deathSound = minerDeathSounds[Random.Range(0, minerDeathSounds.Count)];
                }
                else if(enemyMov.hostile)
                {
                    deathSound = hostileDeathSounds[Random.Range(0, hostileDeathSounds.Count)];
                }
                deathSound.Play();
                timeToDeath = deathSound.clip.length - 0.15f;
            }
        }
        if(deathTimer >= timeToDeath && !isDying)
        {
            isDying = true;
            gun.unLock();
            GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
            explosion.SetActive(true);
            shake.shakeFactor = (100f / gun.Distance);
            shake.shakeTime = 1f;
            shake.StartCoroutine(shake.SHAKE());
            if (console.day != 0)
            {
                Destroy(gameObject, 0.1f);
            }
            else
            {
                GameObject audioSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position, Quaternion.identity);
                audioSphere.GetComponent<MeshRenderer>().enabled = false;
                audioSphere.GetComponent<SphereCollider>().enabled = false;
                audioSphere.AddComponent<AudioSource>();
                audioSphere.GetComponent<AudioSource>().clip = tutLine.clip;
                if (!audioSphere.GetComponent<AudioSource>().isPlaying)
                {
                    audioSphere.GetComponent<AudioSource>().Play();
                }
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
