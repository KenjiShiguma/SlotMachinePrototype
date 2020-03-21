// Author: Kermit Mitchell III
// Start Date: 03/17/2020 9:30 PM | Last Edited: 03/21/2020 12:05 AM
// This script helps modify Panels

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Panel : MonoBehaviour
{
    // Declare the variables

    private PanelIcon fruit; // the fruit on the panel
    private SpriteRenderer icon; // a reference to Sprite component | the fruit image on the panel
    private int points; // the points gained by this fruit
    public static Dictionary<PanelIcon, Sprite> panelSprites; // Holds all panel Sprites // TODO: Make this a Singleton
    public static Dictionary<PanelIcon, int> panelScores; // Holds all fruit scores // TODO: Make this a Singleton
    private PanelState state = 0; // determines how panel is displayed
    private RawImage panelBorder; // the border around the panel

    private void Start()
    {
        // Create the panelSprites dictionary
        if (panelSprites == null)
        {
            panelSprites = new Dictionary<PanelIcon, Sprite>();
            panelSprites.Add(PanelIcon.NULL, null);
            panelSprites.Add(PanelIcon.Apple, Resources.Load<Sprite>("_Images/apple"));
            panelSprites.Add(PanelIcon.Kiwi, Resources.Load<Sprite>("_Images/kiwi"));
            panelSprites.Add(PanelIcon.Lemon, Resources.Load<Sprite>("_Images/lemon"));
            panelSprites.Add(PanelIcon.Orange, Resources.Load<Sprite>("_Images/orange"));
            panelSprites.Add(PanelIcon.Peach, Resources.Load<Sprite>("_Images/peach"));
            panelSprites.Add(PanelIcon.Pear, Resources.Load<Sprite>("_Images/pear"));
            panelSprites.Add(PanelIcon.Pomegranate, Resources.Load<Sprite>("_Images/pomegranate"));
            panelSprites.Add(PanelIcon.Watermelon, Resources.Load<Sprite>("_Images/watermelon"));
        }

        // Create the fruitScores dictionary
        if (panelScores == null)
        {
            panelScores = new Dictionary<PanelIcon, int>();
            panelScores.Add(PanelIcon.NULL, 0);
            panelScores.Add(PanelIcon.Apple, 100);
            panelScores.Add(PanelIcon.Kiwi, 100);
            panelScores.Add(PanelIcon.Lemon, 100);
            panelScores.Add(PanelIcon.Orange, 100);
            panelScores.Add(PanelIcon.Peach, 100);
            panelScores.Add(PanelIcon.Pear, 100);
            panelScores.Add(PanelIcon.Pomegranate, 100);
            panelScores.Add(PanelIcon.Watermelon, 100);
        }

        // Grab the sprite reference and change icon to match the fruit
        icon = this.transform.Find("Icon").GetComponent<SpriteRenderer>();

        // Pick the fruit
        this.SetFruit(PanelIcon.NULL);

        // Grab a reference to the panelBorder
        this.panelBorder = this.transform.Find("Canvas").transform.Find("PanelBorder").GetComponent<RawImage>();
        this.SetState(PanelState.Default);

    }

    // TODO: Make an event listener if the fruit value changes dynamically to call this function
    

    // Randomly chooses a fruit for the panel, 1-8 or 1-(how many fruit are in the game)
    public void SpinPanel()
    {
        int randFruit = UnityEngine.Random.Range(1, Enum.GetNames(typeof(PanelIcon)).Length);
        /*Debug.Log("Time: " + Time.time + " | " + "Spinning this panel..." + " | " +
            "CurrentFruit: " + fruit + " | " + "Rolled Fruit: " + (PanelIcon)randFruit + 
            "(" + randFruit + ")");*/
        SetFruit((PanelIcon)randFruit);
    }

    // Setters

    // Sets the fruit and updates the image and value
    public void SetFruit(PanelIcon icon)
    {
        fruit = icon;
        this.icon.sprite = panelSprites[fruit]; // Sets the sprite of the fruit
        points = panelScores[fruit]; // Set the value of the fruit

    }

    // Sets the PanelState and displays the panel accordingly
    public void SetState(PanelState state)
    {
        this.state = state;
        Color c = new Color(this.icon.color.r, this.icon.color.g, this.icon.color.b, 1);
        switch (this.state)
        {
            case PanelState.Default:
                this.icon.color = c;
                c.a = 0;
                this.panelBorder.color = c;
                break;

            case PanelState.Win:
                this.icon.color = c;
                this.panelBorder.color = c;
                break;

            case PanelState.Lose:
                this.panelBorder.color = c;
                c.a = 0.25f;
                this.icon.color = c;
                break;
        }

    }

    // Getters
    public PanelIcon GetFruit()
    {
        return this.fruit;
    }

    public PanelState GetState()
    {
        return this.state;
    }

}


// Defines the fruit icons in the game
public enum PanelIcon
{
    NULL = 0,
    Apple = 1,
    Kiwi = 2,
    Lemon = 3,
    Orange = 4,
    Peach = 5,
    Pear = 6,
    Pomegranate = 7,
    Watermelon = 8
}

// Defines how to display the panel after each Spin
public enum PanelState
{
    Default = 0, // panel has full transparency but no border
    Win = 1, // panel has full transparency and a border
    Lose = 2 // panel has reduced transpency and a border
}

