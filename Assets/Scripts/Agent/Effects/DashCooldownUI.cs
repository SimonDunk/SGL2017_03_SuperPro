using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour {

    private bool isDashActive;
    private bool isCooldownActive;
    private bool isSet;
    private float activeTime;
    private float dashCooldownMaxTime = GLOBAL_VALUES.DASH_COOLDOWN;
    private Timer dashTimer = new Timer();
    private float scaleMult;
    private float baseScaleMult = 0.0025f;
    private Vector3 baseScale;

    public Timer dashCooldownTimer;
    public Image dashCooldownImage;
    public Image dashCooldownBackgroundImage;
    //public ParticleSystem dashOffCooldownPart;

    // Sets up the bar so that it is full
    // and sets them as inactive.
	void Start ()
    {
        dashCooldownImage.fillAmount = 1.0f;
        dashCooldownImage.gameObject.SetActive(false);
        dashCooldownBackgroundImage.gameObject.SetActive(false);

        scaleMult = baseScaleMult;
        baseScale = gameObject.transform.localScale;
    }
	
	// Updates the timer every frame, as well as
    // checking each timer.
	void Update ()
    {
        dashTimer.Update();

        // If both the duration and cooldown are complete
        // then hide the slider. Else, if the image isn't active
        // (and one of the timers is running) then show the slider.
        if (isSet)
        {
            if (dashTimer.isComplete() && dashCooldownTimer.isComplete())
            {
                //dashCooldownImage.gameObject.SetActive(false);
                //dashCooldownBackgroundImage.gameObject.SetActive(false);

                // If the alpha of the image is at 1, then run the fade function
                if (dashCooldownImage.GetComponent<CanvasRenderer>().GetAlpha() == 1.0f)
                {

                    Fade();
                    //dashOffCooldownPart.Play();
                    //StartCoroutine(Expand());
                }
                if (dashCooldownImage.GetComponent<CanvasRenderer>().GetAlpha() > 0.0f)
                {
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + scaleMult, gameObject.transform.localScale.y + scaleMult, gameObject.transform.localScale.z + scaleMult);
                    scaleMult += baseScaleMult;
                }
            }
            else if (!dashCooldownImage.IsActive())
            {
                dashCooldownImage.gameObject.SetActive(true);
                dashCooldownBackgroundImage.gameObject.SetActive(true);
            }
        }
        
        // If the duration is over then tell the script it is over.
        if (dashTimer.isComplete())
        {
            isDashActive = false;
        }

        // If the mod is active and the timer isn't complete then
        // create a 'percentage' out of the leftover time on the timer
        // and the max duration of the mod, then reduce the fill amount
        // based on this.
        // Else, if the cooldown is active and the mod duration is over
        // then do the same for the cooldown, except increase it instead.
        if (isDashActive && !dashTimer.isComplete())
        {
            gameObject.transform.localScale = baseScale;
            scaleMult = baseScaleMult;

            dashCooldownImage.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            dashCooldownBackgroundImage.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            float percent = dashTimer.Get_Time() / GLOBAL_VALUES.DASH_DURATION;
            dashCooldownImage.fillAmount = Mathf.Lerp(0, 1, percent);
        } else if (isCooldownActive && dashTimer.isComplete())
        {
            gameObject.transform.localScale = baseScale;
            scaleMult = baseScaleMult;

            dashCooldownImage.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            dashCooldownBackgroundImage.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            float percent = dashCooldownTimer.Get_Time() / dashCooldownMaxTime;
            dashCooldownImage.fillAmount = Mathf.Lerp(1, 0, percent);
        }

        // If the cooldown timer has been set and the cooldown timer is
        // complete then tell the script that it is over.
        if (isSet == true)
        {
            if (dashCooldownTimer.isComplete())
            {
                isCooldownActive = false;
            }
        }
	}

    public void Fade()
    {
        dashCooldownImage.CrossFadeAlpha(0.0f, 0.5f, false);
        dashCooldownBackgroundImage.CrossFadeAlpha(0.0f, 0.5f, false);
    }

    IEnumerator Expand()
    {
        float currentScaleMult = 1.0f;
        //gameObject.transform.localScale = Vector3.Lerp(baseScale, new Vector3(transform.localScale.x * scaleMult, transform.localScale.y * scaleMult, transform.localScale.z * scaleMult), 0.5f);
        while (dashCooldownImage.GetComponent<CanvasRenderer>().GetAlpha() > 0.0f) 
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * currentScaleMult, gameObject.transform.localScale.y * currentScaleMult, gameObject.transform.localScale.z * currentScaleMult);
            currentScaleMult += 10.0f;
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    // Starts the duration timer and sets the fill images as active.
    public void StartDashTimer()
    {
        dashTimer.Add(GLOBAL_VALUES.DASH_DURATION, true);
        isDashActive = true;
        dashCooldownImage.gameObject.SetActive(true);
        dashCooldownBackgroundImage.gameObject.SetActive(true);
    }

    // Takes in the cooldown timer and lets the script know that it
    // is active and has been set.
    public void SetTimer(Timer dTimer)
    {
        dashCooldownTimer = dTimer;
        isCooldownActive = true;
        isSet = true;
    }
}
