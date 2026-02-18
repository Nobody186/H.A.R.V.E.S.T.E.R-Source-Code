using UnityEngine;

public class dogfightStateChecker : MonoBehaviour
{
    [SerializeField] SoundManager soundManager;
    float timer = 0f;
    float checkIntervals = 0f;

    private void Start()
    {
        checkIntervals = Random.Range(0.1f, 0.3f);
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= checkIntervals)
        {
            timer = 0f;
            int i = 0;
            foreach(Transform child in transform)
            {
                i++;
            }
            if(i > 1)
            {
                soundManager.Request(3); //This object has "threat pointers" attached to it. When we have 1 or less threat pointers, that means there are no enemies engaging us. Vice versa.
            }
            else
            {
                soundManager.EndRequest(3);
            }
        }
    }
}
