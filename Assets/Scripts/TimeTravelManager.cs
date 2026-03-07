using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeTravelManager : MonoBehaviour
{
    // List of objs to time travel
    private TimeTravelable[] timeTravelObj;

    private int timeTravelDistance = 0; // Num frames to travel backwards in time by
    private int currentInterval = 0; // Current number of frames in timeline
    private bool timeTravelCalled = false; // Bool to ensure button down detection with fixedUpdate
    [SerializeField] private readonly int timeLogInterval = 50; // How often to record time travel, measured in fixed update ticks
    [SerializeField] private readonly int maxIntervalsTimeTravelable = 6;
    private int frameTracking = 0; // var to track how often to log time travel
    private float currentTime = 0;


    private float scrollNecessary = 0.1f;
    private float scrollTotal = 0;
    private float noScrollTime = 0;
    private float maxNoScrollTime = 0.5f;

    [Header("Clock Displays")]
    [SerializeField] private TextMeshProUGUI currentTimeText;
    [SerializeField] private TextMeshProUGUI travelDestinationText;


    // Start is called before the first frame update
    void Start()
    {
        timeTravelObj = FindObjectsOfType<TimeTravelable>();
        foreach (TimeTravelable obj in timeTravelObj)
        {
            obj.LogFrame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update general world clock
        currentTime += Time.deltaTime;
        SetClock();

        // Update time travel clock
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAmount == 0)
        {
            noScrollTime += Time.deltaTime;
            if (noScrollTime > maxNoScrollTime)
            {
                scrollAmount = 0;
                noScrollTime = 0;
            }
        }
        else
        {
            scrollTotal += scrollAmount;
            noScrollTime = 0;
        }
        Debug.Log(scrollTotal);
        if (scrollTotal > scrollNecessary)
        {
            timeTravelDistance--;
            scrollTotal = 0;
        }
        if (scrollTotal < -scrollNecessary)
        {
            timeTravelDistance++;
            scrollTotal = 0;
        }



        timeTravelDistance = Mathf.Clamp(timeTravelDistance, 0, currentInterval);
        int timeToDisplay = (int)((currentInterval - timeTravelDistance) * (timeLogInterval * Time.fixedDeltaTime));
        if (timeToDisplay != 0) timeToDisplay++; // tbh I don't know why this is necessary but it is, don't remove
        TimeSpan t = TimeSpan.FromSeconds(timeToDisplay);
        travelDestinationText.text = "Time Destination: " + t.ToString(@"mm\:ss");

        // Check time travel button
        if (Input.GetKeyDown(KeyCode.LeftShift)) timeTravelCalled = true;
    }

    // Sets time of clock
    void SetClock()
    {
        TimeSpan t = TimeSpan.FromSeconds((int)(currentTime));
        currentTimeText.text = t.ToString(@"mm\:ss");
    }

    void FixedUpdate()
    {
        frameTracking++;
        if (frameTracking >= timeLogInterval)
        {
            // Log new travel frame
            frameTracking = 0;
            foreach (TimeTravelable obj in timeTravelObj)
            {
                obj.LogFrame();
            }
            currentInterval++;
            timeTravelDistance++;
        }


        if (timeTravelCalled)
        {
            TimeTravel();
        }
        timeTravelCalled = false;
    }

    void TimeTravel()
    {
        int timeTravelLocation = currentInterval - timeTravelDistance;

        // Time traveling each obj
        foreach (TimeTravelable obj in timeTravelObj)
        {
            obj.TimeTravel(timeTravelLocation);
        }

        // Resetting to new time
        currentInterval = timeTravelLocation;
        currentTime = currentInterval * timeLogInterval * Time.fixedDeltaTime;
        frameTracking = 0;
        SetClock();
        timeTravelCalled = false;
        timeTravelDistance = 0;
    }
}
