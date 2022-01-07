/*****************************************************************************************************************
 - LevelFailedUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the text stating that the player has failed the level and two options, either restart the level
     or return to the main menu.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LevelFailedUI : MonoBehaviour
{
    [SerializeField] Button buttonRestart,
                            buttonMainMenu;


    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Sets up the correct logic for the buttons.
    /// </summary>
    void Start()
    {
        buttonRestart.onClick.AddListener(delegate { GameManager.Instance.RestartGame(false); });
        buttonMainMenu.onClick.AddListener(GameManager.Instance.MainMenu);
    }
}
