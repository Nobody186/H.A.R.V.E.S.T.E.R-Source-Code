using UnityEngine;

public class enemyMaterial : MonoBehaviour
{
    [SerializeField] GameObject custompass;
    [SerializeField] GameObject LOD0;
    [SerializeField] GameObject LOD1;
    private Material matLOD0;
    private Material matLOD1;
    private Transform player;
    private float distance;
    [SerializeField] float distanceToSee;
    [SerializeField] ConsoleController console;
    private float lerpAmount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = console.gameObject.transform;
        matLOD0 = LOD0.GetComponent<Renderer>().material;
        if (LOD1 != null)
        {
            matLOD1 = LOD1.GetComponent<Renderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if(console.isSeeing && distance <= distanceToSee)
        {
            custompass.SetActive(true);
            matLOD0.SetFloat("_Lerper", lerpAmount);
            if (LOD1 != null)
            {
                matLOD1.SetFloat("_Lerper", lerpAmount);
            }
            if (lerpAmount < 1)
            {
                lerpAmount += Time.deltaTime;
                print("LERP AMOUNT: " + lerpAmount);
            }
            else if(lerpAmount > 1)
            {
                lerpAmount = 1;
            }
        }
        else
        {
            custompass.SetActive(false);
            if(lerpAmount == 0)
            {
                return;
            }
            else
            {
                matLOD0.SetFloat("_Lerper", lerpAmount);
                if (LOD1 != null)
                {
                    matLOD1.SetFloat("_Lerper", lerpAmount);
                }
                if (lerpAmount > 0)
                {
                    lerpAmount -= Time.deltaTime;
                }
                else if (lerpAmount < 0)
                {
                    lerpAmount = 0;
                }
            }
        }
    }
}
