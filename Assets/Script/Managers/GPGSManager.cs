/*****************************************************************************************************************
 - GPGSManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class handles all work that will be required to have a Google Play Games Service (GPGS) leaderboard, 
     achievements and save games to the cloud.

     NOTE: Leaderboards will not be used in this game.
*****************************************************************************************************************/

using System;
using System.Text;

// Google API's
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public static class GPGSManager
{
    #region Fields

    /// <summary>
    /// The name of the save file that will be on the cloud.
    /// </summary>
    public const string CLOUD_SAVE_FILENAME = "CloudSave_Gravity";

    /// <summary>
    /// Is the player currently signed in?
    /// </summary>
    public static bool IsSignedIn { get; private set; }

    static bool initialized = false;
    static ISavedGameMetadata CurrentGameMetaData;

    #endregion


    #region Initialize

    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    public static void Initialize(bool forceInit = false)
    {
        // setup buttons and configure Google Play Games Service if enabled, else
        // deactivate all buttons related to Google Play Games
        if (Data.GPGS_ENABLED)
        {
            if (!initialized || forceInit)
            {
                initialized = true;
                GPGSConfiguration(!SaveManager.PlayerSaveData.first_time);
            }
        }
        else
        {
            IsSignedIn = false;
        }
    }

    #endregion


    #region Sign-In/Setup For Google Play Games Service

    /// <summary>
    /// Activate Google Play Games Service to allow signing into Google Play to access achievements, leaderboards
    /// and to save the player's progress.
    /// </summary>
    static void GPGSConfiguration(bool silent)
    {
        // GPGS configurations
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        // attempt silent sign-in
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, silent);
    }

    /// <summary>
    /// Called whenever the users presses the sign-in button from either the start menu or the settings menu.
    /// </summary>
    public static void GPGSSignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            IsSignedIn = false;
            PlayGamesPlatform.Instance.SignOut();

            if (MainMenuManager.Instance)
                MainMenuManager.Instance.SetGPGSButtonActive(true);
        }
    }

    /// <summary>
    /// Callback function for when the sign-in button is clicked, or when the game automatically
    /// signs in the player.
    /// </summary>
    /// <param name="success"></param>
    static void SignInCallback(bool success)
    {
        IsSignedIn = success;
        CloudLoad(CLOUD_SAVE_FILENAME);

        // display the sign-in button on the start UI if sign-in failed
        if (MainMenuManager.Instance)
            MainMenuManager.Instance.SetGPGSButtonActive(!success);

        // unlock achievement if the player signs in for the first time
        if (success)
            Achievements.CheckBeginningOfTheTiltAchievement();
    }

    #endregion


    #region Achievement

    /// <summary>
    /// Unlocks an achievement with the given id. Do not use this method for incremental achievements.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnlockAchievement(string id)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && Data.GPGS_ENABLED)
        {
            bool unlocked = false;
            PlayGamesPlatform.Instance.ReportProgress(id, 100, success => { unlocked = success; });
            return unlocked;
        }

        return false;
    }

    /// <summary>
    /// Increments an achievement with the given id by a certain number of steps. Exclusive to GPGS.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="steps"></param>
    /// <returns></returns>
    public static bool IncrementAchivement(string id, int steps)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && Data.GPGS_ENABLED)
        {
            bool incremented = false;
            PlayGamesPlatform.Instance.IncrementAchievement(id, steps, success => { incremented = success; });
            return incremented;
        }

        return false;
    }

    /// <summary>
    /// Displays Google Play Platform achievement UI.
    /// </summary>
    public static void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    #endregion

    
    #region Leaderboard (not in use)

    /// <summary>
    /// Reports a score to the Google Play Platform leaderboard with the given id leaderboardID.
    /// </summary>
    /// <param name="leaderboardID"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    public static bool AddScoreToLeaderboard(string leaderboardID, long score)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && Data.GPGS_ENABLED)
        {
            bool added = false;
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, success => { added = success; });
            return added;
        }

        return false;
    }

    /// <summary>
    /// Displays Google Play Platform leaderboard UI.
    /// </summary>
    public static void ShowLeaderboard()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    #endregion
    

    #region Cloud Save/Load

    /// <summary>
    /// Saves the game to the Google Play cloud.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool CloudLoad(string filename)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // if there are conflicts with saving the game (i.e. two devices trying to save at the same time), then 
            // the save file with the longest playing time will be the one that is saved to the cloud.
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, HandleCloudLoad);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Saves the reported score to the cloud. This is a helper function for CloudSave() below which will actually
    /// save the data. This just takes the integer and converts it into an array of bytes.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static bool CloudSave(int level)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            return CloudSave(CurrentGameMetaData, Encoding.UTF8.GetBytes(level.ToString()));
        }

        return false;
    }

    /// <summary>
    /// Saves the data to the Google Play cloud. Keeps record of the time and date of when the data was saved.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="savedData"></param>
    /// <returns></returns>
    public static bool CloudSave(ISavedGameMetadata game, byte[] savedData)
    {
        try
        {
            // update the file to include the date and time that it was saved
            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
                .WithUpdatedDescription("Saved at: " + DateTime.Now);

            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            // commit the save to the cloud
            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, updatedMetadata, savedData, HandleCloudSave);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    /// <summary>
    /// Callback function for when the data has been loaded from the cloud.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="game"></param>
    static void HandleCloudLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            CurrentGameMetaData = game;
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, HandleCloudBinaryLoad);
        }
    }

    /// <summary>
    /// This function will take the data from the cloud and convert it into the correct data type suported
    /// by the game. Which in this case will be an integer becaue the only data saved onto the cloud is
    /// the player's highest score obtained.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="data"></param>
    static void HandleCloudBinaryLoad(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            try
            {
                SaveManager.CloudSaveData = new SaveData
                {
                    highest_level = Convert.ToInt32(Encoding.UTF8.GetString(data))
                };

                SaveManager.CheckSaveData();
            }
            catch (Exception e)
            {
                // empty for now, here for future implementations
            }
        }
    }

    /// <summary>
    /// Call back function for whenever the game has saved the data. Nothing happens here but it is here for
    /// future implementations.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="game"></param>
    static void HandleCloudSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        // empty for now, here for future implementations
    }

    #endregion
}
