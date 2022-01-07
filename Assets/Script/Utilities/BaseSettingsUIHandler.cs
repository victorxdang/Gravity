/*****************************************************************************************************************
 - BaseSettingsUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class will hold all of the functionality needed to properly use the Settings UI. This is its own
     super class because the same functionality is shared between the Start UI and the Pause UI.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class BaseSettingsUIHandler : BaseUI
{
    #region Fields

    [SerializeField] protected SettingsUI uiSettings;

    protected bool settingsExpanded, 
                   readyForInput;

    Vector2 touchStart, touchEnd;
    Animator anim;
    Touch touch;

    #endregion


    #region Built-In Functions

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Retrieves the animator component from this game object.
    /// </summary>
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        uiSettings.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called every frame.
    /// 
    /// Retracts the settings menu when the player touches somewhere on the screen that isn't a button.
    /// The player can only achieve this when the settings bar is currently in an animation.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    ContractSettings();
            }
        #elif (UNITY_ANDROID || UNITY_IOS)
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStart = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    touchEnd = touch.position;

                    // contract the settings menu if the player did not move their finger on the screen and that the finger
                    // is not over a button
                    if (Mathf.Abs(touchEnd.x - touchStart.x) < 25 && EventSystem.current.currentSelectedGameObject == null)
                        ContractSettings();
                }
            }
        #endif
    }

    #endregion


    #region Settings

    /// <summary>
    /// Update the slider of the volume bars so that it is set to the current volume value.
    /// </summary>
    public void UpdateVolume()
    {
        uiSettings.UpdateVolume();
    }

    /// <summary>
    /// Sets the help menu to be active or not depending on the value passed on to active paramter.
    /// </summary>
    /// <param name="active"></param>
    public void SetHelpMenuActive(bool active)
    {
        uiSettings.SetHelpMenuActive(active);
    }

    /// <summary>
    /// Expands out the settings if it is currently contracted and vice versa.
    /// </summary>
    public void ToggleSettings()
    {
        if (!settingsExpanded)
            ExpandSettings();
        else
            ContractSettings();
    }

    /// <summary>
    /// Displays the settings menu.
    /// </summary>
    protected void ExpandSettings()
    {
        if (!uiSettings.gameObject.activeSelf)
            uiSettings.gameObject.SetActive(true);

        settingsExpanded = true;

        anim.ResetTrigger("Contract");
        anim.SetTrigger("Expand");
    }

    /// <summary>
    /// Hides the settings menu.
    /// </summary>
    protected void ContractSettings()
    {
        settingsExpanded = false;

        anim.ResetTrigger("Expand");
        anim.SetTrigger("Contract");
        uiSettings.ZeroSliderScale();
    }

    /// <summary>
    /// Sets readyForInput to true when the animation is completed.
    /// </summary>
    protected void ReadyForInput()
    {
        readyForInput = true;
    }

    /// <summary>
    /// Sets readyForInput to false when the animation is currently playing.
    /// </summary>
    protected void NotReadyForInput()
    {
        readyForInput = false;
    }

    #endregion
}
