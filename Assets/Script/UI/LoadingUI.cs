/*****************************************************************************************************************
 - LoadingUI.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Loads the level with the specified name.
*****************************************************************************************************************/

using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Helper method to call IEnumLoadingScene coroutine.
    /// </summary>
    /// <param name="name"></param>
    public void LoadNextScene(string name)
    {
        StartCoroutine(IEnumLoadNextScene(name));
    }
    
    /// <summary>
    /// Loads the level with the specified name in the background. Once loading is complete, then the
    /// screen will switch over to the new level.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    System.Collections.IEnumerator IEnumLoadNextScene(string name)
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
    }
}
