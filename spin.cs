using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name.Contains("wrech"))
        {
            gameObject.GetComponent<Rigidbody>().AddTorque(Random.Range(0, 2f), Random.Range(0, 2f), Random.Range(0, 2f));
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().AddTorque(Random.Range(0, transform.localScale.x * 15f), Random.Range(0, transform.localScale.y * 15f), Random.Range(0, transform.localScale.z * 15f));
        }
    }
}
