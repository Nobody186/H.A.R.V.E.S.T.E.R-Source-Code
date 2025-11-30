using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ConsoleController console;
    [SerializeField] SenseOfFlight sense;
    [SerializeField] float speed;
    [SerializeField] float actualSpeed;
    [SerializeField] float reverseSpeed;
    [SerializeField] float RollTime;
    [SerializeField] float PitchTime;
    private float actualRollTime;
    private float actualPitchTime;
    public Rigidbody rb;
    public float VehicleSpeed;
    public float forceApplied;

    public Vector3 playerDirection;
    public bool canMove = true;
    public bool useMouseAim = true;
    [SerializeField] Transform mouseAim;
    [SerializeField] GameObject fakeCursor;
    Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    Vector2 virtualCursor = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    [SerializeField] AudioSource engineNoise;
    [SerializeField] AudioSource dashNoise;
    [SerializeField] ParticleSystem antiVel;
    [SerializeField] AudioSource dampSFX;

    bool isDashing = false;
    bool playedDampSFX = false;

    bool playedTutorialMessage1;
    [SerializeField] AudioSource tutMessage1;
    [SerializeField] storyTrigger trigger1;

    public float ironMass = 7.8f;       // Real density values (g/cm³)
    public float nickelMass = 8.9f;
    public float clayMass = 2.6f;
    public float cobaltMass = 8.9f;
    public float diamondMass = 3.5f;
    public float plutoniumMass = 19.8f;
    public float magnesiumMass = 1.7f;
    public float aluminumMass = 2.7f;
    public float platiniumMass = 21.4f;
    public float helium3Mass = 0.0002f; 
    public float hydrogenMass = 0.0001f;
    public float carbonMass = 2.2f;
    public float iceMass = 0.92f;

    [SerializeField] Slider jettisonSlider;

    [SerializeField] GameObject Monitor;
    [SerializeField] Transform monitorHolder;
    // Start is called before the first frame update
    void Start()
    {
        //Load in player movement preferences, and load in any upgrades the player has bought.
        useMouseAim = console.usesMouseMode;
        rb = GetComponent<Rigidbody>();
        if (!useMouseAim)
        {
            fakeCursor.SetActive(false);
        }
        actualPitchTime = PitchTime + (2000f * console.sideThrustLvl);
        actualRollTime = RollTime + (2000f * console.sideThrustLvl);
        actualPitchTime = PitchTime;
        actualRollTime = RollTime;
        actualSpeed = speed + (console.mainThrustLvl * 130f);
        reverseSpeed = actualSpeed / 10f;
        engineNoise.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && console.usingDashUpgrade && isDashing == false && !console.isWarping) //Cool movement option
        {
            StartCoroutine(dash());
        }
        if(console.day == 0 && !playedTutorialMessage1 && trigger1.finishedTrigger && Input.GetKeyDown(KeyCode.W)) //Tutorial only line. This could probably be repurposed as its own monobehavior
        {
            tutMessage1.Play();
            playedTutorialMessage1 = true;
            canMove = false;
        }
        if(console.day == 0 && tutMessage1.isPlaying == false)
        {
            canMove = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Big fat one liner
        float totalCargoMass = console.ironHeld * ironMass + console.nickelHeld * nickelMass + console.clayHeld * clayMass + console.cobaltHeld * cobaltMass + console.diamondHeld * diamondMass + console.plutoniumHeld * plutoniumMass + console.magnesiumHeld * magnesiumMass + console.aluminumHeld * aluminumMass + console.platiniumHeld * platiniumMass + console.helium3Held * helium3Mass + console.hydrogenHeld * hydrogenMass + console.carbonHeld * carbonMass + console.iceHeld * iceMass;

        rb.mass = 1 + totalCargoMass / 10000000f; //Change our mass depending on our cargo load.

        float ADInput = Input.GetAxis("Horizontal");
        float WSInput = Input.GetAxis("Vertical");
        float throttle = Input.GetAxis("Throttle");

        forceApplied = throttle * speed;

        if (Input.GetButton("Jump") && console.usingdampDisablerUpgrade) //One of our movement options. This is the equivelant to the AoA limiter in games like Project Wingman.
        {
            rb.linearDamping = 0;
            if (!playedDampSFX)
            {
                dampSFX.Play();
                playedDampSFX = true;
            }
        }
        if(Input.GetButtonUp("Jump") && console.usingdampDisablerUpgrade)
        {
            playedDampSFX = false;
        }

        if (throttle > 0 && canMove) //This handles our forward movement.
        {
            engineNoise.volume += Time.deltaTime;
            rb.AddRelativeForce(0f, 0f, throttle * actualSpeed * Time.deltaTime);
            if (!Input.GetButton("Jump") && console.usingdampDisablerUpgrade)
            {
                rb.linearDamping = 0.3f;
            }
        }
        else if (throttle < 0 && canMove) //This handles our reverse movement.
        {
            engineNoise.volume += Time.deltaTime;
            rb.AddRelativeForce(0f, 0f, throttle * reverseSpeed * Time.deltaTime);
            if (!Input.GetButton("Jump") && console.usingdampDisablerUpgrade)
            {
                rb.linearDamping = 1.5f;
            }
        }
        else
        {
            engineNoise.volume -= Time.deltaTime; //If we are not moving, then lower the volume of the engine.
        }
        if(engineNoise.volume >= .5f) //Clamp engine volume to 0.5
        {
            engineNoise.volume = .5f;
        }
        if (!useMouseAim && canMove) //WASD rotation
        {
            fakeCursor.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            rb.AddRelativeTorque(WSInput * actualPitchTime * Time.deltaTime, 0f, -ADInput * actualRollTime * Time.deltaTime);
        }
        else if(useMouseAim && canMove) //Mouse based rotation is harder to handle. Simplify as a function.
        {
            RotateShip();
        }

        rb.maxAngularVelocity = 2f; //Set some limits on how fast we can go in an environment with no air resistance. Have I ever mentioned how much I love realistic simulators?
        rb.maxLinearVelocity = 120f;
        VehicleSpeed = rb.linearVelocity.magnitude; //We can use this number for calculations.

        engineNoise.pitch = (Mathf.Clamp01(VehicleSpeed))/5; //Who needs FMOD anyway?

        playerDirection = rb.linearVelocity;

        if(!Input.GetButton("Jump") && throttle == 0) //I don't know if this is necessary, but I don't want to cause any errors with the speed exponentially going lower but never reaching zero. 
        {
            rb.linearDamping = .5f; //Maybe there'd be too many bits in the speed alone.
        }
    }

    private void LateUpdate() //I don't fully remember why I did this but it works fine and that's good enough
    {
        Monitor.transform.position = monitorHolder.position;
        Monitor.transform.rotation = monitorHolder.rotation;
    }

    private void RotateShip()
    {
        if(Input.GetKey(KeyCode.LeftAlt)) //We still need a way to use the actual mouse. This does that.
        {
            Cursor.visible = true;
            fakeCursor.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }
        else if(Input.GetKey(KeyCode.Mouse1)) //This just allows us to use the mouse to look around without spinning the ship everywhere.
        {
            fakeCursor.SetActive(false);
            return;
        }
        fakeCursor.SetActive(true);
        Cursor.visible = false;
        virtualCursor += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sense.mouseSensitivity/50f; //Move the virtual cursor every frame based on our mouse movements
        virtualCursor.x = Mathf.Clamp(virtualCursor.x, 0, Screen.width); //Make sure we're not going out of bounds.
        virtualCursor.y = Mathf.Clamp(virtualCursor.y, 0, Screen.height);
        virtualCursor = Vector2.Lerp(virtualCursor, screenCenter, Time.deltaTime); //Here's the cool thing. The player doesn't need to reset their mouse. It automatically centers itself.
        fakeCursor.transform.position = virtualCursor;

        Vector2 offset = (virtualCursor - screenCenter) / screenCenter; //This allows us to convert mouse position into basically the same thing as an Input.GetAxis. We got a range of (-1, 1).

        float curvedX = (offset.x * Mathf.Abs(offset.x))/8f; //Sensitivity scaling
        float curvedY = (offset.y * Mathf.Abs(offset.y))/6f;

        // then scale:
        float yawTorque = curvedX * actualRollTime/3f; //Funny thing is, because of some flaws in our math, using KBM or the mouse input can give you a performance boost. I'm too tired to figure it out now. The difference is negligible anyway.
        float pitchTorque = -curvedY * actualRollTime/1.7f;
        float rollTorque = -curvedX * actualRollTime; //We roll the ship a little bit into the direction of the turn.

        rb.AddRelativeTorque(Vector3.up * yawTorque);
        rb.AddRelativeTorque(Vector3.right * pitchTorque);
        rb.AddRelativeTorque(Vector3.forward * rollTorque);

        if (virtualCursor.magnitude < 5f) // If we're less than 5 units away from the center
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None; //This seems counter-intuitive but I bet I put this in because of some weird bug and this fixed it.
        }
    }

    IEnumerator dash()
    {                       //This is dumb.
        isDashing = true;
        antiVel.Play();
        dashNoise.Play();
        rb.AddRelativeForce(0, 0, 200);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 400);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 500);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 600);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 500);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 400);
        yield return new WaitForEndOfFrame();
        rb.AddRelativeForce(0, 0, 200);
        yield return new WaitForSeconds(1.6f);
        isDashing = false;
    }
}
