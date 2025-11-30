using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{

    [SerializeField] float torqueForce = 500f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddTorque(torqueForce, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
