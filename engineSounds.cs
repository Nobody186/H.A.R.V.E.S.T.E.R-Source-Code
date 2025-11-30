using UnityEngine;

public class engineSounds : MonoBehaviour
{
    [SerializeField] AudioSource thruster;
    [SerializeField] AudioSource tonal;
    [SerializeField] PlayerController player;

    public float thrusterMultiplier = 1;
    public float tonalMultiplier = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thruster.volume = player.VehicleSpeed * thrusterMultiplier;
        tonal.pitch = player.VehicleSpeed * tonalMultiplier;
    }
}
