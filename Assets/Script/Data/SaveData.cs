/*****************************************************************************************************************
 - SaveData.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains the data that are going to be saved to the local device (and the cloud if internet connection
     is available).
*****************************************************************************************************************/

public class SaveData
{
    /// <summary>
    /// Is the player playing for the first time?
    /// </summary>
    public bool first_time = true;

    /// <summary>
    /// The highest level that the player has completed.
    /// </summary>
    public int highest_level = 0;

    /// <summary>
    /// The volume for background music.
    /// </summary>
    public float bgm_volume = 0.5f;

    /// <summary>
    /// The volume for sound effects.
    /// </summary>
    public float se_volume = 0.5f;
}
