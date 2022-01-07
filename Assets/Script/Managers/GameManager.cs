/*****************************************************************************************************************
 - GameManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Controls the status of the UI's and the logic of the game during gameplay.
*****************************************************************************************************************/

using UnityEngine;

public class GameManager : BaseManager, IUpdatable
{
    #region Editable Region

    //---------------------------------------------------------------------------
    // Begin Editable Variables
    //---------------------------------------------------------------------------

    /// <summary>
    /// The countdown timer before resuming the game.
    /// </summary>
    public const float RESUME_DELAY = 3;

    /// <summary>
    /// The force of gravity.
    /// </summary>
    public const float GRAVITY_SCALE = -9.81f;

    /// <summary>
    /// How fast the map should move.
    /// </summary>
    public const float MAP_SPEED = 4;

    /// <summary>
    /// How fast the obstacles should fall.
    /// </summary>
    public const float OBSTACLE_SPEED = 3;

    //---------------------------------------------------------------------------
    // End Editable Variables
    //---------------------------------------------------------------------------

    #endregion


    #region Fields

    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Did the player just exit the tutorial UI? 
    /// Used to make sure that the player can't switch gravity
    /// as soon as they touch screen to exit the tutorial because
    /// a touch input will be registered and the ball will immediately 
    /// switch gravity when exiting the tutorial UI.
    /// </summary>
    public bool ExitedTutorial { get; set; }

    /// <summary>
    /// Has the game started?
    /// </summary>
    public bool GameStart { get; private set; }

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool GamePaused { get; private set; }

    /// <summary>
    /// Has the game ended?
    /// </summary>
    public bool GameOver { get; private set; }

    /// <summary>
    /// Is the player currently in the help menu?
    /// </summary>
    public bool InHelpMenu { get; set; }

    /// <summary>
    /// Is the resume timer currently counting down?
    /// </summary>
    public bool CountingDown { get; private set; }

    /// <summary>
    /// How far the player has travelled in this playthrough.
    /// </summary>
    public float DistanceTravelled { get; private set; }


    [Tooltip("Display the FPS counter on the top left of the screen?")]
    public bool EnableFPSCounter;

    [Tooltip("Can the ball be destroyed?")]
    public bool InvincibleBall;

    [Tooltip("Should the tutorial UI display even after playing for the first time?")]
    public bool DisplayTutorial;

    [SerializeField] GameObject uiPauseButtons;
    [SerializeField] TutorialUI uiTutorial;
    [SerializeField] LoadingUI uiLoading;
    [SerializeField] InGameUI uiInGame;
    [SerializeField] PauseUI uiPause;
    [SerializeField] LevelCompleteUI uiComplete;
    [SerializeField] LevelFailedUI uiFailed;


    int i = 0, 
        count = 0;
    float deltaTime = 0;
    WaitForSecondsRealtime waitCountdown = new WaitForSecondsRealtime(1);

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the object is created.
    /// 
    /// Initializes fields and required classes.
    ///
    /// BaseManager awake method will initialize SaveManager, GPGSManager and AdManager
    /// classes.
    /// </summary>
    protected override void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length == 1)
        {
            base.Awake();

            Instance = this;
            GameStart = false;
            GamePaused = false;
            GameOver = false;
            CountingDown = false;
            DistanceTravelled = 0;

            UISetup();
            UpdateManager.Updatable.Add(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the object is enabled.
    /// 
    /// Sets the correct volume on the slider and spawn the map based on the currently 
    /// selected level.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        uiPause.UpdateVolume();
        MapParser.Instance.CreateMap(false, SaveManager.PlayerPersistentData.selected_level);
    }

    /// <summary>
    /// Called when the application is no longer in focus.
    /// 
    /// Pauses the game and ensures that the game is saved at its current state.
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (pause && !GameOver)
        {
            PauseGame();
            SaveManager.Save();
        }
    }

    /// <summary>
    /// Called when the object is destroyed.
    /// 
    /// Removes this class from the UpdateManager to avoid any potential errors.
    /// </summary>
    void OnDestroy()
    {
        UpdateManager.Updatable.Remove(this);
    }

    #endregion


    #region Update Manager

    /// <summary>
    /// UpdateMe used in lieu of built-in Upate() method. (See Player class for more details
    /// on UpdateMe() and UpdateManager)
    /// 
    /// Calculates the FPS (if FPSCounter variable is checked) and displays it in a text box
    /// on the top-left corner of the phone.
    /// </summary>
    public void UpdateMe()
    {
        uiInGame.SetFPSCounterActive(EnableFPSCounter);

        if (EnableFPSCounter)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            uiInGame.UpdateFPS(Mathf.Round(1.0f / deltaTime));
        }
    }

    /// <summary>
    /// Used by UpdateManager to only update if the value returned by this method is true. Set
    /// conditions as necessary.
    /// </summary>
    /// <returns></returns>
    public virtual bool Active()
    {
        return gameObject.activeSelf;
    }

    #endregion


    #region Game Logic

    /// <summary>
    /// Starts the game. Shows the tutorial screen first, if specified. StartGame() will be called
    /// from ParseMap() in MapParser after the map has finished spawning.
    /// </summary>
    /// <param name="showTutorial"></param>
    public void StartGame(bool showTutorial = false)
    {
        if (showTutorial && SaveManager.PlayerPersistentData.selected_level == 1)
        {
            uiTutorial.gameObject.SetActive(true);
        }
        else
        {
            GameStart = true;
            SaveManager.PlayerSaveData.first_time = false;
            SaveManager.PlayerPersistentData.number_of_tries++;
        }

        uiLoading.gameObject.SetActive(false);
        AdManager.DestroyBanner();
    }

    /// <summary>
    /// Pauses the game, displays an ad and the pause menu. The time scale will be set to 0,
    /// so any objects that require updating during pause must use Time.unscaledTime, 
    /// Time.unscaledDeltaTime if in Update() or WaitForSecondsRealtime if in coroutine.
    /// </summary>
    public void PauseGame()
    {
        GamePaused = true;
        Time.timeScale = 0;
        AdManager.ShowBannerAd();
        SwitchUI(uiPause.gameObject);
    }

    /// <summary>
    /// Starts the countdown timer, the game will not yet resume until the countdown is complete.
    /// </summary>
    public void ResumeGame()
    {
        StartCoroutine(IEnumResumeGame(RESUME_DELAY));
    }

    /// <summary>
    /// Reloads the scene with the same level. The amount of games until an intersitital ad will
    /// be decremented but will not be displayed unless the player finishes a level either by
    /// completing it or hitting an obstacle.
    /// </summary>
    public void RestartGame(bool completedLevel)
    {
        if (completedLevel)
            SaveManager.PlayerPersistentData.number_of_tries = 0;

        SaveManager.PlayerPersistentData.games_until_ad--;
        uiLoading.gameObject.SetActive(true);
        uiLoading.LoadNextScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Returns to the main mneu.
    /// </summary>
    public void MainMenu()
    {
        SaveManager.PlayerPersistentData.number_of_tries = 0;
        uiLoading.gameObject.SetActive(true);
        uiLoading.LoadNextScene("StartScene");
    }

    /// <summary>
    /// Opens or closes the settings menu depending on its current state.
    /// </summary>
    public void SettingsMenu()
    {
        uiPause.ToggleSettings();
    }

    /// <summary>
    /// Set the game to be over but allow a few second delay before displaying the level complete or failed UI.
    /// </summary>
    /// <param name="completedLevel"></param>
    public void EndGame(bool completedLevel = false)
    {
        GameOver = true;

        // delay displaying the UI
        Wait((completedLevel ? 3 : 2), delegate
        {
            AdManager.ShowBannerAd();

            // display an interstital ad if the player has played enough games
            if ((--SaveManager.PlayerPersistentData.games_until_ad) <= 0)
            {
                AdManager.ShowInterstitialAd();
                SaveManager.PlayerPersistentData.ResetGamesUntilAd();
            }

            // display the correct UI
            SwitchUI((completedLevel ? uiComplete.gameObject : uiFailed.gameObject), uiInGame.gameObject, uiPause.gameObject);

            // only display the Next Level button if there is another level to be played
            uiComplete.SetNextLevelButtonActive((SaveManager.PlayerPersistentData.selected_level < Data.MAX_LEVELS));

            // update the highest_level score in PlayerSaveData if the current level that the player just completed is higher
            // then what is recorded.
            if (SaveManager.PlayerPersistentData.selected_level > SaveManager.PlayerSaveData.highest_level && completedLevel)
                SaveManager.PlayerSaveData.highest_level = SaveManager.PlayerPersistentData.selected_level;

            // check for achievements
            Achievements.CheckTooEZAchievement(completedLevel, SaveManager.PlayerPersistentData.number_of_tries);
            Achievements.CheckSoSoCloseAchievement(completedLevel, DistanceTravelled, MapParser.Instance.TotalDistance);
            Achievements.CheckNoStressNoMessAchievement(completedLevel, SaveManager.PlayerPersistentData.selected_level, Data.MAX_LEVELS);
            Achievements.CheckYayYouDidItAchievement(completedLevel, SaveManager.PlayerPersistentData.selected_level);
            Achievements.CheckThisGameIsHardAchievement(SaveManager.PlayerPersistentData.number_of_tries);

            SaveManager.Save();
            System.GC.Collect();
        });
    }

    /// <summary>
    /// Increment selected_level field in PlayerPersistentData and reload this scene.
    /// </summary>
    public void NextLevel()
    {
        SaveManager.PlayerPersistentData.selected_level++;
        RestartGame(true);
    }

    #endregion


    #region UI Logic

    /// <summary>
    /// Updates the distance of the ball in the in-game UI.
    /// </summary>
    /// <param name="value"></param>
    public void UpdateScore(float value)
    {
        DistanceTravelled = value;
        uiInGame.UpdateScore(value);
    }

    /// <summary>
    /// Opens or closes the pause menu depending on its current state.
    /// </summary>
    public void TogglePauseMenu()
    {
        uiPauseButtons.gameObject.SetActive(!uiPauseButtons.gameObject.activeSelf);
    }

    /// <summary>
    /// Sets the state of the pause menu directly.
    /// </summary>
    /// <param name="active"></param>
    public void SetPauseMenuActive(bool active)
    {
        uiPauseButtons.gameObject.SetActive(active);
    }

    #endregion


    #region Utilities

    /// <summary>
    /// Sets up all things relating UI.
    /// </summary>
    void UISetup()
    {
        uiLoading.gameObject.SetActive(true);
        uiInGame.gameObject.SetActive(true);
        uiPause.gameObject.SetActive(false);
        uiComplete.gameObject.SetActive(false);
        uiFailed.gameObject.SetActive(false);
    }

    /// <summary>
    /// Begins the countdown to resume game. During countdown, the escape key will not
    /// function until the countdown is complete.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    System.Collections.IEnumerator IEnumResumeGame(float time)
    {
        CountingDown = true;

        // hide just the buttons in the pause menu to only display the text
        uiPause.SetButtonsActive(false); 

        while (time > 0)
        {
            // update the pause text with the current countdown time
            uiPause.UpdatePauseText("Resuming In: \n" + time);
            time--;

            yield return waitCountdown; // waitCountdown is set to 1 second
        }

        Time.timeScale = 1;
        uiPause.SetButtonsActive(true); // display the buttons again so they can be used next time the pause menu appears
        uiPause.UpdatePauseText("Paused"); // set the proper string for the pause text
        SwitchUI(null, uiPause.gameObject); // turn off the pause UI
        AdManager.DestroyBanner();

        GamePaused = false;
        CountingDown = false;
        System.GC.Collect();
    }

    #endregion


    #region Overriden Methods

    /// <summary>
    /// Overridden method, sets all of the debugging variables to false.
    /// </summary>
    protected override void DebuggingVariablesSetup()
    {
        EnableFPSCounter = false;
        InvincibleBall = false;
        DisplayTutorial = false;
    }

    #endregion
}