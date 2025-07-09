using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance;

    [Header("Settings")]
    [Space(10)]
    [Header("Sound")]
    [SerializeField] private Slider soundSlider;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [HideInInspector] public SoundData GameSoundData;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InitSound(); 
    }
    private void InitSound()
    {
        GetGameSoundData();

        float savedVolume = PlayerDataManager.Instance.CurrentPlayerData.GameSound;

        AudioListener.volume = 0f;
        DOTween.To(() => AudioListener.volume,
                   x => AudioListener.volume = x,
                   savedVolume / 100f,
                   2f);

        soundSlider.onValueChanged.AddListener(OnSliderChanged);
        soundSlider.value = savedVolume;
    }
    private void GetGameSoundData()
    {
        GameSoundData = Resources.Load<SoundData>("Sound/Sounds");

        if (GameSoundData != null)
            PlayMusic(GameSoundData.BackgroundMusic);
        else
            Debug.LogError("Sound Data Not Found!");
    }
    public void OnSliderChanged(float value)
    {
        FadeSound(value);
        PlayerDataManager.Instance.CurrentPlayerData.GameSound = value;
        PlayerDataManager.Instance.SaveData();
    }
    //public void SetSound()
    //{
    //    float value = soundSlider.value;
    //    AudioListener.volume = value / 100f;
    //}
    private void FadeSound(float targetVolume, float duration = 0.5f)
    {
        float startVolume = AudioListener.volume;
        DOTween.To(() => startVolume,
                   x => AudioListener.volume = x,
                   targetVolume / 100f,
                   duration);
    }
    //Sounds
    public void ClassicButtonSound()
    {
        PlaySFX(GameSoundData.ClickSound);
    }
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning("Clip Not Found!");
    }
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip is null!");
        }
    }
}