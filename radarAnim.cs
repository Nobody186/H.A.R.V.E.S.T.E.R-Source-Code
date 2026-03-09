using UnityEngine;

public class radarAnim : MonoBehaviour
{
    [SerializeField] GunController gun;
    [SerializeField] float xRange = 65f;
    [SerializeField] float palRange = 15f;
    [SerializeField] float barSpacing = 3f;

    [HideInInspector] public float minX = -65f;
    [HideInInspector] public float maxX = 65f;

    [HideInInspector] public float minY = -65f;
    [HideInInspector] public float maxY = 65f;

    [SerializeField] float vertBars = 4f;

    [SerializeField] float horizontalSpeed = 240f;

    private bool scanningRight = true;

    bool dueVertStep = false;

    void Start()
    {
        minX = -xRange;
        maxX = xRange;

        maxY = -(vertBars * barSpacing);
        minY = -maxY;

        GunController.onLockAttempt += attemptLock;
    }

    // Update is called once per frame
    void Update()
    {
        if (gun.searching || gun.palMode)
        {
            //Im not gonna micro-optimize this
            minX = -xRange;
            maxX = xRange;

            float newElevation = transform.localEulerAngles.x;

            if (dueVertStep)
            {
                newElevation = elevation();
                dueVertStep = false;
            }

            Vector3 newRotation = new Vector3(newElevation, azimuth(), 0);

            transform.localEulerAngles = newRotation;
        }
        else
        {
            if(gun.Target != null)
            {
                transform.LookAt(gun.Target.transform.position);
            }
        }
    }

    float elevation()
    {
        float newYForFrame = transform.localEulerAngles.x;

        if (newYForFrame > 180f) newYForFrame -= 360f;
        if (newYForFrame < -180f) newYForFrame -= 360f;

        float maxElev = -(vertBars * barSpacing);
        newYForFrame = newYForFrame - barSpacing; //Subtracting because in game space, subtraction makes the transform point up.

        if (newYForFrame < maxElev)
        {
            if (!gun.palMode)
            {
                newYForFrame = -maxElev; //Go back to the lowest possible elevation.
            }
            else
            {
                newYForFrame = 0f;
            }
        }
        return newYForFrame;
    }

    float azimuth()
    {
        float newXForFrame = transform.localEulerAngles.y; //Y-Axis rotation handles horizontal movement.
        if (newXForFrame > 180f) newXForFrame -= 360f;
        if (newXForFrame < -180f) newXForFrame -= 360f;

        float newMaxX = maxX;
        float newMinX = minX;

        if(gun.palMode)
        {
            newMaxX = palRange;
            newMinX = -palRange;
        }

        if (newXForFrame > newMaxX)
        {
            newXForFrame = newMaxX;
            scanningRight = false;
            dueVertStep = true;
        }
        if (newXForFrame < newMinX)
        {
            newXForFrame = newMinX;
            scanningRight = true;
            dueVertStep = true;
        }

        if (scanningRight)
        {
            newXForFrame += Time.deltaTime * horizontalSpeed;
        }
        else
        {
            newXForFrame -= Time.deltaTime * horizontalSpeed;
        }
        return newXForFrame;
    }

    void attemptLock(Vector3 targetPos)
    {
        transform.LookAt(targetPos);
    }    
}
