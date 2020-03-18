// Author: Kermit Mitchell III
// Start Date: 03/17/2020 11:10 PM | Last Edited: 03/17/2020 11:30 PM
// This script helps modify Slots

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    // Declare the variables

    private Panel[] panels; // the panels in the slot

    private void Start()
    {
        // Create the slot by grabbing the references
        panels = new Panel[4]; // 4 because the GameBoard is a 4x5
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i] = this.transform.Find("Panel (" + i + ")").GetComponent<Panel>();
        }

    }

    public Panel[] getPanels()
    {
        return this.panels;
    }

}