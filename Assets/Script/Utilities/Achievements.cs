/*****************************************************************************************************************
 - Achievements.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles all of the achievements in the game.
*****************************************************************************************************************/

public static class Achievements
{
    /// <summary>
    /// Name: The Beginning of the Tilt
    /// Unlock Condition: Unlocked when the player sigs into GPGS for the first time.
    /// Points: 5
    /// </summary>
    public static void CheckBeginningOfTheTiltAchievement()
    {
        GPGSManager.UnlockAchievement(GPGSIds.achievement_begin_the_tilt);
    }

    /// <summary>
    /// Name: Ooh, That's Gotta Hurt...
    /// Unlock Condition: Unlocked when the player hits a spike for the first time.
    /// Points: 10
    /// </summary>
    public static void CheckOohThatsGottaHurtAchievement()
    {
        GPGSManager.UnlockAchievement(GPGSIds.achievement_ooh_thats_gotta_hurt);
    }

    /// <summary>
    /// Name: Obstacle SMASH!
    /// Unlock Condition: Unlocked when the player hits an obstacle for the first time.
    /// Points: 10
    /// </summary>
    public static void CheckObstacleSmashAchievment()
    {
        GPGSManager.UnlockAchievement(GPGSIds.achievement_obstacle_smash);
    }

    /// <summary>
    /// Name: Banished to the Shadow Realm
    /// Unlock Condition: Unlocked when the player falls through a hole (void) in the map.
    /// Points: 15
    /// </summary>
    public static void CheckBanishedToTheShadowRealmAchievement()
    {
        GPGSManager.UnlockAchievement(GPGSIds.achievement_banished_to_the_shadow_realm);
    }

    /// <summary>
    /// Name: Yay, You Did It!
    /// Unlock Condition: Unlocked when the player completes the first level.
    /// Points: 25
    /// </summary>
    /// <param name="completedLevel"></param>
    /// <param name="currentLevel"></param>
    public static void CheckYayYouDidItAchievement(bool completedLevel, int currentLevel)
    {
        if (completedLevel && currentLevel == 1)
            GPGSManager.UnlockAchievement(GPGSIds.achievement_yay_you_did_it);
    }

    /// <summary>
    /// Name: This Game Is Actually Hard
    /// Unlock Condition: Unlocked when the player has failed and replayed a level 20 times. This cannot be unlocked
    ///                   when the player successfully completes a level but then restart the same level.
    /// Points: 35 
    /// </summary>
    /// <param name="numOfTries"></param>
    public static void CheckThisGameIsHardAchievement(int numOfTries)
    {
        if (numOfTries >= 20)
            GPGSManager.UnlockAchievement(GPGSIds.achievement_this_game_is_actually_hard);
    }

    /// <summary>
    /// Name: So, So Close...
    /// Unlock Condition: Unlocked when the player nearly completes a level (about 10m left on the map).
    /// Points: 50
    /// </summary>
    /// <param name="completedLevel"></param>
    /// <param name="distanceTravelled"></param>
    /// <param name="maxDistance"></param>
    public static void CheckSoSoCloseAchievement(bool completedLevel, float distanceTravelled, float maxDistance)
    {
        if (!completedLevel && maxDistance - distanceTravelled <= 10)
            GPGSManager.UnlockAchievement(GPGSIds.achievement_so_so_close);
    }

    /// <summary>
    /// Name: Too EZ
    /// Unlock Condition: Unlocked when the player completes a level playthrough without having to restart the level.
    /// Points: 50
    /// 
    /// NOTE: There is a way around this by returning to the main menu and then selecting the level again. If the player
    /// keeps pressing the restart button in-game, then that's only when the tries will actually be counted.
    /// </summary>
    /// <param name="completedLevel"></param>
    /// <param name="numOfTries"></param>
    public static void CheckTooEZAchievement(bool completedLevel, int numOfTries)
    {
        if (completedLevel && numOfTries == 1)
            GPGSManager.UnlockAchievement(GPGSIds.achievement_too_ez);
    }

    /// <summary>
    /// Name: No Stress, No Mess
    /// Unlock Condition: Unlocked when the player has completed all of the levels in the game.
    /// Points: 100
    /// </summary>
    /// <param name="completedLevel"></param>
    /// <param name="currentLevel"></param>
    /// <param name="maxLevels"></param>
    public static void CheckNoStressNoMessAchievement(bool completedLevel, int currentLevel, int maxLevels)
    {
        if (completedLevel && currentLevel == maxLevels)
            GPGSManager.UnlockAchievement(GPGSIds.achievement_no_stress_no_mess);
    }

    /// <summary>
    /// Name: Instructions Unclear, Got Stuck in a Cube
    /// Unlock Condition: Unlocked when the player gets inside a cube (it can happen).
    /// Points: 100
    /// </summary>
    /// <param name="yCoord"></param>
    public static void CheckInstructionsUnclearAchievement()
    {
        GPGSManager.UnlockAchievement(GPGSIds.achievement_instructions_unclear_got_stuck_in_a_cube);
    }
}