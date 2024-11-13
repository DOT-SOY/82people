using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public EazySoundDemoAudioControls[] SFXControls; // SFX 컨트롤 저장
    public EazySoundDemoAudioControls[] AudioControls; // 브금 컨트롤 저장
    public Slider globalVolSlider; // 전체 볼륨 슬라이더
    public Slider globalMusicVolSlider; // 배경음악 슬라이더
    public Slider globalSoundVolSlider; // SFX 슬라이더
    public AudioSource audioSource;

    private const string GlobalVolumeKey = "GlobalVolume"; // 전체 볼륨 저장 키
    private const string GlobalMusicVolumeKey = "GlobalMusicVolume"; // 배경음악 볼륨 저장 키
    private const string GlobalSoundVolumeKey = "GlobalSoundVolume"; // SFX 볼륨 저장 키

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 슬라이더 초기화 및 저장된 볼륨 로드
        LoadVolumeSettings();

        // 초기 배경음악 출력
        PlayBGM(0);
    }

    // SFX 출력 메서드
    public void PlaySFX(int sfxId)
    {
        EazySoundDemoAudioControls audioControl = SFXControls[sfxId];
        int audioID = EazySoundManager.PlaySound(audioControl.audioclip, audioControl.volumeSlider.value);
        SFXControls[sfxId].audio = EazySoundManager.GetAudio(audioID);
    }

    // 브금 출력 메서드
    public void PlayBGM(int bgmId)
    {
        EazySoundDemoAudioControls audioControl = AudioControls[bgmId];
        int audioID = EazySoundManager.PlayMusic(audioControl.audioclip, audioControl.volumeSlider.value, true, false);
    }

    // 전체 볼륨 변경
    public void GlobalVolumeChanged()
    {
        EazySoundManager.GlobalVolume = (globalVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalVolumeKey, globalVolSlider.value); // 볼륨 저장
        PlayerPrefs.Save();
    }

    // 배경음악 볼륨 변경
    public void GlobalMusicVolumeChanged()
    {
        EazySoundManager.GlobalMusicVolume = (globalMusicVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalMusicVolumeKey, globalMusicVolSlider.value); // 배경음악 볼륨 저장
        PlayerPrefs.Save();
    }

    // SFX 볼륨 변경
    public void GlobalSoundVolumeChanged()
    {
        EazySoundManager.GlobalSoundsVolume = (globalSoundVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalSoundVolumeKey, globalSoundVolSlider.value); // SFX 볼륨 저장
        PlayerPrefs.Save();
    }

    // 저장된 볼륨 값 불러오기
    private void LoadVolumeSettings()
    {
        // 전체 볼륨 로드
        if (PlayerPrefs.HasKey(GlobalVolumeKey))
        {
            globalVolSlider.value = PlayerPrefs.GetFloat(GlobalVolumeKey);
            GlobalVolumeChanged(); // 값 로드 후 볼륨 적용
        }

        // 배경음악 볼륨 로드
        if (PlayerPrefs.HasKey(GlobalMusicVolumeKey))
        {
            globalMusicVolSlider.value = PlayerPrefs.GetFloat(GlobalMusicVolumeKey);
            GlobalMusicVolumeChanged(); // 값 로드 후 볼륨 적용
        }

        // SFX 볼륨 로드
        if (PlayerPrefs.HasKey(GlobalSoundVolumeKey))
        {
            globalSoundVolSlider.value = PlayerPrefs.GetFloat(GlobalSoundVolumeKey);
            GlobalSoundVolumeChanged(); // 값 로드 후 볼륨 적용
        }
    }
}
