/*****************************************************************************************************************
 - StartUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains logic for the start menu.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class StartUI : BaseSettingsUIHandler
{
    #region Fields

    [SerializeField] Button buttonSettings,
                            buttonGPGSSignIn,
                            buttonCredits,
                            buttonPrivacyPolicy;

    float timeOfClick;

    AndroidJavaObject currentActivity;
    AndroidJavaClass toastClass,
                     unityPlayer;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Calls the method that will set up toasts notifcations.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        SetupToast();
        SetGPGSButtonActive(false);
    }

    #endregion


    #region Class Methods

    /// <summary>
    /// Sets the sign-in button on the main page of the screen (not in the settings UI) to either 
    /// on or off depending on value passed on to active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetGPGSButtonActive(bool active)
    {
        buttonGPGSSignIn.gameObject.SetActive(active);
    }

    #endregion


    #region Toasts

    /// <summary>
    /// Sets up the variables that will display a toast message on the bottom of the screen.
    /// </summary>
    void SetupToast()
    {
        toastClass = new AndroidJavaClass("android.widget.Toast");
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    /// <summary>
    /// Displays a toast notification on the bottom of the screen.
    /// Reference Material: https://medium.com/@agrawalsuneet/native-android-in-unity-8ebfb42edfe8
    /// </summary>
    /// <param name="message"></param>
    void ShowToast(string message)
    {
        #if UNITY_EDITOR
            Debug.Log(message);
        #elif UNITY_ANDROID
            toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity, message, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
        #endif
    }

    #endregion


    #region Overriden Methods

    /// <summary>
    /// Sets up the buttons to the correpsonding logic.
    /// </summary>
    protected override void ButtonSetup()
    {
        buttonCredits.onClick.AddListener(MainMenuManager.Instance.CreditsButtonClicked);
        buttonSettings.onClick.AddListener(ToggleSettings);
        buttonGPGSSignIn.onClick.AddListener(GPGSManager.GPGSSignIn);
        buttonPrivacyPolicy.onClick.AddListener(delegate { Application.OpenURL(Data.PRIVACY_POLICY_URL); });
    }

    /// <summary>
    /// For this method, when the player presses the back button on the phone or the escape key on Windows, then
    /// if the settings menu is currently expanded, then contract the settings menu. Otherwise, if the credits menu is 
    /// open or the help menu is open, then close which ever one is open. Finally, if the player is currently on the 
    /// start menu with no other menus open, then the player will have to click twice to fully exit the game. A toast
    /// message should appear on the bottom of the screen to signify another back button press is required to exit
    /// the game.
    /// </summary>
    protected override void Back()
    {
        if (settingsExpanded)
        {
            if (readyForInput)
                ContractSettings();
        }
        else if (MainMenuManager.Instance.InCreditMenu)
        {
            MainMenuManager.Instance.SetCreditsMenuActive(false);
            MainMenuManager.Instance.SetMainMenuActive(true);
        }
        else if (MainMenuManager.Instance.InHelpMenu)
        {
            SetHelpMenuActive(false);
            MainMenuManager.Instance.SetMainMenuActive(true);
        }
        else if (Time.time > timeOfClick)
        {
            timeOfClick = Time.time + MainMenuManager.EXIT_TIME;
            ShowToast("Press back again to exit");
        }
        else
        {
            AudioManager.Instance.StopBGM();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    #endregion
}