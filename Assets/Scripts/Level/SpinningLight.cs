using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLight : MonoBehaviour {

    private int direction;
    private int speed;
    private float lightDuration = 0;
    private GameObject mainLight;

    private void Start()
    {
        mainLight = GameObject.FindGameObjectWithTag("GlobalLighting");
    }

    public void Update()
    {
        if (lightDuration > 0)
        {
            mainLight.GetComponent<Light>().intensity = 0.5f;

            if (direction == 0)
            {
                Vector3 angle = new Vector3(gameObject.transform.rotation.eulerAngles.x, (gameObject.transform.rotation.eulerAngles.y + speed), gameObject.transform.rotation.eulerAngles.z);
                gameObject.transform.rotation = Quaternion.Euler(angle);
            }
            else
            {
                Vector3 angle = new Vector3(gameObject.transform.rotation.eulerAngles.x, (gameObject.transform.rotation.eulerAngles.y - speed), gameObject.transform.rotation.eulerAngles.z);
                gameObject.transform.rotation = Quaternion.Euler(angle);
            }

            lightDuration -= Time.deltaTime;
        }
        else
        {
            mainLight.GetComponent<Light>().intensity = 1.1f;
            gameObject.GetComponent<Light>().intensity = 0;
        }
    }

    //private string colour;
    public void Activate(int dir, int spd, float duration, int colourNum = 3)
    {
        direction = dir;
        speed = spd;
        gameObject.GetComponent<Light>().color = GLOBAL_VALUES.COLOR_NUMBERS[colourNum];
        gameObject.GetComponent<Light>().intensity = 100;
        lightDuration = duration;
    }
}