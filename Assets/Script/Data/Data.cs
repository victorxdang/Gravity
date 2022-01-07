/*****************************************************************************************************************
 - Player.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Extra data for use anywhere in the game.
*****************************************************************************************************************/

public static class Data 
{
    /// <summary>
    /// Enable Google Play Games Service?
    /// </summary>
    public static bool GPGS_ENABLED = true;

    /// <summary>
    /// Display only test ads? (NOTE: must be set to true during development, 
    /// but remember to set to false when publishing app!)
    /// </summary>
    public static bool AD_TEST_MODE = false;

    /// <summary>
    /// The max amount of level available in the game.
    /// 
    /// NOTE: this is a variable and not a constant despite how it is all-caps like how I name all my constants.
    /// However, this is the only variable that will be named this way.
    /// </summary>
    public static int MAX_LEVELS;

    /// <summary>
    /// The URL to the privacy policy page.
    /// </summary>
    public const string PRIVACY_POLICY_URL = "https://tiltinggamesdevelopment.wordpress.com/privacy-policy/";

    /// <summary>
    /// The URL to Sappheiros's soundcloud.
    /// </summary>
    public const string SAPPHEIROS_URL = "https://soundcloud.com/sappheirosmusic";

    /// <summary>
    /// The URL to ZapSplat's home page.
    /// </summary>
    public const string ZAPSPLAT_URL = "https://www.zapsplat.com/";
}
