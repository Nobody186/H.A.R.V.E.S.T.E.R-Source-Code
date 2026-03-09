using Unity.VisualScripting;
using UnityEngine;

public class scopeIndicator : MonoBehaviour
{
    [SerializeField] GunController gun;
    [SerializeField] radarAnim radarSweeper;
    //We need to know where the radar is looking
    [SerializeField] Transform radar;

    //We use these pooints to move our lines across something like a grid.
    [SerializeField] Transform origin;
    [SerializeField] Transform maxX;
    [SerializeField] Transform maxY;

    //These lines will indicate where the radar is looking
    [SerializeField] Transform lineX;
    [SerializeField] Transform lineY;

    [SerializeField] float xInterval = 10f;
    [SerializeField] float yInterval = 20f;

    [SerializeField] GameObject contact;
    [SerializeField] Transform minRange;
    [SerializeField] Transform maxRange;

    private float maxHoloY = 200f;
    private float minHoloY = 160f;
    private float maxHoloX = 170f;
    private float minHoloX = 190f;

    [SerializeField] Transform holoThing;

    private Vector3 newRotation = Vector3.zero;

    //We will use these for liner interpolation.
    float normalizedX;
    float normalizedY;

    //Just to cache some stuff.
    float radarX;
    float radarY;

    float maxRdrX;
    float minRdrX;
    float minRdrY;
    float maxRdrY;

    private void Start()
    {
        maxHoloX = 180f - xInterval;
        minHoloX = 180f + xInterval;

        maxHoloY = 180f + yInterval;
        minHoloY = 180f - yInterval;

        maxRdrX = radarSweeper.maxX;
        minRdrX = radarSweeper.minX;
        maxRdrY = radarSweeper.maxY;
        minRdrY = radarSweeper.minY;

        GunController.OnLock += disableLines;
        GunController.OnUnlock += enableLines;
        GunController.onContact += showContact;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Unintuitive? I know.
        radarX = radar.transform.localRotation.eulerAngles.y;
        radarY = radar.transform.localRotation.eulerAngles.x;

        if (radarX > 180f) radarX -= 360f;
        if (radarY > 180f) radarY -= 360f;

        //Normalization
        normalizedX = (radarX - minRdrX)/(maxRdrX-minRdrX);
        normalizedY = (radarY - minRdrY) / (maxRdrY - minRdrY);

        //Now we just move the lines depending on these values.
        lineX.position = Vector3.Lerp(origin.position, maxX.position, normalizedX);
        lineY.position = Vector3.Lerp(origin.position, maxY.position, normalizedY);

        float newX = Mathf.Lerp(minHoloX, maxHoloX, normalizedX);
        float newY = Mathf.Lerp(minHoloY, maxHoloY, normalizedY);

        //Create a vector3 that corresponds with some stuf
        newRotation = new Vector3(-newY, -newX, -180);

        holoThing.localEulerAngles = newRotation;
    }

    void disableLines()
    {
        lineX.gameObject.SetActive(false);
        lineY.gameObject.SetActive(false);
        holoThing.gameObject.SetActive(false);
    }
    void enableLines()
    {
        lineX.gameObject.SetActive(true);
        lineY.gameObject.SetActive(true);
        holoThing.gameObject.SetActive(true);
    }

    void showContact(float range)
    {
        float lerpAmount = (range-gun.MaxRange)/(gun.MaxRange-gun.MinRange);

        Vector3 contactPos = Vector3.Lerp(minRange.position, maxRange.position, lerpAmount);
        GameObject newContact = Instantiate(contact, contactPos, Quaternion.identity, transform);
        newContact.SetActive(true);
        Destroy(newContact, 1f);
    }
}
