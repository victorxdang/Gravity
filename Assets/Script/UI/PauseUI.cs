/*****************************************************************************************************************
 - PauseUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles the buttons and text that appear on the pause menu in game.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class PauseUI : BaseSettingsUIHandler
{
    #region Fields

    [SerializeField] Button buttonResume,
                            buttonRestart,
                            buttonMainMenu,
                            buttonSettings;

    [SerializeField] Text textPause;

    #endregion


    #region Update Buttons/Texts

    /// <summary>
    /// Updates the pause text.
    /// </summary>
    /// <param name="text"></param>
    public void UpdatePauseText(string text)
    {
        textPause.text = text;
    }

    /// <summary>
    /// Sets all the buttons on the pause menu to either be active or not, depending
    /// on the value passed on to the active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetButtonsActive(bool active)
    {
        buttonResume.gameObject.SetActive(active);
        buttonRestart.gameObject.SetActive(active);
        buttonMainMenu.gameObject.SetActive(active);
        buttonSettings.gameObject.SetActive(active);
    }

    /// <summary>
    /// Calls ResumeGame() in GameManager and hides the SettingsUI.
    /// </summary>
    void ResumeGame()
    {
        uiSettings.gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    #endregion


    #region Overriden Methods

    /// <summary>
    /// Sets up the corresponding logic to the respective button.
    /// </summary>
    protected override void ButtonSetup()
    {
        buttonResume.onClick.AddListener(delegate 
        {
            // when the resume button is pressed, then if the settings menu is expanded, then 
            // first set the x-scale of the volume sliders to 0 and contract the settings menu;
            // after the settings UI animation is completed, then actually resume the game
            if (settingsExpanded)
            {
                uiSettings.ZeroSliderScale();
                ContractSettings();
                GameManager.Instance.Wait(0.5f, ResumeGame);
            }
            else
            {
                ResumeGame();
            }
        });

        buttonRestart.onClick.AddListener(delegate { GameManager.Instance.RestartGame(false); });
        buttonMainMenu.onClick.AddListener(GameManager.Instance.MainMenu);
        buttonSettings.onClick.AddListener(GameManager.Instance.SettingsMenu);
    }

    /// <summary>
    /// Logic to return to the previous menu if certain condition are met. If the settings menu is expanded, then
    /// contract the settings menu, otherwise then resume the game. 
    /// 
    /// NOTE: this method can only be called if the settings animation isn't currently playing.
    /// </summary>
    protected override void Back()
    {
        if (readyForInput)
        {
            if (GameManager.Instance.GamePaused && !GameManager.Instance.CountingDown && !settingsExpanded)
            {
                ResumeGame();
            }
            else if (GameManager.Instance.InHelpMenu)
            {
                SetHelpMenuActive(false);
                GameManager.Instance.SetPauseMenuActive(true);
            }
            else if (settingsExpanded)
            {
                ContractSettings();
            }
        }
    }

    #endregion
}