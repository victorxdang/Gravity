/*****************************************************************************************************************
 - UpdateManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
Description: 
     This class is a global update manager that will update any game object that extends from IUpdatable.
     Using this implementation, there is only one update method (only in gameplay; in the start menu each UI
     still gets its' own Update method), meaning there will be less individual update methods to call and execute 
     and performance should be greatly improved.
*****************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    /// <summary>
    /// List of all objects that is required to update every frame.
    /// </summary>
    public static List<IUpdatable> Updatable = new List<IUpdatable>();

    // cached variables
    int i, count;


    /// <summary>
    /// Called every frame.
    /// 
    /// Executes UpdateMe() method in all elements in the list Updatable. Updates will only happen
    /// when the game has started.
    /// </summary>
    void Update()
    {
        if (GameManager.Instance.GameStart)
        {
            count = Updatable.Count;

            if (count > 0)
            {
                for (i = 0; i < count; i++)
                {
                    if (Updatable[i].Active())
                        Updatable[i].UpdateMe();
                }
            }
        }
    }
}