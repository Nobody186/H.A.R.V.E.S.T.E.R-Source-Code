using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class dataPodManager : MonoBehaviour
{
    //Good work day! Tomorrow you may have 18 hour shift. Hope you have good fun!

    [SerializeField] ConsoleController console;
    [SerializeField] GameObject waypointToGive;
    [SerializeField] Button decoderButton;
    [SerializeField] string message;
    [SerializeField] TextMeshProUGUI messageText;

    [SerializeField] bool TutorialPod;
    [SerializeField] int tutPodNumb = 1;

    [SerializeField] GunController gun;
    [SerializeField] monitorManager monitor;
    [SerializeField] GameObject app;

    bool openedMonitor;

    GameObject originalWaypointToGive;

    private void Start()
    {
        originalWaypointToGive = new GameObject(waypointToGive.name);
        originalWaypointToGive.transform.position = waypointToGive.transform.position;
    }

    //private void OnTriggerEnter(Collider other)
    //{
       // messageText.text = message;
        //if(other.gameObject.name == "PlayerShip" && console.navUpgrade)
        //{
          //  console.tutorialButton = TutorialPod;
           // console.tutPodNum = tutPodNumb;
            //console.waypointToAdd = originalWaypointToGive;
            //decoderButton.gameObject.SetActive(true);
        //}
    //}

    private void Update()
    {
        if(gun.Target == gameObject && console.navUpgrade &&!openedMonitor)
        {
            monitor.openMonitor();
            monitor.loadApp(app);
            messageText.text = message;
            console.tutorialButton = TutorialPod;
            console.tutPodNum = tutPodNumb;
            console.waypointToAdd = originalWaypointToGive;
            openedMonitor = true;
        }
        if(openedMonitor && gun.Target != gameObject)
        {
            openedMonitor = false;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.name == "PlayerShip")
    //    {
    //        StartCoroutine(hideUI());
    //    }
    //}

    //IEnumerator hideUI()
    //{
    //    decoderAnimator.SetBool("shouldShow", false);
    //    yield return new WaitForSeconds(1.95f);
    //    decoderButton.gameObject.SetActive(false);
    //}
}
