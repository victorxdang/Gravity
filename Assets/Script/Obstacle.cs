/*****************************************************************************************************************
 - Obstacle.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class will handle obstacle movements during gameplay. When in game, an obstacle will only move
     downwards, unaffected by gravity. Once the obstacle reaches the ground, there will be a slight delay
     before setting the position of the obstacle back to its original spawn point and restarting the downward
     movement.
*****************************************************************************************************************/

using UnityEngine;

public class Obstacle : MonoBehaviour, IUpdatable
{
    #region Constants

    /// <summary>
    /// The distance on y-axis that this obstacle will travel from its initial spawn point.
    /// </summary>
    const float TRAVEL_DISTANCE = 3.5f;
    
    /// <summary>
    /// The default value for speed.
    /// </summary>
    public const float DEFAULT_SPEED = 4;

    /// <summary>
    /// The delay in between each cycle of the obstacle dropping.
    /// </summary>
    const float WAIT_TIME = 1;

    #endregion


    #region Fields

    public float Speed { get; set; }

    bool correctEndPos;
    float waitTime,
          startY,
          endY;

    Renderer render;
    Vector3 obstaclePos = Vector3.zero, startPos = Vector3.zero;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Grabs the renderer of this object and add this class to the UpdateManager.
    /// </summary>
    void Awake()
    {
        // renderer is grabbed in order to not have this object update when it is out of camera view
        // to reduce amount of objects being updated during a frame. 
        render = GetComponent<Renderer>();

        UpdateManager.Updatable.Add(this);
    }

    /// <summary>
    /// Removes this class from UpdateManager in order to not have any errors.
    /// </summary>
    void OnDestroy()
    {
        UpdateManager.Updatable.Remove(this);
    }

    #endregion


    #region Update Manager

    /// <summary>
    /// Replacement to Unity's update method, updates all updatable objects within one Update() call in UpdateManager.
    /// 
    /// Updates the position of the obstacle in each frame. If the obstacle reaches its end, then respawn
    /// the obstacle at its spawn point and wait a short duration before having the obstacle fall again.
    /// </summary>
    public void UpdateMe()
    {
        if (Time.time > waitTime)
        {
            // if the obstacle has reached its end, then respawn the obstacle at its spawn point
            if ((Speed < 0 && (transform.localPosition.y < endY || transform.localPosition.y > startY)) ||
                (Speed > 0 && (transform.localPosition.y > endY || transform.localPosition.y < startY)))
            {
                transform.localPosition = startPos;
                waitTime = Time.time + WAIT_TIME; // set delay
            }
            else
            {
                UpdateMovement();
            }
        }
    }

    /// <summary>
    /// Used by Udpate Manager to check if this object should update or not.
    /// 
    /// Update this obstacle only if the game object is active and it is visible by a camera.
    /// </summary>
    /// <returns></returns>
    public bool Active()
    {
        return gameObject.activeSelf && render.isVisible && correctEndPos;
    }

    #endregion


    #region Class Methods

    /// <summary>
    /// Update the obstacles position on the y-axis.
    /// </summary>
    void UpdateMovement()
    {
        obstaclePos = transform.localPosition;
        obstaclePos.y += Speed * Time.deltaTime;
        transform.localPosition = obstaclePos;
    }

    /// <summary>
    /// Setups the obstacle block by setting the speed of the block and 
    /// gets the initial position of the obstacle object and then calculates 
    /// how far the obstacle should travel before respawning at the original
    /// spawn point.
    /// </summary>
    public void SetupBlock(float speed, Vector3 start)
    {
        Speed = speed;

        startPos = start;
        startY = start.y;
        endY = start.y + (Speed < 0 ? -TRAVEL_DISTANCE : TRAVEL_DISTANCE);

        correctEndPos = true;
    }

    #endregion
}
