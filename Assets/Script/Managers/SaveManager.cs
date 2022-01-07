/*****************************************************************************************************************
 - SaveManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles all of the data that is to be saved to the local device. Also contains the variable holding the
     PersistentData class.
*****************************************************************************************************************/

public static class SaveManager
{
    #region Editable Region

    //---------------------------------------------------------------------------
    // Begin Editable Variables
    //---------------------------------------------------------------------------

    /// <summary>
    /// The name of the save file. This file is a json, so the ".json" 
    /// extension is required!
    /// </summary>
    const string LOCAL_SAVE_NAME = "Save_Gravity.json";

    //---------------------------------------------------------------------------
    // End Editable Variables
    //---------------------------------------------------------------------------

    #endregion


    #region Fields

    /// <summary>
    /// The absolute path where the data will be saved to.
    /// </summary>
    public static string SavePath { get { return UnityEngine.Application.persistentDataPath + "/" + LOCAL_SAVE_NAME; } }

    /// <summary>
    /// The save data that is located on the drive. This is the data file that will be manipulated throughout the game.
    /// </summary>
    public static SaveData PlayerSaveData { get; private set; }

    /// <summary>
    /// The save data that is stored on the user's Google Drive. This is only used to ensure that all data is up-to-date.
    /// </summary>
    public static SaveData CloudSaveData { get; set; }

    /// <summary>
    /// The data file that contains all of the variables needed to persist between level transitions.
    /// </summary>
    public static PersistentData PlayerPersistentData { get; private set; }

    static bool initialized = false;

    #endregion


    #region SaveManager Methods

    /// <summary>
    /// Initializes this class by loading the player data from the local drive and creating a new PersistentData variable.
    /// Recommended to be called within Awake() at the start of the game.
    /// </summary>
    /// <param name="forceInit"></param>
    public static void Initialize(bool forceInit = false)
    {
        if (!initialized || forceInit)
        {
            initialized = true;

            SaveData temp;
            PlayerSaveData = (Load(out temp, SavePath) ? temp : new SaveData());
            PlayerPersistentData = new PersistentData(PlayerSaveData.highest_level);
        }
    }

    /// <summary>
    /// Checks to ensure that the data from the cloud matches the data on the local drive. If not, then overwrite the lower
    /// value with the higher value.
    /// </summary>
    public static void CheckSaveData()
    {
        if (CloudSaveData.highest_level > PlayerSaveData.highest_level)
            PlayerSaveData.highest_level = CloudSaveData.highest_level;
    }

    #endregion


    #region Save/Load

    /// <summary>
    /// Sets the volume to the correct value and then saves the data to the local device.
    /// </summary>
    /// <returns></returns>
    public static bool Save()
    {
        PlayerSaveData.bgm_volume = AudioManager.Instance.BGMVolume;
        PlayerSaveData.se_volume = AudioManager.Instance.SEVolume;
        GPGSManager.CloudSave(PlayerSaveData.highest_level);

        return Save(PlayerSaveData, SavePath);
    }

    /// <summary>
    /// Saves the specifed file to the specified directory path. The file will be saved to the 
    /// device as a JSON file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="path"></param>
    /// <returns> True if the save was a success, false if an error occured. </returns>
    public static bool Save<T>(T data, string path)
    {
        try
        {
            System.IO.File.WriteAllText(path, UnityEngine.JsonUtility.ToJson(data));
            return true;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
            return false;
        }
    }

    /// <summary>
    /// Loads the data from the device if one exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="path"></param>
    /// <returns> True if the file was found and returned, otherwise false if the file does not exists or an error was encountered. </returns>
    public static bool Load<T>(out T data, string path)
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                data = UnityEngine.JsonUtility.FromJson<T>(System.IO.File.ReadAllText(path));
                return true;
            }

            data = default(T);
            return false;
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
            data = default(T);
            return false;
        }
    }

    #endregion
}
