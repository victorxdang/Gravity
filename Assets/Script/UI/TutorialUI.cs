/*****************************************************************************************************************
 - TutorialUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the logic for when the player touches the screen to exit the UI.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Button buttonTutorialExit;

    /// <summary>
    /// Called when the object is enabled.
    /// 
    /// Initializes the exit tutorial button so that the game starts when the button is clicked.
    /// </summary>
    void Start()
    {
        buttonTutorialExit.onClick.AddListener(delegate
        {
            GameManager.Instance.ExitedTutorial = true;
            GameManager.Instance.StartGame();
            gameObject.SetActive(false);
        });
    }
}