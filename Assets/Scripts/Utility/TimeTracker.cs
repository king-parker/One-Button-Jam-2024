using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    private Stopwatch stopwatch;
    
    void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    public float GetElapsedTime()
    {
        return (float)stopwatch.Elapsed.TotalSeconds;
    }

    public void RestartTime()
    {
        stopwatch.Restart();
    }
}
