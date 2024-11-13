using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public EazySoundDemoAudioControls[] SFXControls; // SFX ��Ʈ�� ����
    public EazySoundDemoAudioControls[] AudioControls; // ��� ��Ʈ�� ����
    public Slider globalVolSlider; // ��ü ���� �����̴�
    public Slider globalMusicVolSlider; // ������� �����̴�
    public Slider globalSoundVolSlider; // SFX �����̴�
    public AudioSource audioSource;

    private const string GlobalVolumeKey = "GlobalVolume"; // ��ü ���� ���� Ű
    private const string GlobalMusicVolumeKey = "GlobalMusicVolume"; // ������� ���� ���� Ű
    private const string GlobalSoundVolumeKey = "GlobalSoundVolume"; // SFX ���� ���� Ű

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

        // �����̴� �ʱ�ȭ �� ����� ���� �ε�
        LoadVolumeSettings();

        // �ʱ� ������� ���
        PlayBGM(0);
    }

    // SFX ��� �޼���
    public void PlaySFX(int sfxId)
    {
        EazySoundDemoAudioControls audioControl = SFXControls[sfxId];
        int audioID = EazySoundManager.PlaySound(audioControl.audioclip, audioControl.volumeSlider.value);
        SFXControls[sfxId].audio = EazySoundManager.GetAudio(audioID);
    }

    // ��� ��� �޼���
    public void PlayBGM(int bgmId)
    {
        EazySoundDemoAudioControls audioControl = AudioControls[bgmId];
        int audioID = EazySoundManager.PlayMusic(audioControl.audioclip, audioControl.volumeSlider.value, true, false);
    }

    // ��ü ���� ����
    public void GlobalVolumeChanged()
    {
        EazySoundManager.GlobalVolume = (globalVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalVolumeKey, globalVolSlider.value); // ���� ����
        PlayerPrefs.Save();
    }

    // ������� ���� ����
    public void GlobalMusicVolumeChanged()
    {
        EazySoundManager.GlobalMusicVolume = (globalMusicVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalMusicVolumeKey, globalMusicVolSlider.value); // ������� ���� ����
        PlayerPrefs.Save();
    }

    // SFX ���� ����
    public void GlobalSoundVolumeChanged()
    {
        EazySoundManager.GlobalSoundsVolume = (globalSoundVolSlider.value / 6);
        PlayerPrefs.SetFloat(GlobalSoundVolumeKey, globalSoundVolSlider.value); // SFX ���� ����
        PlayerPrefs.Save();
    }

    // ����� ���� �� �ҷ�����
    private void LoadVolumeSettings()
    {
        // ��ü ���� �ε�
        if (PlayerPrefs.HasKey(GlobalVolumeKey))
        {
            globalVolSlider.value = PlayerPrefs.GetFloat(GlobalVolumeKey);
            GlobalVolumeChanged(); // �� �ε� �� ���� ����
        }

        // ������� ���� �ε�
        if (PlayerPrefs.HasKey(GlobalMusicVolumeKey))
        {
            globalMusicVolSlider.value = PlayerPrefs.GetFloat(GlobalMusicVolumeKey);
            GlobalMusicVolumeChanged(); // �� �ε� �� ���� ����
        }

        // SFX ���� �ε�
        if (PlayerPrefs.HasKey(GlobalSoundVolumeKey))
        {
            globalSoundVolSlider.value = PlayerPrefs.GetFloat(GlobalSoundVolumeKey);
            GlobalSoundVolumeChanged(); // �� �ε� �� ���� ����
        }
    }
}
