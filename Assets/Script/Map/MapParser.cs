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
     gravity scale. This game will only have one input, a touch (if on mobile) or a multiple keys (if on editor).
*****************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class MapParser : MonoBehaviour
{
    #region Editable Region

    //---------------------------------------------------------------------------
    // Begin Editable Variables
    //---------------------------------------------------------------------------

    // initial x spawn coordinate of all blocks.
    const float X_SPAWN = -3;

    /// <summary>
    /// Y spawn coordinates for cube 
    /// </summary>
    const float Y_SPAWN_C0 = 7,
                Y_SPAWN_C3 = 3.5f,
                Y_SPAWN_C6 = 0;

    /// <summary>
    /// Y spawn coordinates for spikes 
    /// </summary>
    const float Y_SPAWN_T0 = 7.5f,
                Y_SPAWN_T1 = 6.5f,
                Y_SPAWN_T2 = 4,
                Y_SPAWN_T4 = 3,
                Y_SPAWN_T5 = 0.5f,
                Y_SPAWN_T6 = -0.5f;

    /// <summary>
    /// Y spawn coordinates for player
    /// </summary>
    const float Y_SPAWN_P1 = 6,
                Y_SPAWN_P2 = 4.5f,
                Y_SPAWN_P4 = 2.5f,
                Y_SPAWN_P5 = 1;

    /// <summary>
    /// Y spawn coordinates for flag
    /// </summary>
    const float Y_SPAWN_F2 = 4.05f,
                Y_SPAWN_F5 = 0.57f;
                

    /// <summary>
    /// The name of the file that contains the data for spawning the map.
    /// </summary>
    const string MAP_FILENAME = "/MapData/level_{0}.mlvl";

    /// <summary>
    /// The name of the respective object.
    /// </summary>
    const string NAME_PLAYER = "Player",
                 NAME_OBSTACLE = "Obstacle",
                 NAME_FLAG = "Flag";

    /// <summary>
    /// Tags for the respective block.
    /// </summary>
    const string TAG_PLATFORM = "Platform",
                 TAG_SPIKE = "Spike";

    /// <summary>
    /// The names of the parent block that will hold the respective shapes.
    /// Extras will primarily hold obstacles and the finish flag.
    /// </summary>
    const string MAP_BLOCK = "Block Map",
                 MAP_SPIKE = "Spike Map",
                 MAP_HOLDER_EXTRAS = "Extra Objects";

    /// <summary>
    /// The name of the parent blocks that will be used in order to hold the 
    /// individual blocks/spikes before they get merged into a single mesh.
    /// These parent blocks won't appear in the heirarchy since they will
    /// be deleted. Only the blocks with the two names above will appear.
    /// </summary>
    const string MAP_HOLDER_NAME = "Map",
                 MAP_HOLDER_TOP = "Top",
                 MAP_HOLDER_MID = "Middle",
                 MAP_HOLDER_BOT = "Bottom";

    //---------------------------------------------------------------------------
    // End Editable Variables
    //---------------------------------------------------------------------------

    #endregion


    #region Fields

    public static MapParser Instance { get; private set; }

    /// <summary>
    /// The distance from the player's starting point to the finish flag.
    /// </summary>
    public float TotalDistance { get; private set; }


    [Tooltip("Should the map be re-created when the game starts?")]
    public bool RespawnMapOnPlay;

    [Tooltip("Determine if gray blocks should be used where obstacles will come out from")]
    public bool UseGrayBlocks;

    [Tooltip("Uses the level that was inputted by \"Level\" slider below")]
    public bool DebugMode;


    [Tooltip("The material of the cubes and spikes, used after combining the map's mesh")]
    [SerializeField] Material objectMaterial;

    [SerializeField] Transform prefabPlayer,
                               prefabCube,
                               prefabTriangle,
                               prefabFlag,
                               prefabObstacle,
                               prefabObstacleCube;

    [Tooltip("The level (map) to spawn")]
    [Range(1, 30)] public int level;


    // references the spawned player and parent blocks
    Transform player, 
              goBlockParent,
              goTopBlockParent,
              goMidBlockParent,
              goBotBlockParent,
              goExtraParent;

    List<int> listTopVoidIndex = new List<int>();
    List<int> listBotVoidIndex = new List<int>();

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the game object is created.
    /// 
    /// Sets the Instance variable to this class.
    /// </summary>
    void Awake()
    {
        Instance = this;
        DebuggingVariablesSetup();
    }

    #endregion


    #region Parse Map

    /// <summary>
    /// Starts the process of spawning the map based on the level supplied.
    /// </summary>
    /// <param name="lvl"></param>
    public void CreateMap(bool fromEditor, int lvl = 0)
    {
        if (fromEditor || RespawnMapOnPlay)
        {
            if (!DebugMode && lvl > 0)
                level = lvl;

            ClearMap();
            GetBlockList(level);
        }
        else
        {
            GameManager.Instance.StartGame(SaveManager.PlayerSaveData.first_time || GameManager.Instance.DisplayTutorial);
        }
    }

    /// <summary>
    /// Destroys all parent blocks and recreates new parent blocks. 
    /// All parent blocks are destroyed since they can potentially hold hundreds of object, so the
    /// quickest way to reset the parent blocks is simply to destroy and re-create them.
    /// </summary>
    /// <param name="reinitialize"></param>
    void ClearMap(bool reinitialize = true)
    {
        // destroy the parent blocks
        DestroyImmediate((goBlockParent != null ? goBlockParent.gameObject : GameObject.Find(MAP_HOLDER_NAME)));
        DestroyImmediate((player != null ? player.gameObject : GameObject.Find(NAME_PLAYER)));
        DestroyImmediate((goExtraParent != null ? goExtraParent.gameObject : GameObject.Find(MAP_HOLDER_EXTRAS)));

        // recreate all parent blocks, if specified.
        if (reinitialize)
        {
            goBlockParent = CreateParentBlock(MAP_HOLDER_NAME, transform);
            goTopBlockParent = CreateParentBlock(MAP_HOLDER_TOP, goBlockParent);
            goMidBlockParent = CreateParentBlock(MAP_HOLDER_MID, goBlockParent);
            goBotBlockParent = CreateParentBlock(MAP_HOLDER_BOT, goBlockParent);
            goExtraParent = CreateParentBlock(MAP_HOLDER_EXTRAS, transform);
        }
    }

    /// <summary>
    /// Spawn the map based on the data obtained from the map file. After spawning of the objects are complete,
    /// then it will then proceed to call a method that will combine the meshes in order to reduce the amount
    /// of batches and draw calls at runtime.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="blockList"></param>
    void ParseMap(int level, string[] blockList)
    {
        // references to the objecst being spawned
        Transform parent = null,
                  spawnedObject = null;

        // spawn points of each type of object
        Vector3 spawnPoint = Vector3.zero;

        // rotations of the spawned objects
        Quaternion rotationIdentity = Quaternion.identity,
                   rotationTriangle = Quaternion.Euler(0, 0, 45),
                   rotationFlag = Quaternion.Euler(0, 180, 0);

        // used when combining the meshes of the objects that were spawned
        List<Transform> blocks = new List<Transform>();
        List<Transform> spikes = new List<Transform>();

        // iterate through each line that were read in from the file,
        // set appropriate spawn points for the objects based on the index the object is in
        for (int i = 0; i < blockList.Length; i++)
        {
            // initialize x coordinate of vector to correct position after each iteration
            spawnPoint.x = X_SPAWN;

            // for each line, iterate through each letter in the line
            for (int j = 0; j < blockList[i].Length; j++)
            {
                // get the y spawn coordinate and the parent block for the block that is to be 
                // spawned based on the current index of the map and block type
                spawnPoint.y = GetYSpawn(i, blockList[i][j]);
                parent = GetParent(i);

                if (blockList[i][j] == 'C') // cube
                {
                    spawnedObject = Instantiate(prefabCube, spawnPoint, rotationIdentity);
                    spawnedObject.parent = parent;

                    blocks.Add(spawnedObject);
                }
                else if (blockList[i][j] == 'G') // gray blocks (used to indicate where the obstacles are coming from)
                {
                    spawnedObject = Instantiate(prefabObstacleCube, spawnPoint, rotationIdentity);
                    spawnedObject.parent = (UseGrayBlocks ? goExtraParent : parent);

                    if (!UseGrayBlocks)
                        blocks.Add(spawnedObject);
                }
                else if (blockList[i][j] == 'T') // spike
                {
                    spawnedObject = Instantiate(prefabTriangle, spawnPoint, rotationTriangle);
                    spawnedObject.parent = parent;

                    spikes.Add(spawnedObject);
                }
                else if (blockList[i][j] == 'O') // obstacle
                {
                    spawnedObject = Instantiate(prefabObstacle, spawnPoint, rotationIdentity);
                    spawnedObject.transform.parent = goExtraParent;
                    spawnedObject.name = string.Join(" ", NAME_OBSTACLE, (spawnPoint.x - X_SPAWN));
                    spawnedObject.GetComponent<Obstacle>().SetupBlock((i == 1 || i == 4) ? -Obstacle.DEFAULT_SPEED : Obstacle.DEFAULT_SPEED, spawnPoint);
                }
                else if (blockList[i][j] == 'P') // player
                {
                    player = Instantiate(prefabPlayer, spawnPoint, rotationIdentity);
                    player.name = NAME_PLAYER;

                    Physics.gravity = new Vector3(0, Mathf.Abs(Physics.gravity.y) * ((i == 1 || i == 4) ? 1 : -1), 0);
                }
                else if (blockList[i][j] == 'F') // finish flag
                {
                    spawnedObject = Instantiate(prefabFlag, spawnPoint, rotationFlag);
                    spawnedObject.parent = goExtraParent;
                    spawnedObject.name = NAME_FLAG;

                    TotalDistance = spawnPoint.x - 3;
                }
                else if (blockList[i][j] == ' ') // void
                {
                    if (i == 0)
                        listTopVoidIndex.Add((int) spawnPoint.x);
                    else if (i == 6)
                        listBotVoidIndex.Add((int) spawnPoint.x);
                }

                spawnPoint.x++;
            }
        }

        // combine the meshes together
        CombineMeshes(blocks.ToArray(), spikes.ToArray());
        System.GC.Collect();

        if (GameManager.Instance)
            GameManager.Instance.Wait(1, delegate { GameManager.Instance.StartGame(SaveManager.PlayerSaveData.first_time || GameManager.Instance.DisplayTutorial); });
    }

    #endregion


    #region Utilities

    /// <summary>
    /// Overridden method, sets all of the debugging variables to false.
    /// </summary>
    void DebuggingVariablesSetup()
    {
        #if !UNITY_EDITOR
            RespawnMapOnPlay = true;
            UseGrayBlocks = false;
            DebugMode = false;
        #endif
    }

    /// <summary>
    /// Determines if the current index is a void or not.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool IsVoidAtIndex(int index)
    {
        return listTopVoidIndex.Contains(index) || listBotVoidIndex.Contains(index);
    }

    /// <summary>
    /// Returns the y spawn point of the object based on what type of block it is and
    /// the index of the map.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="blockType"></param>
    /// <returns></returns>
    float GetYSpawn(int index, char blockType)
    {
        switch (index)
        {
            case 0:
                if (blockType == 'C' || blockType == 'G')
                    return Y_SPAWN_C0;
                else if (blockType == 'T')
                    return Y_SPAWN_T0;
                else
                    return -1;
            case 1:
                if (blockType == 'T')
                    return Y_SPAWN_T1;
                else if (blockType == 'P')
                    return Y_SPAWN_P1;
                else if (blockType == 'O')
                    return Y_SPAWN_C0;
                else
                    return -1;
            case 2:
                if (blockType == 'P')
                    return Y_SPAWN_P2;
                else if (blockType == 'T')
                    return Y_SPAWN_T2;
                else if (blockType == 'F')
                    return Y_SPAWN_F2;
                else if (blockType == 'O')
                    return Y_SPAWN_C3;
                else
                    return -1;
            case 3:
                if (blockType == 'C' || blockType == 'G')
                    return Y_SPAWN_C3;
                else
                    return -1;
            case 4:
                if (blockType == 'P')
                    return Y_SPAWN_P4;
                else if (blockType == 'T')
                    return Y_SPAWN_T4;
                else if (blockType == 'O')
                    return Y_SPAWN_C3;
                else
                    return -1;
            case 5:
                if (blockType == 'T')
                    return Y_SPAWN_T5;
                else if (blockType == 'P')
                    return Y_SPAWN_P5;
                else if (blockType == 'F')
                    return Y_SPAWN_F5;
                else if (blockType == 'O')
                    return Y_SPAWN_C6;
                else
                    return -1;
            case 6:
                if (blockType == 'C' || blockType == 'G')
                    return Y_SPAWN_C6;
                else if (blockType == 'T')
                    return Y_SPAWN_T6;
                else
                    return -1;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Returns the parent block based on the index of the map.
    /// 
    /// NOTE: Does not return goExtraParent!
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Transform GetParent(int index)
    {
        if (index <= 1)
            return goTopBlockParent;
        else if (index >= 2 && index <= 4)
            return goMidBlockParent;
        else
            return goBotBlockParent;
    }

    /// <summary>
    /// Helper method to the coroutine IEnumGetBlockList().
    /// </summary>
    /// <param name="lvl"></param>
    void GetBlockList(int lvl)
    {
        StartCoroutine(IEnumGetBlockList(lvl));
    }

    /// <summary>
    /// Grabs the data from the file with the corresponding level in the filename. For loading in Windows, System.IO.StreamReader is 
    /// used and on android, UnityWebRequest is used. The folder that hold the map data is stored within the streaming assets folder.
    /// The way this method works is by ignoring all lines until "<start>" is encountered. From there, it will read until "<end>" is
    /// encountered. Everything after "<end>" will be ignored.
    /// </summary>
    /// <param name="lvl"></param>
    /// <returns></returns>
    System.Collections.IEnumerator IEnumGetBlockList(int lvl)
    {
        int index = -1;
        string path = Application.streamingAssetsPath + string.Format(MAP_FILENAME, lvl);
        string text;
        string[] list = new string[7];
        string[] split;

        #if UNITY_EDITOR
            text = System.IO.File.ReadAllText(path);
            yield return null;
        #else
            UnityEngine.Networking.UnityWebRequest reader = UnityEngine.Networking.UnityWebRequest.Get(path);
            yield return reader.SendWebRequest();

            text = reader.downloadHandler.text;
        #endif

        split = text.Split(new[] { System.Environment.NewLine }, System.StringSplitOptions.None);

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Trim() == "<start>")
            {
                index = 0;
            }
            else if (split[i].Trim() == "<end>")
            {
                break;
            }
            else if (index >= 0)
            {
                if (split[i].Length > 0)
                    list[index] = (split[i][0] != '#') ? split[i] : string.Empty;
                else
                    list[index] = string.Empty;

                index++;
            }
        }

        ParseMap(lvl, list); // read the data and spawn the map
    }

    /// <summary>
    /// Creates a game object of name that will be the parent block for other game object and adding any components,
    /// if specified.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    Transform CreateParentBlock(string name, Transform parent = null, params System.Type[] components)
    {
        Transform temp = new GameObject(name).transform;

        if (parent)
            temp.parent = parent;

        if (components.Length > 0)
        {
            foreach (System.Type c in components)
                temp.gameObject.AddComponent(c);
        }

        return temp;
    }

    /// <summary>
    /// Combine the meshes of the cubes and spikes that were spawned. Keep in mind that this does not 
    /// combine the meshes of the player, flag or obstacles since they will either move or serve a 
    /// different purpose.
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="spikes"></param>
    void CombineMeshes(Transform[] blocks, Transform[] spikes)
    {
        // find mesh blocks and destroy them if they exist
        GameObject tempBlock = GameObject.Find(MAP_BLOCK);
        GameObject tempSpike = GameObject.Find(MAP_SPIKE);

        if (tempBlock != null)
            DestroyImmediate(tempBlock);

        if (tempSpike != null)
            DestroyImmediate(tempSpike);


        // create new mesh block with MeshFilter and MeshRenderer components
        Transform blockHolder = CreateParentBlock(MAP_BLOCK, transform, typeof(MeshFilter), typeof(MeshRenderer));
        Transform spikeHolder = CreateParentBlock(MAP_SPIKE, transform, typeof(MeshFilter), typeof(MeshRenderer));

        // create arrays that will hold the mesh of each object 
        MeshFilter mf = new MeshFilter();
        CombineInstance[] combineBlocks = new CombineInstance[blocks.Length];
        CombineInstance[] combineSpikes = new CombineInstance[spikes.Length];

        // get the mesh of the blocks (cubes)
        for (int i = 0; i < blocks.Length; i++)
        {
            mf = blocks[i].GetComponent<MeshFilter>();
            combineBlocks[i].mesh = mf.sharedMesh;
            combineBlocks[i].transform = mf.transform.localToWorldMatrix;
        }

        // get the mesh fo the spikes (triangles)
        for (int i = 0; i < spikes.Length; i++)
        {
            mf = spikes[i].GetComponent<MeshFilter>();
            combineSpikes[i].mesh = mf.sharedMesh;
            combineSpikes[i].transform = mf.transform.localToWorldMatrix;
        }

        // combine the meshes and apply the correct settings for the mesh block
        SetupGameObject(blockHolder, combineBlocks);
        SetupGameObject(spikeHolder, combineSpikes);

        // change the tags of the cube and spike objects so that the ball can interact with 
        // them correctly
        blockHolder.tag = TAG_PLATFORM;
        spikeHolder.tag = TAG_SPIKE;

        // destroy the parent block that held all of the indiviual cubes and spikes, it is no
        // longer needed
        DestroyImmediate(goBlockParent.gameObject);
    }

    /// <summary>
    /// Combine all of the meshes that is within the combine array and assign it a material. Get
    /// the mesh block that will hold this mesh and ensure all of the settings in the MeshRenderer
    /// is correctly set.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="combine"></param>
    void SetupGameObject(Transform obj, CombineInstance[] combine)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

        // combine the mesh from the meshes within the combine array
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);
        obj.gameObject.AddComponent<MeshCollider>();
        obj.GetComponent<Renderer>().material = objectMaterial;

        // turn off uncessary settings
        meshRenderer.receiveShadows = false;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion; 
    }

    #endregion
}