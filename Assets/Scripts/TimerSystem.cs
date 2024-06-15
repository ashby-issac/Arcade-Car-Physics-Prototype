using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSystem
{
    private float timer = 10f;
    private float timerIncrement = 5f;

    public float Timer => timer;

    public float TimerIncrement = 5f;

    public TimerSystem()
    {
        GameplayController.OnCheckpointReached += UpdateTimer;
    }

    private void IncreaseTimer()
    {
        timer += timerIncrement;
    }

    public void UpdateTimer(float remTime)
    {
        timer = remTime;
        IncreaseTimer();
    }
}
