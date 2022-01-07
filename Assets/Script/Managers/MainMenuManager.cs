/*****************************************************************************************************************
 - MainMenuManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Controls the status of the UI's and the scrolling of the level preview in the start menu.
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : BaseManager
{
    #region Editable Region

    //---------------------------------------------------------------------------
    // Begin Editable Variables
    //---------------------------------------------------------------------------

    /// <summary>
    /// The window of time where the player can click back again to quit 
    /// the game.
    /// </summary>
    public const float EXIT_TIME = 2;

    /// <summary>
    /// How fast the image previews will move.
    /// </summary>
    public const float SCROLL_SPEED = 50;

    //---------------------------------------------------------------------------
    // End Editable Variables
    //---------------------------------------------------------------------------

    #endregion


    #region Fields

    public static MainMenuManager Instance { get; private set; }

    /// <summary>
    /// Is the player currently in the credits menu?
    /// </summary>
    public bool InCreditMenu { get; set; }

    /// <summary>
    /// Is the player currently in the help menu?
    /// </summary>
    public bool InHelpMenu { get; set; }


    [Tooltip("Allow all levels to be immediately unlocked?")]
    public bool UnlockAllLevels;


    [SerializeField] Image imagePreview;
    [SerializeField] GameObject uiMainMenu, 
                                uiCredits;
    [SerializeField] StartUI uiStart;
    [SerializeField] LevelSelectUI uiLevelSelect;
    [SerializeField] LoadingUI uiLoading;

    [Tooltip("IMPORTANT: Place the images in level order!")]
    [SerializeField] Sprite[] previews;


    bool reset;   
    float xCoord;
    Vector3 imagePos;
    Animator anim;
    RectTransform rect;
    Touch touch;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Obtains the specified components and initializes the necessary static classes if
    /// they have not already been initialized in the start menu.
    /// 
    /// BaseManager awake method will initialize SaveManager, GPGSManager and AdManager
    /// classes.
    /// </summary>
    protected override void Awake()
    {
        if (FindObjectsOfType<MainMenuManager>().Length == 1)
        {
            base.Awake();

            Instance = this;
            anim = GetComponent<Animator>();
            rect = imagePreview.GetComponent<RectTransform>();
            xCoord = rect.anchoredPosition.x;

            // contrary to my naming standard, MAX_LEVEL is a variable and not a constant 
            // (this will be the only case of where this will occur)
            Data.MAX_LEVELS = previews.Length;

            AdManager.ShowBannerAd();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the game object is enabled. 
    /// 
    /// Sets the UI's to be active or not and loads a preview to be displayed on screen.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        uiMainMenu.gameObject.SetActive(true);
        uiCredits.gameObject.SetActive(false);
        uiLoading.gameObject.SetActive(false);
        LoadPreview(SaveManager.PlayerSaveData.highest_level + 1);

        uiStart.UpdateVolume();
    }

    /// <summary>
    /// Called every frame.
    /// 
    /// Scrolls through the preview image.
    /// </summary>
    void Update()
    {
        ScrollPreview();
    }

    #endregion



    #region Menus

    /// <summary>
    /// Sets the sign-in button on the main page of the screen (not in the settings UI) to either 
    /// on or off depending on value passed on to active parameter.
    /// </summary>
    /// <param name="active"></param>
    public void SetGPGSButtonActive(bool active)
    {
        uiStart.SetGPGSButtonActive(active);
    }

    /// <summary>
    /// Sets the credits menu to be active or not depending on the value passed on to active paramter.
    /// </summary>
    /// <param name="active"></param>
    public void SetCreditsMenuActive(bool active)
    {
        InCreditMenu = active;
        uiCredits.SetActive(active);
    }

    /// <summary>
    /// Toggles the credits UI to be on or off depending on its current state.
    /// </summary>
    public void ToggleCreditsMenu()
    {
        SetCreditsMenuActive(!uiCredits.activeSelf);
    }

    /// <summary>
    /// Sets the main menu to be active or not depending on the value passed on to active paramter.
    /// </summary>
    /// <param name="active"></param>
    public void SetMainMenuActive(bool active)
    {
        uiMainMenu.SetActive(active);
    }

    /// <summary>
    /// Toggles the main menu to be on or off depending on its current state.
    /// </summary>
    public void ToggleMainMenu()
    {
        uiMainMenu.SetActive(!uiMainMenu.gameObject.activeSelf);
    }

    #endregion


    #region Buttons

    /// <summary>
    /// Load up the currently selected level if the selected level is less than the highest level that the player
    /// has achieved.
    /// </summary>
    /// <param name="level"></param>
    public void PlayButtonClicked(int level)
    {
        if (level <= SaveManager.PlayerSaveData.highest_level + 1 || UnlockAllLevels)
        {
            SaveManager.PlayerPersistentData.selected_level = level;
            SaveManager.Save();

            uiLoading.gameObject.SetActive(true);
            uiLoading.LoadNextScene("GameScene");
        }
    }

    /// <summary>
    /// Display the credits menu and either hide the help menu (if active) or the main menu.
    /// </summary>
    public void CreditsButtonClicked()
    {
        if (InHelpMenu)
            uiStart.SetHelpMenuActive(false);
        else
            ToggleMainMenu();

        ToggleCreditsMenu();
    }

    /// <summary>
    /// Exit the credits menus and display the main menu.
    /// </summary>
    public void CreditsExitButtonClicked()
    {
        SetCreditsMenuActive(false);
        SetMainMenuActive(true);
    }

    #endregion


    #region Preview

    /// <summary>
    /// Loads the preview image onto the screen if the currently selected level is less than the highest level the player 
    /// has achieved. The player will not be able to preview levels that they have not yet completed or is not currently
    /// on.
    /// </summary>
    /// <param name="level"></param>
    public void LoadPreview(int level)
    {
        if ((level <= SaveManager.PlayerSaveData.highest_level + 1 || UnlockAllLevels) && level - 1 < previews.Length && previews[level - 1])
        {
            if (imagePreview.sprite != previews[level - 1])
            {
                imagePreview.sprite = previews[level - 1];
                reset = true;
            }
        }
    }

    /// <summary>
    /// Scroll the image so that the player may see a preview of the map.
    /// </summary>
    void ScrollPreview()
    {
        if (rect.anchoredPosition.x > -xCoord && !reset)
        {
            imagePos = rect.anchoredPosition;
            imagePos.x -= SCROLL_SPEED * Time.unscaledDeltaTime;
        }
        else
        {
            reset = false;
            imagePos.x = xCoord;
        }

        rect.anchoredPosition = imagePos;
    }

    #endregion


    #region Overriden Methods

    /// <summary>
    /// Overridden method, sets all of the debugging variables to false.
    /// </summary>
    protected override void DebuggingVariablesSetup()
    {
        UnlockAllLevels = false;
    }

    #endregion
}
