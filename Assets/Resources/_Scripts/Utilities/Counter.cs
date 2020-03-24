// Author: Kermit Mitchell III
// Start Date: 08/06/2018 6:55 AM | Last Edited: 08/09/2018 12:40 AM
// This script is used as a general purpose integer based counter, for any script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter
{
    // Deeclare the variables

    private int count = 0; // the amount of things in the counter
    private int maxCount = int.MaxValue; // the capacity of the counter
    private int minCount = 0; // the minimum capacity of the counter

    // Constructors
    public Counter()
    {
        // Assume these to be true:
        this.SetMaxCount(int.MaxValue);
        this.SetMinCount(0);
        this.SetCount(0);
    }

    public Counter(int capacity, int minimum, int current)
    {
        this.SetMaxCount(capacity);
        this.SetMinCount(minimum);
        this.SetCount(current);
    }

    // Methods

    // Adds to the current value, default is 1
    public void Add(int a = 1)
    {
        if (a > 0)
        {
            this.SetCount(this.count + a);
        }
        else
        {
            Debug.LogError("Time: " + Time.time + " | " + "Must add a positive amount: " + a);
        }
    }

    // Subtracts from the current value, default is 1
    public void Remove(int a = 1)
    {
        if (a > 0)
        {
            this.SetCount(this.count - a);
        }
        else
        {
            Debug.LogError("Time: " + Time.time + " | " + "Must remove a positive amount: " + a);
        }
    }

    // Sets the current value to the min
    public void Clear()
    {
        this.SetCount(this.minCount);
    }

    // Sets the current value to the max
    public void Fill()
    {
        this.SetCount(this.maxCount);
    }

    // Checks if is full
    public bool IsFull()
    {
        return (this.count == this.maxCount);
    }

    // Checks if is minimized
    public bool IsMinimized()
    {
        return (this.count == this.minCount);
    }

    // Setters

    public void SetCount(int c)
    {
        if(c <= this.maxCount && c >= this.minCount)
        {
            this.count = c;
        }
        else
        {
            Debug.LogError("Time: " + Time.time + " | " + "This amount is out of bounds: " + c);
        }
    }

    public void SetMaxCount(int c)
    {
        if(c > this.minCount)
        {
            this.maxCount = c;
        }
        else
        {
            Debug.LogError("Time: " + Time.time + " | " + "This amount is less than the current minCount: " + c);
        }
    }

    public void SetMinCount(int c)
    {
        if (c < this.maxCount)
        {
            this.minCount = c;
        }
        else
        {
            Debug.LogError("Time: " + Time.time + " | " + "This amount is greater than the current maxCount: " + c);
        }
    }

    // Getters

    public int GetCount()
    {
        return this.count;
    }

    public int GetMaxCount()
    {
        return this.maxCount;
    }

    public int GetMinCount()
    {
        return this.minCount;
    }

}
