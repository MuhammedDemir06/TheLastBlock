using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Game/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Game Sounds")]
    public AudioClip BackgroundMusic;
    public AudioClip LoseSound;
    public AudioClip ClickSound;
    public AudioClip PauseSound;
    public AudioClip UnpauseSound;
    public AudioClip NextLevelSound;
    [Header("Player")]
    public AudioClip PlayerJumpSound;
    public AudioClip PlayerHitSound;
    // add as much as you want
}
