/*****************************************************************************************************************
 - Map.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles the movement of the map. This will only move the map on the x-axis in the negative direction.
*****************************************************************************************************************/

using UnityEngine;

public class Map : MonoBehaviour, IUpdatable
{
    #region Fields

    float delta, 
          distance;
    Vector3 mapPos;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Add this object to the UpdateManager to have it update every frame.
    /// </summary>
    void Awake()
    {
        UpdateManager.Updatable.Add(this);
    }

    /// <summary>
    /// Remove this object from the UpdateManager.
    /// </summary>
    void OnDestroy()
    {
        UpdateManager.Updatable.Remove(this);
    }

    #endregion


    #region Update Manager

    /// <summary>
    /// Updates the position of the map so that it moves in the negative x-direction.
    /// </summary>
    public void UpdateMe()
    {
        delta = GameManager.MAP_SPEED * Time.deltaTime;
        distance += delta;
        GameManager.Instance.UpdateScore(distance);

        mapPos = transform.localPosition;
        mapPos.x -= delta;
        transform.localPosition = mapPos;
    }

    /// <summary>
    /// Update the map if this object is active and the game is not over.
    /// </summary>
    /// <returns></returns>
    public bool Active()
    {
        return gameObject.activeSelf && !GameManager.Instance.GameOver;
    }

    #endregion
}