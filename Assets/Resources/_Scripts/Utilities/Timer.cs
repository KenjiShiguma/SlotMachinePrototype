// Author: Kermit Mitchell III
// Start Date: 06/01/2018 | Last Edited: 08/18/2019 9:55 AM
// A generic Timer script that can be used in any Unity project

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer //: MonoBehaviour
{
    // Declare the variables

    private float currentTime; // the current time left on the Timer
    private float cooldown; // the number to count down from

    // Constructors
    public Timer()
    {
        this.SetCooldown(0.0f);
        this.SetCurrentTime(0.0f);
    }

    public Timer(float ct, float c)
    {
        this.SetCooldown(c);
        this.SetCurrentTime(ct);
    }

    public Timer(float c)
    {
        this.SetCooldown(c);
        this.SetCurrentTime(c);
    }

    // Subtracts time each FixedFrame
    public void Countdown() 
    {
        if (this.currentTime >= 0)
        {
            this.currentTime -= Time.fixedDeltaTime;
        }
        else
        {
            this.currentTime = 0;
        }
        
    }

    // Makes the current time back to the cooldown
    public void ResetTimer()
    {
        this.currentTime = this.cooldown;
    }

    // Reverses the timer so that timer counts down from it's current place
    public void Reverse()
    {
        this.SetCurrentTime(this.cooldown - this.currentTime);
    }

    // Checks if time is up, returns true if current time has elasped cooldown, false otherwise
    public bool IsTimeUp()
    {
        bool b = false;
        
        if(this.currentTime <= 0.0f)
        {
            b = true;
        }

        return b;
    }

    // Returns the percent complete the timer is to winding down; great for lerping
    public float GetPercentDone()
    {
        return (this.cooldown - this.currentTime) / this.cooldown;
    }

    public void SetCurrentTime(float t)
    {
            this.currentTime = t;
        
    }

    public void SetCooldown(float c)
    {
        this.cooldown = c;
    }

    public float GetTimeLeft()
    {
        return this.currentTime;
    }

    public float GetCooldown()
    {
        return this.cooldown;
    }
}
