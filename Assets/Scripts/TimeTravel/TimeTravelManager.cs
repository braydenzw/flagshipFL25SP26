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
    private int currentFrame = 0; // Current number of frames in timeline
    private bool timeTravelCalled = false; // Bool to ensure button down detection with fixedUpdate
    private readonly int timeLogInterval = 250; // How often to record time travel, measured in fixed update ticks
    private int currentInterval = 0; // var to track how often to log time travel
    private float currentTime = 0;
    
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
        if (Input.GetKeyDown(KeyCode.G)) timeTravelDistance++;
        if(Input.GetKeyDown(KeyCode.H)) timeTravelDistance--;
        timeTravelDistance = Mathf.Clamp(timeTravelDistance, 0, currentFrame);
        int timeToDisplay = (int)((timeTravelDistance) * (timeLogInterval * Time.fixedDeltaTime));
        if (timeToDisplay != 0) timeToDisplay++; // tbh I don't know why this is necessary but it is, don't remove
        TimeSpan t = TimeSpan.FromSeconds(timeToDisplay);
        travelDestinationText.text = '-' + t.ToString(@"mm\:ss");
        
        // Check time travel button
        if(Input.GetKeyDown(KeyCode.T)) timeTravelCalled = true;
    }

    // Sets time of clock
    void SetClock()
    {
        TimeSpan t = TimeSpan.FromSeconds((int)(currentTime));
        currentTimeText.text = t.ToString(@"mm\:ss");
    }

    void FixedUpdate()
    {
        currentInterval++;
        if (currentInterval >= timeLogInterval)
        {
            // Log new travel frame
            currentInterval = 0;
            foreach (TimeTravelable obj in timeTravelObj)
            {
                obj.LogFrame();
            }
            currentFrame++;
        }
        
        
        if(timeTravelCalled)
        {
            TimeTravel();
        }
        timeTravelCalled = false; 
    }

    void TimeTravel()
    {
        int timeTravelLocation = currentFrame - timeTravelDistance;
        
        // Time traveling each obj
        foreach (TimeTravelable obj in timeTravelObj)
        {
            obj.TimeTravel(timeTravelLocation);
        }
        
        // Resetting to new time
        currentFrame = timeTravelLocation;
        currentTime = currentFrame * timeLogInterval * Time.fixedDeltaTime;
        currentInterval = 0;
        SetClock();
        timeTravelCalled = false;
    }
}
