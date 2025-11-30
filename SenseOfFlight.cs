using Unity.VisualScripting;
using UnityEngine;


public class SenseOfFlight : MonoBehaviour
{
    [SerializeField] bool isMenu = false;

    public PlayerController playerController;
    public ConsoleController console;
    [SerializeField] Camera cam;
    [SerializeField] Transform cameraPos;
    public float rotSpeed = 1f;
    public float zoomSpeed = 1f;
    public float posSpeed = 1f;
    public Vector3 velocity = Vector3.zero;

    private float mouseX;
    private float mouseY;
    private float mouseXclamp;
    private float mouseYclamp;
    public float mouseSensitivity;

    private float baseFOV = 70f;

    bool lerpCool = false;
    float Timer;
    private Quaternion newTransform;

    // Start is called before the first frame update

    private void Update()
    {
        if (!isMenu && Input.GetKey(KeyCode.Mouse1))
        { 
            baseFOV -= (Input.GetAxis("Mouse ScrollWheel")) * 5000 * Time.deltaTime;
    
            if (baseFOV <= 30)
            {
                baseFOV = 30;
            }
            else if (baseFOV >= 90)
            {
                baseFOV = 90;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isMenu)
        {
            cam.fieldOfView = (baseFOV + (playerController.VehicleSpeed * Time.deltaTime * zoomSpeed));


            if (Input.GetMouseButton(1))
            {
                lerpCool = true;
                Timer = 0;
                cam.gameObject.transform.parent = cameraPos.transform;
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                mouseXclamp -= mouseX;
                mouseXclamp = Mathf.Clamp(mouseXclamp, -60, 60);
                mouseYclamp -= mouseY;
                mouseYclamp = Mathf.Clamp(mouseYclamp, -80, 80);

                newTransform = Quaternion.Euler(mouseYclamp, -mouseXclamp, 0);
                cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, newTransform, .5f);
            }
            if (lerpCool == true && Timer <= 1f) //As we wait for the cooldown, increase the timer.
            {
                Timer += Time.deltaTime;
                cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, newTransform, rotSpeed * Time.deltaTime);
            }
            else if (lerpCool == true && Timer >= 1f) //Once our cooldown is over, turn off our cooldown and reset our timer for future use.
            {
                lerpCool = false;
                Timer = 0;
            }
            if (lerpCool == false)
            {
                mouseXclamp = 0f;
                mouseYclamp = 0f;
                cameraPos.DetachChildren();
                cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, cameraPos.rotation, rotSpeed * Time.deltaTime);
            }
        }
    }

    private void LateUpdate()
    {
        if (!console.isWarping)
        {
            cam.gameObject.transform.position = Vector3.MoveTowards(cam.gameObject.transform.position, cameraPos.position, posSpeed * Time.deltaTime);
        }
        else
        {
            cam.gameObject.transform.position = cameraPos.position;
        }
    }
}

