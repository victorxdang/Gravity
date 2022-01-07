/*****************************************************************************************************************
 - Player.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     The main player of the game. The game object's only movement is upwards or downwards, depending on the 
     current gravity scale. The gravity is directly influenced by the player touching the screen to change the
     gravity scale. This game will only have one input, a touch (if on mobile) or a mouse click (if on editor).
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IUpdatable
{
    //---------------------------------------------------------------------------
    // Begin Editable Variables
    //---------------------------------------------------------------------------

    /// <summary>
    /// The positive and negative y position where the ball will switch to
    /// depending on the ball's current position.
    /// </summary>
    const float SWITCH_POS_Y = 10,
                SWITCH_NEG_Y = -3;

    //---------------------------------------------------------------------------
    // End Editable Variables
    //---------------------------------------------------------------------------


    #region Fields

    [SerializeField] GameObject prefabFracturedBall;

    bool canMove, // is the ball able to switch gravity?
         onGround,  // is the ball currently on the ground? 
         switchingPos;
    float distanceStartedInCube;
    Rigidbody rb;
    Vector3 playerPos, 
            cameraPos;
    Touch touch;
    ParticleSystem particleTrail;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Initializes the field to their appropriate vales. Ensures that there is only one 
    /// Player object on the map.
    /// </summary>
    void Awake()
    {
        if (FindObjectsOfType<Player>().Length == 1)
        {
            onGround = false;
            canMove = false;
            rb = GetComponent<Rigidbody>();
            particleTrail = transform.GetChild(0).GetComponent<ParticleSystem>();

            // add this class to the UpdateManager so that it can be updated every frame
            UpdateManager.Updatable.Add(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// FixedUpdate() is used to update pyhsics events (i.e. rigidbody movements).
    /// 
    /// Adds gravity force since the map is a single, combined mesh using a mesh collider, gravity 
    /// on the rigidbody doesn't function correctly so by using AddForce(), we can just simulate it
    /// like how the rigidbody would does it.
    /// </summary>
    void FixedUpdate()
    {
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);
    }

    /// <summary>
    /// When the ball hits a spike, set the game to be over.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spike") && !GameManager.Instance.InvincibleBall)
        {
            Achievements.CheckOohThatsGottaHurtAchievement();
            GameOver(false, true);
        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            canMove = true;
            onGround = true;
        }
    }

    /// <summary>
    /// Sets the onGround boolean to be false after the ball stops touching a
    /// platform. This is different than the variable canMove.
    /// </summary>
    void OnCollisionExit()
    {
        onGround = false;
        distanceStartedInCube = 0;
    }

    /// <summary>
    /// When the ball hits a spike or an obstacle, then the game will end. Or if the player
    /// reaches the flag, then the player has completed the level.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Obstacle") && !GameManager.Instance.InvincibleBall)
        {
            Achievements.CheckObstacleSmashAchievment();
            GameOver(false, true);
        }
        else if (collider.CompareTag("Flag"))
        {
            Physics.gravity = new Vector3(0, -Mathf.Abs(Physics.gravity.y), 0);
            GameOver(true);
        }
    }

    /// <summary>
    /// Removes this class from the UpdateManager so that there won't be any
    /// errors or exceptions.
    /// </summary>
    void OnDestroy()
    {
        UpdateManager.Updatable.Remove(this);
    }

    #endregion

    #region Update Manager

    /// <summary>
    /// An UpdateManager is used, so every code that needs to be executed every frame must be
    /// added into this function. Ensure that this class implements IUpdatable for this method
    /// to work properly.
    /// 
    /// Check to make sure that the player is on the ground (touching a square block), if they are
    /// then allow the player to switch the gravity. The ball will react accordingly to the current
    /// gravity scale. This will also make sure to end the game if the player go outside of the map.
    /// Gravity is updated by using rigidbody AddForce() and player's movement is updated by
    /// manipulated the transoform.localPosition in UpdatePlayerMovement().
    /// </summary>
    public void UpdateMe()
    {
        if (!GameManager.Instance.GameOver)
        {
            // start the particle system if it has not already started 
            if (!particleTrail.isPlaying)
            {
                particleTrail.Play();
            }

            if (canMove)
            {
                // if the player is currently on a square block ("platform"), then get the input of the 
                // player from either touch (if on Android) or keyboard/mouse (if on editor)
                GetPlayerInput();
            }

            if (onGround)
            {
                // check if the player went through or is stuck in a cube.
                CheckIfInCube(transform.localPosition.y);
            }

            // check to see if the player is still within the bounds of the map
            CheckIfOutOfBounds();
        }
        else
        {
            if (transform.localPosition.x > 15)
            {
                DetachParticles();

                // set the game object to be false after the game has eneded and after the ball
                // has travelled about 15 block or so until the ball is out of camera view
                gameObject.SetActive(false);
            }
            else
            {
                // move player along x-axis
                UpdatePlayerPosition();
            }
        }
    }
    
    /// <summary>
    /// Used by UpdateManager to only update if the value returned by this method is true. Set
    /// conditions as necessary.
    /// </summary>
    /// <returns></returns>
    public bool Active()
    {
        return gameObject.activeSelf && !GameManager.Instance.GamePaused && GameManager.Instance.GameStart;
    }

    #endregion


    #region Player Methods

    /// <summary>
    /// Gets the player input depending on the current platform:
    /// - If the playing on the Editor, then the input will come from a mouse and keyboard. 
    /// - If the player is playing on a mobile device, then get the input from the player's touch.
    /// 
    /// Once the input is retrieved, if the player is not currently toucing a button on the screen, 
    /// then switch the gravity of the player.
    /// </summary>
    void GetPlayerInput()
    {
        #if UNITY_EDITOR
            if (Input.GetAxis("Submit") == 1 || Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    SwitchGravity();
            }
        #elif (UNITY_ANDROID || UNITY_IOS)
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    if (!GameManager.Instance.ExitedTutorial)
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                            SwitchGravity();
                    }
                    else
                    {
                        GameManager.Instance.ExitedTutorial = false;
                    }
                }
            }
        #endif
    }

    /// <summary>
    /// Moves the player on the x-axis by the value determined by MAP_SPEED per second.
    /// 
    /// NOTE: the player will only have its position updated after the level is completed.
    /// </summary>
    void UpdatePlayerPosition()
    {
        playerPos = transform.localPosition;
        playerPos.x += GameManager.MAP_SPEED * Time.deltaTime;
        transform.localPosition = playerPos;
    }

    /// <summary>
    /// If the player travels above or below the map, then switch the ball to be in the top or bottom of
    /// the map, dependeing on the last position of the ball, otherwise, set the game to be over.
    /// </summary>
    void CheckIfOutOfBounds()
    {
        if (transform.localPosition.y >= 20 || transform.localPosition.y <= -10)
        {
            Achievements.CheckBanishedToTheShadowRealmAchievement();
            GameOver(false);
        }
        else if (MapParser.Instance.IsVoidAtIndex((int) GameManager.Instance.DistanceTravelled))
        {
            if ((transform.localPosition.y > SWITCH_POS_Y || transform.localPosition.y < SWITCH_NEG_Y))
            {
                playerPos = transform.localPosition;
                playerPos.y = ((playerPos.y > SWITCH_POS_Y) ? SWITCH_NEG_Y : SWITCH_POS_Y) - 0.1f;
                transform.localPosition = playerPos;
            }
        }
        else
        {
            if (transform.localPosition.y >= 7.5f || transform.localPosition.y <= -0.5f)
            {
                Achievements.CheckBanishedToTheShadowRealmAchievement();
                GameOver(false);
            }
        }
    }

    /// <summary>
    /// Checks whether the ball is within a cube or not and unlocks the "Instructions Unclear..."
    /// achievement appropriately.
    /// </summary>
    /// <param name="yCoord"></param>
    void CheckIfInCube(float yCoord)
    {
        // checks if the ball is currently overlapping a cube
        if ((yCoord >= 6.8f && yCoord <= 7.2f) || (yCoord >= 3.3f && yCoord <= 3.7f) || (yCoord >= -0.2f && yCoord <= 0.2f))
        {
            if (distanceStartedInCube == 0)
                distanceStartedInCube = GameManager.Instance.DistanceTravelled;
        }
        else
        {
            distanceStartedInCube = 0;
        }

        // if the ball is in the cube for more than 2 units, then unlock the achievement,
        // this will prevent false readings, like when the ball touches the out left edge
        // of the platform and lines up with the middle of the cube.
        if (distanceStartedInCube > 0)
        {
            if (GameManager.Instance.DistanceTravelled - distanceStartedInCube >= 2)
                Achievements.CheckInstructionsUnclearAchievement();
        }
    }

    /// <summary>
    /// Negates the gravity to have the ball switch directions on the y-axis.
    /// </summary>
    void SwitchGravity()
    {
        canMove = false;
        Physics.gravity = -Physics.gravity;
    }

    /// <summary>
    /// Detaches the particle system so that the particle system will be stopped
    /// correctly (meaning not abprutly disappear when the game ends).
    /// </summary>
    void DetachParticles()
    {
        particleTrail.Stop();
        transform.DetachChildren();
    }

    /// <summary>
    /// Set the game to be over based on whether or not the player completed the level
    /// and whether or not the player hit an obstacle.
    /// 
    /// If the player hits an obstacle, then a fractured ball will be spawned and its
    /// fragments will fly outwards, a level failed UI will appear and the game object
    /// will be set to inactive. If the player completes the level, then the level 
    /// compelete UI will appear.
    /// </summary>
    /// <param name="completedLevel"></param>
    /// <param name="hitObstacle"></param>
    public void GameOver(bool completedLevel, bool hitObstacle = false)
    {
        GameManager.Instance.EndGame(completedLevel);

        if (hitObstacle)
        {
            DetachParticles();

            AudioManager.Instance.PlayBallDestroyedSE();
            Instantiate(prefabFracturedBall, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }     
    }

    #endregion
}