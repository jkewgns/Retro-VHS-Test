using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    private float timeElapsed = 0f;  // Track time elapsed
    private int minutes = 0;
    private int hours = 0;

    void Update()
    {
        // Increment the timeElapsed by the time passed since the last frame (deltaTime)
        timeElapsed += Time.deltaTime;

        // Only update every minute (60 seconds)
        if (timeElapsed >= 60f)
        {
            timeElapsed = 0f; // Reset the elapsed time every minute

            // Increment the minutes, reset after 59
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }

            // Loop the hours between 0 and 11 (12-hour format)
            if (hours >= 12)
            {
                hours = 0;
            }

            // Format time as hh:mmAM (no space before AM)
            int displayHour = hours == 0 ? 12 : hours;  // 0 should be displayed as 12
            string timeFormatted = $"{displayHour:D2}:{minutes:D2}AM";  // No space between time and AM

            // Update the TextMeshPro text
            timeText.text = timeFormatted;
        }
    }
}
