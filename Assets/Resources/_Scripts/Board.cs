// Author: Kermit Mitchell III
// Start Date: 03/17/2020 8:45 PM | Last Edited: 03/25/2020 12:45 AM
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

    private bool isSpinning = false; // flag variable to lock the SpinButton if the user is already spinning
    private bool isLastSlotDone = false; // flag variable to lock EvaluateBoard() until last Slot finishes spinning
    private Toggle autoSpin; // flag variable to loop Spin() if user has AutoSpin enabled

    // TODO: Refactor code to make a GameManager, and move Score, Options code into that section
    [SerializeField] private int score; // number of points player has
    private Text scoreText; // the text for the score
    private Text gainedText; // shows how many points were gained per payline per spin

    private Button OptionsButton; // button to open options tab
    private Button OptionsCloseButton; // button to close options tab

    private static List<Sprite> BackgroundImages; // contains all background images
    private Image BackgroundImage; // the currently selected background art

    private Slider masterVolume; // slider to control master volume
    private Slider musicVolume; // slider to control music volume
    private Slider SFXVolume; // slider to control SFX volume

    public void OpenOptionsTab()
    {
        this.OptionsCloseButton.transform.parent.gameObject.SetActive(true);
    }

    public void CloseOptionsTab()
    {
        this.OptionsCloseButton.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeBackgroundArt(int backgroundNum)
    {
        BackgroundImage.sprite = BackgroundImages[backgroundNum-1];
    }

    public void ChangeBackgroundMusic(int backgroundNum)
    {
        AudioManager.instance.Play((AudioName)backgroundNum);
    }

    // We use Log10(Volume as Percent) * 20 to naturally scale down the sound of the music
    // Changes the Master Volume using LogScale
    public void ChangeMasterVolume()
    {
        float vol = Mathf.Clamp(masterVolume.value, 0.0001f, 1.0f);
        AudioManager.instance.mixer.SetFloat("MasterVolume", Mathf.Log10(vol) * 20);  
    }

    public void ChangeMusicVolume()
    {
        float vol = Mathf.Clamp(musicVolume.value, 0.0001f, 1.0f);
        AudioManager.instance.mixer.SetFloat("MusicVolume", Mathf.Log10(vol) * 20);
    }

    public void ChangeSFXVolume()
    {
        float vol = Mathf.Clamp(SFXVolume.value, 0.0001f, 1.0f);
        AudioManager.instance.mixer.SetFloat("SFXVolume", Mathf.Log10(vol) * 20);
    }


    // Initalize the variables
    private void Start()
    {
        // Play bgm1 Music
        AudioManager.instance.Play(AudioName.BGM1);

        // Create the options references
        OptionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
        OptionsCloseButton = GameObject.Find("OptionsTab").transform.Find("CloseButton").GetComponent<Button>();
        
        if(BackgroundImages == null)
        {
            BackgroundImages = new List<Sprite>();
            BackgroundImages.Add(Resources.Load<Sprite>("_Images/background1"));
            BackgroundImages.Add(Resources.Load<Sprite>("_Images/background2"));
            BackgroundImages.Add(Resources.Load<Sprite>("_Images/background3"));
        }
        BackgroundImage = GameObject.Find("BackgroundImage").GetComponent<Image>();
        masterVolume = GameObject.Find("MasterVolumeSlider").GetComponent<Slider>();
        musicVolume = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        SFXVolume = GameObject.Find("SFXVolumeSlider").GetComponent<Slider>();

        // Create the spin counter and button
        isSpinning = false;
        spinCounter = 999;
        spinText = GameObject.Find("SpinText").GetComponent<Text>();
        spinText.text = "Spins: " + spinCounter.ToString("D3");
        spinButton = GameObject.Find("SpinButton").GetComponent<Button>();
        autoSpin = GameObject.Find("AutoSpinToggle").GetComponent<Toggle>();


        // Create the score text
        score = 0;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = score.ToString("D7");
        gainedText = GameObject.Find("PointsGainedText").GetComponent<Text>();
        gainedText.text = "";

        // Create the GameBoard
        slots = new Slot[5]; // because the GameBoard is a 4x5
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = this.transform.Find("Slot (" + i + ")").GetComponent<Slot>();
        }

        // Close options tab
        CloseOptionsTab();

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
            // Lock the Spin Button until the Spin is over
            this.isSpinning = true;
            this.spinButton.interactable = false;
            spinCounter--;
            spinText.text = "Spins: " + spinCounter.ToString("D3");

            // Spin the reel of each Slot and generate a new board
            StartCoroutine(SpinEachSlot());
            
            // Evaluate the board for the pay table
            EvaluateBoard();
            
        }

        // Coroutine to make the reel spin
        IEnumerator SpinSlot(Slot slot)
        {
            isLastSlotDone = false;
            Vector2 pos = Vector3.zero;

            Timer timer = new Timer(3.75f); // Controls how long each Slot spins for
            while (!timer.IsTimeUp())
            {
                // Make all icons in the slot move down
                foreach (Image image in slot.GetIcons())
                {
                    image.rectTransform.Translate(Vector2.down * 100 * Time.deltaTime);
                    // Randomly pick a new PanelIcon for the Icon once it goes offscreen, and move it to the top
                    if (image.rectTransform.position.y <= -35)
                    {
                        int randFruit = UnityEngine.Random.Range(1, Enum.GetNames(typeof(PanelIcon)).Length);
                        image.sprite = Panel.panelSprites[(PanelIcon)randFruit];
                        pos.x = image.rectTransform.localPosition.x;
                        pos.y = 15;
                        image.rectTransform.position = pos;
                        pos = Vector2.zero;
                        image.rectTransform.SetAsFirstSibling();
                    }
                }

                //Debug.Log("Time: " + Time.time + " | " + "Time Left on Timer: " + timer.GetTimeLeft());
                timer.Countdown();
                yield return null;

            }

            // Smoothly mount the icons into the correct locations
            int[] lerpYTargets = { 10, 0, -10, -20, -30 }; // the yPos of the resting position of each Slot Icon
            timer.SetCooldown(0.50f);
            timer.ResetTimer();
            
            while (!timer.IsTimeUp())
            {
                for (int i = 0; i < lerpYTargets.Length; i++)
                {
                    var image = slot.GetIcons()[i];
                    pos.x = image.rectTransform.localPosition.x;
                    pos.y = Mathf.Lerp(image.rectTransform.localPosition.y, 
                        lerpYTargets[image.rectTransform.GetSiblingIndex()] + 10, timer.GetPercentDone());
                    image.rectTransform.localPosition = pos;
                }

                timer.Countdown();
                yield return null;
            }

            // Update each Panel in this Slot with the new PanelIcons/Sprites
            slot.SetEachPanelIcons();

            // Allow the EvaluateBoard() function to run after the last Slot finishes spinning
            if(slot == this.slots[this.slots.Length - 1])
            {
                isLastSlotDone = true;
            }

            
        }

        // Coroutine to make all the reels spin almost concurrently
        IEnumerator SpinEachSlot()
        {
            foreach(Slot slot in this.slots)
            {
                StartCoroutine(SpinSlot(slot));
                yield return new WaitForSeconds(0.25f);
            }
        }

    }

    // Determines the payout based on predefined pay tables/permutations of the icons
    private void EvaluateBoard()
    {
        PayTableLine tableLine = PayTableLine.HorizontalTop; // Start with first PayTableLine, horizontal top row 
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
        StartCoroutine(EvaluateEachPayline());


        // Local Function that picks panelIndex based on current PayTableLine and slotIndex
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

                case PayTableLine.BigVTop:
                    if(slotIndex == 0 || slotIndex == slots.Length-1)
                    {
                        panelIndex = 0;
                    }
                    else if(slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 1;
                    }
                    else
                    {
                        panelIndex = 2;
                    }
                    break;

                case PayTableLine.BigVBottom:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 1;
                    }
                    else if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 2;
                    }
                    else
                    {
                        panelIndex = 3;
                    }
                    break;

                case PayTableLine.BigABottom:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 3;
                    }
                    else if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 2;
                    }
                    else
                    {
                        panelIndex = 1;
                    }
                    break;

                case PayTableLine.BigATop:
                    if (slotIndex == 0 || slotIndex == slots.Length - 1)
                    {
                        panelIndex = 2;
                    }
                    else if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 1;
                    }
                    else
                    {
                        panelIndex = 0;
                    }
                    break;

                case PayTableLine.WTop:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 1;
                    }
                    else
                    {
                        panelIndex = 0;
                    }
                    break;

                case PayTableLine.WMiddle:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 2;
                    }
                    else
                    {
                        panelIndex = 1;
                    }
                    break;

                case PayTableLine.WBottom:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 3;
                    }
                    else
                    {
                        panelIndex = 2;
                    }
                    break;

                case PayTableLine.MTop:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 0;
                    }
                    else
                    {
                        panelIndex = 1;
                    }
                    break;

                case PayTableLine.MMiddle:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 1;
                    }
                    else
                    {
                        panelIndex = 2;
                    }
                    break;

                case PayTableLine.MBottom:
                    if (slotIndex == 1 || slotIndex == slots.Length - 2)
                    {
                        panelIndex = 2;
                    }
                    else
                    {
                        panelIndex = 3;
                    }
                    break;



            }
        }

        // Local Coroutine Function to Evaluate All Paylines Procedurally
        IEnumerator EvaluateEachPayline()
        {
            // Only start this after all Slot reels have finished spinning
            yield return new WaitUntil(() => isLastSlotDone);

            // Main Loop For Each PayTableLine:
            for (int i = 1; i <= /*14*/Enum.GetNames(typeof(PayTableLine)).Length-1; i++)
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
                referencePanel = slots[slotIndex].GetPanels()[panelIndex].GetFruit();
                localMaxIndex = slotIndex;
                //Debug.LogWarning("Time: " + Time.time + " | " + "ReferencePanel: " + referencePanel + " | " +
                //                "slotIndex: " + slotIndex + " | " + "localMaxIndex: " + localMaxIndex);

                // Move the slotIndex to the left, and pick the panel based on the current payline
                slotIndex--;
                PanelIndexFromPayline();

                nextPanel = slots[slotIndex].GetPanels()[panelIndex].GetFruit();
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
                        nextPanel = slots[slotIndex].GetPanels()[panelIndex].GetFruit();
                    }

                    //Debug.Log("Time: " + Time.time + " | " + "NextPanel: " + nextPanel + " | " +
                    //          "slotIndex: " + slotIndex);
                }

                // Report the max occurance and pay the player if needed
                int pointsGained = 0;
                if (absMaxOccurance >= 3) // TODO: Change to 3 on Release Build; Keep 2 for Debug Build
                {
                    pointsGained = (Panel.panelScores[absMaxPanel] * absMaxOccurance);
                }
                Debug.Log("Time: " + Time.time + "| " + "PayTableLine: " + tableLine + " | " +
                    "Icon: " + absMaxPanel + " | " +
                    "AbsMaxOcc: " + absMaxOccurance + " | " + "AbsMaxIndex: " + absMaxIndex + " | " +
                    "Points: " + pointsGained + " (" + Panel.panelScores[absMaxPanel] + "*" + absMaxOccurance + ")");

                // Display the winning configuration's PayTableLine as a Coroutine
                if(pointsGained > 0)
                {
                    yield return new WaitForSeconds(0.50f);
                    gainedText.text = "+" + pointsGained;
                    yield return DisplayWinningPayline();
                }

                // Update Score UI // TODO: Make this a EventListner triggered UI update thing.
                score += pointsGained;
                scoreText.text = score.ToString("D7");
                gainedText.text = "";
            }

            // Reset the isSpinning lock
            this.isSpinning = false;
            // Spin again if AutoSpin is enabled
            if (autoSpin.isOn)
            {
                Spin();
            }
            else
            {
                this.spinButton.interactable = true;
            }

            
        }

        // Coroutine to show the winning panels animation
        IEnumerator DisplayWinningPayline()
        {
            // Get a list of all panels to change their display state
            Panel[] affectedPanels = new Panel[slots.Length]; 

            switch (tableLine)
            {
                case PayTableLine.HorizontalTop:
                    affectedPanels[0] = slots[0].GetPanels()[0];
                    affectedPanels[1] = slots[1].GetPanels()[0];
                    affectedPanels[2] = slots[2].GetPanels()[0];
                    affectedPanels[3] = slots[3].GetPanels()[0];
                    affectedPanels[4] = slots[4].GetPanels()[0];
                    break;

                case PayTableLine.HorizontalOuter:
                    affectedPanels[0] = slots[0].GetPanels()[1];
                    affectedPanels[1] = slots[1].GetPanels()[1];
                    affectedPanels[2] = slots[2].GetPanels()[1];
                    affectedPanels[3] = slots[3].GetPanels()[1];
                    affectedPanels[4] = slots[4].GetPanels()[1];
                    break;

                case PayTableLine.HorizontalInner:
                    affectedPanels[0] = slots[0].GetPanels()[2];
                    affectedPanels[1] = slots[1].GetPanels()[2];
                    affectedPanels[2] = slots[2].GetPanels()[2];
                    affectedPanels[3] = slots[3].GetPanels()[2];
                    affectedPanels[4] = slots[4].GetPanels()[2];
                    break;

                case PayTableLine.HorizontalBottom:
                    affectedPanels[0] = slots[0].GetPanels()[3];
                    affectedPanels[1] = slots[1].GetPanels()[3];
                    affectedPanels[2] = slots[2].GetPanels()[3];
                    affectedPanels[3] = slots[3].GetPanels()[3];
                    affectedPanels[4] = slots[4].GetPanels()[3];
                    break;

                case PayTableLine.BigVTop:
                    affectedPanels[0] = slots[0].GetPanels()[0];
                    affectedPanels[1] = slots[1].GetPanels()[1];
                    affectedPanels[2] = slots[2].GetPanels()[2];
                    affectedPanels[3] = slots[3].GetPanels()[1];
                    affectedPanels[4] = slots[4].GetPanels()[0];
                    break;

                case PayTableLine.BigVBottom:
                    affectedPanels[0] = slots[0].GetPanels()[1];
                    affectedPanels[1] = slots[1].GetPanels()[2];
                    affectedPanels[2] = slots[2].GetPanels()[3];
                    affectedPanels[3] = slots[3].GetPanels()[2];
                    affectedPanels[4] = slots[4].GetPanels()[1];
                    break;

                case PayTableLine.BigABottom:
                    affectedPanels[0] = slots[0].GetPanels()[3];
                    affectedPanels[1] = slots[1].GetPanels()[2];
                    affectedPanels[2] = slots[2].GetPanels()[1];
                    affectedPanels[3] = slots[3].GetPanels()[2];
                    affectedPanels[4] = slots[4].GetPanels()[3];
                    break;

                case PayTableLine.BigATop:
                    affectedPanels[0] = slots[0].GetPanels()[2];
                    affectedPanels[1] = slots[1].GetPanels()[1];
                    affectedPanels[2] = slots[2].GetPanels()[0];
                    affectedPanels[3] = slots[3].GetPanels()[1];
                    affectedPanels[4] = slots[4].GetPanels()[2];
                    break;

                case PayTableLine.WTop:
                    affectedPanels[0] = slots[0].GetPanels()[0];
                    affectedPanels[1] = slots[1].GetPanels()[1];
                    affectedPanels[2] = slots[2].GetPanels()[0];
                    affectedPanels[3] = slots[3].GetPanels()[1];
                    affectedPanels[4] = slots[4].GetPanels()[0];
                    break;

                case PayTableLine.WMiddle:
                    affectedPanels[0] = slots[0].GetPanels()[1];
                    affectedPanels[1] = slots[1].GetPanels()[2];
                    affectedPanels[2] = slots[2].GetPanels()[1];
                    affectedPanels[3] = slots[3].GetPanels()[2];
                    affectedPanels[4] = slots[4].GetPanels()[1];
                    break;

                case PayTableLine.WBottom:
                    affectedPanels[0] = slots[0].GetPanels()[2];
                    affectedPanels[1] = slots[1].GetPanels()[3];
                    affectedPanels[2] = slots[2].GetPanels()[2];
                    affectedPanels[3] = slots[3].GetPanels()[3];
                    affectedPanels[4] = slots[4].GetPanels()[2];
                    break;

                case PayTableLine.MTop:
                    affectedPanels[0] = slots[0].GetPanels()[1];
                    affectedPanels[1] = slots[1].GetPanels()[0];
                    affectedPanels[2] = slots[2].GetPanels()[1];
                    affectedPanels[3] = slots[3].GetPanels()[0];
                    affectedPanels[4] = slots[4].GetPanels()[1];
                    break;

                case PayTableLine.MMiddle:
                    affectedPanels[0] = slots[0].GetPanels()[2];
                    affectedPanels[1] = slots[1].GetPanels()[1];
                    affectedPanels[2] = slots[2].GetPanels()[2];
                    affectedPanels[3] = slots[3].GetPanels()[1];
                    affectedPanels[4] = slots[4].GetPanels()[2];
                    break;

                case PayTableLine.MBottom:
                    affectedPanels[0] = slots[0].GetPanels()[3];
                    affectedPanels[1] = slots[1].GetPanels()[2];
                    affectedPanels[2] = slots[2].GetPanels()[3];
                    affectedPanels[3] = slots[3].GetPanels()[2];
                    affectedPanels[4] = slots[4].GetPanels()[3];
                    break;

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
   BigVTop = 5, // panelIndex = {0,1,2,1,0} (makes a V on upper half)
   BigVBottom = 6, // panelIndex = {1,2,3,2,1} (makes a V on bottom half)
   BigABottom = 7, // panelIndex = {3,2,1,2,3} (makes an A on bottom half)
   BigATop = 8, // panelIndex = {2,1,0,1,2} (makes an A on upper half)
   WTop = 9, // panelIndex = {0,1,0,1,0} (makes a W on Top)
   WMiddle = 10, // panelIndex = {1,2,1,2,1} (makes a W in Middle)
   WBottom = 11, // panelIndex = {2,3,2,3,2} (makes a W on Bottom)
   MTop = 12, // panelIndex = {1,0,1,0,1} (makes a M on Top)
   MMiddle = 13, // panelIndex = {2,1,2,1,2} (makes a M in Middle)
   MBottom = 14 // panelIndex = {3,2,3,2,3} (makes a M on Bottom)
}
