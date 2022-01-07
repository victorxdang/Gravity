/*****************************************************************************************************************
 - BaseUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     The parent class to most of the UI used in this game. Doesn't contain much, but has basic functionality
     for most UI classes.
*****************************************************************************************************************/

using UnityEngine;

public class BaseUI : MonoBehaviour
{
    /// <summary>
    /// Should this game object update every frame?
    /// </summary>
    protected bool shouldUpdate = true;


    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    protected virtual void Start()
    {
        ButtonSetup();
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void Update()
    {
        // stop updating if specified
        if (!shouldUpdate)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    /// <summary>
    /// Method for code that returns to the previous UI or exits the game completely depending 
    /// on the UI it is being called from.
    /// </summary>
    protected virtual void Back()
    {
        // to be overridden by inherited UI
    }

    /// <summary>
    /// Method for adding code that initializes buttons in script.
    /// </summary>
    protected virtual void ButtonSetup()
    {
        // to be overridden by inherited UI
    }
}
