using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    [Header("Time Settings")]
    public TextMeshProUGUI timeText;
    private float timeElapsed = 0f;
    private int minutes = 0;
    private int hours = 0;

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 60f)
        {
            timeElapsed = 0f;
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }

            if (hours >= 12)
            {
                hours = 0;
            }

            int displayHour = hours == 0 ? 12 : hours;
            string timeFormatted = $"{displayHour:D2}:{minutes:D2}AM";

            timeText.text = timeFormatted;
        }
    }
}