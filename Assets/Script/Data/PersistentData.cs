/*****************************************************************************************************************
 - PersistentData.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains data that will persist through level transitions but will not be saved.
*****************************************************************************************************************/

public class PersistentData
{
    /// <summary>
    /// The amount of games to play before displaying an interstital ad.
    /// </summary>
    const int SHOW_ADS_AFTER_GAMES = 8;

    /// <summary>
    /// The player's selected level.
    /// </summary>
    public int selected_level = 0;

    /// <summary>
    /// The amount of games before displaying an interstitial ad.
    /// (Note: completing a level or restarting a level will affect this value, but
    ///  the intersitital ad won't actually be displayed until the end of a game,
    ///  either by being destroyed or completing the level.)
    /// </summary>
    public int games_until_ad = SHOW_ADS_AFTER_GAMES;

    /// <summary>
    /// The number of playthroughs it took to complete one levevl. Used for
    /// an achievement. Restarting the game or failing a level will increment the 
    /// number of tries.
    /// </summary>
    public int number_of_tries = 0;

    /// <summary>
    /// Initializes this class with the specified level. This specified level is 
    /// typically the highest level that player has completed that was recorded
    /// on file.
    /// </summary>
    /// <param name="current_level"></param>
    public PersistentData(int current_level)
    {
        selected_level = current_level;
    }

    /// <summary>
    /// Resets the amount of games left until display an interstitial ad.
    /// </summary>
    public void ResetGamesUntilAd()
    {
        games_until_ad = SHOW_ADS_AFTER_GAMES;
    }
}
