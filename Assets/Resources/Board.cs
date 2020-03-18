// Author: Kermit Mitchell III
// Start Date: 03/17/2020 8:45 PM | Last Edited: 03/17/2020 9:25 PM
// This script runs the game board and creates new spins and etc.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Declare the variables

    [SerializeField] private int spinCounter; // number of spins player has
    private Text spinText; // the text for the spinCounter
    private Slot[] slots; // the slots on the Board, from left to right 0-4

    // Initalize the variables
    private void Start()
    {
        // Create the spin counter
        spinCounter = 3;
        spinText = GameObject.Find("SpinText").GetComponent<Text>();
        spinText.text = "Spins: " + spinCounter.ToString("D3");

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
            
        }
    }

  
}
