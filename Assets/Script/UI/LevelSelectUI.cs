/*****************************************************************************************************************
 - LevelSelectUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the logic for buttons and text for the level select UI on the start menu. The components that 
     are emcompassed within this class is the play button, the two buttons changing the level and the text
     that shows the currently selected level.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    #region Fields

    [SerializeField] Button buttonPlay,
                            buttonNextLevel,
                            buttonPrevLevel;

    [SerializeField] Text textPlay,
                          textLevel;

    int level = 0;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Sets up the buttons, level text and buttons.
    /// </summary>
    void Start()
    {
        level = SaveManager.PlayerSaveData.highest_level + (SaveManager.PlayerSaveData.highest_level == Data.MAX_LEVELS ? 0 : 1);
  
        ButtonSetup();
        UpdateText();
        UpdateButtons();
    }

    #endregion


    #region Update Buttons/Texts

    /// <summary>
    /// Updates the play button so that it reflects the ability for the player to be able to press play or not. If the currently
    /// selected level is less than or equal to 1 level than the highest level recorded on file, then set the color of the 
    /// button to green and allow the player to start the game with the selected level. Otherwise, the play button will display
    /// as red, signifying that the player cannot continue until the correct level is selected.
    /// </summary>
    /// <param name="level"></param>
    void UpdatePlayButton(int level)
    {
        if (level <= SaveManager.PlayerSaveData.highest_level + 1 || MainMenuManager.Instance.UnlockAllLevels)
            textPlay.color = Color.green;
        else
            textPlay.color = Color.red;
    }

    /// <summary>
    /// Updates the text of the currently selected level to reflect which level the player has played, is currently on and cannot play.
    /// If the level is less than 1 higher than the highest level recorded, then the currently selected level text will display as green.
    /// If the player is currently on the level that is 1 plus the highest level recorded, then the text color will be white, otherwise,
    /// the text will display as black, signifying that the level cannot be played. After a level is selected, then a new level preview
    /// will be displayed on screen only if the level is not colored black.
    /// </summary>
    void UpdateText()
    {
        if ((level < SaveManager.PlayerSaveData.highest_level + 1 && textLevel.color != Color.green) || MainMenuManager.Instance.UnlockAllLevels)
            textLevel.color = Color.green;
        else if (level == SaveManager.PlayerSaveData.highest_level + 1 && textLevel.color != Color.white)
            textLevel.color = Color.white;
        else if (level > SaveManager.PlayerSaveData.highest_level + 1 && textLevel.color != Color.black)
            textLevel.color = Color.black;

        textLevel.text = level.ToString();
        UpdatePlayButton(level);
        MainMenuManager.Instance.LoadPreview(level);
    }

    /// <summary>
    /// Updates the next and previous level buttons accordingly. If the player selects level 1, then the previous button
    /// will be turned off to disallow continuously decrementing the level. Likewise, if the player is currently on the
    /// highest level available in the game, then the next button will be disabled. Otherwise, both buttons should be
    /// on.
    /// </summary>
    void UpdateButtons()
    {
        if (level == 1)
        {
            buttonPrevLevel.gameObject.SetActive(false);
        }
        else if (level == Data.MAX_LEVELS)
        {
            buttonNextLevel.gameObject.SetActive(false);
        }
        else
        {
            if (!buttonPrevLevel.gameObject.activeSelf)
                buttonPrevLevel.gameObject.SetActive(true);

            if (!buttonNextLevel.gameObject.activeSelf)
                buttonNextLevel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets up the logic of the buttons accordingly.
    /// </summary>
    void ButtonSetup()
    {
        buttonPlay.onClick.AddListener(delegate 
        {
            MainMenuManager.Instance.PlayButtonClicked(level);
        });

        buttonPrevLevel.onClick.AddListener(delegate
        {
            level--;
            UpdateText();
            UpdateButtons();
        });

        buttonNextLevel.onClick.AddListener(delegate
        {
            level++;
            UpdateText();
            UpdateButtons();
        });
    }

    #endregion
}