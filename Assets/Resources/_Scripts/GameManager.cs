// Author: Kermit Mitchell III
// Start Date: 04/07/2020 2:45 PM | Last Edited: 04/08/2020 7:35 PM
// This script manages the Game

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // TODO: Refactor code to make a GameManager, and move Score, Options code into that section
    public static GameManager instance; // singleton of the GameManager

    [SerializeField] public int score; // number of points player has
    public Text scoreText; // the text for the score
    public Text gainedText; // shows how many points were gained per payline per spin

    private Button PayTableButton; // button to open pay table tab
    private Button PayTableCloseButton; // button to close pay table tab

    private Button OptionsButton; // button to open options tab
    private Button OptionsCloseButton; // button to close options tab

    private static List<Sprite> BackgroundImages; // contains all background images
    private Image BackgroundImage; // the currently selected background art

    private Slider masterVolume; // slider to control master volume
    private Slider musicVolume; // slider to control music volume
    private Slider SFXVolume; // slider to control SFX volume

    private void Awake()
    {
        // Singleton Logic
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }


    private void Start()
    {
        // Create the options and pay table references
        OptionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
        OptionsCloseButton = GameObject.Find("OptionsTab").transform.Find("CloseButton").GetComponent<Button>();
        PayTableButton = GameObject.Find("PayTableButton").GetComponent<Button>();
        PayTableCloseButton = GameObject.Find("PayTableTab").transform.Find("CloseButton").GetComponent<Button>();

        if (BackgroundImages == null)
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

        // Create the score text
        score = 0;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = score.ToString("D7");
        gainedText = GameObject.Find("PointsGainedText").GetComponent<Text>();
        gainedText.text = "";

        // Close options and pay table tabs
        CloseOptionsTab();
        ClosePayTableTab();
    }


    public void OpenOptionsTab()
    {
        this.OptionsCloseButton.transform.parent.gameObject.SetActive(true);
    }

    public void CloseOptionsTab()
    {
        this.OptionsCloseButton.transform.parent.gameObject.SetActive(false);
    }

    public void OpenPayTableTab()
    {
        this.PayTableCloseButton.transform.parent.gameObject.SetActive(true);
    }

    public void ClosePayTableTab()
    {
        this.PayTableCloseButton.transform.parent.gameObject.SetActive(false);
    }

    public void ChangeBackgroundArt(int backgroundNum)
    {
        BackgroundImage.sprite = BackgroundImages[backgroundNum - 1];
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

}
