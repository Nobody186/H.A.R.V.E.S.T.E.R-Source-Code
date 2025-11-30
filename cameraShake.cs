using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    [SerializeField] Transform cameraPos;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Health health;
    public float shakeFactor = 0f; //A number we set depending on who is causing the camera shaking.
    private Vector3 shakePosition; //The point in space the camera will be going to

    private float duration; //How long the camera has been shaking

    public float shakeTime = 1f; //How long the camera should shake

    public bool isShaking = false; //These are to keep track of things, make sure we don't do anything stupid
    public bool notAlreadyStarted = true; //Putting a 'not' here is counter-intuitive but whatever

    private void Update()
    {
        if(!isShaking)  //If we are not shaking the camera, just operate the camera normally.
        {
            gameObject.transform.position = cameraPos.position;
        }
        if(health.force != 0f && notAlreadyStarted) //If we collide with something and we're not already shaking, we are gonna shake that camera
        {
            shakeFactor = (Mathf.Pow(0.00000111111f * health.force, 2f) + (0.00133333f * health.force) + (0.0555556f)); //A quadratic equation to get a nice exponential shake with depending on collison force
            StartCoroutine("SHAKE");
            notAlreadyStarted = false;
        }
    }

    public IEnumerator SHAKE() //Whenever we run this, the camera will shake.
    {
        float shakeAmount; //The magnitude of shakiness
        duration = 0f; //Our stopwatch
        while (shakeTime > duration) 
        {
            isShaking = true;
            duration += Time.deltaTime;

            shakeAmount = (curve.Evaluate(duration / shakeTime) * shakeFactor); //We have a pre-defined curve. We set our shake amount to a point on that curve depending on the duration to shake time ratio (0-1). 

            shakePosition = cameraPos.position + (Random.onUnitSphere * shakeAmount); //Make a random point in space that is near where the camera should be going. Multiply the radius from origin depending on shake amount.
            gameObject.transform.position = shakePosition; //Set our camera position to that point in space
            yield return null;
        }
        isShaking = false;
        health.force = 0f;
        notAlreadyStarted = true;
    }
}
