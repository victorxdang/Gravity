/*****************************************************************************************************************
 - LevelCompleteUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the text stating that the player has successfully completed this level and will show three 
     options: restart the level, return to the main menu and move on to the next level.

     NOTE: if the player is currently on the last level, the next level button will no longer appear and there
     will only be two buttons, restart or return to main menu buttons.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] Button buttonRestart,
                            buttonMainMenu,
                            buttonNextLevel;


    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Sets up the button's repsective logic.
    /// </summary>
    void Start()
    {
        buttonRestart.onClick.AddListener(delegate { GameManager.Instance.RestartGame(true); });
        buttonMainMenu.onClick.AddListener(GameManager.Instance.MainMenu);
        buttonNextLevel.onClick.AddListener(GameManager.Instance.NextLevel);
    }

    /// <summary>
    /// Turns on or off the next level button depending on the value provided by the active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetNextLevelButtonActive(bool active)
    {
        buttonNextLevel.gameObject.SetActive(active);
    }
}
