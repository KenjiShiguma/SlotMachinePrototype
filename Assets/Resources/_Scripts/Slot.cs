// Author: Kermit Mitchell III
// Start Date: 03/17/2020 11:10 PM | Last Edited: 03/24/2020 1:05 AM
// This script helps modify Slots

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    // Declare the variables

    private Panel[] panels; // the panels in the slot
    private Image[] icons; // the images in the slot used for spinning (assigned to panels after spinning)

    private void Start()
    {
        // Create the slot by grabbing the panel references
        panels = new Panel[4]; // 4 because the GameBoard is a 4x5
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i] = this.transform.Find("Panel (" + i + ")").GetComponent<Panel>();
            panels[i].SetImage(this.transform.Find("Canvas").Find("MaskImage").Find("Icon (" + i + ")").GetComponent<Image>());
            //panels[i].SetFruit(PanelIcon.Pear);
            panels[i].SetState(PanelState.Default);
        }

        // Grab the references to the icons
        icons = new Image[this.transform.Find("Canvas").Find("MaskImage").childCount];
        for(int i = 0; i < icons.Length; i++)
        {
            icons[i] = this.transform.Find("Canvas").Find("MaskImage").Find("Icon (" + i + ")").GetComponent<Image>();
        }

    }


    // Assigns the PanelIcon to the Panel based on the Sprite in the Slot at the Panel's Location
    public void SetEachPanelIcons()
    {
        PanelIcon icon = PanelIcon.NULL;
        int panelIndex = -1;

        foreach(Image image in icons)
        {
            // Ignore the last image since it's offscreen
            if (image.rectTransform.GetSiblingIndex() == image.rectTransform.parent.childCount-1)
            {
                //Debug.LogWarning("Time: " + Time.time + " | " + "Image: " + image.name + " | " + "Index: " + panelIndex);
                continue;
            }
            
            // Determine the PanelIcon based on the Sprite (would be better with two-way lookup table)
            if(image.sprite == Panel.panelSprites[PanelIcon.Apple])
            {
                icon = PanelIcon.Apple;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Kiwi])
            {
                icon = PanelIcon.Kiwi;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Lemon])
            {
                icon = PanelIcon.Lemon;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Orange])
            {
                icon = PanelIcon.Orange;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Peach])
            {
                icon = PanelIcon.Peach;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Pear])
            {
                icon = PanelIcon.Pear;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Pomegranate])
            {
                icon = PanelIcon.Pomegranate;
            }
            else if (image.sprite == Panel.panelSprites[PanelIcon.Watermelon])
            {
                icon = PanelIcon.Watermelon;
            }

            // Change the fruit on the panel of the icon where the image is
            panelIndex = image.rectTransform.GetSiblingIndex();
            //Debug.Log("Time: " + Time.time + " | " + "Image: " + image.name + " | " + "Index: " + panelIndex);
            panels[panelIndex].SetImage(image);
            panels[panelIndex].SetFruit(icon);
        }
    }


    // Getters
    public Panel[] GetPanels()
    {
        return this.panels;
    }

    public Image[] GetIcons()
    {
        return this.icons;
    }

}