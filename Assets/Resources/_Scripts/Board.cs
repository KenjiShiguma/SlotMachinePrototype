// Author: Kermit Mitchell III
// Start Date: 03/17/2020 8:45 PM | Last Edited: 03/19/2020 12:45 AM
// This script runs the game board and creates new spins and etc.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Declare the variables

    // TODO: Make a singleton of the board for use in a game manager script
    private Slot[] slots; // the slots on the Board, from left to right 0-4

    [SerializeField] private int spinCounter; // number of spins player has
    private Text spinText; // the text for the spinCounter

    [SerializeField] private int score; // number of points player has
    private Text scoreText; // the text for the score

    // Initalize the variables
    private void Start()
    {
        // Create the spin counter
        spinCounter = 999;
        spinText = GameObject.Find("SpinText").GetComponent<Text>();
        spinText.text = "Spins: " + spinCounter.ToString("D3");

        score = 0;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score: " + score.ToString("D7");

        // Create the GameBoard
        slots = new Slot[5]; // because the GameBoard is a 4x5
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = this.transform.Find("Slot (" + i + ")").GetComponent<Slot>();
        }

    }

    // Generates a new game board, deducts a spin
    public void Spin()
    {
        Debug.Log("Time: " + Time.time + " Attempting to spin..." + " | " + "spinCounter: " + spinCounter);

        if (spinCounter < 1)
        {
            // Error
            Debug.LogError("Time: " + Time.time + " Error: " + "You need at least one Spin to Spin " +
                " | " + "spinCounter: " + spinCounter);
            return;
        }
        else
        {
            spinCounter--;
            spinText.text = "Spins: " + spinCounter.ToString("D3");
            // Spin each Panel on the Board
            foreach(Slot slot in slots)
            {
                foreach(Panel panel in slot.getPanels())
                {
                    panel.SpinPanel();
                }
            }

            // Evaluate the board for the pay table
            EvaluateBoard();
            
        }
    }

    // Determines the payout based on predefined pay tables/permutations of the icons
    private void EvaluateBoard()
    { 
        int slotIndex = slots.Length - 1; // current slotIndex; starts at top right, moves left
        int panelIndex = 0; // current panelIndex; starts at top, moves down
        // panelIndedx will remain 0 for the first row's pay table

        PanelIcon referencePanel = 0; // the icon of reference panel we are comparing to
        PanelIcon nextPanel = 0; // the icon of each subsequent neighboring icon to compare

        int localMaxOccurance = 1; // the current largest combination of the same icon
        int localMaxIndex = 0; // where the index of the currentMaxOccurance starts

        // For the absMax found in this search
        PanelIcon absMaxPanel = 0; // the icon associated with absMaxOccurance
        int absMaxOccurance = 1; // the highest recorded combination of the same icon
        int absMaxIndex = 0; // where the index of the absMaxOcc starts

        // Horizontal Row 1 // TODO: Horizontal Rows 2, 3, 4, V shape, W shape
        // Start at the right-most panel in this row 
        referencePanel = slots[slotIndex].getPanels()[panelIndex].GetFruit();
        localMaxIndex = slotIndex;
        //Debug.LogWarning("Time: " + Time.time + " | " + "ReferencePanel: " + referencePanel + " | " +
        //                "slotIndex: " + slotIndex + " | " + "localMaxIndex: " + localMaxIndex);
        slotIndex--;
        nextPanel = slots[slotIndex].getPanels()[panelIndex].GetFruit();
        //Debug.Log("Time: " + Time.time + " | " + "NextPanel: " + nextPanel + " | " +
        //           "slotIndex: " + slotIndex);

        while (slotIndex >= -1)
        { 
            // Keep track of the same occurances
            if (nextPanel == referencePanel/* && !((slotIndex + 1) < absMaxOccurance) && !()*/)
            {
                //Debug.Log("Time: " + Time.time + " | " + "Same icon found! Continuing search...");
                localMaxOccurance++;
            }
            // The chain is broken
            else
            {
                //Debug.Log("Time: " + Time.time + " | " + "Local chain broken..." + " | " +
                //        "LocalMaxOcc: " + localMaxOccurance + " | " +
                //        "AbsMaxOcc (BEFORE): " + absMaxOccurance);

                // Update the max occurance if needed
                if (localMaxOccurance > absMaxOccurance && Panel.panelScores[referencePanel] > Panel.panelScores[absMaxPanel])
                {
                    absMaxOccurance = localMaxOccurance;
                    absMaxPanel = referencePanel;
                    absMaxIndex = localMaxIndex;

                    //Debug.Log("Time: " + Time.time + " | " + "Updating AbsMax..." + " | " +
                    //    "AbsMaxOcc: " + absMaxOccurance + " | " + "AbsMaxIndex: " + absMaxIndex);
                }

                // Stop the search if you can't best the current absMaxOcc
                if ((slotIndex + 1) < absMaxOccurance)
                {
                    break;
                }
                // Otherwise, try the next panel to the left and reset the occurance counters
                else
                {
                    referencePanel = nextPanel;
                    localMaxOccurance = 1;
                    localMaxIndex = slotIndex;
                    //Debug.LogWarning("Time: " + Time.time + " | " + "ReferencePanel: " + referencePanel + " | " +
                    //    "slotIndex: " + slotIndex + " | " + "localMaxIndex: " + localMaxIndex);

                }
            }

            slotIndex--;
            if(slotIndex < 0)
            {
                nextPanel = PanelIcon.Default;
            }
            else
            {
                nextPanel = slots[slotIndex].getPanels()[panelIndex].GetFruit();
            }

            //Debug.Log("Time: " + Time.time + " | " + "NextPanel: " + nextPanel + " | " +
            //          "slotIndex: " + slotIndex);
        }

        // Report the max occurance and pay the player if needed

        int pointsGained = (Panel.panelScores[absMaxPanel] * absMaxOccurance);
        Debug.Log("Time: " + Time.time + "| " + "Icon: " + absMaxPanel + " | " + 
            "AbsMaxOcc: " + absMaxOccurance + " | " + "AbsMaxIndex: " + absMaxIndex + " | " +
            "Points: " + pointsGained + " (" + Panel.panelScores[absMaxPanel] + "*" + absMaxOccurance + ")");

        // TODO: Link up points gained and reference panel to the reference panel UI
        // Update Score UI // TODO: Make this a EventListner triggered UI update thing.
        score += pointsGained;
        scoreText.text = "Score: " + score.ToString("D7");

    }




}
