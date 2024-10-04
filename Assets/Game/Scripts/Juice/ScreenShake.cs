using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;

    private float shakePower;
    private float shakeTimeRemaining;
    private float shakeFadeTime;
    private float shakeRot;
    private float rotMultiplier = 15;


    void Awake()
    {
        instance = this; // one instance allowed
    }

    // since we don't have camera controller, this can go here
    void Update()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;

        shakeFadeTime = power / length;

        shakeRot = power * rotMultiplier;
    }

    void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;

            transform.position += new Vector3(xAmount, yAmount, 0);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);

            shakeRot = Mathf.MoveTowards(shakeRot, 0, shakeFadeTime * rotMultiplier * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0, 0, shakeRot * Random.Range(-1f, 1f));
    }
}
