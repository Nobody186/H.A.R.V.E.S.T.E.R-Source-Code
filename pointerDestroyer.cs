using UnityEngine;

public class pointerDestroyer : MonoBehaviour
{
    [SerializeField] ConsoleController console;

    // Update is called once per frame
    void Update()
    {
        if (!console.beingAttacked)
        {
            Destroy(gameObject);
        }
    }
}
