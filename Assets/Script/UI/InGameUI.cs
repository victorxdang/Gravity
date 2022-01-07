/*****************************************************************************************************************
 - InGameUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the definition to update score text, FPS text and pause button.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class InGameUI : BaseUI
{
    [SerializeField] Text textScore,
                          textFPSCounter;

    [SerializeField] Button buttonPause;


    /// <summary>
    /// Called when the game object is enabled.
    /// 
    /// Set the FPS counter to active or not depending on the value set in the Debugging variable.
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Turns the FPS counter on the top left of the screen on or off depending on the value passed
    /// on by the active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetFPSCounterActive(bool active)
    {
        if (textFPSCounter.gameObject.activeSelf != active)
            textFPSCounter.gameObject.SetActive(active);
    }

    /// <summary>
    /// Updates the score text to reflect the current distance that the player has travelled.
    /// </summary>
    /// <param name="value"></param>
    public void UpdateScore(float value)
    {
        textScore.text = Mathf.Round(value) + " m/ " + Mathf.Round(MapParser.Instance.TotalDistance) + " m";
    }

    /// <summary>
    /// Update the FPS text to reflect the current FPS. This will also change the color of the text accordingly if the FPS
    /// starts dipping.
    /// </summary>
    /// <param name="value"></param>
    public void UpdateFPS(float value)
    {
        textFPSCounter.text = "FPS: " + value;

        if (value >= 50 && textFPSCounter.color != Color.green)
            textFPSCounter.color = Color.green;
        else if (value > 30 && value < 50 && textFPSCounter.color != Color.yellow)
            textFPSCounter.color = Color.yellow;
        else if (value <= 25 && textFPSCounter.color != Color.red)
            textFPSCounter.color = Color.red;
    }

    /// <summary>
    /// Overriden method, initializes pause button logic.
    /// </summary>
    protected override void ButtonSetup()
    {
        buttonPause.onClick.AddListener(GameManager.Instance.PauseGame);
    }

    /// <summary>
    /// Overriden method, add Back() method functionality.
    /// </summary>
    protected override void Back()
    {
        GameManager.Instance.PauseGame();
    }
}
