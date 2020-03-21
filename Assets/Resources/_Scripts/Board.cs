// Author: Kermit Mitchell III
// Start Date: 03/17/2020 8:45 PM | Last Edited: 03/21/2020 12:05 AM
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
    private Button spinButton; // the button clicked to make spinning happen

    [SerializeField] private int score; // number of points player has
    private Text scoreText; // the text for the score
    private Text gainedText; // shows how many points were gained per payline per spin
    private bool isSpinning = false; // a flag variable to lock the spinning if the user is already spinning

    // Initalize the variables
    private void Start()
    {
        // Create the spin counter and button
        isSpinning = false;
        spinCounter = 999;
        spinText = GameObject.Find("SpinText").GetComponent<Text>();
        spinText.text = "Spins: " + spinCounter.ToString("D3");
        spinButton = GameObject.Find("SpinButton").GetComponent<Button>();

        // Create the score text
        score = 0;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score: " + score.ToString("D7");
        gainedText = GameObject.Find("PointsGainedText").GetComponent<Text>();
        gainedText.text = "Gained: " + 0;

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

        if(this.isSpinning)
        {
            Debug.LogError("Time: " + Time.time + "Error! Currently spinning. Must wait for this spin to end.");
            this.spinButton.interactable = false;
            return;
        }

        if (spinCounter < 1)
        {
            // Error
            Debug.LogError("Time: " + Time.time + " Error: " + "You need at least one Spin to Spin " +
                " | " + "spinCounter: " + spinCounter);
            return;
        }
        else
        {
            this.isSpinning = true;
            this.spinButton.interactable = false;
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
        PayTableLine tableLine = PayTableLine.HorizontalTop; // Start with the first paytableine, horizontal top row 
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

        // Main Loop For Each PayTableLine:
        StartCoroutine(EvaluateEachPayline());//EvaluateEachPayline();


        // Local Function to Pick the panel based on the current payline
        void PanelIndexFromPayline()
        {
            switch (tableLine)
            {
                case PayTableLine.HorizontalTop:
                    panelIndex = 0;
                    break;

                case PayTableLine.HorizontalOuter:
                    panelIndex = 1;
                    break;

                case PayTableLine.HorizontalInner:
                    panelIndex = 2;
                    break;

                case PayTableLine.HorizontalBottom:
                    panelIndex = 3;
                    break;

                case PayTableLine.ParabolicUTop:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 0;
                    }
                    else
                    {
                        panelIndex = 1;
                    }
                    break;

                case PayTableLine.ParabolicUMiddle:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 1;
                    }
                    else
                    {
                        panelIndex = 2;
                    }
                    break;

                case PayTableLine.ParabolicUBottom:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 2;
                    }
                    else
                    {
                        panelIndex = 3;
                    }
                    break;

                    //TODO: ParabolicNTop~Bottom (8-10), BigVTop~Bottom (11-12), BigATop~Bottom (13-14)
            }
        }

        // Local Coroutine Function to Evaluate All Paylines Procedurally
        IEnumerator EvaluateEachPayline()
        {
            // Main Loop For Each PayTableLine:
            for (int i = 1; i <= 7/*Enum.GetNames(typeof(PayTableLine)).Length*/; i++)
            {
                // Reset the counters and change the PayTableLine
                tableLine = (PayTableLine)i;
                slotIndex = slots.Length - 1;
                absMaxPanel = 0;
                absMaxOccurance = 1;
                absMaxIndex = 0;

                // Pick the panel based on the current payline
                PanelIndexFromPayline();

                // Start at the right-most panel in this row 
                referencePanel = slots[slotIndex].getPanels()[panelIndex].GetFruit();
                localMaxIndex = slotIndex;
                //Debug.LogWarning("Time: " + Time.time + " | " + "ReferencePanel: " + referencePanel + " | " +
                //                "slotIndex: " + slotIndex + " | " + "localMaxIndex: " + localMaxIndex);

                // Move the slotIndex to the left, and pick the panel based on the current payline
                slotIndex--;
                PanelIndexFromPayline();

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
                        if (localMaxOccurance > absMaxOccurance && Panel.panelScores[referencePanel] >= Panel.panelScores[absMaxPanel])
                        {
                            absMaxOccurance = localMaxOccurance;
                            absMaxPanel = referencePanel;
                            absMaxIndex = localMaxIndex;

                            //Debug.Log("Time: " + Time.time + " | " + "Updating AbsMax..." + " | " +
                            //    "AbsMaxOcc: " + absMaxOccurance + " | " + "AbsMaxIndex: " + absMaxIndex);
                        }


                        // Try the next panel to the left and reset the occurance counters

                        referencePanel = nextPanel;
                        localMaxOccurance = 1;
                        localMaxIndex = slotIndex;
                        //Debug.LogWarning("Time: " + Time.time + " | " + "ReferencePanel: " + referencePanel + " | " +
                        //    "slotIndex: " + slotIndex + " | " + "localMaxIndex: " + localMaxIndex);

                    }

                    // Move the slotIndex to the left, and pick the panel based on the current payline
                    slotIndex--;
                    PanelIndexFromPayline();

                    if (slotIndex < 0)
                    {
                        nextPanel = PanelIcon.NULL;
                    }
                    else
                    {
                        nextPanel = slots[slotIndex].getPanels()[panelIndex].GetFruit();
                    }

                    //Debug.Log("Time: " + Time.time + " | " + "NextPanel: " + nextPanel + " | " +
                    //          "slotIndex: " + slotIndex);
                }

                // Report the max occurance and pay the player if needed

                // TODO: Only count chains of 3 or higher (absMaxOcc >= 3)
                int pointsGained = (Panel.panelScores[absMaxPanel] * absMaxOccurance);
                Debug.Log("Time: " + Time.time + "| " + "PayTableLine: " + tableLine + " | " +
                    "Icon: " + absMaxPanel + " | " +
                    "AbsMaxOcc: " + absMaxOccurance + " | " + "AbsMaxIndex: " + absMaxIndex + " | " +
                    "Points: " + pointsGained + " (" + Panel.panelScores[absMaxPanel] + "*" + absMaxOccurance + ")");

                // Display the winning configuration's PayTableLine as a Coroutine
                if(pointsGained > 0)
                {
                    yield return new WaitForSeconds(0.50f);
                    gainedText.text = "Gained: " + pointsGained;
                    yield return DisplayWinningPayline();
                }

                // TODO: Link up points gained and reference panel to the reference panel UI
                // Update Score UI // TODO: Make this a EventListner triggered UI update thing.
                score += pointsGained;
                scoreText.text = "Score: " + score.ToString("D7");
                gainedText.text = "Gained: " + 0;
            }

            // Reset the isSpinning lock
            this.isSpinning = false;
            this.spinButton.interactable = true;
        }

        // Coroutine to show the winning panels animation
        IEnumerator DisplayWinningPayline()
        {
            // Get a list of all panels to change their display state
            Panel[] affectedPanels = new Panel[slots.Length]; 

            switch (tableLine)
            {
                case PayTableLine.HorizontalTop:
                    affectedPanels[0] = slots[0].getPanels()[0];
                    affectedPanels[1] = slots[1].getPanels()[0];
                    affectedPanels[2] = slots[2].getPanels()[0];
                    affectedPanels[3] = slots[3].getPanels()[0];
                    affectedPanels[4] = slots[4].getPanels()[0];
                    break;

                case PayTableLine.HorizontalOuter:
                    affectedPanels[0] = slots[0].getPanels()[1];
                    affectedPanels[1] = slots[1].getPanels()[1];
                    affectedPanels[2] = slots[2].getPanels()[1];
                    affectedPanels[3] = slots[3].getPanels()[1];
                    affectedPanels[4] = slots[4].getPanels()[1];
                    break;

                case PayTableLine.HorizontalInner:
                    affectedPanels[0] = slots[0].getPanels()[2];
                    affectedPanels[1] = slots[1].getPanels()[2];
                    affectedPanels[2] = slots[2].getPanels()[2];
                    affectedPanels[3] = slots[3].getPanels()[2];
                    affectedPanels[4] = slots[4].getPanels()[2];
                    break;

                case PayTableLine.HorizontalBottom:
                    affectedPanels[0] = slots[0].getPanels()[3];
                    affectedPanels[1] = slots[1].getPanels()[3];
                    affectedPanels[2] = slots[2].getPanels()[3];
                    affectedPanels[3] = slots[3].getPanels()[3];
                    affectedPanels[4] = slots[4].getPanels()[3];
                    break;

                case PayTableLine.ParabolicUTop:
                    affectedPanels[0] = slots[0].getPanels()[0];
                    affectedPanels[1] = slots[1].getPanels()[1];
                    affectedPanels[2] = slots[2].getPanels()[1];
                    affectedPanels[3] = slots[3].getPanels()[1];
                    affectedPanels[4] = slots[4].getPanels()[0];
                    break;

                case PayTableLine.ParabolicUMiddle:
                    affectedPanels[0] = slots[0].getPanels()[1];
                    affectedPanels[1] = slots[1].getPanels()[2];
                    affectedPanels[2] = slots[2].getPanels()[2];
                    affectedPanels[3] = slots[3].getPanels()[2];
                    affectedPanels[4] = slots[4].getPanels()[1];
                    break;

                case PayTableLine.ParabolicUBottom:
                    affectedPanels[0] = slots[0].getPanels()[2];
                    affectedPanels[1] = slots[1].getPanels()[3];
                    affectedPanels[2] = slots[2].getPanels()[3];
                    affectedPanels[3] = slots[3].getPanels()[3];
                    affectedPanels[4] = slots[4].getPanels()[2];
                    break;

                    //TODO: ParabolicNTop~Bottom (8-10), BigVTop~Bottom (11-12), BigATop~Bottom (13-14)
            }

            // Change the PanelState of each panel based on if it's a win or lose
            for (int i = 0; i < affectedPanels.Length; i++)
            {
                if (i <= absMaxIndex && i >= (absMaxIndex-absMaxOccurance) + 1)
                {
                    affectedPanels[i].SetState(PanelState.Win);
                }
                else
                {
                    affectedPanels[i].SetState(PanelState.Lose);
                }
            }

            // View the winning arrangement for 1.5 seconds
            yield return new WaitForSeconds(1.5f);

           // Reset all affected panels back to normal
           foreach(Panel panel in affectedPanels)
            {
                panel.SetState(PanelState.Default);
            }

           // Wait fourth a second before moving on to the next PayTableLine
           yield return new WaitForSeconds(0.25f);

        }

    }



}


// The list of all valid paylines in the game, the permutation of icons and how they payout
public enum PayTableLine
{
   NULL = 0, // used for debugging, doesn't exist
   HorizontalTop = 1, // the first row from top down, panelIndex = 0
   HorizontalOuter = 2, // the second row from top down, panelIndex = 1
   HorizontalInner = 3, // the third from the top down, panelIndex = 2
   HorizontalBottom = 4, // the last (fourth) row from the top down, panelIndex = 3
   ParabolicUTop = 5, // left-most and right-most panels are panelIndex = 0, the rest are panelIndex = 1
   ParabolicUMiddle = 6, // left-most and right-most panels are panelIndex = 1, the rest are panelIndex = 2
   ParabolicUBottom = 7 // left-most and right-most panels are panelIndex = 2, the rest are panelIndex = 3
   //TODO: ParabolicNTop~Bottom (8-10), BigVTop~Bottom (11-12), BigATop~Bottom (13-14)

}

