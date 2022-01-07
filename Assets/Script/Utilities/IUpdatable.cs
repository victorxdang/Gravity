/*****************************************************************************************************************
 - IUpdatable.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     An interface that is to be implemented in any class that requires an update every frame. This interface
     is required because this project uses an Update Manager instead of implementing Update() in each class.
     By doing this, we can reduce the amount of Update() calls to just only one within the UpdateManager. This
     can potentially save a lot of performance since there will only be one Update() call.
*****************************************************************************************************************/

public interface IUpdatable
{
    /// <summary>
    /// Determines if this object should be updated by the UpdateManager or not. Implement the condition(s)
    /// required to have this object updated (or not updated).
    /// </summary>
    /// <returns></returns>
    bool Active();

    /// <summary>
    /// The update method, place all code that is required to update every frame here.
    /// </summary>
    void UpdateMe();
}
