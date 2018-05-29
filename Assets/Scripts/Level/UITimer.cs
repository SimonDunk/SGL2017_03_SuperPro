using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour {

    public Image uiClockImage;
    public Text preGameTime;

	void Start () {
        uiClockImage.fillAmount = 1.0f;
    }

    public void UpdateTimer(float time)
    {
        uiClockImage.color = time <= 10f ? new Color32(255, 0, 0, 255) : new Color32(255, 255, 255, 255);
        float percent = time / 120f;
        uiClockImage.fillAmount = Mathf.Lerp(0, 1, percent);
    }

    public void UpdatePreGameTimer(int time)
    {
        uiClockImage.color = new Color32(255, 255, 255, 255);
        uiClockImage.fillAmount = 1;
        preGameTime.gameObject.SetActive(true);
        preGameTime.text = time == 0 ? "Go!" : "" + time;
    }
}
