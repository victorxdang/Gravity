/*****************************************************************************************************************
 - SettingsUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the logic for interacting with the settings UI including animations to control showing or hiding
     the settings UI.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject uiHelp;

    [SerializeField] Button buttonBGM,
                            buttonSE,
                            buttonAchievements,
                            buttonSignIn,
                            buttonHelp,
                            buttonExitHelp;

    [SerializeField] Slider sliderVolumeBGM,
                            sliderVolumeSE;

    [SerializeField] Animator animBGM,
                              animSE;

    bool sliderBGMExpanded = false,
         sliderSEExpanded = false;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Calls the method to set up buttons.
    /// </summary>
    void Start()
    {
        ButtonSetup();
    }

    #endregion


    #region Settings UI Animation

    /// <summary>
    /// Expands or contracts the background music volume slider depending on its current state.
    /// </summary>
    public void ToggleBGMSlider()
    {
        if (!sliderBGMExpanded)
            ExpandBGMSlider();
        else
            ContractBGMSlider();
    }

    /// <summary>
    /// Expands or contracts the sound effect volume slider depending on its current state.
    /// </summary>
    public void ToggleSESlider()
    {
        if (!sliderSEExpanded)
            ExpandSESlider();
        else
            ContractSESlider();
    }

    /// <summary>
    /// Plays the animation to expand the background music slider.
    /// </summary>
    void ExpandBGMSlider()
    {
        sliderBGMExpanded = true;
        animBGM.SetTrigger("Expand");
    }

    /// <summary>
    /// Plays the animation to contract the background music slider.
    /// </summary>
    void ContractBGMSlider()
    {
        sliderBGMExpanded = false;
        animBGM.SetTrigger("Contract");
    }

    /// <summary>
    /// Plays the animation to expand the sound effect slider.
    /// </summary>
    void ExpandSESlider()
    {
        sliderSEExpanded = true;
        animSE.SetTrigger("Expand");
    }

    /// <summary>
    /// Plays the animation to contract the sound effect slider.
    /// </summary>
    void ContractSESlider()
    {
        sliderSEExpanded = false;
        animSE.SetTrigger("Contract");
    }

    #endregion


    #region Utilties

    /// <summary>
    /// Updates the volume sliders so that they reflect the current volume that is set within the AudioManager class.
    /// </summary>
    public void UpdateVolume()
    {
        sliderVolumeBGM.value = AudioManager.Instance.BGMVolume;
        sliderVolumeSE.value = AudioManager.Instance.SEVolume;
    }

    /// <summary>
    /// Set the help menu to on or off depending on the value passed by the active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetHelpMenuActive(bool active)
    {
        if (GameManager.Instance)
            GameManager.Instance.InHelpMenu = active;
        else
            MainMenuManager.Instance.InHelpMenu = active;

        uiHelp.gameObject.SetActive(active);
    }

    /// <summary>
    /// Set the x-scale of the volume sliders to zero.
    /// 
    /// NOTE: to ensure proper execution of this code, ensures that the game object containing the animation clips is first disabled
    /// and then set the proper scale of the volume sliders. Also remember to set sliderBGMExpanded and sliderSEExpanded booleans
    /// to false so that the animation will work properly when opening up the settings UI again.
    /// </summary>
    public void ZeroSliderScale()
    {
        buttonBGM.interactable = true;
        buttonSE.interactable = true;

        animBGM.gameObject.SetActive(false);
        animSE.gameObject.SetActive(false);

        sliderVolumeBGM.transform.parent.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
        sliderVolumeSE.transform.parent.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);

        animBGM.gameObject.SetActive(true);
        animSE.gameObject.SetActive(true);

        sliderBGMExpanded = false;
        sliderSEExpanded = false;
    }

    /// <summary>
    /// Logic when the help button is pressed. If in game, then hide the help menu and display the pause menu. If in the start menu,
    /// then make sure that the credits menu is no longer active and enable the help menu.
    /// </summary>
    void HelpButtonClick()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.TogglePauseMenu();
            SetHelpMenuActive(!uiHelp.gameObject.activeSelf);
        }
        else
        {
            if (MainMenuManager.Instance.InCreditMenu)
                MainMenuManager.Instance.SetCreditsMenuActive(false);
            else
                MainMenuManager.Instance.ToggleMainMenu();

            SetHelpMenuActive(!uiHelp.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Logic when the exit button on the help screen is pressed. Enable pause menu (if in game) or main menu (if in start menu) and
    /// disable the help menu.
    /// </summary>
    void HelpExitButtonClick()
    {
        if (GameManager.Instance)
            GameManager.Instance.SetPauseMenuActive(true);
        else
            MainMenuManager.Instance.SetMainMenuActive(true);

        SetHelpMenuActive(false);
    }

    /// <summary>
    /// Setup the appropriate logic for the buttons. For the two volume sliders, whenever the value is changed,
    /// then update the corresponding variables in the AudioManager class.
    /// </summary>
    void ButtonSetup()
    {
        buttonBGM.onClick.AddListener(ToggleBGMSlider);
        buttonSE.onClick.AddListener(ToggleSESlider);
        buttonAchievements.onClick.AddListener(GPGSManager.ShowAchievements);
        buttonSignIn.onClick.AddListener(GPGSManager.GPGSSignIn);
        buttonHelp.onClick.AddListener(HelpButtonClick);
        buttonExitHelp.onClick.AddListener(HelpExitButtonClick);

        sliderVolumeBGM.onValueChanged.AddListener(delegate  { AudioManager.Instance.BGMVolume = sliderVolumeBGM.value; });
        sliderVolumeSE.onValueChanged.AddListener(delegate { AudioManager.Instance.SEVolume = sliderVolumeSE.value; });
    }

    #endregion
}