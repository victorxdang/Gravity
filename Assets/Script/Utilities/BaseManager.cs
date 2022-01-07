/*****************************************************************************************************************
 - BaseManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class holds all of the shared methods between the UI manager classes (MainMenuManager and 
     GameManager).
*****************************************************************************************************************/

using UnityEngine;

public class BaseManager : MonoBehaviour
{
    WaitForSecondsRealtime waitRealtime;

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Calls DebuggingVariablesSetup() to ensure that debugging varaiables are 
    /// the correct value when played on the phone. Will also initialize SaveManager,
    /// GPGSManager, AdManager classes.
    /// </summary>
    protected virtual void Awake()
    {
        #if !UNITY_EDITOR
            DebuggingVariablesSetup();
        #endif

        SaveManager.Initialize();
        GPGSManager.Initialize();
        AdManager.Initialize();

        Time.timeScale = 1;
    }

    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    protected virtual void Start()
    {
        AudioManager.Instance.BGMVolume = SaveManager.PlayerSaveData.bgm_volume;
        AudioManager.Instance.SEVolume = SaveManager.PlayerSaveData.se_volume;
    }

    /// <summary>
    /// Stop background music, save the data and destroy all ad instances when the game is quitting.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        AudioManager.Instance.StopBGM();
        SaveManager.Save();
        AdManager.CleanUp();
    }

    /// <summary>
    /// Sets all of the debugging variables to false.
    /// </summary>
    protected virtual void DebuggingVariablesSetup()
    {
        // to be overridden by inherited class.
    }

    /// <summary>
    /// Deactivates all UI passed onto active and activates the target UI. 
    /// The target and active parameters can both be null, but that wouldn't make sense, now would it? 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="active"></param>
    protected void SwitchUI(GameObject target, params GameObject[] active)
    {
        if (active.Length > 0)
        {
            for (int i = 0; i < active.Length; i++)
            {
                if (active[i])
                    active[i].SetActive(false);
            }
        }

        if (target)
            target.SetActive(true);
    }

    /// <summary>
    /// Helper method the coroutine IEnumWait(). Waits for a 
    /// time specified by duration and then executes code passed
    /// onto the action parameter.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="action"></param>
    public void Wait(float duration, System.Action action)
    {
        StartCoroutine(IEnumWait(duration, action));
    }

    /// <summary>
    /// Waits for an amount of time specified by duration and then execute logic passed on
    /// to action parameter.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    System.Collections.IEnumerator IEnumWait(float duration, System.Action action)
    {
        waitRealtime = new WaitForSecondsRealtime(duration);
        yield return waitRealtime;
        action();
    }
}
