using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BASICScript : MonoBehaviour
{

    public bool returnSignal = false;

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 7)
        {
            print("BASIC SIGNAL RETURNED!");
            returnSignal = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        returnSignal = false;
    }

}
