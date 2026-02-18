using UnityEngine;

public class laserExplosionManager : MonoBehaviour
{
    [SerializeField] AudioSource sound;
    [SerializeField] ParticleSystem me;

    [SerializeField] float explosionRate = 0f;
    float timer = 0f;

    float originalExRate = 0f;

    void Start()
    {
        originalExRate = explosionRate;    
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= explosionRate)
        {
            timer = 0f;
            sound.pitch = Random.Range(0.8f, 1.2f);
            sound.Play();
            me.Play();
            explosionRate = originalExRate + Random.Range(-0.19f, 0.2f);
        }
    }
}
