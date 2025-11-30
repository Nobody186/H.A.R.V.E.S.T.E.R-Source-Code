using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserCollisionHandler : MonoBehaviour
{

    public GunController gun;
    public Transform laserPos;
    public Transform gunPos;
    public GameObject physicalLaser;

    CapsuleCollider capCollider;

    private void Start()
    {
        capCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {

        gameObject.transform.position = gunPos.transform.position;
        gameObject.transform.rotation = physicalLaser.transform.rotation;

        if(gun.isMining && gun.chargingLaser == false)
        {
            capCollider.enabled = true;
        }
        else
        {
            capCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider laserHit)
    {
        print("LASER COLLIDED! " + laserHit.name);
        if (laserHit.transform.gameObject.layer == 11) //Laser no touch
        {
            gun.safeAngle = false;
            print("DANGER ZONE!");
        }
    }
    private void OnTriggerExit(Collider laserHit)
    {
        if (laserHit.transform.gameObject.layer == 11) //Laser no touch
        {
            gun.safeAngle = true;
            gunPos.localRotation = Quaternion.Euler(0, 0 ,0);
        }
    }
}
