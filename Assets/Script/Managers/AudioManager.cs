/*****************************************************************************************************************
 - AudioManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Gravity
 Engine Version:     Unity 2019.1.3f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles all of the audio for the game. All background music and sound effect will play from this game
     object. This game object will persist throughout the entire game and will be the only audio source object
     in the game.
*****************************************************************************************************************/

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Fields

    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// Sound effects volume. Will automatically set the volume of the audio source.
    /// </summary>
    public float BGMVolume
    {
        get { return audioBGMSource.volume; }
        set { audioBGMSource.volume = value; }
    }

    /// <summary>
    /// Background music volume. Will automatically set the volume of the audio source.
    /// </summary>
    public float SEVolume
    {
        get { return audioSESource.volume; }
        set { audioSESource.volume = value; }
    }

    [SerializeField] AudioClip audioSEBallDestroyed;
    [SerializeField] AudioClip[] audioBGM;


    bool stopBGM, playingBGM;
    int bgmIndex;
    AudioSource audioBGMSource, audioSESource;
    WaitForSecondsRealtime songLength;

    #endregion


    #region Built-In Methods

    /// <summary>
    /// Called when the object is created.
    /// 
    /// Starts playing a random bgm.
    /// </summary>
    void Awake()
    {
        if (FindObjectsOfType<AudioManager>().Length == 1)
        {
            Instance = this;
            stopBGM = false;

            audioBGMSource = transform.GetChild(0).GetComponent<AudioSource>();
            audioSESource = transform.GetChild(1).GetComponent<AudioSource>();

            PlayBGM();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion


    #region Sound Effect

    /// <summary>
    /// Plays the ball destroyed sound effect. 
    /// </summary>
    public void PlayBallDestroyedSE()
    {
        PlaySE(audioSEBallDestroyed);
    }

    /// <summary>
    /// Plays a sound effect clip at a volume specified by SEVolume field.
    /// </summary>
    /// <param name="se"></param>
    public void PlaySE(AudioClip se)
    {
        audioSESource.clip = se;
        audioSESource.Play();
    }

    #endregion


    #region Background Music

    /// <summary>
    /// Helper method to coroutine.
    /// </summary>
    public void PlayBGM()
    {
        if (audioBGM.Length > 0 && !playingBGM)
            StartCoroutine(IEnumPlayBGM());
    }

    /// <summary>
    /// Stops the audio source and coroutine from continuing to play the bgm.
    /// </summary>
    public void StopBGM()
    {
        playingBGM = false;
        stopBGM = true;
        audioBGMSource.Stop();
        StopCoroutine(IEnumPlayBGM());
    }

    /// <summary>
    /// Coroutine to play the bgm. It first selects a random clip from the audioBGM array and
    /// then plays it. This coroutine will yield for the duration of the song length.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator IEnumPlayBGM()
    {
        playingBGM = true;

        while (!stopBGM)
        {
            audioBGMSource.clip = GetRandomBGMClip();
            audioBGMSource.Play();

            songLength = new WaitForSecondsRealtime(audioBGMSource.clip.length);
            yield return songLength;
        }

        playingBGM = false;
    }

    /// <summary>
    /// Returns a random bgm clip from the audioBGM array.
    /// </summary>
    /// <returns></returns>
    AudioClip GetRandomBGMClip()
    {
        int index = 0;

        do
        {
            index = Random.Range(0, audioBGM.Length);
        } while (index == bgmIndex);

        bgmIndex = index;
        return audioBGM[bgmIndex];
    }

    #endregion
}
