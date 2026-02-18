using Unity.VisualScripting;
using UnityEngine;

public class RedactorPathfinder : MonoBehaviour
{
    public Transform home; //Stay close to home. Don't stray too far.
    [SerializeField] Transform player;
    [Tooltip("How far can we stray from home?")]
    public float maxStrayDistance = 8000f;
    [Tooltip("How far does a point need to be for changing course to be acceptable?")]
    public float minPointDistance = 1000f;
    public float maxPointDistance = 3000f;
    [Tooltip("What distance threshold do we need before we can change routes?")]
    public float threshholdDistance = 200f;
    [Tooltip("Physics-related tweaking")]
    public float rotationSpeed = 0.3f;
    public float acceleration = 100f;
    public float maxSpeed = 100000f;

    private Rigidbody rb;
    private Transform redactor;

    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem thrustEffect;
    [SerializeField] SoundManager soundManager;

    private Vector3 Target = Vector3.zero; //Where are we going?

    float staggeredUpdateInterval = 0.2f;
    float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        redactor = transform;
        rb = GetComponent<Rigidbody>();
        thrustEffect.Stop();
        rb.maxLinearVelocity = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > staggeredUpdateInterval)
        {
            timer = 0f;
            CheckRoute();
            handleMusic();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void GenerateNewTarget() //Self explanatory
    {
        if (Vector3.Distance(transform.position, home.position) >= maxStrayDistance)
        {
            Target = home.position;
        }
        else
        {
            Vector3 potentialTarget = redactor.position + (Random.onUnitSphere * Random.Range(minPointDistance, maxPointDistance));
            if (Vector3.Distance(redactor.position, home.position) < maxStrayDistance)
            {
                Target = potentialTarget;
            }
        }
    }

    void CheckRoute() //Make sure we're always going somewhere meaningful
    {
        if(Target == Vector3.zero)
        {
            GenerateNewTarget();
        }
        if(Vector3.Distance(redactor.position, Target) <= threshholdDistance)
        {
            GenerateNewTarget();
        }
    }

    void HandleMovement()
    {
        Quaternion whereToLook = Quaternion.LookRotation((Target - redactor.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, whereToLook, Time.fixedDeltaTime * rotationSpeed);

        if(Vector3.Dot(redactor.forward, (Target - redactor.position).normalized) > 0.8f)
        {
            rb.AddForce(transform.forward * acceleration * Time.fixedDeltaTime);
            rb.linearDamping = 0f;
            animator.SetBool("maneuvering", false);
            animator.SetBool("enroute", true);
            thrustEffect.Play();
        }

        else
        {
            rb.linearDamping = 5f;
            animator.SetBool("maneuvering", true);
            animator.SetBool("enroute", false);
            thrustEffect.Stop();
        }
    }

    void handleMusic()
    {
        if(Vector3.Distance(transform.position, player.position) <= 15000)
        {
            soundManager.Request(4);
        }
        else
        {
            soundManager.EndRequest(4);
        }
    }
}
